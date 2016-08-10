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
                if (beg.ToString() != "0:00:00")
                    return new TimeInterval(beg, en);
                return TimeInterval.CreateDefault();
            }
        }
        
        //Словарь объектов. Один элемент словаря - один выход
        private readonly Dictionary<ObjectIndex, ObjectKosm> _outs = new Dictionary<ObjectIndex, ObjectKosm>();
        //Словарь аналоговых объектов
        private readonly Dictionary<ObjectIndex, ObjectKosm> _analogs = new Dictionary<ObjectIndex, ObjectKosm>();
        //Объект действий оператора
        private ObjectKosmOperator _operatorObject;

        //Очистка списка сигналов
        protected override void ClearObjects()
        {
            _outs.Clear();
            _analogs.Clear();
            _operatorObject = null;
        }

        //Добавляет один сигнал в список
        protected override SourceObject AddObject(InitialSignal sig)
        {
            if (sig.Inf.Get("ObjectType") == "Operator")
                return _operatorObject ?? (_operatorObject = new ObjectKosmOperator(this));
            
            var ind = new ObjectIndex
            {
                Sn = sig.Inf.GetInt("SysNum"),
                NumType = sig.Inf.GetInt("NumType"),
                Appartment = sig.Inf.GetInt("Appartment"),
                Out = sig.Inf.GetInt("NumOut")
            };
            ObjectKosm obj;
            if (ind.Out == 1 && (ind.NumType == 1 || ind.NumType == 3 || ind.NumType == 32))
            {
                if (_analogs.ContainsKey(ind)) obj = _analogs[ind];
                else _analogs.Add(ind, obj = new ObjectKosm(this, ind));
            }
            else
            {
                if (_outs.ContainsKey(ind)) obj = _outs[ind];
                else _outs.Add(ind, obj = new ObjectKosm(this, ind));
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
        private int PartSize()
        {
            double len = PeriodEnd.Subtract(PeriodBegin).TotalHours;
            int res = 3000;
            if (len > 0.0001) res = Math.Min(3000, Convert.ToInt32(2500 / len));
            if (res == 0) res = 1;
            return res;
        }

        //Запрос значений по одному блоку сигналов
        protected override IRecordRead QueryValues(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            var nums = new ushort[part.Count, IsAnalog ? 3 : 4];
            for (int i = 0; i < part.Count; i++)
            {
                var ob = (ObjectKosm)part[i];
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

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            int dn = this is KosmotronikaRetroSource ? 1 : 0;
            var ind = new ObjectIndex
            {
                Sn = rec.GetInt(0),
                NumType = rec.GetInt(1),
                Appartment = rec.GetInt(2),
                Out = IsAnalog ? 1 : rec.GetInt(5+dn)
            };
            if (IsAnalog && _analogs.ContainsKey(ind))
                return _analogs[ind];
            if (_outs.ContainsKey(ind))
                return _outs[ind];
            return null;
        }

        //Запрос значений действий оператора
        protected IRecordRead QueryValuesOperator(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
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
                vc += ReadByParts(_analogs.Values, PartSize(), PeriodBegin, PeriodEnd, true, "Срез данных по аналоговым сигналам");
            if (vc.IsBad) return vc;

            IsAnalog = false;
            using (Start(AnalogsProcent(), 100))
                vc += ReadByParts(_outs.Values, PartSize(), PeriodBegin, PeriodEnd, true, "Срез данных по выходам");
            return vc;
        }

        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            var vc = new ValuesCount();
            IsAnalog = true;
            using (Start(0, AnalogsProcent()))
                vc += ReadByParts(_analogs.Values, PartSize(), "Изменения значений по аналоговым сигналам");
            if (vc.IsBad) return vc;

            IsAnalog = false;
            using (Start(AnalogsProcent(), OutsProcent()))
                vc += ReadByParts(_outs.Values, PartSize(), "Изменения значений по выходам");
            if (vc.IsBad) return vc;

            using (Start(OutsProcent(), 100))
                vc += ReadOneObject(_operatorObject, QueryValuesOperator);
            return vc;
        }

        private double AnalogsProcent()
        {
            int op = _operatorObject == null ? 0 : 1;
            if (_outs.Count + _analogs.Count + op == 0) return 0;
            return _analogs.Count * 100.0 / (_outs.Count + _analogs.Count + op);
        }
        private double OutsProcent()
        {
            int op = _operatorObject == null ? 0 : 1;
            if (_outs.Count + _analogs.Count + op == 0) return 0;
            return (_analogs.Count + _outs.Count) * 100.0 / (_outs.Count + _analogs.Count + op);
        }
    }
}