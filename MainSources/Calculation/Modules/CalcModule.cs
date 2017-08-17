using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;
using CompileLibrary;

namespace Calculation
{
    //Модуль для расчета
    public class CalcModule : DataModule, IReadingConnect
    {
        public CalcModule(SchemeProject project, string code)
            : base(project, code) { }

        //Список связанных модулей
        private readonly List<CalcModule> _linkedModules = new List<CalcModule>();
        public List<CalcModule> LinkedModules { get { return _linkedModules; } }
        //Список связанных источников
        private readonly List<IReadingConnect> _linkedSources = new List<IReadingConnect>();
        public List<IReadingConnect> LinkedSources { get { return _linkedSources; } }
        //Список связанных приемников
        private readonly List<IWritingConnect> _linkedReceivers = new List<IWritingConnect>();
        public List<IWritingConnect> LinkedReceivers { get { return _linkedReceivers; } }

        //Выходные сигналы
        public IDicSForRead<IReadSignal> ReadingSignals
        {
            get { throw new NotImplementedException(); }
        }

        //Объекты
        private readonly DicS<CalcObject> _objects = new DicS<CalcObject>();
        internal DicS<CalcObject> Objects { get { return _objects; } }
        //Таблицы
        public TablsList TablsList { get; private set; }
        //Графики

        //Расчетные параметры
        private readonly DicS<CalcParam> _calcParams = new DicS<CalcParam>();
        public DicS<CalcParam> CalcParams { get { return _calcParams; } }
        //Порожденные параметры
        private readonly DicS<DerivedParam> _derivedParams = new DicS<DerivedParam>();
        internal DicS<DerivedParam> DerivedParams { get { return _derivedParams; } }
        
        //Загрузка
        public void Load()
        {
            LoadSignals();
            LoadTabls();
            AddEvent("Загрузка расчетных параметров");
            LoadParams("CalcParams", "Calc");
            AddEvent("Загрузка сгенерированных параметров");
            LoadParams("Compiled", "Generated");
            LoadDerivedParams();
        }

        //Загрузка сигналов
        private void LoadSignals()
        {
            AddEvent("Загрузка объектов");
            using (var db = new DaoDb(Dir + "UsedSignals.accdb"))
            {
                var objectsId = new DicI<CalcObject>();
                using (var rec = new DaoRec(db, "UsedObjects"))
                    while (rec.Read())
                    {
                        var ob = new CalcObject(rec);
                        Objects.Add(ob.Code, ob);
                        objectsId.Add(rec.GetInt("ObjectId"), ob);
                    }

                AddEvent("Загрузка свойств объектов");
                using (var rec = new DaoRec(db, "UsedObjectsProps"))
                    while (rec.Read())
                    {
                        var mean = MFactory.NewMean(rec.GetString("DataType").ToDataType(), rec.GetString("Mean"));
                        objectsId[rec.GetInt("ObjectId")].Props.Add(rec.GetString("CodeProp"), mean);
                    }

                AddEvent("Загрузка сигналов");
                using (var rec = new DaoRec(db, "UsedSignals"))
                    while (rec.Read())
                    {
                                
                    }
            }
        }

        private void LoadTabls()
        {
            AddEvent("Загрузка графиков");
            using (var db = new DaoDb(Dir + "Grafics.accdb"))
            {
                using (var rec = new DaoRec(db, "GraficsList"))
                    while (rec.Read())
                    {
                        
                    }
            }

            AddEvent("Загрузка таблиц");
            using (var db = new DaoDb(Dir + "Tables.accdb"))
            {
                TablsList.AddDbStructs(db);
                TablsList.LoadValues(db, false);
            }
        }

        private void LoadParams(string fileName, string tablName)
        {
            using (var db = new DaoDb(Dir + fileName + ".accdb"))
            {
                var paramsId = new DicI<CalcParam>();
                using (var rec = new DaoRec(db, tablName + "Params"))
                    while (rec.Read())
                    {
                        var cp = new CalcParam(this, rec, false);
                        CalcParams.Add(cp.Code, cp);
                        paramsId.Add(rec.GetInt("ParamId"), cp);
                    }

                using (var rec = new DaoRec(db, tablName + "SubParams"))
                    while (rec.Read())
                    {
                        var cp = new CalcParam(this, rec, true);
                        var owner = paramsId[rec.GetInt("ParamId")];
                        owner.SubParams.Add(cp.Code, cp);
                        cp.Owner = owner;
                    }
            }
        }

        private void LoadDerivedParams()
        {
            AddEvent("Загрузка порожденных параметров");
            using (var rec = new DaoRec(Dir + "Compiled.accdb", "DerivedParams"))
                while (rec.Read())
                {
                    var dp = new DerivedParam(rec);
                    DerivedParams.Add(dp.Code, dp);
                }
            using (var rec = new DaoRec(Dir + "CalcParams.accdb", "DerivedParamsEdit"))
                while (rec.Read())
                    DerivedParams[rec.GetString("FullCode")].LoadEdited(rec);
        }

        //Произвести вычисления
        public void Calculate()
        {
            throw new NotImplementedException();
        }
    }
}