using System;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;
using Generator;

namespace ComClients
{
    //Клиент работы с функциями InfoTask, написанными на C#, вызываемыми из внешних приложений через COM
    public class InfoTaskClient : ProvidersClient
    {
        public string GenerateParams(string moduleDir)
        {
            using (Logger.StartLog("Генерация параметров", moduleDir))
            {
                try
                {
                    var dir = moduleDir.EndsWith("\\") ? moduleDir : moduleDir + "\\";
                    var table = new GenTemplateTable("GenParams", "GenRule", "ErrMess", "CalcOn", "ParamId");
                    var subTable = new GenTemplateTable("GenSubParams", table, "GenRule", "ErrMess", "CalcOn", "SubParamId", "ParamId");
                    var dataTabls = new TablsList();
                    Logger.AddEvent("Загрузка структуры исходных таблиц", dir + "Tables.accdb");
                    using (var db = new DaoDb(dir + "Tables.accdb"))
                    {
                        dataTabls.AddDbStructs(db);
                        Logger.AddEvent("Загрузка значений из исходных таблиц");
                        dataTabls.LoadValues(db, true);
                    }
                    Logger.AddEvent("Загрузка и проверка генерирующих параметров");
                    var generator = new TablGenerator(Logger, dataTabls, dir + "CalcParams.accdb", table, subTable);
                    Logger.AddEvent("Генерация");
                    generator.Generate(dir + "Compiled.accdb", "GeneratedParams", "GeneratedSubParams");
                    Logger.AddEvent("Генерация завершена", generator.GenErrorsCount + " ошибок");
                    if (generator.GenErrorsCount == 0) return "";
                    return "Шаблон генерации содержит " + generator.GenErrorsCount + " ошибок";    
                }
                catch (Exception ex)
                {
                    Logger.AddError("Ошибка при генерации параметров", ex);
                    return ex.MessageString("Ошибка при генерации параметров");
                }
            }
        }
    }
}
