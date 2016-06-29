﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "SimaticSource")]
    public class SimaticSource : AdoSource
    {
        //Код провайдера
        public override string Code { get { return "SimaticSource"; } }
        //Комплект
        public override string Complect { get { return "Siemens"; } }
        //Создание подключения
        protected override ProviderConnect CreateConnect()
        {
            return new SimaticConnect();
        }
        //Подключение
        internal SimaticConnect Connect { get { return (SimaticConnect)CurConnect; } }

        //Словари сигналов, ключи полные коды и Id
        private readonly DicI<ObjectSimatic> _objectsId = new DicI<ObjectSimatic>();

        //Добавить сигнал в провайдер
        protected override SourceObject AddObject(SourceSignal sig)
        {
            int id = sig.Inf.GetInt("Id");
            if (!_objectsId.ContainsKey(id))
                return _objectsId.Add(id, new ObjectSimatic(this, sig.Inf["Archive"], sig.Inf["Tag"], id));
            return _objectsId[id];
        }
        
        //Очистка списка сигналов
        public override void ClearObjects()
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
        protected override IRecordRead QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en, bool isCut)
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
            return new ReaderAdo(Connect.Connection, sb.ToString());
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            return _objectsId[rec.GetInt(0)];
        }
        
        //Чтение среза
        protected override void ReadCut()
        {
            ReadValuesByParts(_objectsId.Values, 500, PeriodBegin.AddSeconds(-60), PeriodBegin, true);
        }
        //Чтение изменений
        protected override void ReadChanges()
        {
            ReadValuesByParts(_objectsId.Values, 500, PeriodBegin, PeriodEnd, false);
        }
    }
}

