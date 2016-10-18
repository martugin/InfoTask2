using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;
using Microsoft.Office.Interop.Access.Dao;

namespace Generator
{
    //Базовый класс для ряда таблицы и ряда подтаблицы шаблона генерации
    internal class RowGenBase
    {
        public RowGenBase(TablGenerator generator, //Ссылка на генератор
                          RecDao rec, //Рекордсет таблицы шаблона генерации
                          string idField, //Имя поля Id таблицы или ParentId подтаблицы
                          string ruleField, //Имя поля с условием генерации
                          string errField, //Имя поля ошибок генерации
                          bool isSubTabl) //Генерация  
        {
            IdField = idField;
            ErrField = errField;
            Keeper = new GenKeeper(generator);

            RuleString = rec.GetString(ruleField);
            if (!RuleString.IsEmpty())
            {
                var parse = isSubTabl ? new SubRuleParsing(Keeper, ruleField, RuleString) : new RuleParsing(Keeper, ruleField, RuleString);
                Rule = parse.ResultTree;
            }

            var special = new HashSet<string> { idField, ruleField, errField };
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
                        if (Rule == null || !s.Contains("["))
                            Fields.Add(name, new NodeGenConst(null, s));
                        else
                        {
                            var fparse = new FieldsParsing(Keeper, name, rec.GetString(name));
                            var f = (INodeExpr)fparse.ResultTree;
                            if (f != null) Fields.Add(name, f);
                        }
                    }
                }
            }
        }

        //Условие генерации
        public string RuleString { get; private set; }
        internal INode Rule { get; private set; }

        //Имя поля ошибки генерации
        protected string ErrField { get; private set; }
        //Накопитель ошибок
        public GenKeeper Keeper { get; private set; }
        
        //Имя поля Id таблицы, или ParentId подтаблицы шаблона
        public string IdField { get; private set; }
        
        //Словарь остальных полей таблицы шаблонов
        private readonly DicS<INodeExpr> _fields = new DicS<INodeExpr>();
        protected DicS<INodeExpr> Fields { get { return _fields; } }

        //Сгенерировать значения полей по ряду таблицы
        internal void GenerateFields(SubRows row, RecDao rec)
        {
            foreach (var field in Fields.Keys)
                if (rec.ContainsField(field))
                    rec.PutMean(field, Fields[field].Generate(row));
        }
    }
}