using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Office.Interop.Access.Dao;

namespace BaseLibrary
{
    //База данных Access, проверки, запросы
    public partial class DaoDb : IDisposable
    {
        public DaoDb(string file)
        {
            File = file;
        }
        
        //Путь к файлу
        public string File { get; internal set; }
        //Соединение DAO с базой данных
        public Database Database { get; private set; }
        public DBEngine Engine { get; private set; }
        //Соединение ADO с базой данных
        public OleDbConnection Connection { get; private set; }

        //Установить соединение по Ado или Dao
        public DaoDb ConnectDao()
        {
            if (Database == null)
            {
                if (!new FileInfo(File).Exists)
                    throw new FileNotFoundException("Файл базы данных не найден", File);
                Engine = new DBEngine();
                Database = Engine.OpenDatabase(File);
            }
            return this;
        }
        public DaoDb ConnectAdo()
        {
            if (Connection == null)
            {
                if (!new FileInfo(File).Exists)
                    throw new FileNotFoundException("Файл базы данных не найден", File);
                Connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + File);
                Connection.Open();
            }
            return this;
        }

        public void Dispose()
        {
            try
            {
                if (Connection != null) Connection.Close();
                Connection = null;
            } catch { }
            try
            {
                if (Database != null) Database.Close();
                Database = null;
            } catch { }
            try
            {
                if (Engine != null)
                {
                    Engine.FreeLocks();
                    Engine = null;
                    //GC.Collect();
                }
            } catch {}
        }

        //Выполнить запрос DAO
        public void Execute(string stSql, //строка запроса
                                     object options = null) //опции запроса
        {
            ConnectDao();
            if (options == null) Database.Execute(stSql);
            else Database.Execute(stSql, options);
        }

        //Выполнить запрос ADO
        public void ExecuteAdo(string stSql) //строка запроса
        {
            ConnectAdo();
            new OleDbCommand(stSql, Connection).ExecuteNonQuery();
        }

        //Работа с таблицами

        //Проверка на наличие поля в таблице
        public bool ColumnExists(string tableName, string columnName)
        {
            ConnectDao();
            return Database.TableDefs[tableName].Fields.Cast<Field>().Any(c => c.Name.ToUpper().Equals(columnName.ToUpper()));
        }
        //Проверка на наличие таблицы в БД
        public bool TableExists(string tableName)
        {
            ConnectDao();
            return Database.TableDefs.Cast<TableDef>().Any(t => t.Name.ToUpper().Equals(tableName.ToUpper()));
        }
        //Проверка на наличие индекса в БД
        public bool IndexExists(string tableName, string indexName)
        {
            ConnectDao();
            return Database.TableDefs[tableName].Indexes.Cast<Index>().Any(i => i.Name.ToUpper().Equals(indexName.ToUpper()));
        }

