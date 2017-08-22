using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Kvint
{
    [Export(typeof(Provider))]
    [ExportMetadata("Code", "KvintSource")]
    public class KvintSource : ListSource
    {
        //Код провайдера
        public override string Code { get { return "KvintSource"; } }

        //Настройки провайдера
        protected override void ReadInf(DicS<string> dic)
        {
            _serverName = dic["ServerName"];
        }
        //Имя сервера, если архив на другом компьютере
        private string _serverName;

        //Список выходов, в каждом по одному сигналу
        internal readonly List<KvintOut> Outs = new List<KvintOut>();

        //Удалить все выходы
        protected override void ClearOuts()
        {
            Outs.Clear();
        }

        //Добавить выход
        protected override ProviderOut AddOut(ProviderSignal sig)
        {
            var kvintOut = new KvintOut(this, sig.Inf["Marka"], sig.Inf["ParamName"], sig.Inf.GetInt("CardId"), sig.Inf.GetInt("ParamNo"));
            Outs.Add(kvintOut);
            return kvintOut;
        }

        //Соединение с параметром
        protected override void ConnectProvider()
        {
            if (!CsApi.Init())
                AddError("Соединение с провадером не установлено");
        }
        //Закрытие соединения
        protected override void DisconnectProvider()
        {
            CsApi.Done();
        }

        //Подготовка провайдера
        protected override void PrepareProvider()
        {
            foreach (var ou in Outs)
                ou.GetHandler(_serverName);
        }

        //Чтение среза
        protected override ValuesCount ReadCut()
        {
            var vc = new ValuesCount();
            foreach (var ou in Outs)
                vc += ou.ReadCut(PeriodBegin);
            return vc;
        }

        //Чтение изменений
        protected override ValuesCount ReadChanges(DateTime beg, DateTime en)
        {
            var vc = new ValuesCount();
            foreach (var ou in Outs)
                vc += ou.ReadChanges(beg, en);
            return vc;
        }
    }
}