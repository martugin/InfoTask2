using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Соединение с источником для получения клона
    public class ClonerConnect : SourceConnect
    {
        public ClonerConnect(Logger logger, string complect) 
            : base(logger, "Clone", complect) { }

        //Рекордсеты таблиц значений клона
        internal DaoRec CloneRec { get; private set; }
        internal DaoRec CloneCutRec { get; private set; }
        internal DaoRec CloneStrRec { get; private set; }
        internal DaoRec CloneStrCutRec { get; private set; }
        //Рекордсет таблицы ошибок создания клона
        internal DaoRec CloneErrorsRec { get; private set; }

        //Чтение Id сигналов клона
        private void ReadCloneSignals(DaoDb cloneDb)
        {
            AddEvent("Чтение сигналов клона");
            ClearSignals();
            using (var rec = new DaoRec(cloneDb, "Signals"))
                while (rec.Read())
                {
                    var sig = (CloneSignal)AddSignal(rec.GetString("FullCode"),
                                                                     rec.GetString("DataType").ToDataType(),
                                                                     rec.GetString("SignalType").ToSignalType() == SignalType.Uniform ? SignalType.UniformClone : SignalType.Clone,
                                                                     rec.GetString("InfObject"),
                                                                     rec.GetString("InfOut"),
                                                                     rec.GetString("InfProp"));
                    sig.IdInClone = rec.GetInt("SignalId");
                }
        }

        //Запись в клон списка описаний ошибок
        private void WriteMomentErrors(DaoDb cloneDb)
        {
            AddEvent("Запись описаний ошибок");
            using (var rec = new DaoRec(cloneDb, "MomentErrors"))
                foreach (var ed in Source.ErrPool.UsedErrorDescrs)
                    ed.ToRecordset(rec);
        }

        //Создание клона источника
        public void MakeClone(string cloneDir) //Каталог клона
        {
            try
            {
                if (PeriodIsUndefined()) return;
                using (var db = new DaoDb(cloneDir.EndDir() + "Clone.accdb"))
                {
                    ReadCloneSignals(db);
                    using (CloneRec = new DaoRec(db, "MomentValues"))
                    using (CloneCutRec = new DaoRec(db, "MomentValuesCut"))
                    using (CloneStrRec = new DaoRec(db, "MomentStrValues"))
                    using (CloneStrCutRec = new DaoRec(db, "MomentStrValuesCut"))
                    using (CloneErrorsRec = new DaoRec(db, "ErrorsObjects"))
                        GetValues();
                    WriteMomentErrors(db);
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при создании клона", ex);
            }
        }

        //Определяет время среза в клоне для указанного момента времени 
        internal DateTime RemoveMinultes(DateTime time)
        {
            int m = time.Minute;
            int k = m / 10;
            var d = time.AddMinutes(-time.Minute).AddSeconds(-time.Second).AddMilliseconds(-time.Millisecond);
            return d.AddMinutes(k * 10);
        }
    }
}