using Microsoft.Office.Interop.Access.Dao;

namespace BaseLibrary
{
    //Интерфейс для класса, допускающего сохранение в рекордсет RecDao
    public interface ISaveToRecDao
    {
        //Записывает характеристики объекта в рекордсет rec 
        //addnew = true - добавлять новую запись, false - записывать в текущую
        //Возвращает Id счетчика или 0, если счетчика нет
        int ToRecordset(RecDao rec, bool addnew);
    }

    //Работа с сохранением объектов в таблицы
    public partial class DaoDb
    {
        //Сохранение объектов из словаря в таблицу 
        //Старые записи удаляются, только если их нет в словаре, иначе просто обновляются
        public static void SaveDicToTable<T>(string dbFile, //dbFile - файл базы данных
                                                                DicS<T> dic, //Словарь
                                                                string tabl, //Имя таблицы
                                                                string keyField = "Code", //keyField - ключевое поле
                                                                string keyField2 = "", //если ключ по двум полям, то второе ключевое поле
                                                                string delField = "Del") //delField - поле отметки для удаления, 
                                                                where T : ISaveToRecDao
        {
            using (var db = new DaoDb(dbFile))
                db.SaveDicToTable(dic, tabl, keyField, keyField2, delField);
        }

        //То же самое, но база данных не задается
        public void SaveDicToTable<T>(DicS<T> dic, string tabl, string keyField = "Code", string keyField2 = "", string delField = "Del") where T : ISaveToRecDao
        {
            var old = new SetS();
            var add = new SetS();
            using (var rec = new RecDao(this, tabl, RecordsetTypeEnum.dbOpenTable))
                while (rec.Read())
                {
                    var code = rec.GetString(keyField);
                    if (!keyField2.IsEmpty()) code += "." + rec.GetString(keyField2);
                    if (dic.ContainsKey(code))
                        old.Add(code);
                    else rec.Put(delField, true);
                }
            Execute("DELETE * FROM " + tabl + " WHERE " + delField + "=True");

            foreach (var ap in dic.Keys)
                if (!old.Contains(ap)) add.Add(ap);
            using (var rec = new RecDao(this, tabl, RecordsetTypeEnum.dbOpenTable))
            {
                while (rec.Read())
                    dic[rec.GetString(keyField) + (keyField2.IsEmpty() ? "" : ("." + rec.GetString(keyField2)))].ToRecordset(rec, false);
                foreach (var p in add.Keys)
                    dic[p].ToRecordset(rec, true);
            }
        }
    }
}