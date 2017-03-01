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
        //Проверка соединения
        protected override bool CheckConnection()
        {
            if (Reconnect())
            {
                var ti = GetTime();
                if (ti != null)
                {
                    CheckConnectionMessage = "Успешное соединение. Диапазон источника: " + ti.Begin + " - " + ti.End;
                    return true;
                }
            }
            AddError(CheckConnectionMessage = "Ошибка соединения с Ретро-сервером");
            return false;
        }

        //Получение времени архива ПТК
        protected override TimeInterval GetTimeSource()
        {
            using (var rec = new ReaderAdo(Connection, "Exec RT_ARCHDATE"))
            {
                var beg = rec.GetTime(0);
                var en = rec.GetTime(1);
                return beg.ToString() != "0:00:00" 
                    ? new TimeInterval(beg, en) 
                    : TimeInterval.CreateDefault();
            }
        }
        
        //Словарь выходов. Один элемент словаря - один выход
        private readonly Dictionary<OutIndex, OutKosm> _outs = new Dictionary<OutIndex, OutKosm>();
        //Словарь аналоговых выходов
        private readonly Dictionary<OutIndex, OutKosm> _analogs = new Dictionary<OutIndex, OutKosm>();
        //Выход действий оператора
        private OutKosmOperator _operatorOut;

        //Очистка списка сигналов
        protected override void ClearOuts()
        {
            _outs.Clear();
            _analogs.Clear();
            _operatorOut = null;
        }

        //Добавляет один сигнал в список
        protected override SourceOut AddOut(InitialSignal sig)
        {
            if (sig.Inf.Get("ObjectType") == "Operator")
                return _operatorOut ?? (_operatorOut = new OutKosmOperator(this));
            
            var ind = new OutIndex
            {
                Sn = sig.Inf.GetInt("SysNum"),
                NumType = sig.Inf.GetInt("NumType"),
                Appartment = sig.Inf.GetInt("Appartment"),
                Out = sig.Inf.GetInt("NumOut")
            };
            OutKosm obj;
            if (ind.Out == 1 && (ind.NumType == 1 || ind.NumType == 3 || ind.NumType == 32))
            {
                if (_analogs.ContainsKey(ind)) obj = _analogs[ind];
                else _analogs.Add(ind, obj = new OutKosm(this, ind));
            }
            else
            {
                if (_outs.ContainsKey(ind)) obj = _outs[ind];
                else _outs.Add(ind, obj = new OutKosm(this, ind));
            }
            return obj;
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            return new ErrMomFactoryKosm();
        }

        //Производится считывание аналоговых сигналов
        internal bool IsAnalog { get; private set; }

        //Определяет размер блока для считывания, исходя из длины периода
        protected abstract int PartSize { get; }

        //Запрос значений по одному блоку выходов
        protected override IRecordRead QueryValues(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            var nums = new ushort[part.Count, IsAnalog ? 3 : 4];
            for (int i = 0; i < part.Count; i++)
            {
                var ob = (OutKosm)part[i];
                nums[i, 0] = (ushort)ob.Sn;
                nums[i, 1] = (ushort)ob.NumType;
                nums[i, 2] = (ushort)ob.Appartment;
                if (!IsAnalog) nums[i, 3] = (ushort)ob.Out;
            }

            var parSysNums = new OleDbParameter("Sysnums", OleDbType.Variant) { Value = nums };
            var parBeginTime = new OleDbParameter("BeginTime", OleDbType.DBTimeStamp) { Value = beg };
            var parEndTime = new OleDbParameter("EndTime", OleDbType.DBTimeStamp) { Value = en };
            var rec = isCut
                ? new ReaderAdo(Connection, IsAnalog ? "Exec ST_ANALOG ?, ?" : "Exec ST_OUT ?, ?", parBeginTime, parSysNums)
                : new ReaderAdo(Connection, IsAnalog ? "Exec RT_ANALOGREAD ? , ? , ?" : "Exec RT_EXTREAD ? , ? , ?", parBeginTime, parEndTime, parSysNums);

            if (isCut && !rec.HasRows)
                AddWarning("Значения из источника не получены", null, part[0].Context + " и др.");
            return rec;
        }

        //Определение текущего считываемого выхода
        protected override SourceOut DefineOut(IRecordRead rec)
        {
            int dn = this is KosmotronikaRetroSource ? 1 : 0;
            var ind = new OutIndex
            {
                Sn = rec.GetInt(0),
                NumType = rec.GetInt(1),
                Appartment = rec.GetInt(2),
                Out = IsAnalog ? 1 : rec.GetInt(5+dn)
            };
            if (IsAnalog && _analogs.ContainsKey(ind))
                return _analogs[ind];
            return _outs.ContainsKey(ind) ? _outs[ind] : null;
        }

        //Запрос значений действий оператора
        protected IRecordRead QueryValuesOperator(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            var parBeginTime = new OleDbParameter("BeginTime", OleDbType.DBTimeStamp) { Value = beg };
            var parEndTime = new OleDbParameter("EndTime", OleDbType.DBTimeStamp) { Value = en };
            return new ReaderAdo(Connection, "Exec RT_OPRREAD ?, ?, ?", parBeginTime, parEndTime);
        }

        //Чтение среза
        protected override ValuesCount ReadCut()
        {
            var vc = new ValuesCount();
            IsAnalog = true;
            using (Start(0, AnalogsProcent()))
                vc += ReadByParts(_analogs.Values, PartSize, PeriodBegin, PeriodEnd, true, "Срез данных по аналоговым сигналам");
            if (vc.IsFail) return vc;

            IsAnalog = false;
            using (Start(AnalogsProcent(), 100))
                vc += ReadByParts(_outs.Values, PartSize, PeriodBegin, PeriodEnd, true, "Срез данных по выходам");
            return vc;
        }

        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            var vc = new ValuesCount();
            IsAnalog = true;
            using (Start(0, AnalogsProcent()))
                vc += ReadByParts(_analogs.Values, PartSize, "Изменения значений по аналоговым сигналам");
            if (vc.IsFail) return vc;

            IsAnalog = false;
            using (Start(AnalogsProcent(), OutsProcent()))
                vc += ReadByParts(_outs.Values, PartSize, "Изменения значений по выходам");
            if (vc.IsFail) return vc;

            using (Start(OutsProcent(), 100))
                vc += ReadOneOut(_operatorOut, QueryValuesOperator);
            return vc;
        }

        private double AnalogsProcent()
        {
            int op = _operatorOut == null ? 0 : 1;
            if (_outs.Count + _analogs.Count + op == 0) return 0;
            return _analogs.Count * 100.0 / (_outs.Count + _analogs.Count + op);
        }
        private double OutsProcent()
        {
            int op = _operatorOut == null ? 0 : 1;
            if (_outs.Count + _analogs.Count + op == 0) return 0;
            return (_analogs.Count + _outs.Count) * 100.0 / (_outs.Count + _analogs.Count + op);
        }
    }
}