using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Хранилище ошибок
    public class MomErrPool
    {
        public MomErrPool(IMomErrFactory factory)
        {
            Factory = factory;
        }

        //Фабрика описаний ошибок
        public IMomErrFactory Factory { get; private set; }

        //Двухуровневый словарь ошибок, первый ключ - номер ошибки, второй ключ - адрес ошибки
        private readonly DicI<Dictionary<IContextable, MomErr>> _errs = new DicI<Dictionary<IContextable, MomErr>>();
        //Словарь используемых описаний ошибок
        private readonly DicI<ErrDescr> _usedErrorDescrs = new DicI<ErrDescr>();

        //Создает MomErr или берет уже существующий
        public MomErr MakeError(int number, IContextable addr)
        {
            var errd = Factory.GetDescr(number);
            if (errd == null) return null;
            var dic = _errs[number] ?? 
                          _errs.Add(number, new Dictionary<IContextable, MomErr>());
            if (dic.ContainsKey(addr))
                return dic[addr];
            var err = new MomErr(errd, addr);
            dic.Add(addr, err);
            _usedErrorDescrs.Add(err.Number, err.ErrDescr);
            return err;
        }

        //Возвращает список используемых описаний ошибок
        public IEnumerable<ErrDescr> UsedErrorDescrs 
        {
            get { return _usedErrorDescrs.Values; }
        }

        //Очищает список используемых описаний ошибок
        public void ClearErrors()
        {
            _errs.Clear();
            _usedErrorDescrs.Clear();
        }
    }
}