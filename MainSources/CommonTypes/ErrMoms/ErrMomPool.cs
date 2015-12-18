using System.Collections.Generic;
using System.Linq;
using BaseLibrary;

namespace CommonTypes
{
    //Хранилище ошибок
    public class ErrMomPool
    {
        public ErrMomPool(IErrMomFactory factory)
        {
            _factory = factory;
        }

        //Фабрика описаний ошибок
        private readonly IErrMomFactory _factory;

        //Двухуровневый словарь ошибок, первый ключ - номер ошибки, второй ключ - адрес ошибки
        private readonly DicI<Dictionary<IContextable, ErrMom>> _errs = new DicI<Dictionary<IContextable, ErrMom>>();

        //Создает ErrMom или берет уже существующий
        public ErrMom MakeError(int number, IContextable addr)
        {
            var dic = _errs[number] ?? 
                          _errs.Add(number, new Dictionary<IContextable, ErrMom>());
            if (dic.ContainsKey(addr))
                return dic[addr];
            var errd = _factory.GetDescr(number);
            if (errd == null) return null;
            var err = new ErrMom(errd, addr, _factory);
            dic.Add(addr, err);
            return err;
        }

        //Возвращает список используемых описаний ошибок
        public IEnumerable<ErrDescr> UsedErrors 
        {
            get { return _errs.Values.Select(err => err.Values.First().ErrDescr); }
        }

        //Очищает список используемых описаний ошибок
        public void ClearErrors()
        {
            _errs.Clear();
        }
    }
}