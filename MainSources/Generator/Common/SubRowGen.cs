using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Ряд подтаблицы с разобранными выражениями для генерации
    internal class SubRowGen : RowGenBase
    {
        public SubRowGen(TablGenerator generator, //Ссылка на генератор
                         TablStruct tabl, //Таблица - шаблон генерации
                         RecDao rec, string idField, string ruleField, string errField) 
            : base(generator, rec, idField, ruleField, errField, true)
        {
            ParentId = rec.GetInt(idField);
            var subTabl = Rule == null ? tabl : ((NodeRSubTabl)Rule).Check(tabl);
            foreach (INodeExpr expr in Fields.Values)
                expr.Check(subTabl);        
            rec.Put(errField, Keeper.ErrMess); 
        }

        //ParentId ряда подтаблицы шаблона
        public int ParentId { get; private set; }
    }
}