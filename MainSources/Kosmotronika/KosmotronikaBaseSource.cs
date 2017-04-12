using System;
using System.Collections.Generic;
using System.Data.OleDb;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    //Базовый класс для источников космотроники
    public abstract class KosmotronikaBaseSource : OleDbSource
    {
        //Получение времени архива ПТК
        protected override TimeInterval GetTimeSource()
        {
            using (var rec = new AdoReader(Connection, "Exec RT_ARCHDATE"))
            {
                var beg = rec.GetTime(0);
                var en = rec.GetTime(1);
                return beg.ToString() != "0:00:00" 
                    ? new TimeInterval(beg, en) 
                    : TimeInterval.CreateDefault();
            }
        }
        
        //Словарь выходов. Один элемент словаря - один выход
        internal readonly Dictionary<OutIndex, KosmOut> Outs = new Dictionary<OutIndex, KosmOut>();
        //Словарь аналоговых выходов
        internal readonly Dictionary<OutIndex, KosmOut> Analogs = new Dictionary<OutIndex, KosmOut>();
        //Выход действий оператора
        internal KosmOperatorOut OperatorOut;

        //Очистка списка сигналов
        protected override void ClearOuts()
        {
            Outs.Clear();
            Analogs.Clear();
            OperatorOut = null;
        }

        //Добавляет один сигнал в список
        protected override ProviderOut AddOut(ProviderSignal sig)
        {
            if (sig.Inf.Get("ObjectType") == "Operator")
                return OperatorOut ?? (OperatorOut = new KosmOperatorOut(this));

            var ind = new OutIndex(sig.Inf.GetInt("SysNum"), sig.Inf.GetInt("NumType"), sig.Inf.GetInt("Appartment"), sig.Inf.GetInt("NumOut"));
            KosmOut obj;
            if (ind.Out == 1 && (ind.NumType == 1 || ind.NumType == 3 || ind.NumType == 32))
            {
                if (Analogs.ContainsKey(ind)) obj = Analogs[ind];
                else Analogs.Add(ind, obj = new KosmOut(this, ind));
            }
            else
            {
                if (Outs.ContainsKey(ind)) obj = Outs[ind];
                else Outs.Add(ind, obj = new KosmOut(this, ind));
            }
            return obj;
        }

        //Создание фабрики ошибок
        protected override IMomErrFactory MakeErrFactory()
        {
            return new KosmMomErrFactory();
        }

        //Производится считывание аналоговых сигналов
        internal bool IsAnalog { get; private set; }

        //Определяет размер блока для считывания, исходя из длины периода
        protected abstract int PartSize { get; }

        //Запрос значений по одному блоку выходов
        protected override IRecordRead QueryValues(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            var nums = new ushort[part.Count, IsAnalog ? 3 : 4];
            for (int i = 0; i < part.Count; i++)
            {
                var ob = (KosmOut)part[i];
                nums[i, 0] = (ushort)ob.Sn;
                nums[i, 1] = (ushort)ob.NumType;
                nums[i, 2] = (ushort)ob.Appartment;
                if (!IsAnalog) nums[i, 3] = (ushort)ob.Out;
            }

            var parSysNums = new OleDbParameter("Sysnums", OleDbType.Variant) { Value = nums };
            var parBeginTime = new OleDbParameter("BeginTime", OleDbType.DBTimeStamp) { Value = beg };
            var parEndTime = new OleDbParameter("EndTime", OleDbType.DBTimeStamp) { Value = en };
            var rec = isCut
                ? new AdoReader(Connection, IsAnalog ? "Exec ST_ANALOG ?, ?" : "Exec ST_OUT ?, ?", parBeginTime, parSysNums)
                : new AdoReader(Connection, IsAnalog ? "Exec RT_ANALOGREAD ? , ? , ?" : "Exec RT_EXTREAD ? , ? , ?", parBeginTime, parEndTime, parSysNums);

            if (isCut && !rec.HasRows)
                AddWarning("Значения из источника не получены", null, part[0].Context + " и др.");
            return rec;
        }

        //Определение текущего считываемого выхода
        protected override ListSourceOut DefineOut(IRecordRead rec)
        {
            int dn = this is KosmotronikaRetroSource ? 1 : 0;
            var ind = new OutIndex(rec.GetInt(0), rec.GetInt(1), rec.GetInt(2), IsAnalog ? 1 : rec.GetInt(5 + dn));
            if (IsAnalog && Analogs.ContainsKey(ind))
                return Analogs[ind];
            return Outs.ContainsKey(ind) ? Outs[ind] : null;
        }

        //Запрос значений действий оператора
        protected IRecordRead QueryValuesOperator(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            var parBeginTime = new OleDbParameter("BeginTime", OleDbType.DBTimeStamp) { Value = beg };
            var parEndTime = new OleDbParameter("EndTime", OleDbType.DBTimeStamp) { Value = en };
            return new AdoReader(Connection, "Exec RT_OPRREAD ?, ?, ?", parBeginTime, parEndTime);
        }

        //Чтение среза
        protected override ValuesCount ReadCut()
        {
            var vc = new ValuesCount();
            IsAnalog = true;
            using (Start(0, AnalogsProcent()))
                vc += ReadByParts(Analogs.Values, PartSize, PeriodBegin, PeriodEnd, true, "Срез данных по аналоговым сигналам");
            if (vc.IsFail) return vc;

            IsAnalog = false;
            using (Start(AnalogsProcent(), 100))
                vc += ReadByParts(Outs.Values, PartSize, PeriodBegin, PeriodEnd, true, "Срез данных по выходам");
            return vc;
        }

        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            var vc = new ValuesCount();
            IsAnalog = true;
            using (Start(0, AnalogsProcent()))
                vc += ReadByParts(Analogs.Values, PartSize, "Изменения значений по аналоговым сигналам");
            if (vc.IsFail) return vc;

            IsAnalog = false;
            using (Start(AnalogsProcent(), OutsProcent()))
                vc += ReadByParts(Outs.Values, PartSize, "Изменения значений по выходам");
            if (vc.IsFail) return vc;

            using (Start(OutsProcent(), 100))
                vc += ReadOneOut(OperatorOut, QueryValuesOperator);
            return vc;
        }

        private double AnalogsProcent()
        {
            int op = OperatorOut == null ? 0 : 1;
            if (Outs.Count + Analogs.Count + op == 0) return 0;
            return Analogs.Count * 100.0 / (Outs.Count + Analogs.Count + op);
        }
        private double OutsProcent()
        {
            int op = OperatorOut == null ? 0 : 1;
            if (Outs.Count + Analogs.Count + op == 0) return 0;
            return (Analogs.Count + Outs.Count) * 100.0 / (Outs.Count + Analogs.Count + op);
        }
    }
}