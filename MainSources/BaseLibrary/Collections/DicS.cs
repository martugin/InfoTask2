using System;
using System.Collections.Generic;

namespace BaseLibrary
{
    //Словарь со строковоми ключами без учета регистра
    public class DicS<T> : IDicSForRead<T>
    {
         public DicS()
         {
         }

        public DicS(T defVal)
        {
            DefVal = defVal;
        }

        //Значение по умолчанию
        public T DefVal { get; set; }

        //Словарь, ключи - строки в нижнем регистре
        private readonly Dictionary<string, T> _dic = new Dictionary<string, T>();
        public Dictionary<string, T> Dic { get { return _dic; } }

        //Получение элемента
        private T Get(string s)
        {
            string sl = s.ToUpper();
            if (_dic.ContainsKey(sl)) return _dic[sl];
            return DefVal;
        }
        //Получение элемента по индексу
        public T this[string s]
        {
            get { return Get(s); }
            set { Add(s, value, true); }
        }
        public T Get(string s, T defVal)
        {
            string sl = s.ToUpper();
            if (_dic.ContainsKey(sl)) return _dic[sl];
            return defVal;
        }

        //Добавление элемента, replace - заменять, если ключи совпадают
        //Возвращает элемент, который в итоге содержится в словаре с ключем s
        public T Add(string s, T v, bool replace = false)
        {
            var sl = s.ToUpper();
            if (!_dic.ContainsKey(sl)) _dic.Add(sl, v);
            else if (replace) _dic[sl] = v;
            return Get(s);
        }

        //Содержит элемент
        public bool ContainsKey(string s)
        {
            if (s == null) return false;
            return _dic.ContainsKey(s.ToUpper());
        }

        //Удалить элемент
        public bool Remove(string s)
        {
            string sl = s.ToUpper();
            if (_dic.ContainsKey(sl)) return _dic.Remove(sl);
            return false;
        }
        //Удаляет элемены по заданной функции условия
        public void Remove(Func<string, T, bool> cond)
        {
            var rset = new HashSet<string>();
            foreach (var k in _dic.Keys)
                if (cond(k, _dic[k])) rset.Add(k);
            foreach (var k in rset)
                _dic.Remove(k);
        }

        //Количество элементов
        public int Count { get { return _dic.Count; } }

        //Очистить 
        public void Clear()
        {
            _dic.Clear();
        }

        //Коллекция ключей словаря
        public ICollection<string> Keys { get { return _dic.Keys; }}
        //Коллекция значений словаря
        public ICollection<T> Values { get { return _dic.Values; } } 
        
        //Добавляет все элементы из другого словаря, replace - заменять если ключи совпадают
        public void AddDic(DicS<T> dicI2, bool replace = true)
        {
            foreach (var d in dicI2.Dic)
                Add(d.Key, d.Value, replace);
        }
    }
}