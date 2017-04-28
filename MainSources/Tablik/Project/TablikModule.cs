using System.Collections.Generic;
using Calculation;

namespace Tablik
{
    //Модуль для компиляции
    public class TablikModule : DataModule
    {
        public TablikModule(TablikProject tablik, string code)
            : base(tablik.Project, code)
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
    }
}