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
                          string ruleField, //Имя поля с условием генерации
                          string errField, //Имя поля ошибок генерации
                          string idField, //Имя поля Id таблицы, если нет такого, то null
                          string parentIdField = null) //Имя ParentId подтаблицы, если применимо
        {
            IdField = idField;
            ParentIdField = parentIdField;
            ErrField = errField;
            Keeper = new GenKeeper(generator);

            RuleString = rec.GetString(ruleField);
            if (!RuleString.IsEmpty())
            {
                var parse = parentIdField != null  ? new SubRuleParsing(Keeper, ruleField, RuleString) : new RuleParsing(Keeper, ruleField, RuleString);
                Rule = parse.ResultTree;
            }

            var special = new HashSet<string> { ruleField, errField};
            if (!IdField.IsEmpty()) special.Add(IdField);
            if (!ParentIdField.IsEmpty()) special.Add(ParentIdField);
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
        
        //Имена полей Id и ParentId
        public string IdField { get; private set; }
        public string ParentIdField { get; private set; }

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
