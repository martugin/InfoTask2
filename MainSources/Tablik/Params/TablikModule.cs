using System;
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

        //Флаг для построения графа зависимости модулей
        public DfsStatus DfsStatus { get; set; }

        //Словарь расчетных параметров, ключи - коды, содержит только отмеченные и без грубых ошибок
        private readonly DicS<TablikParam> _params = new DicS<TablikParam>();
        public DicS<TablikParam> Params { get { return _params; } }
        //Словарь всех расчетных параметров, ключи - коды
        private readonly DicS<TablikParam> _paramsAll = new DicS<TablikParam>();
        public DicS<TablikParam> ParamsAll { get { return _paramsAll; } }
        //Словарь расчетных параметров, ключи - Id, содержит все параметры
        private readonly DicI<TablikParam> _paramsId = new DicI<TablikParam>();
        public DicI<TablikParam> ParamsId { get { return _paramsId; } }

        //Список параметров в порядке расчета
        private readonly List<TablikParam> _paramsOrder = new List<TablikParam>();
        public List<TablikParam> ParamsOrder { get { return _paramsOrder; }}

        //Компиляция модуля
        public void Compile()
        {
            LoadParams();
            Parse();
            MakeParamsGraph();
            DefineDataTypes();
            SaveCompile();
        }

        //Загрузить список параметров
        private void LoadParams()
        {
            AddEvent("Загрузка расчетных параметров");
            using (var db = new DaoDb(Dir + "CalParams.accdb")) 
            { 
                LoadPars(new DaoRec(db, "CalcParams"));
                AddEvent("Загрузка расчетных подпараметров");
                LoadSubPars(new DaoRec(db, "CalcSubParams"));
            }
            AddEvent("Загрузка сгенерированных параметров");
            using (var db = new DaoDb(Dir + "Compiled.accdb"))
            {
                LoadPars(new DaoRec(db, "GeneratedParams"));
                AddEvent("Загрузка сгенерированных подпараметров");
                LoadSubPars(new DaoRec(db, "GeneratedSubParams"));
            }
        }
        
        //Загрузить параметры из таблицы
        private void LoadPars(DaoRec rec)
        {
            using (rec)
                while (rec.Read())
                {
                    var par = new TablikParam(this, rec, false, false);
                    ParamsId.Add(par.ParamId, par);
                    ParamsAll.Add(par.Code, par);
                    if (par.CalcOn && !par.IsFatalError)
                        Params.Add(par.Code, par);
                }
        }

        //Загрузить подпараметры из таблицы
        private void LoadSubPars(DaoRec rec)
        {
            using (rec)
                while (rec.Read())
                {
                    var par = new TablikParam(this, rec, true, false);
                    var opar = ParamsId[par.OwnerId];
                    opar.ParamsId.Add(par.ParamId, par);
                    opar.ParamsAll.Add(par.Code, par);
                    if (par.CalcOn && !par.IsFatalError)
                        opar.Params.Add(par.Code, par);
                }   
        }

        //Синтаксический анализ всех параметров
        private void Parse()
        {
            foreach (var par in ParamsId.Values)
            {
                par.Parse();
                foreach (var spar in par.ParamsId.Values)
                    spar.Parse();
            }
        }

        //Построение графа зависимости параметров
        private void MakeParamsGraph()
        {
            throw new NotImplementedException();
        }

        //Определение типов данных и формирование порожденных параметров
        private void DefineDataTypes()
        {
            foreach (var par in ParamsOrder)
                par.DefineDataTypes();
        }

        //Запись результатов компиляции
        private void SaveCompile()
        {
            throw new NotImplementedException();
        }
    }
}