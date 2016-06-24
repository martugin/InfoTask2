using System;
using System.Collections.Generic;
using System.Data.OleDb;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Базовый класс для источников космотроники
    public abstract class KosmotronikaBaseSource : OleDbSour
    {
        //Ссылка на соединение
        internal KosmotronikaConn KosmotronikaConn { get { return (KosmotronikaConn) Conn; } }

        //Проверка соединения
        public override bool CheckConnection()
        {
            if (Check() && GetTime() != null)
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

        //Получение времени архива, True - если успешно
        protected override TimeInterval GetSourceTime()
        {
            if (!Danger(TryGetTime, 2, 500, "Не удалось определить временной диапазон Ретро-сервера")) return null;
            return new TimeInterval(_beg, _en);
        }

        private DateTime _beg;
        private DateTime _en;

        private bool TryGetTime()
        {
            if (!IsConnected && !Check()) return false;
            try
            {
                using (var rec = new ReaderAdo(Connection, "Exec RT_ARCHDATE"))
                {
                    _beg = rec.GetTime(0);
                    _en = rec.GetTime(1);
                    AddEvent("Диапазон источника определен", _beg + " - " + _en);
                    return _beg.ToString() != "0:00:00";
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка определения диапазона Ретро-сервера", ex);
                return IsConnected = false;
            }
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
        protected override IRecordRead QueryPartValues(List<SourObject> part, DateTime beg, DateTime en, bool isCut)
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
            {
                AddWarning("Значения из источника не получены", null, part[0].CodeObject + " и др.");
                IsConnected = false;
            }
            return rec;
        }

        //Определение текущего считываемого объекта
        protected override SourObject DefineObject(IRecordRead rec)
        {
            var ind = new ObjectIndex
            {
                Sn = rec.GetInt(0),
                NumType = rec.GetInt(1),
                Appartment = rec.GetInt(2),
                Out = IsAnalog ? 1 : rec.GetInt(6)
            };
            if (IsAnalog && KosmotronikaConn.Analogs.ContainsKey(ind))
                return KosmotronikaConn.Analogs[ind];
            if (KosmotronikaConn.Outs.ContainsKey(ind))
                return KosmotronikaConn.Outs[ind];
            return null;
        }

        private double AnalogsProcent()
        {
            if (KosmotronikaConn.Outs.Count + KosmotronikaConn.Analogs.Count == 0) return 0;
            return KosmotronikaConn.Analogs.Count * 100.0 / (KosmotronikaConn.Outs.Count + KosmotronikaConn.Analogs.Count);
        }

        //Чтение среза
        protected override void ReadCut()
        {
            IsAnalog = true;
            using (Start(0, AnalogsProcent()))
                ReadValuesByParts(KosmotronikaConn.Analogs.Values, PartSize(), PeriodBegin, PeriodEnd, true, "Срез данных по аналоговым сигналам");

            IsAnalog = false;
            using (Start(AnalogsProcent(), 100))
                ReadValuesByParts(KosmotronikaConn.Outs.Values, PartSize(), PeriodBegin, PeriodEnd, true, "Срез данных по выходам");
        }

        //Чтение изменений
        protected override void ReadChanges()
        {
            IsAnalog = true;
            using (Start(0, AnalogsProcent()))
                ReadValuesByParts(KosmotronikaConn.Analogs.Values, PartSize(), PeriodBegin, PeriodEnd, false, "Изменения значений по аналоговым сигналам");

            IsAnalog = false;
            using (Start(AnalogsProcent(), 100))
                ReadValuesByParts(KosmotronikaConn.Outs.Values, PartSize(), PeriodBegin, PeriodEnd, false, "Изменения значений по выходам");
        }
    }
}