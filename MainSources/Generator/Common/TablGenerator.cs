using System;
using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Главный гласс для вызова генерации
    public class TablGenerator : ExternalLogger
    {
        public TablGenerator(Logger logger) 
            : base(logger) { }

        //Генерация расчетных параметров для одного модуля
        public void GenerateParams(string moduleDir) //Каталог модуля                                         
        {
                using (StartLog("Генерация параметров", moduleDir))
                {
                    try
                    {
                        var dir = moduleDir.EndDir();
                        var table = new GenTemplateTable("GenParams", "GenRule", "ErrMess", "CalcOn", "ParamId");
                        var subTable = new GenTemplateTable("GenSubParams", table, "GenRule", "ErrMess", "CalcOn", "SubParamId", "ParamId");
                        var dataTabls = new TablsList();
                        AddEvent("Загрузка структуры исходных таблиц", dir + "Tables.accdb");
                        using (var db = new DaoDb(dir + "Tables.accdb"))
                        {
                            dataTabls.AddDbStructs(db);
                            AddEvent("Загрузка значений из исходных таблиц");
                            dataTabls.LoadValues(db, true);
                        }
                        AddEvent("Загрузка и проверка генерирующих параметров");
                        var generator = new ModuleGenerator(Logger, dataTabls, dir + "CalcParams.accdb", table, subTable);
                        generator.Generate(dir + "Compiled.accdb", "GeneratedParams", "GeneratedSubParams");
                        AddEvent("Генерация завершена", generator.GenErrorsCount + " ошибок");
                        if (generator.GenErrorsCount != 0) ;
                        {
                            SetLogResults(generator.GenErrorsCount + " ошибок");
                            AddCollectResult("Шаблон генерации содержит " + generator.GenErrorsCount + " ошибок");
                        }
                    }
                    catch (Exception ex)
                    {
                        AddError("Ошибка при генерации параметров", ex);
                    }
                }
        }
    }
}