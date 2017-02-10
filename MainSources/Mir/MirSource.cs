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
        
        //Словари выходов, ключи коды и IdChanell
        private readonly DicS<OutMir> _outs = new DicS<OutMir>();
        private readonly DicI<OutMir> _outsId = new DicI<OutMir>();
        
        //Очистка списка выходов
        protected override void ClearOuts()
        {
            _outs.Clear();
            _outsId.Clear();
        }

        //Добавить выход в провайдер
        protected override SourceOut AddOut(InitialSignal sig)
        {
            string ocode = sig.Inf.Get("Name_Object") + "." + sig.Inf.Get("Name_Device") + "." + sig.Inf.Get("Name_Type");
            if (!_outs.ContainsKey(ocode))
                return _outs.Add(ocode, new OutMir(this));
            return _outs[ocode];
        }
        
        //Подготовка провайдера, чтение значений IDCHANNEL
        protected override void PrepareSource()
        {
            _outsId.Clear();
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
                    if (_outs.ContainsKey(ocode))
                    {
                        var ob = _outs[ocode];
                        ob.IdChannel = id;
                        _outsId.Add(id, ob);
                    }
                }
        }

        //Запрос значений по одному блоку выходов
        protected override IRecordRead QueryValues(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            string queryString = "SELECT IDCHANNEL, TIME, VALUE, VALUE_UNIT, VALUE_INDICATION FROM IZM_TII" +
                                     " WHERE (TIME >= " + beg.ToSqlString() + ") AND (TIME <=" + en.ToSqlString() + ") ORDER BY TIME";
            return new ReaderAdo(SqlProps, queryString);
        }

        //Определение текущего считываемого выхода
        protected override SourceOut DefineOut(IRecordRead rec)
        {
            return _outsId[rec.GetInt("IDCHANNEL")];
        }

        //Чтение среза
        protected override ValuesCount ReadCut()
        {
            return ReadWhole(_outs.Values, PeriodBegin.AddMinutes(-30), PeriodBegin, true);
        }

        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            return ReadWhole(_outs.Values);
        }
    }
}
