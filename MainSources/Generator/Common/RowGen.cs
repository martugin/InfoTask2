using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;
using Microsoft.Office.Interop.Access.Dao;

namespace Generator
{
    //Ряд подтаблицы с разобранными выражениями для генерации
    internal class SubRowGen
    {
        public SubRowGen(RecDao rec, //Рекордсет таблицы
                                    string idField, //Имя поля Id надтаблицы
                                    string ruleField, //Имя поля с условием генерации
                                    string errField) //Имя поля ошибок генерации
        {
            Load(rec, true, idField, ruleField, errField);
        }
        public SubRowGen() {}

        protected void Load(RecDao rec, //Рекордсет таблицы
                                      bool isSubTabl, //Является рядом подтаблицы
                                      string idField, //Имя поля Id или имя поля Id надтаблицы
                                      string ruleField, //Имя поля с условием генерации
                                      string errField) //Имя поля ошибок генерации
        {
            string err = "";
            Id = rec.GetInt(idField);
            RuleString = rec.GetString(ruleField);
            if (!RuleString.IsEmpty())
            {
                var special = new HashSet<string> { idField, ruleField, errField ?? "" };
                var rparse = new RuleParsing(ruleField, RuleString, isSubTabl);
                err += rparse.ErrMess;
                Rule = (NodeIter)rparse.ResultTree;
                foreach (Field field in rec.Recordset.Fields)
                {
                    var name = field.Name;
                    if (!special.Contains(name) && !rec.IsNull(name))
                    {
                        var dataType = field.Type.ToDataType();
                        if (dataType != DataType.String)
                            Fields.Add(name, new NodeConst(rec.GetMean(dataType, name)));
                        else
                        {
                            string s = rec.GetString(name);
                            if (!s.Contains("["))
                                Fields.Add(name, new NodeConst(null, s));
                            else
                            {
                                var fparse = new FieldsParsing(name, rec.GetString(name));
                                err += fparse.ErrMess;
                                Fields.Add(name, (NodeExpr)fparse.ResultTree);
                            }
                        }
                    }
                }
            }
            rec.Put(errField, err);
        }

        //Id ряда или Id ряда надтаблицы
        public int Id { get; private set; }
        //Условие генерации
        public string RuleString { get; private set; }
        protected NodeIter Rule { get; private set; }
        //Словарь полей
        private readonly DicS<NodeExpr> _fields = new DicS<NodeExpr>();
        protected DicS<NodeExpr> Fields { get { return _fields; }}

        //Сгенерировать таблицу по данному ряду
        public void Generate(int parentId, //Id сгенерированного ряда родителя
                                       TablRow row, //Текущий исходный ряд в процессе генерации по ряду родителю
                                       IRecordAdd subrec) //Рекордсет генерируемой подтаблицы
        {
            
        }
    }

    //-----------------------------------------------------------------------------------------------------------
    //Ряд таблицы с разобранными выражениями для генерации
    internal class RowGen : SubRowGen
    {
        public RowGen(RecDao rec, //Рекордсет таблицы
                              string idField, //Имя поля Id
                              string ruleField, //Имя поля с условием генерации
                              string errField) //Имя поля ошибок генерации
        {
            Load(rec, true, idField, ruleField, errField);
        }

        //Подчиненные ряды подтаблицы
        private List<SubRowGen> _subRows;
        internal List<SubRowGen> SubRows { get { return _subRows ?? (_subRows = new List<SubRowGen>()); } }

        //Сгенерировать таблицу по данному ряду
        public void Generate(TablsList dataTabls, //Список таблиц с данными для генерации
                                       IRecordAdd rec, //Рекордсет генерируемой таблицы
                                       IRecordAdd subrec) //Рекордсет генерируемой подтаблицы
        {
                
        }
    }
}