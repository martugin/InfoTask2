using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "MirSource")]
    public class MirSource : SqlServerSource
    {
        //Код провайдера
        public override string Code { get { return "MirSource"; } }
        
        //Словари объектов, ключи коды и IdChanell
        private readonly DicS<ObjectMir> _objects = new DicS<ObjectMir>();
        private readonly DicI<ObjectMir> _objectsId = new DicI<ObjectMir>();
        
        //Очистка списка объектов
        protected override void ClearObjects()
        {
            _objects.Clear();
            _objectsId.Clear();
        }

        //Добавить объект в провайдер
        protected override SourceObject AddObject(InitialSignal sig)
        {
            string ocode = sig.Inf.Get("Name_Object") + "." + sig.Inf.Get("Name_Device") + "." + sig.Inf.Get("Name_Type");
            if (!_objects.ContainsKey(ocode))
                return _objects.Add(ocode, new ObjectMir(this));
            return _objects[ocode];
        }
        
        //Подготовка провайдера, чтение значений IDCHANNEL
        protected override void Prepare()
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

        //Запрос значений по одному блоку сигналов
        protected override IRecordRead QueryValues(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            string queryString = "SELECT IDCHANNEL, TIME, VALUE, VALUE_UNIT, VALUE_INDICATION FROM IZM_TII" +
                                     " WHERE (TIME >= " + beg.ToSqlString() + ") AND (TIME <=" + en.ToSqlString() + ") ORDER BY TIME";
            return new ReaderAdo(SqlProps, queryString);
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            return _objectsId[rec.GetInt("IDCHANNEL")];
        }

        //Чтение среза
        protected override ValuesCount ReadCut()
        {
            return ReadByParts(_objects.Values, 5000, PeriodBegin.AddMinutes(-30), PeriodBegin, true);
        }

        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            return ReadByParts(_objects.Values, 5000, PeriodBegin, PeriodEnd, false);
        }
    }
}
