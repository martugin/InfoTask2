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
                                         string file, //Файл шаблона генерации
                                         GenTemplateTable table, //Главная таблица шаблона генерации
                                         GenTemplateTable subTable = null) //Подчиненная таблица шаблона генерации
        {
            Logger = logger;
            AddEvent("Загрузка списка функций");
            FunsChecker = new FunsChecker(FunsCheckType.Gen);
            Functions = new FunctionsGen();

            DataTabls = dataTabls;
            try
            {
                bool hasSub = subTable != null;
                AddEvent("Загрузка таблиц шаблона генерации", file + ", " + table.Name);
                
                using (var rec = new RecDao(file, table.QueryString))
                    using (var subRec = !hasSub ? null : new RecDao(rec.DaoDb, subTable.QueryString))
                    {
                        if (hasSub && !subRec.EOF) subRec.MoveFirst();
                        while (rec.Read())
                        {
                            var row = new RowGen(this, dataTabls, table, rec, subTable, subRec);
                            _rowsGen.Add(row.Id, row);
                        }
                    }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при загрузке шаблона генерации", ex, file);
            }
        }

        //Список функций
        internal FunsChecker FunsChecker { get; private set; }
        internal FunctionsGen Functions { get; private set; }

        //Список таблиц с данными для генерации
        internal TablsList DataTabls { get; private set; }
        //Ряды таблицы с шаблоном генерации
        private readonly DicI<RowGen> _rowsGen = new DicI<RowGen>();
        
        //Сгенерировать 
        public void Generate(string makedFile, //Файл сгенерированных таблиц
                                       string makedTabl, //Главная сгенерированная таблица
                                       string makedSubTabl = null) //Подчиненная сгенерированная таблица
        {
            try
            {
                AddEvent("Открытие рекордсетов для генерации");
                using (var rec = new RecDao(makedFile, makedTabl))
                    using (var subRec = makedSubTabl == null ? null : new RecDao(rec.DaoDb, makedSubTabl))
                        foreach (var row in _rowsGen.Values)
                            if (row.Keeper.Errors.Count == 0)
                            {
                                if (!row.RuleString.IsEmpty() )
                                    AddEvent("Генерация данных по шаблону", row.RuleString);
                                row.Generate(DataTabls, rec, subRec);
                            }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при генерации", ex, makedFile);
            }
        }
    }
}
