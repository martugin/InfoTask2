using System;
using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Генератор
    public class TablGenerator : ExternalLogger
    {
        public TablGenerator(Logger logger, //Логгер
                                         TablsList dataTabls, //Таблицы с данными для генерации
                                         string templateFile, //Файл шаблона генерации
                                         GenTemplateTable table, //Главная таблица шаблона генерации
                                         GenTemplateTable subTable = null) //Подчиненная таблица шаблона генерации
        {
            GenErrorsCount = 0;
            Logger = logger;
            AddEvent("Загрузка списка функций");
            FunsChecker = new FunsChecker(FunsCheckType.Gen);
            Functions = new FunctionsGen();
            
            DataTabls = dataTabls;
            try
            {
                AddEvent("Загрузка таблиц шаблона генерации", templateFile + ", " + table.Name);
                bool hasSub = subTable != null;
                using (var rec = new RecDao(templateFile, table.QueryString))
                    using (var subRec = !hasSub ? null : new RecDao(rec.DaoDb, subTable.QueryString))
                    {
                        if (hasSub && !subRec.EOF) subRec.MoveFirst();
                        while (rec.Read())
                        {
                            var row = new RowGen(this, dataTabls, table, rec, subTable, subRec);
                            GenErrorsCount += row.Keeper.Errors.Count;
                            _rowsGen.Add(row.Id, row);
                        }
                    }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при загрузке шаблона генерации", ex, templateFile);
            }
        }

        //Список функций
        internal FunsChecker FunsChecker { get; private set; }
        internal FunctionsGen Functions { get; private set; }

        //Список таблиц с данными для генерации
        internal TablsList DataTabls { get; private set; }
        //Ряды таблицы с шаблоном генерации
        private readonly DicI<RowGen> _rowsGen = new DicI<RowGen>();

        //Количество ошибок при последней проверке шаблона генерации
        public int GenErrorsCount { get; private set; }

        //Сгенерировать 
        public void Generate(string makedFile, //Файл сгенерированных таблиц
                                       string makedTabl, //Главная сгенерированная таблица
                                       string makedSubTabl = null) //Подчиненная сгенерированная таблица
        {
            try
            {
                AddEvent("Открытие рекордсетов для генерации");
                using (var db = new DaoDb(makedFile))
                {
                    if (makedSubTabl != null)
                        db.Execute("DELETE * FROM " + makedSubTabl);
                    db.Execute("DELETE * FROM " + makedTabl);
                    using (var rec = new RecDao(db, makedTabl))
                        using (var subRec = makedSubTabl == null ? null : new RecDao(db, makedSubTabl))
                            foreach (var row in _rowsGen.Values)
                                if (row.Keeper.Errors.Count == 0)
                                {
                                    if (!row.RuleString.IsEmpty())
                                        AddEvent("Генерация данных по шаблону", row.RuleString);
                                    row.Generate(DataTabls, rec, subRec);
                                }    
                }
                
            }
            catch (Exception ex)
            {
                AddError("Ошибка при генерации", ex, makedFile);
            }
        }
    }
}
