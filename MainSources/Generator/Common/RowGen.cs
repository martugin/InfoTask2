using System.Collections.Generic;
using Antlr4.Runtime;
using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Ряд таблицы с разобранными выражениями для генерации
    internal class RowGen : RowGenBase
    {
        public RowGen(TablsList dataTabls, //Исходные таблицы для генерации
                              RecDao rec, string idField, string ruleField, string errField, //Рекортсет и поля таблицы
                              RecDao subRec, string subIdField, string subRuleField, string subErrField) //Рекордсет и и поля подтаблицы
            : base(rec, idField, ruleField, errField, false)
        {
            Id = rec.GetInt(idField);
            var tabl = ((INodeTabl)Rule).Check(dataTabls);
            if (tabl == null) return;
            foreach (INodeExpr expr in Fields.Values)
                expr.Check(tabl);
            if (subRec != null)
            {
                bool subErr = false;
                while (subRec.GetInt(subIdField) == Id)
                {
                    var row = new SubRowGen(tabl, subRec, subIdField, subRuleField, subErrField);
                    if (row.Keeper.Errors.Count != 0 && !subErr)
                    {
                        Keeper.AddError("Ошибки в рядах подтаблицы", (IToken)null);
                        subErr = true;
                    }
                    SubRows.Add(row);
                    if (!subRec.MoveNext()) break;
                }
            }
            rec.Put(errField, Keeper.ErrMess); 
        }

        //Id ряда таблицы 
        public int Id { get; private set; }

        //Подчиненные ряды подтаблицы
        private List<SubRowGen> _subRows;
        internal List<SubRowGen> SubRows { get { return _subRows ?? (_subRows = new List<SubRowGen>()); } }

        //Сгенерировать таблицу по данному ряду
        public void Generate(TablsList dataTabls, //Список таблиц с данными для генерации
                                       RecDao rec, //Рекордсет генерируемой таблицы
                                       RecDao subrec) //Рекордсет генерируемой подтаблицы
        {
            foreach (var row in ((INodeTabl)Rule).SelectRows(dataTabls))
            {
                rec.AddNew();
                int id = rec.GetInt(IdField);
                GenerateFields(row, rec);
                rec.Update();
                if (subrec != null)
                    foreach (var subRow in SubRows)
                        subRow.Generate(id, row, subrec);
            }
        }
    }
}