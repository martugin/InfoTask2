using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "MirSource")]
    public class MirSource : SqlSourceBase
    {
        //Код провайдера
        public override string Code { get { return "MirSource"; } }
        
        //Список сигналов
        #region
        //Словари объектов, ключи коды и IdChanell
        private readonly DicS<ObjectMir> _objects = new DicS<ObjectMir>();
        private readonly DicI<ObjectMir> _objectsId = new DicI<ObjectMir>();
        
        //Очистка списка сигналов
        public override void ClearSignals()
        {
            base.ClearSignals();
            _objects.Clear();
            _objectsId.Clear();
        }

        //Добавляет один сигнал в список
        protected override SourceObject AddObject(SourceSignal sig, string context)
        {
            string ocode = sig.Inf.Get("Name_Object") + "." + sig.Inf.Get("Name_Device") + "." + sig.Inf.Get("Name_Type");
            if (!_objects.ContainsKey(ocode))
                return _objects.Add(ocode, new ObjectMir(this, context));
            return _objects[ocode];
        }
        
        //Подготовка провайдера, чтение значений IDCHANNEL
        public override void Prepare()
        {
            _objectsId.Clear();
            using (var rec = new ReaderAdo(SqlProps, "SELECT OBJECTS.NAME_OBJECT, DEVICES.NAME_DEVICE, LIB_CHANNELS.NAME_TYPE, LIB_CHANNELS.UNIT, CHANNELS.IDCHANNEL, LIB_CHANNELS.TABLE_NAME " +
            "FROM CHANNELS INNER JOIN DEVICES ON CHANNELS.IDDEVICE = DEVICES.IDDEVICE INNER JOIN " +
            "LIB_CHANNELS ON dbo.CHANNELS.IDTYPE_CHANNEL = dbo.LIB_CHANNELS.IDTYPE_CHANNEL INNER JOIN " +
            "POINT_DEVICES ON dbo.DEVICES.IDDEVICE = dbo.POINT_DEVICES.IDDEVICE INNER JOIN " +
            "POINT_CONNECTIONS ON dbo.POINT_DEVICES.IDPOINT_CONNECTION = dbo.POINT_CONNECTIONS.IDPOINT_CONNECTION INNER JOIN " +
            "POINT_OBJ ON dbo.POINT_CONNECTIONS.IDPOINT_CONNECTION = dbo.POINT_OBJ.IDPOINT_CONNECTION INNER JOIN " +
            "OBJECTS ON dbo.POINT_OBJ.IDOBJECT = dbo.OBJECTS.IDOBJECT"))
                while (rec.Read())
                {
                    string ocode = rec.GetString("NAME_OBJECT") + "." + rec.GetString("NAME_DEVICE") + "." + rec.GetString("NAME_TYPE");
                    var id = rec.GetInt("IDCHANNEL");
                    if (_objects.ContainsKey(ocode))
                    {
                        var ob = _objects[ocode];
                        ob.IdChannel = id;
                        _objectsId.Add(id, ob);
                    }
                }
        }
        #endregion

        //Чтение данных из архива
        #region
        //Запрос значений по одному блоку сигналов
        protected override bool QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en)
        {
            string queryString = "SELECT IDCHANNEL, TIME, VALUE, VALUE_UNIT, VALUE_INDICATION FROM IZM_TII" +
                                     " WHERE (TIME >= " + beg.ToSqlString() + ") AND (TIME <=" + en.ToSqlString() +
                                     ") ORDER BY TIME";
            AddEvent("Запрос значений из базы");
            Rec = new ReaderAdo(SqlProps, queryString);
            AddEvent("Запрос из базы отработал");
            return true;
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject()
        {
            return _objectsId[Rec.GetInt("IDCHANNEL")];
        }

        //Чтение среза
        protected override void ReadCut()
        {
            ReadValuesByParts(_objects.Values, 5000, PeriodBegin.AddMinutes(-30), PeriodBegin, true);
        }

        //Чтение изменений
        protected override void ReadChanges()
        {
            ReadValuesByParts(_objects.Values, 5000, PeriodBegin, PeriodEnd, false);
        }
        #endregion
    }
}