        //Добавление логического поля (если существует, просто навешивание комбобокса на ячейку)
        public void SetColumnBool(string tableName, string columnName, IndexModes indexMode = IndexModes.WithoutChange,
            bool? defaultValue = null)
        {
            if (!ColumnExists(tableName, columnName))
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] YESNO");
            else ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] YESNO");
                
            Dispose();
            ConnectDao();
            try
            {
                var field = Database.TableDefs[tableName].Fields[columnName];
                field.Properties.Append(field.CreateProperty("DisplayControl", 3, 106, false));
            }
            catch {}

            SetColumnIndex(tableName, columnName, indexMode);
            ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] SET DEFAULT " + defaultValue);
        }

        //Добавление поля в таблицу для разных типов данных

        public void SetColumnDouble(string tableName, string columnName, IndexModes indexMode = IndexModes.WithoutChange,
            double? defaultValue = null, bool required = false)
        {
            if (!ColumnExists(tableName, columnName))
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] DOUBLE");
            else ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] DOUBLE");
            
            Dispose();
            SetColumnIndex(tableName, columnName, indexMode);

            ConnectDao();
            var field = Database.TableDefs[tableName].Fields[columnName];
            field.Required = required;
            field.DefaultValue = defaultValue.HasValue ? defaultValue.ToString() : "";
        }

        public void SetColumnLong(string tableName, string columnName, IndexModes indexMode = IndexModes.WithoutChange,
            long? defaultValue = null, bool required = false)
        {
            if (!ColumnExists(tableName, columnName))
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] LONG");
            else ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] LONG");
            
            Dispose();
            SetColumnIndex(tableName, columnName, indexMode);

            ConnectDao();
            var field = Database.TableDefs[tableName].Fields[columnName];
            field.Required = required;
            field.DefaultValue = defaultValue.HasValue ? defaultValue.ToString() : "";
        }

        public void SetColumnString(string tableName, string columnName, int length = 255,
            IndexModes indexMode = IndexModes.WithoutChange, string defaultValue = null, bool required = false, bool emptyStrings = true)
        {
            if (!ColumnExists(tableName, columnName))
            {
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] TEXT(" + length + ")");
                Dispose();
            }
            ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] TEXT(" + length + ") WITH COMPRESSION");
            Dispose();
            SetColumnIndex(tableName, columnName, indexMode);

            ConnectDao();
            var field = Database.TableDefs[tableName].Fields[columnName];
            field.Required = required;
            field.AllowZeroLength = emptyStrings;
            field.DefaultValue = defaultValue ?? "";
        }

        public void SetColumnMemo(string tableName, string columnName, string defaultValue = null, bool required = false, bool emptyStrings = true)
        {
            if (!ColumnExists(tableName, columnName))
            {
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] MEMO");
                Dispose();
            }
            ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] MEMO WITH COMPRESSION");
            Dispose();

            ConnectDao();
            var field = Database.TableDefs[tableName].Fields[columnName];
            field.Required = required;
            field.AllowZeroLength = emptyStrings;
            field.DefaultValue = defaultValue ?? "";
        }

        public void SetColumnDateTime(string tableName, string columnName, IndexModes indexMode = IndexModes.WithoutChange,
            DateTime? defaultValue = null, bool required = false)
        {
            if (!ColumnExists(tableName, columnName))
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] DATETIME");
            else ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] DATETIME");
            Dispose();
            SetColumnIndex(tableName, columnName, indexMode);

            ConnectDao();
            var field = Database.TableDefs[tableName].Fields[columnName];
            field.Required = required;
            field.DefaultValue = defaultValue.HasValue ? defaultValue.ToString() : "";
        }

        //Удаление поля из таблицы
        public void DeleteColumn(string tableName, string columnName)
        {
            if (ColumnExists(tableName, columnName))
                Execute("ALTER TABLE " + tableName + " DROP COLUMN " + columnName);
        }

        //Удаление таблицы из БД
        public void DeleteTable(string tableName)
        {
            if (TableExists(tableName))
                Database.TableDefs.Delete(tableName);
        }

        //Добавление таблицы в БД, как копии другой таблицы
        public void AddTable(string tableName, string sourceTable, bool replace = false)
        {
            if (replace) DeleteTable(tableName);
            if (!TableExists(tableName))
                Execute("SELECT * INTO " + tableName + " FROM " + sourceTable);
        }

        //Переименование таблицы
        public void RenameTable(string tableNameOld, string tableNameNew)
        {
            if (TableExists(tableNameOld))
            {
                ConnectDao();
                Database.TableDefs[tableNameOld].Name = tableNameNew;
            }
        }

        //Добавление параметров в SysTabl
        public void AddSysParam(string templatePath, //Файл с шаблонным SysTabl
                                              string paramName) //Имя параметра
        {
            using (var rec = new RecDao(File, "SELECT ParamId, ParamName FROM SysTabl WHERE ParamName='" + paramName + "'"))
                if (rec.HasRows) return;
            Execute("INSERT INTO SysTabl SELECT ParamName, ParamType, ParamValue, ParamDescription, ParamTag " +
                    "FROM [" + templatePath + "].SysTabl t1 WHERE t1.ParamName='" + paramName + "';");
        }

        //Добавление подпараметров в SysTabl
        public void AddSysSubParam(string templatePath, //Файл с шаблонным SysTabl
                                                    string paramName,  //Имя параметра
                                                    string subParamName) //Имя подпараметра
        {
            int paramId;
            using (var sysTablRs = new RecDao(templatePath, "SELECT ParamId,ParamName FROM SysTabl WHERE ParamName='" + paramName + "'"))
                paramId = sysTablRs.GetInt("ParamId");
            Execute("INSERT INTO SysSubTabl " +
                    "SELECT ParamId, SubParamNum, SubParamName,SubParamType, SubParamValue, SubParamDescription, SubParamTag, SubParamRowSource " +
                    "FROM [" + templatePath + "].SysSubTabl t1 WHERE t1.ParamId=" + paramId + " AND t1.SubParamName='" + subParamName +
                    "' AND NOT EXISTS(SELECT * FROM SysSubTabl t2 WHERE t1.SubParamName = t2.SubParamName)");
        }

        //Добавляет в базу связь многие к одному
        public void AddForeignLink(string tableName, string columnName, string linkedTable, string linkedColumn, bool cascade = true)
        {
            string cascadeS = cascade ? " ON DELETE CASCADE ON UPDATE CASCADE" : "";
            ExecuteAdo("ALTER TABLE " + tableName + " ADD CONSTRAINT " + linkedTable + tableName
                       + " FOREIGN KEY ([" + columnName + "]) REFERENCES " + linkedTable + "(" + linkedColumn + ")" + cascadeS);
        }

        //Добавление индекса по одному полю
        public void SetColumnIndex(string tableName, string columnName, IndexModes indexMode = IndexModes.WithoutChange, string oldIndexName = null)
        {
            //важно, что в случае indexMode = EmptyIndex columnName на самом деле - название индекса, а не поля,
            //т.к. они могут и не совпадать
            switch (indexMode)
            {
                case IndexModes.CommonIndex:
                    if (oldIndexName != null) SetColumnIndex(tableName, oldIndexName, IndexModes.EmptyIndex);
                    if (!IndexExists(tableName, columnName))
                        ExecuteAdo("CREATE INDEX [" + columnName + "] ON " + tableName + " ([" + columnName + "])");
                    break;
                case IndexModes.UniqueIndex:
                    if (oldIndexName != null) SetColumnIndex(tableName, oldIndexName, IndexModes.EmptyIndex);
                    if (!IndexExists(tableName, columnName))
                        ExecuteAdo("CREATE UNIQUE INDEX [" + columnName + "] ON " + tableName + " ([" + columnName + "])");
                    break;
                case IndexModes.EmptyIndex:
                    if (IndexExists(tableName, columnName))
                        ExecuteAdo("DROP INDEX [" + columnName + "] ON " + tableName);
                    break;
            }
        }

        //Добавление индекса по двум полям
        public void SetColumnIndex(string tableName, string columnName, string columnName2, bool primary, IndexModes indexMode = IndexModes.WithoutChange, string oldIndexName = null)
        {
            switch (indexMode)
            {
                case IndexModes.CommonIndex:
                    if (oldIndexName != null) SetColumnIndex(tableName, oldIndexName, IndexModes.EmptyIndex);
                    if (!IndexExists(tableName, columnName))
                        ExecuteAdo("CREATE INDEX [" + columnName + "] ON " + tableName + " ([" + columnName + "], [" + columnName2 + "])" + (primary ? " WITH PRIMARY" : ""));
                    break;
                case IndexModes.UniqueIndex:
                    if (oldIndexName != null) SetColumnIndex(tableName, oldIndexName, IndexModes.EmptyIndex);
                    if (!IndexExists(tableName, columnName))
                        ExecuteAdo("CREATE UNIQUE INDEX [" + (primary ? "PrimaryKey" : columnName) + "] ON " + tableName + " ([" + columnName + "], [" + columnName2 + "])" + (primary ? " WITH PRIMARY" : ""));
                    break;
            }
        }

        //Метод выдает список индексов данной таблицы или сообщает о том, есть ли в ней данный индекс
        public static bool RunOverIndexList(string dbFile, string table, string index = "")
        {
            try
            {
                string indexesS = "";
                bool fieldFinded = false;
                using (var db = new DaoDb(dbFile))
                {
                    db.ConnectDao();
                    foreach (Index ind in db.Database.TableDefs[table].Indexes)
                    {
                        indexesS += ind.Name + Environment.NewLine;
                        if (ind.Name == index)
                            fieldFinded = true;
                    }
                }
                if (index == "") MessageBox.Show(indexesS);
                else return fieldFinded;
                return false;
            }
            catch (Exception)
            { return false; }
        }

        //Статические члены

        //Выполнить запрос 
        public static void Execute(string file, //путь к файлу базы данных
                                               string stSql, //строка запроса
                                               object options = null) //опции запроса
        {
            if (file.IsEmpty() || stSql.IsEmpty())
                throw new NullReferenceException("Файл базы данных и строка запроса не могут быть пустыми или null");
            var en = new DBEngine();
            var db = en.OpenDatabase(file);
            try
            {
                if (options == null) db.Execute(stSql);
                else db.Execute(stSql, options);
            }
            finally
            {
                try { db.Close(); } catch { }
                db = null;
                en = null;
                GC.Collect(); 
            }
        }

        //Выполнить запрос ADO
        public static void ExecuteAdo(string file, //путь к файлу базы данных
                                                     string stSql, //строка запроса
                                                     object options = null) //опции запроса
        {
            if (file.IsEmpty() || stSql.IsEmpty())
                throw new NullReferenceException("Файл базы данных и строка запроса не могут быть пустыми или null");
            using (var connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file))
            {
                connection.Open();
                var command = new OleDbCommand(stSql, connection);
                command.ExecuteNonQuery();
            }
        }

        //Проверяет, что файл file является файлом типа fileType (в SysSubTab параметр FileOptions\FileType)
        //Также производится проверка на содержание в файле списка указанных таблиц tables
        //Возвращает true, если проверка прошла удачно
        public static bool Check(string file, string fileType, IEnumerable<string> tables = null)
        {
            try
            {
                if (file.IsEmpty() || !new FileInfo(file).Exists)
                    return false;
                if (!fileType.IsEmpty())
                    using (var daodb = new DaoDb(file))
                        using (var sys = new SysTabl(daodb))
                            if (sys.SubValue("FileOptions", "FileType") != fileType)
                                return false;
                var en = new DBEngine();
                var db = en.OpenDatabase(file);
                try
                {
                    var missing = new SortedSet<string>();
                    if (tables != null)
                    {
                        foreach (var table in tables)
                            missing.Add(table);
                        foreach (var t in db.TableDefs)
                        {
                            string s = ((TableDef) t).Name;
                            if (missing.Contains(s)) missing.Remove(s);
                        }
                    }
                    return missing.Count == 0;
                }
                finally
                {
                    try
                    {
                        try { db.Close(); } catch { }
                        db = null;
                        en = null;
                        GC.Collect();
                    }
                    catch { }
                }
            }
            catch { return false; }
        }

        //Та же проверка, но без указания типа файла, только по списку таблиц
        public static bool Check(string file, IEnumerable<string> tables = null)
        {
            return Check(file, "", tables);
        }

        //Сжатие базы данных
        public static void Compress(string file, //файл базы
                                                 int size, //размер а байтах, после которого нужно сжимать
                                                 string tmpDir = null, //каталог временных фалов
                                                 int timeout = 0) //время ожидания после сжатия в мс
        {
            if (file.IsEmpty())
                throw new NullReferenceException("Файл сжимаемой базы данных не может быть пустой строкой или null");

            var fdb = new FileInfo(file);
            if (fdb.Length < size) return;
            string sdir = fdb.Directory.FullName;
            if (tmpDir != null)
            {
                var dir = new DirectoryInfo(tmpDir);
                if (!dir.Exists) dir.Create();
                sdir = tmpDir;
            }
            var ftmp = new FileInfo(sdir + @"\Tmp" + fdb.Name);
            if (ftmp.Exists) ftmp.Delete();
            fdb.MoveTo(ftmp.FullName);
            new FileInfo(file).Delete();
            var en = new DBEngine();
            en.CompactDatabase(ftmp.FullName, file);
            en.FreeLocks();
            en = null;
            GC.Collect();
            if (timeout > 0) Thread.Sleep(timeout);
        }

        //Создает новый файл из шаблона, если его еще нет или версия не совпадает с шаблоном
        //Возвращает true, если файл был скопирован
        public static bool FromTemplate(string template, //путь к шаблону
                                                        string file, //путь к создаваемому файлу, 
                                                        ReplaceByTemplate replace, //когда заменять сущесвующий файл
                                                        bool saveOld = false) //копировать старый файл в текущий каталог, добавляя на конце _1, _2 и т. д.
        {
            var f = new FileInfo(file);
            if (!f.Directory.Exists) f.Directory.Create();
            bool needCopy = !f.Exists;
            needCopy |= replace == ReplaceByTemplate.Always;
            if (!needCopy && replace == ReplaceByTemplate.IfNewVersion)
            {
                string st = null, sf = null;
                try { st = SysTabl.SubValueS(template, "AppOptions", "AppVersion"); }  
                catch {}
                try { sf = SysTabl.SubValueS(file, "AppOptions", "AppVersion"); }  
                catch { }
                needCopy |= st != sf;
            }
            if (needCopy)
            {
                if (saveOld && f.Exists)
                {
                    bool b = true;
                    string s = file.Substring(0, file.Length - 6);
                    int i = 1;
                    string ss = "";
                    while (b && i < 10000)
                    {
                        ss = s + "_" + (i++) + ".accdb";
                        b = new FileInfo(ss).Exists;
                    }
                    if (i <= 10000) new FileInfo(file).MoveTo(ss);
                    else new FileInfo(file).Delete();
                    Thread.Sleep(2000);
                }
                new FileInfo(template).CopyTo(file, true);
                Thread.Sleep(500);
            }
            return needCopy;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Как проверять, нужно ли копировать файл из шаблона
    public enum ReplaceByTemplate
    {
        Always,
        IfNewVersion,
        IfNotExists
    }
    
    //Перечисление состояний индексов в SetColumn
    public enum IndexModes
    {
        UniqueIndex, 
        CommonIndex, 
        EmptyIndex, 
        WithoutChange
    }
}
