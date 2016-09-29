using System;
using System.Collections.Generic;

namespace BaseLibrary
{
    //Обертка над словарем с целочисленными ключами
    public class DicI<T> : IDicIForRead<T>
    {
        public DicI()
        {
        }

        public DicI(T defVal)
        {
            DefVal = defVal;
        }

        //Значение по умолчанию
        public T DefVal { get; set; }

        //Словарь
        private readonly Dictionary<int, T> _dic = new Dictionary<int, T>();
        public Dictionary<int, T> Dic { get { return _dic; } }

        //Получение элемента
        public T Get(int k)
        {
            if (_dic.ContainsKey(k)) return _dic[k];
            return DefVal;
        }
        //Получение элемента по индексу
        public T this[int k]
        {
            get { return Get(k); }
            set { Add(k, value, true); }
        }
        
        //Получение элемента с указанием значения по умолчанию
        public T Get(int k, T defVal)
        {
            if (_dic.ContainsKey(k)) return _dic[k];
            return defVal;
        }

        //Добавление элемента, replace - заменять, если ключи совпадают
        public T Add(int k, T v, bool replace = false)
        {
            if (!_dic.ContainsKey(k))
            {
                _dic.Add(k, v);
                return v;
            }
            if (replace) _dic[k] = v;
            return _dic[k];
        }

        //Содержит ключ
        public bool ContainsKey(int k)
        {
            return _dic.ContainsKey(k);
        }
        //Содержит значение
        public bool ContainsValue(T v)
        {
            return _dic.ContainsValue(v);
        }

        //Удалить элемент
        public bool Remove(int k)
        {
            if (_dic.ContainsKey(k)) return _dic.Remove(k);
            return false;
        }
        
        //Удаляет элемены по заданной функции условия
        public void Remove(Func<int, T, bool> cond)
        {
            var rset = new HashSet<int>();
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
        public ICollection<int> Keys { get { return _dic.Keys; }}
        //Коллекция значений словаря
        public ICollection<T> Values { get { return _dic.Values; } } 
        
        //Добавляет все элементы из другого словаря, replace - заменять если ключи совпадают
        public void AddDic(DicI<T> dicI2, bool replace = true)
        {
            foreach (var d in dicI2.Dic)
                Add(d.Key, d.Value, replace);
        }
    }
}