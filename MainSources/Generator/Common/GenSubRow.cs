using BaseLibrary;
using CompileLibrary;

namespace Generator
{
    //Ряд подтаблицы с разобранными выражениями для генерации
    internal class GenSubRow : GenBaseRow
    {
        public GenSubRow(ModuleGenerator generator, //Ссылка на генератор
                         TablsList dataTabls, //Исходные таблицы для генерации
                         ITablStruct dataTabl, //Таблица - шаблон генерации
                         GenTemplateTable table, //Поля таблицы - шаблона генерации   
                         DaoRec rec) //Рекордсет таблицы результатов
            : base(generator, table, rec)
        {
            var subTabl = Rule == null ? dataTabl : ((INodeRTabl)Rule).Check(dataTabls, dataTabl);
            foreach (var key in Fields.Keys)
            {
                Keeper.SetFieldName(key);
                Fields[key].Check(subTabl);
            }        
            rec.Put(table.ErrField, Keeper.ErrMess); 
        }
    }
}