﻿using System.Collections.Generic;
using Antlr4.Runtime;
using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Ряд таблицы с разобранными выражениями для генерации
    internal class RowGen : RowGenBase
    {
        public RowGen(TablGenerator generator, //Ссылка на генератор
                              TablsList dataTabls, //Исходные таблицы для генерации
                              GenTemplateTable table, RecDao rec, //Рекортсет и поля таблицы
                              GenTemplateTable subTable, RecDao subRec) //Рекордсет и и поля подтаблицы
            : base(generator, table, rec)
        {
            Id = rec.GetInt(table.IdField);
            var dataTabl = Rule == null ? null : ((INodeTabl)Rule).Check(dataTabls);
            foreach (string key in Fields.Keys)
            {
                Keeper.SetFieldName(key);
                Fields[key].Check(dataTabl);
            }
            if (subRec != null && !subRec.EOF)
            {
                bool subErr = false;
                while (subRec.GetInt(subTable.ParentIdField) == Id)
                {
                    var row = new SubRowGen(generator, dataTabl, subTable, subRec);
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
        private List<SubRowGen> _subRows;
        internal List<SubRowGen> SubRows { get { return _subRows ?? (_subRows = new List<SubRowGen>()); } }

        //Значение поля Id
        public int Id { get; protected set; }

        //Сгенерировать таблицу по данному ряду
        public void Generate(TablsList dataTabls, //Список таблиц с данными для генерации
                                       RecDao rec, //Рекордсет генерируемой таблицы
                                       RecDao subrec) //Рекордсет генерируемой подтаблицы
        {
            if (!Keeper.ErrMess.IsEmpty()) return;
            if (Rule == null)
            {
                rec.AddNew();
                int id = rec.GetInt(Table.IdField);
                GenerateFields(null, rec);
                rec.Update();
                if (subrec != null)
                    foreach (var subRowGen in SubRows)
                    {
                        subrec.AddNew();
                        subrec.Put(subRowGen.Table.ParentIdField, id);
                        subRowGen.GenerateFields(null, subrec);
                        subrec.Update();
                    }
            }
            else foreach (var row in ((INodeTabl)Rule).SelectRows(dataTabls))
            {
                rec.AddNew();
                int id = rec.GetInt(Table.IdField);
                GenerateFields(row, rec);
                rec.Update();
                if (subrec != null)
                    foreach (var subRowGen in SubRows)
                        foreach (var subRow in (((NodeRSubTabl)subRowGen.Rule).SelectRows(row)))
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