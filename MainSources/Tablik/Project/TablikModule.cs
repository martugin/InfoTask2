using System.Collections.Generic;
using CommonTypes;

namespace Tablik
{
    //Модуль для компиляции
    public class TablikModule : Module
    {
        public TablikModule(TablikProject tablik, string code)
            : base(tablik, code)
        {
            Tablik = tablik;
        }

        //Проект Таблик
        public TablikProject Tablik { get; private set; }

        //Список связанных модулей
        private readonly List<TablikModule> _linkedModules = new List<TablikModule>();
        public List<TablikModule> LinkedModules { get { return _linkedModules; } }
        //Список связанных источников
        private readonly List<TablikSource> _linkedSources = new List<TablikSource>();
        public List<TablikSource> LinkedSources { get { return _linkedSources; } }
        //Список связанных приемников
        private readonly List<TablikReceiver> _linkedReceivers = new List<TablikReceiver>();
        public List<TablikReceiver> LinkedReceivers { get { return _linkedReceivers; } }

        //Добавить связанный модуль
        protected override void AddLinkedModule(string moduleCode)
        {
            LinkedModules.Add(Tablik.Modules[moduleCode]);
        }

        //Добавить связанное соединение
        protected override void AddLinkedConnect(string connectCode)
        {
            if (Tablik.Sources.ContainsKey(connectCode))
                LinkedSources.Add(Tablik.Sources[connectCode]);
            else LinkedReceivers.Add(Tablik.Receivers[connectCode]);
        }
    }
}