using System.Collections.Generic;
using BaseLibrary;
using Calculation;

namespace Tablik
{
    //Модуль для компиляции
    public class TablikModule : DataModule, ICalcParamNode
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

        //Словарь расчетных параметров, ключи - коды, содержит только отмеченные и без грубых ошибок
        private readonly DicS<TablikParam> _params = new DicS<TablikParam>();
        public DicS<TablikParam> Params { get { return _params; } }
        //Словарь всех расчетных параметров, ключи - коды
        private readonly DicS<TablikParam> _paramsAll = new DicS<TablikParam>();
        public DicS<TablikParam> ParamsAll { get { return _paramsAll; } }
        //Словарь расчетных параметров, ключи - Id, содержит все параметры
        private readonly DicI<TablikParam> _paramsId = new DicI<TablikParam>();
        public DicI<TablikParam> ParamsId { get { return _paramsId; } }

        //Загрузить список параметров
        public void LoadParams()
        {
            
        }
    }
}