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
                                         string rulesFile, //Файл шаблона генерации
                                         string rulesTabl, //Главная таблица шаблона генерации, можно запрос
                                         string rulesSubTabl = null) //Подчиненная таблицы шаблона генерации, можно запрос
        {
            Logger = logger;
            DataTabls = tabls;
            try
            {
                AddEvent("Загрузка шаблона генерации", rulesFile);
                var tlist = new TablsList();
                var db = new DaoDb(rulesFile);
                tlist.AddStruct(db, rulesTabl, rulesSubTabl);
                tlist.LoadValues(db);
                Rules = tlist.Tabls[rulesTabl];
            }
            catch (Exception ex)
            {
                AddError("Ошибка при загрузке шаблона генерации", ex, rulesFile);
            }
            try
            {
                AddEvent("Проверка шаблона генерации", rulesFile);

            }
            catch (Exception ex)
            {
                AddError("Ошибка при провурке шаблона генерации", ex, rulesFile);
            }
        }

        //Таблицы шаблона для генерации
        internal Tabl Rules { get; private set; }
        //Список таблиц с данными для генерации
        internal TablsList DataTabls { get; private set; }

        //Сгенерировать 
        public void Generate(string makedFile, //Файл сгенерированных таблиц
                                       string makedTabl, //Главная сгенерированная таблица
                                       string makedSubTabl = null) //Подчиненная сгенерированная таблица
        {
            try
            {
                using (var rec = new RecDao(makedFile, makedTabl))
                    using (var subRec = new RecDao(rec.DaoDb, makedSubTabl))
                    {
                        
                    }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при генерации", ex, makedFile);
            }
        }
    }
}
