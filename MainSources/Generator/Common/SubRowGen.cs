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
            var subTabl = ((NodeRSubTabl)Rule).Check(tabl);
            foreach (INodeExpr expr in Fields.Values)
                expr.Check(subTabl);    
            rec.Put(errField, Keeper.ErrMess); 
        }

        //ParentId ряда подтаблицы шаблона
        public int ParentId { get; private set; }
        
        //Сгенерировать таблицу по данному ряду
        public void Generate(int parentId, //Id сгенерированного ряда родителя
                             SubRows row, //Текущий исходный ряд в процессе генерации по ряду родителя
                             RecDao subrec) //Рекордсет генерируемой подтаблицы
        {
            subrec.AddNew();
            subrec.Put(IdField, parentId);
            GenerateFields(row, subrec);
            subrec.Update();
        }
    }
}