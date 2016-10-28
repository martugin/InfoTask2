using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Ряд подтаблицы с разобранными выражениями для генерации
    internal class SubRowGen : RowGenBase
    {
        public SubRowGen(TablGenerator generator, //Ссылка на генератор
                         TablStruct dataTabl, //Таблица - шаблон генерации
                         GenTemplateTable table, //Поля таблицы - шаблона генерации   
                         RecDao rec) //Рекордсет таблицы результатов
            : base(generator, table, rec)
        {
            var subTabl = Rule == null ? dataTabl : ((NodeRSubTabl)Rule).Check(dataTabl);
            foreach (var key in Fields.Keys)
            {
                Keeper.SetFieldName(key);
                Fields[key].Check(subTabl);
            }        
            rec.Put(table.ErrField, Keeper.ErrMess); 
        }
    }
}