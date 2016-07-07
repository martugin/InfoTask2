using System;
using System.Collections.Generic;
using System.Data.OleDb;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    //Базовый класс для источников космотроники
    public abstract class KosmotronikaBaseSource : AdoSource
    {
        //Комплект провайдеров
        public override string Complect { get { return "Kosmotronika"; } }
        //Ссылка на соединение
        internal KosmotronikaBaseSettings Settings { get { return (KosmotronikaBaseSettings)CurSettings; } }
        
        //Словарь объектов. Один элемент словаря - один выход, для выхода список битов
        private readonly Dictionary<ObjectIndex, ObjectKosm> _outs = new Dictionary<ObjectIndex, ObjectKosm>();
        //Словарь аналоговых объектов
        private readonly Dictionary<ObjectIndex, ObjectKosm> _analogs = new Dictionary<ObjectIndex, ObjectKosm>();

        //Очистка списка сигналов
        protected override void ClearObjects()
        {
            _outs.Clear();
            _analogs.Clear();
        }

        //Добавляет один сигнал в список
        protected override SourceObject AddObject(InitialSignal sig)
        {
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
        protected override IRecordRead QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en, bool isCut)
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
                ? new ReaderAdo(Settings.Connection, IsAnalog ? "Exec ST_ANALOG ?, ?" : "Exec ST_OUT ?, ?", parBeginTime, parSysNums)
                : new ReaderAdo(Settings.Connection, IsAnalog ? "Exec RT_ANALOGREAD ? , ? , ?" : "Exec RT_EXTREAD ? , ? , ?", parBeginTime, parEndTime, parSysNums);

            if (isCut && !rec.HasRows)
            {
                AddWarning("Значения из источника не получены", null, part[0].Context + " и др.");
                IsConnected = false;
            }
            return rec;
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            var ind = new ObjectIndex
            {
                Sn = rec.GetInt(0),
                NumType = rec.GetInt(1),
                Appartment = rec.GetInt(2),
                Out = IsAnalog ? 1 : rec.GetInt(6)
            };
            if (IsAnalog && _analogs.ContainsKey(ind))
                return _analogs[ind];
            if (_outs.ContainsKey(ind))
                return _outs[ind];
            return null;
        }

        private double AnalogsProcent()
        {
            if (_outs.Count + _analogs.Count == 0) return 0;
            return _analogs.Count * 100.0 / (_outs.Count + _analogs.Count);
        }

        //Чтение среза
        protected override void ReadCut()
        {
            IsAnalog = true;
            using (Start(0, AnalogsProcent()))
                ReadValuesByParts(_analogs.Values, PartSize(), PeriodBegin, PeriodEnd, true, "Срез данных по аналоговым сигналам");

            IsAnalog = false;
            using (Start(AnalogsProcent(), 100))
                ReadValuesByParts(_outs.Values, PartSize(), PeriodBegin, PeriodEnd, true, "Срез данных по выходам");
        }

        //Чтение изменений
        protected override void ReadChanges()
        {
            IsAnalog = true;
            using (Start(0, AnalogsProcent()))
                ReadValuesByParts(_analogs.Values, PartSize(), PeriodBegin, PeriodEnd, false, "Изменения значений по аналоговым сигналам");

            IsAnalog = false;
            using (Start(AnalogsProcent(), 100))
                ReadValuesByParts(_outs.Values, PartSize(), PeriodBegin, PeriodEnd, false, "Изменения значений по выходам");
        }
    }
}