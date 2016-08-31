using System;
using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Генератор
    public class TablGenerator : ExternalLogger
    {
        public TablGenerator(Logger logger, //Логгер
                                         TablsList tabls, //Таблицы с данными для генерации
                                         string file, //Файл шаблона генерации
                                         string tabl, //Главная таблица шаблона генерации, можно запрос
                                         string tablIdField, //Имя поля Id главной таблицы
                                         string tablRuleField, //Имя поля правила генерации главной таблицы
                                         string tablErrField, //Имя поля для записи ошибок генерации в главной таблице
                                         string subTabl = null, //Подчиненная таблицы шаблона генерации, можно запрос
                                         string subTablParentIdField = null, //Имя поля Id из главной таблицы в подчиненной таблице
                                         string subTablRuleField = null, //Имя поля правила генерации подчиненной таблицы
                                         string subTablErrField = null) //Имя поля для записи ошибок генерации в подчиненной таблице
        {
            Logger = logger;
            DataTabls = tabls;
            try
            {
                AddEvent("Загрузка таблицы генерации", file + ", " + tabl);
                using (var rec = new RecDao(file, tabl))
                    while (rec.Read())
                    {
                        var row = new RowGen(rec, tablIdField, tablRuleField, tablErrField);
                        _rowsGen.Add(row.Id, row);
                    }
                if (subTabl != null)
                {
                    AddEvent("Загрузка подтаблицы генерации", file + ", " + subTabl);
                    using (var rec = new RecDao(file, subTabl))
                        while (rec.Read())
                        {
                            var row = new SubRowGen(rec, subTablParentIdField, subTablRuleField, subTablErrField);
                            _rowsGen[row.Id].SubRows.Add(row);
                        }    
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при загрузке шаблона генерации", ex, file);
            }
        }

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
                    using (var subRec = new RecDao(rec.DaoDb, makedSubTabl))
                        foreach (var row in _rowsGen.Values)
                        {
                            if (!row.RuleString.IsEmpty())
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
