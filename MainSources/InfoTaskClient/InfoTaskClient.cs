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
            var dir = moduleDir.EndsWith("\\") ? moduleDir : moduleDir + "\\";
            var table = new GenTemplateTable("GenParams", "GenRule", "ErrMess", "CalcOn", "ParamId");
            var subTable = new GenTemplateTable("GenSubParams", table, "GenRule", "ErrMess", "CalcOn", "SubParamId", "ParamId");
            var dataTabls = new TablsList();
            using (var db = new DaoDb(dir + "Tables.accdb"))
            {
                dataTabls.AddDbStructs(db);
                dataTabls.LoadValues(db);
            }
            var generator = new TablGenerator(Logger, dataTabls, dir + "CalcParams.accdb", table, subTable);
            generator.Generate(dir + "Compiled.accdb", "GeneratedParams", "GeneratedSubParams");
            if (generator.GenErrorsCount == 0) return "";
            return "Шаблон генерации содержит " + generator.GenErrorsCount + " ошибок";
        }
    }
}
