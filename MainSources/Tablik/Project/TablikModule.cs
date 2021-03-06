﻿using System.Collections.Generic;
using BaseLibrary;
using CompileLibrary;

namespace Tablik
{
    //Модуль для компиляции 
    internal class TablikModule : DataModule, ISubParams
    {
        public TablikModule(TablikProject tablikProject, string code, string name, string description)
            : base(tablikProject.Project, code)
        {
            TablikProject = tablikProject;
            Name = name;
            Description = description;
        }

        //Проект Таблик
        public TablikProject TablikProject { get; private set; }

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
        private readonly DicS<TablikCalcParam> _params = new DicS<TablikCalcParam>();
        public DicS<TablikCalcParam> Params { get { return _params; } }
        //Словарь всех расчетных параметров, ключи - коды
        private readonly DicS<TablikCalcParam> _paramsAll = new DicS<TablikCalcParam>();
        public DicS<TablikCalcParam> ParamsAll { get { return _paramsAll; } }
        //Словарь расчетных параметров, ключи - Id, содержит все параметры
        private readonly DicI<TablikCalcParam> _paramsId = new DicI<TablikCalcParam>();
        public DicI<TablikCalcParam> ParamsId { get { return _paramsId; } }
        //Словарь расчетных подпараметров, ключи - Id
        private readonly DicI<TablikCalcParam> _subParamsId = new DicI<TablikCalcParam>();
        public DicI<TablikCalcParam> SubParamsId { get { return _subParamsId; } }

        //Список параметров в порядке расчета
        private readonly List<TablikCalcParam> _paramsOrder = new List<TablikCalcParam>();
        public List<TablikCalcParam> ParamsOrder { get { return _paramsOrder; }}
        //Список порожденных параметров
        private readonly List<TablikDerivedParam> _derivedParams = new List<TablikDerivedParam>();
        public List<TablikDerivedParam> DerivedParams { get { return _derivedParams; } }
        //Словарь используемых объектов
        private readonly DicS<TablikObject> _usedObjects = new DicS<TablikObject>();
        public DicS<TablikObject> UsedObjects { get { return _usedObjects; } }

        //Список графиков
        private readonly DicS<Grafic> _grafics = new DicS<Grafic>();
        public DicS<Grafic> Grafics { get { return _grafics; } }
        //Список таблиц
        public TablsList Tabls { get; private set; }
        
        //Компиляция модуля
        public void Compile()
        {
            LoadModule();
            Parse();
            MakeParamsGraph();
            DefineDataTypes();
            MakeDerivedParams();
            SaveCompile();
        }

        //Загрузить список параметров
        internal void LoadModule()
        {
            AddEvent("Загрузка расчетных параметров");
            using (var db = new DaoDb(Dir + "CalcParams.accdb")) 
            {
                using (var rec = new DaoRec(db, "CalcParams"))
                    LoadPars(rec);
                AddEvent("Загрузка расчетных подпараметров");
                using (var rec = new DaoRec(db, "CalcSubParams"))
                    LoadSubPars(rec);
            }
            AddEvent("Загрузка сгенерированных параметров");
            using (var db = new DaoDb(Dir + "Compiled.accdb"))
            {
                using (var rec = new DaoRec(db, "GeneratedParams"))
                    LoadPars(rec);
                AddEvent("Загрузка сгенерированных подпараметров");
                using (var rec = new DaoRec(db, "GeneratedSubParams"))
                    LoadSubPars(rec);
            }
            LoadGrafics();
            LoadTabls();
        }
        
        //Загрузить параметры из таблицы
        private void LoadPars(DaoRec rec)
        {
            while (rec.Read())
            {
                var par = new TablikCalcParam(this, rec, false, false);
                ParamsId.Add(par.ParamId, par);
                ParamsAll.Add(par.Code, par);
                if (par.CalcOn && !par.IsFatalError)
                    Params.Add(par.Code, par);
            }
        }

        //Загрузить подпараметры из таблицы
        private void LoadSubPars(DaoRec rec)
        {
            while (rec.Read())
            {
                var par = new TablikCalcParam(this, rec, true, false);
                SubParamsId.Add(par.ParamId, par);
                var opar = ParamsId[par.OwnerId];
                opar.ParamsId.Add(par.ParamId, par);
                opar.ParamsAll.Add(par.Code, par);
                if (par.CalcOn && !par.IsFatalError)
                    opar.Params.Add(par.Code, par);
            }   
        }

