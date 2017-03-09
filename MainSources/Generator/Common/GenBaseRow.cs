using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;
using Microsoft.Office.Interop.Access.Dao;

namespace Generator
{
    //Базовый класс для ряда таблицы и ряда подтаблицы шаблона генерации
    internal class GenBaseRow
    {
        public GenBaseRow(ModuleGenerator generator, //Ссылка на генератор
                          GenTemplateTable table, //Описание полей таблицы-шаблона
                          DaoRec rec) //Рекордсет таблицы шаблона генерации
        {
            Table = table;
            Keeper = new GenKeeper(generator);

            RuleString = rec.GetString(Table.RuleField);
            if (!RuleString.IsEmpty())
            {
                var parse = Table.IsSub  
                        ? new SubRuleParsing(Keeper, Table.RuleField, RuleString) 
                        : new RuleParsing(Keeper, Table.RuleField, RuleString);
                Rule = parse.ResultTree;
            }

            foreach (Field field in rec.Recordset.Fields)
            {
                var name = field.Name;
                if (!Table.IsSpecial(name) && !rec.IsNull(name))
                {
                    var dataType = field.Type.ToDataType();
                    if (dataType != DataType.String)
                        Fields.Add(name, new GenConstNode(rec.GetMean(dataType, name)));
                    else
                    {
                        string s = rec.GetString(name);
                        if (!s.Contains("["))
                            Fields.Add(name, new GenConstNode(null, s));
                        else
                        {
                            var fparse = new FieldsParsing(Keeper, name, rec.GetString(name));
                            var f = (IExprNode)fparse.ResultTree;
                            if (f != null) Fields.Add(name, f);
                        }
                    }
                }
            }
        }

        //Описание полей таблицы
        public GenTemplateTable Table { get; private set; }
        //Накопитель ошибок
        public GenKeeper Keeper { get; private set; }

        //Условие генерации
        public string RuleString { get; private set; }
        internal INode Rule { get; private set; }
        
        //Словарь остальных полей таблицы шаблонов
        private readonly Dictionary<string, IExprNode> _fields = new Dictionary<string, IExprNode>();
        protected Dictionary<string, IExprNode> Fields { get { return _fields; } }

        //Сгенерировать значения полей по ряду таблицы
        internal void GenerateFields(SubRows row, DaoRec rec)
        {
            foreach (var field in Fields.Keys)
                if (rec.ContainsField(field))
                    rec.PutMean(field, Fields[field].Generate(row));
        }
    }
}
