using System.Collections.Generic;
using Antlr4.Runtime;
using BaseLibrary;
using Calculation;

namespace Generator
{
    //Ряд таблицы с разобранными выражениями для генерации
    internal class GenRow : GenBaseRow
    {
        public GenRow(ModuleGenerator generator, //Ссылка на генератор
                              TablsList dataTabls, //Исходные таблицы для генерации
                              GenTemplateTable table, DaoRec rec, //Рекортсет и поля таблицы
                              GenTemplateTable subTable, DaoRec subRec) //Рекордсет и и поля подтаблицы
            : base(generator, table, rec)
        {
            Id = rec.GetInt(table.IdField);
            var dataTabl = Rule == null ? null : ((NodeRTabl)Rule).Check(dataTabls, null);
            foreach (var key in Fields.Keys)
            {
                Keeper.SetFieldName(key);
                Fields[key].Check(dataTabl);
            }
            if (subRec != null && !subRec.EOF)
            {
                Keeper.SetFieldName("");
                bool subErr = false;
                while (subRec.GetInt(subTable.ParentIdField) == Id)
                {
                    var row = new GenSubRow(generator, dataTabls, dataTabl, subTable, subRec);
                    if (row.Keeper.Errors.Count != 0 && !subErr)
                    {
                        Keeper.AddError("Ошибки в рядах подтаблицы", (IToken)null);
                        subErr = true;
                    }
                    SubRows.Add(row);
                    if (!subRec.MoveNext()) break;
                }
            }
            rec.Put(table.ErrField, Keeper.ErrMess); 
        }
        
        //Подчиненные ряды подтаблицы
        private List<GenSubRow> _subRows;
        internal List<GenSubRow> SubRows { get { return _subRows ?? (_subRows = new List<GenSubRow>()); } }

        //Значение поля Id
        public int Id { get; protected set; }

        //Сгенерировать таблицу по данному ряду
        public void Generate(TablsList dataTabls, //Список таблиц с данными для генерации
                                       DaoRec rec, //Рекордсет генерируемой таблицы
                                       DaoRec subrec) //Рекордсет генерируемой подтаблицы
        {
            if (!Keeper.ErrMess.IsEmpty()) return;
            IEnumerable<SubRows> rows = Rule == null ? new SubRows[] { null } : ((NodeRTabl)Rule).SelectRows(dataTabls, null);
            foreach (var row in rows)
            {
                rec.AddNew();
                int id = rec.GetInt(Table.IdField);
                GenerateFields(row, rec);
                rec.Update();
                if (subrec != null)
                    foreach (var subRowGen in SubRows)
                    {
                        IEnumerable<SubRows> subRows = subRowGen.RuleString.IsEmpty() ? new [] { row } : (((INodeRTabl)subRowGen.Rule).SelectRows(dataTabls, row));
                        foreach (var subRow in subRows)
                        {
                            subrec.AddNew();
                            subrec.Put(subRowGen.Table.ParentIdField, id);
                            subRowGen.GenerateFields(subRow, subrec);
                            subrec.Update();
                        }
                    }
            }
        }
    }
}