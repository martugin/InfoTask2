﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Text;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "SimaticSource")]
    public class SimaticSource : OleDbSource
    {
        //Код провайдера
        public override string Code { get { return "SimaticSource"; } }

        //Имя сервера
        private string _serverName;
        //Чтение настроек
        protected override void ReadInf(DicS<string> dic)
        {
            _serverName = dic["SQLServer"];
        }
        //Хэш
        public override string Hash { get { return "SQLServer=" + _serverName; } }

        //Строка соединения
        protected override string ConnectionString
        {
            get
            {
                var list = SqlDb.SqlDatabasesList(_serverName);
                var dbName = "";
                foreach (var db in list)
                    if (db.StartsWith("CC_") && db.EndsWith("R"))
                        dbName = db;
                if (dbName.IsEmpty()) return null;
                SqlDb.Connect(_serverName, dbName).GetSchema();//Проверка
                var dic = new Dictionary<string, string>
                    {
                        {"Provider", "WinCCOLEDBProvider.1"},
                        {"Catalog", dbName},
                        {"Data Source", _serverName}
                    };
                return dic.ToPropertyString();
            }
        }

        //Проверка соединения
        public override bool CheckConnection()
        {
            try
            {
                if (Connect() && Connection.State == ConnectionState.Open)
                {
                    CheckConnectionMessage += "Успешное соединение с архивом";
                    return true;
                }
                CheckConnectionMessage += "Не удалось соединиться с архивом";
            }
            catch { CheckConnectionMessage += "Не удалось соединиться с архивом"; }
            return false;
        }

        //Проверка настроек
        public override string CheckSettings(DicS<string> inf)
        {
            if (!inf.ContainsKey("SQLServer"))
                return "Не указано имя архивного сервера";
            return "";
        }

        //Словари сигналов, ключи полные коды и Id
        private readonly DicI<ObjectSimatic> _objectsId = new DicI<ObjectSimatic>();

        //Добавить объект в провайдер
        protected override SourceObject AddObject(InitialSignal sig)
        {
            int id = sig.Inf.GetInt("Id");
            if (!_objectsId.ContainsKey(id))
                return _objectsId.Add(id, new ObjectSimatic(this, sig.Inf["Archive"], sig.Inf["Tag"], id));
            return _objectsId[id];
        }
        
        //Очистка списка сигналов
        protected override void ClearObjects()
        {
            _objectsId.Clear();
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(Code, ErrMomType.Source) {UndefinedErrorText = "Ошибка"};
            factory.AddGoodDescr(128);
            return factory;
        }

        //Запрос значений из архива по списку сигналов и интервалу
        protected override IRecordRead QueryValues(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("TAG:R, ");
            if (part.Count == 1)
                sb.Append(((ObjectSimatic)part[0]).Id);
            else
            {
                sb.Append("(").Append(((ObjectSimatic) part[0]).Id);
                for (int i = 1; i < part.Count; i++)
                    sb.Append(";").Append(((ObjectSimatic) part[i]).Id);
                sb.Append(";-2)");
            }
            sb.Append(", ").Append(beg.ToSimaticString());
            sb.Append(", ").Append(en.ToSimaticString());
            
            AddEvent("Запрос значений из архива", part.Count + " тегов");
            return new ReaderAdo(Connection, sb.ToString());
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            return _objectsId[rec.GetInt(0)];
        }
        
        //Чтение среза
        protected override ValuesCount ReadCut()
        {
            return ReadValuesByParts(_objectsId.Values, 500, PeriodBegin.AddSeconds(-60), PeriodBegin, true);
        }
        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            return ReadValuesByParts(_objectsId.Values, 500, PeriodBegin, PeriodEnd, false);
        }
    }
}

