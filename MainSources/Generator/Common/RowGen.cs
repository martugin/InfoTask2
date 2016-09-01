using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;
using Microsoft.Office.Interop.Access.Dao;

namespace Generator
{
    //Ряд подтаблицы с разобранными выражениями для генерации
    internal class SubRowGen
    {
        public SubRowGen(TablsList dataTabls, //Исходные таблицы для генерации
                                    RecDao rec, //Рекордсет таблицы
                                    string idField, //Имя поля Id надтаблицы
                                    string ruleField, //Имя поля с условием генерации
                                    string errField) //Имя поля ошибок генерации
        {
            DataTabls = dataTabls;
            Load(rec, true, idField, ruleField, errField);
        }
        public SubRowGen() {}

        //Исходные таблицы для генерации
        protected TablsList DataTabls { get; set; }

        protected void Load(RecDao rec, //Рекордсет таблицы
                                      bool isSubTabl, //Является рядом подтаблицы
                                      string idField, //Имя поля Id или имя поля Id надтаблицы
                                      string ruleField, //Имя поля с условием генерации
                                      string errField) //Имя поля ошибок генерации
        {
            string err = "";
            try
            {
                IdField = idField;
                Id = rec.GetInt(idField);
                RuleString = rec.GetString(ruleField);
                if (!RuleString.IsEmpty())
                {
                    var special = new HashSet<string> {idField, ruleField, errField ?? ""};
                    var rparse = new RuleParsing(ruleField, RuleString, isSubTabl);
                    TablStructItem row = null;
                    Rule = (INodeTabl) rparse.ResultTree;
                    if (Rule != null) row = Rule.Check(DataTabls);
                    err += rparse.ErrMess;
                    if (row == null) return;

                    foreach (Field field in rec.Recordset.Fields)
                    {
                        var name = field.Name;
                        if (!special.Contains(name) && !rec.IsNull(name))
                        {
                            var dataType = field.Type.ToDataType();
                            if (dataType != DataType.String)
                                Fields.Add(name, new NodeGenConst(rec.GetMean(dataType, name)));
                            else
                            {
                                string s = rec.GetString(name);
                                if (!s.Contains("["))
                                    Fields.Add(name, new NodeGenConst(null, s));
                                else
                                {
                                    var fparse = new FieldsParsing(name, rec.GetString(name));
                                    var f = (INodeExpr) fparse.ResultTree;
                                    if (f != null) f.Check(row);
                                    err += fparse.ErrMess;
                                    Fields.Add(name, f);
                                }
                            }
                        }
                    }
                }
            }
            finally { rec.Put(errField, err);}
        }

        //Имя поля Id таблицы, или ParentId подтаблицы
        public string IdField { get; private set; }
        //Id ряда или Id ряда надтаблицы
        public int Id { get; private set; }
        //Условие генерации
        public string RuleString { get; private set; }
        protected INodeTabl Rule { get; private set; }
        //Словарь полей
        private readonly DicS<INodeExpr> _fields = new DicS<INodeExpr>();
        protected DicS<INodeExpr> Fields { get { return _fields; }}
        
        //Сгенерировать таблицу по данному ряду
        public void Generate(int parentId, //Id сгенерированного ряда родителя
                                       SubRows row, //Текущий исходный ряд в процессе генерации по ряду родителя
                                       RecDao subrec) //Рекордсет генерируемой подтаблицы
        {
            subrec.AddNew();
            subrec.Put(IdField, parentId);
            ProcessFields(row, subrec);
            subrec.Update();
        }

        //Сгенерировать значения полей по ряду таблицы
        protected void ProcessFields(SubRows row, RecDao rec)
        {
            foreach (var field in Fields.Keys)
                if (rec.ContainsField(field))
                {
                    var node = Fields[field];
                    if (node != null)
                        rec.PutMean(field, node.Process(row));
                }
        }
    }

    //-----------------------------------------------------------------------------------------------------------
    //Ряд таблицы с разобранными выражениями для генерации
    internal class RowGen : SubRowGen
    {
        public RowGen(TablsList dataTabls, //Исходные таблицы для генерации
                              RecDao rec, //Рекордсет таблицы
                              string idField, //Имя поля Id
                              string ruleField, //Имя поля с условием генерации
                              string errField) //Имя поля ошибок генерации
        {
            DataTabls = dataTabls;
            Load(rec, true, idField, ruleField, errField);
        }
        
        //Подчиненные ряды подтаблицы
        private List<SubRowGen> _subRows;
        internal List<SubRowGen> SubRows { get { return _subRows ?? (_subRows = new List<SubRowGen>()); } }

        //Сгенерировать таблицу по данному ряду
        public void Generate(TablsList dataTabls, //Список таблиц с данными для генерации
                                       RecDao rec, //Рекордсет генерируемой таблицы
                                       RecDao subrec) //Рекордсет генерируемой подтаблицы
        {
            foreach (var row in Rule.GetInitialRows(dataTabls))
            {
                rec.AddNew();
                int id = rec.GetInt(IdField);
                ProcessFields(row, rec);
                rec.Update();
                foreach (var subRow in SubRows)
                    subRow.Generate(id, row, subrec);
            } 
        }
    }
}