        //Загрузка списка графиков
        private void LoadGrafics()
        {
            AddEvent("Загрузка списка графиков");
            using (var rec = new DaoRec(Dir + "Grafics.accdb", "GraficsList"))
                while (rec.Read())
                {
                    var code = rec.GetString("Code");
                    Grafics.Add(code, new Grafic(code, rec.GetInt("Dimension"), rec.GetString("GraficType").ToGraficType()));
                }
        }

        //Загрузка структур таблиц
        private void LoadTabls()
        {
            AddEvent("Загрузка структур таблиц");
            Tabls = new TablsList();
            using (var db = new DaoDb(Dir + "Tables.accdb"))
                Tabls.AddDbStructs(db);
        }

        //Синтаксический анализ всех параметров
        internal void Parse()
        {
            AddEvent("Синтаксический разбор формул");
            foreach (var par in ParamsId.Values)
            {
                par.Parse();
                foreach (var spar in par.ParamsId.Values)
                    spar.Parse();
            }
        }

        //Построение графа зависимости параметров
        internal void MakeParamsGraph()
        {
            AddEvent("Построение графа параметров");
            foreach (var p in Params.Values)
            {
                p.DfsStatus = DfsStatus.Before;
                foreach (var sp in p.Params.Values)
                    sp.DfsStatus = DfsStatus.Before;
            }
            foreach (var p in Params.Values) 
                if (p.DfsStatus == DfsStatus.Before)
                    p.Dfs(ParamsOrder);
        }

        //Определение типов данных и формирование порожденных параметров
        internal void DefineDataTypes()
        {
            AddEvent("Опредление типов данных");
            foreach (var par in ParamsOrder)
                par.DefineDataTypes();
        }

        //Сформировать порожденные параметры
        internal void MakeDerivedParams()
        {
            DerivedParams.Clear();
            foreach (var par in ParamsOrder)
                if (par.Inputs.Count == 0)
                    par.AddDerivedParams(par.Code, par.Name, par.Task, true);
        }

        //Запись результатов компиляции
        internal void SaveCompile()
        {
            AddEvent("Сохранение используемых сигналов");
            using (var reco = new DaoRec(Dir + "UsedSignals.accdb", "UsedObjects"))
                using (var recs = new DaoRec(reco.DaoDb, "UsedSignals"))
                    using (var recp = new DaoRec(reco.DaoDb, "UsedObjectsProps"))
                        foreach (var ob in UsedObjects.Values)
                            ob.ToRecordsets(reco, recs, recp);
            
            AddEvent("Сохранение скомпилированных расчетных параметров");
            using (var db = new DaoDb(Dir + "CalParams.accdb"))
            {
                using (var rec = new DaoRec(db, "CalcParams"))
                    SavePars(rec);
                AddEvent("Сохранение скомпилированных расчетных подпараметров");
                using (var rec = new DaoRec(db, "CalcSubParams"))
                    SaveSubPars(rec);
            }

            AddEvent("Сохранение скомпилированных сгенерированных параметров");
            using (var db = new DaoDb(Dir + "Compiled.accdb"))
            {
                using (var rec = new DaoRec(db, "GeneratedParams"))
                    SavePars(rec);    
                AddEvent("Сохранение скомпилированных сгенерированных подпараметров");
                using (var rec = new DaoRec(db, "GeneratedSubParams"))
                    SaveSubPars(rec);

                AddEvent("Сохранение порожденных параметров");
                using (var rec = new DaoRec(db, "DerivedParams"))
                    foreach (var dp in DerivedParams)
                        dp.ToRecordset(rec);
            }
        }

        //Сохранить параметры в таблицу
        private void SavePars(DaoRec rec)
        {
            while (rec.Read())
                ParamsId[rec.GetInt("ParamId")].SaveCompileResults(rec);
        }

        //Сохранить подпараметры в таблицу
        private void SaveSubPars(DaoRec rec)
        {
            while (rec.Read())
                SubParamsId[rec.GetInt("SubParamId")].SaveCompileResults(rec);
        }
    }
}