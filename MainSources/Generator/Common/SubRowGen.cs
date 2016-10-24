using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Ряд подтаблицы с разобранными выражениями для генерации
    internal class SubRowGen : RowGenBase
    {
        public SubRowGen(TablGenerator generator, //Ссылка на генератор
                         TablStruct tabl, //Таблица - шаблон генерации
                         RecDao rec, string ruleField, string errField, string idField, string parentIdField)
            : base(generator, rec, ruleField, errField, idField, parentIdField)
        {
            var subTabl = Rule == null ? tabl : ((NodeRSubTabl)Rule).Check(tabl);
            foreach (INodeExpr expr in Fields.Values)
                expr.Check(subTabl);        
            rec.Put(errField, Keeper.ErrMess); 
        }
    }
}