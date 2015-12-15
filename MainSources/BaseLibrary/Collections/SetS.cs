using System.Collections.Generic;

namespace BaseLibrary
{
    //Множество со строковоми ключами без учета регистра
    public class SetS
    {
        //Словарь, ключи - строки в нижнем регистре, значения - строки
        private readonly Dictionary<string, string> _dic = new Dictionary<string, string>();
        //Коллекция строк в нижнем регистре
        public ICollection<string> Keys { get { return _dic.Keys; } }
        //Коллекция исходных строк
        public ICollection<string> Values { get { return _dic.Values; } }

        //Количество элементов
        public int Count { get { return _dic.Count; } }

        //Очистить 
        public void Clear()
        {
            _dic.Clear();
        }

        //Добавление элемента, replace - заменять, если ключи совпадают
        public void Add(string s, bool replace = false)
        {
            var sl = s.ToUpper();
            if (!_dic.ContainsKey(sl)) _dic.Add(sl, s);
            else if (replace) _dic[sl] = s;
        }

        //Содержит элемент
        public bool Contains(string s)
        {
            return _dic.ContainsKey(s.ToUpper());
        }
        
        //Удалить элемент
        public bool Remove(string s)
        {
            var sl = s.ToUpper();
            if (_dic.ContainsKey(sl)) return _dic.Remove(sl);
            return false;
        }
    }
}