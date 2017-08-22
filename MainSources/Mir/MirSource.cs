using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using BaseLibrary;
using ProvidersLibrary;

namespace Mir
{
    [Export(typeof(Provider))]
    [ExportMetadata("Code", "MirSource")]
    public class MirSource : SqlServerSource
    {
        //Код провайдера
        public override string Code { get { return "MirSource"; } }
        
        //Словари выходов, ключи коды и IdChanell
        //Выход с одним и тем же кодом может встречаться несколько раз под разными Id
        internal readonly DicS<MirOut> Outs = new DicS<MirOut>();
        internal readonly DicI<MirOut> OutsId = new DicI<MirOut>();
        
        //Очистка списка выходов
        protected override void ClearOuts()
        {
            Outs.Clear();
            OutsId.Clear();
        }

        //Добавить выход в провайдер
        protected override ProviderOut AddOut(ProviderSignal sig)
        {
            string ocode = sig.Inf.Get("Name_Object") + "." + sig.Inf.Get("Name_Device") + "." + sig.Inf.Get("Name_Type");
            return !Outs.ContainsKey(ocode) 
                ? Outs.Add(ocode, new MirOut(this)) 
                : Outs[ocode];
        }
        
        //Подготовка провайдера, чтение значений IDCHANNEL
        protected override void PrepareProvider()
        {
            OutsId.Clear();
            using (var rec = new AdoReader(SqlProps, "SELECT OBJECTS.NAME_OBJECT, DEVICES.NAME_DEVICE, LIB_CHANNELS.NAME_TYPE, LIB_CHANNELS.UNIT, CHANNELS.IDCHANNEL, LIB_CHANNELS.TABLE_NAME " +
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
                    if (Outs.ContainsKey(ocode))
                    {
                        var ob = Outs[ocode];
                        ob.IdChannel = id;
                        OutsId.Add(id, ob);
                    }
                }
        }

        //Запрос значений по одному блоку выходов
        protected override IRecordRead QueryValues(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            string queryString = "SELECT IDCHANNEL, TIME, VALUE, VALUE_UNIT, VALUE_INDICATION FROM IZM_TII" +
                                     " WHERE (TIME >= " + beg.ToSqlString() + ") AND (TIME <=" + en.ToSqlString() + ") ORDER BY TIME";
            return new AdoReader(SqlProps, queryString);
        }

        //Определение текущего считываемого выхода
        protected override ListSourceOut DefineOut(IRecordRead rec)
        {
            return OutsId[rec.GetInt("IDCHANNEL")];
        }

        //Чтение среза
        protected override ValuesCount ReadCut()
        {
            return ReadWhole(Outs.Values, PeriodBegin.AddMinutes(-30), PeriodBegin, true);
        }

        //Чтение изменений
        protected override ValuesCount ReadChanges(DateTime beg, DateTime en)
        {
            return ReadWhole(Outs.Values, beg, en, false);
        }
    }
}
