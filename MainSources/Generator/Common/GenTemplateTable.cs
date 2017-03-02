using BaseLibrary;

namespace Generator
{
    //Одна таблица из шаблона генерации
    public class GenTemplateTable
    {
        //Конструктор для главной таблицы
        public GenTemplateTable(string tableName, string ruleField, string errField, string otmField, string idField)
        {
            Name = tableName;
            IsSub = false;
            RuleField = ruleField;
            ErrField = errField;
            OtmField = otmField;
            IdField = idField;
        }

        //Конструктор для подчиненной таблицы
        public GenTemplateTable(string tableName, GenTemplateTable parent, string ruleField, string errField, string otmField, string idField, string parentIdField)
        {
            Name = tableName;
            Parent = parent;
            IsSub = true;
            RuleField = ruleField;
            ErrField = errField;
            OtmField = otmField;
            IdField = idField;
            ParentIdField = parentIdField;
        }

        // Имя таблцы
        public string Name { get; private set; }
        //Таблица родитель
        public GenTemplateTable Parent { get; private set; }
        //Является подтаблицей
        public bool IsSub { get; private set; }

        //Поле условия генерации
        public string RuleField { get; private set; }
        //Поле для записи ошибки генерации
        public string ErrField { get; private set; }
        //Поле отметки включения генерации
        public string OtmField { get; private set; }
        //Поле счетчика текущей таблицы
        public string IdField { get; private set; }
        //Id таблицы-родителя 
        public string ParentIdField { get; private set; }

        //Указанное поле является специальным в данной таблице
        public bool IsSpecial(string field)
        {
            return field == RuleField || field == ErrField || field == OtmField || field == IdField || field == ParentIdField;
        }

        //Возвращает строку запроса для загрузки шаблона генерации
        public string QueryString
        {
            get
            {
                string sql = "";
                if (!IsSub)
                {
                    sql += "SELECT * FROM " + Name;
                    if (!OtmField.IsEmpty()) sql += " WHERE " + OtmField + "=True";
                    if (!IdField.IsEmpty()) sql += " ORDER BY " + IdField;
                    return sql; 
                }
                bool otm = !OtmField.IsEmpty(), otmp = !Parent.OtmField.IsEmpty();
                sql = "SELECT " + Name + ".* FROM ";
                if (!otmp) sql += Name;
                else sql += Parent.Name + " INNER JOIN " + Name + " ON " + Parent.Name + "." + Parent.IdField + "=" + Name + "." + ParentIdField;
                if (otm || otmp) sql += " WHERE ";
                if (otmp) sql += "(" + Parent.Name + "." + Parent.OtmField + "=True)";
                if (otm && otmp) sql += " AND ";
                if (otm) sql += "(" + Name + "." + OtmField + "=True)";
                sql += " ORDER BY " + Name + "." + ParentIdField;
                if (!IdField.IsEmpty()) sql += ", " + Name + "." + IdField;
                return sql;
            }
        }
    }
}