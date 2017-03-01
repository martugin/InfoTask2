using BaseLibrary;

namespace CommonTypes
{
    //Интерфейс для фабрики ошибок
    public interface IErrMomFactory
    {
        //Код источника ошибок
        string ErrSourceCode { get; }
        //Получить описание ошибки по ее номеру
        ErrDescr GetDescr(int number);
        //Текст в случае неопределенной ошибки
        string UndefinedErrorText { get; }
    }

    //-----------------------------------------------------------------------------------------------------
    public class ErrMomFactory : IErrMomFactory
    {
        public ErrMomFactory(string errSourceCode, ErrMomType type)
        {
            ErrSourceCode = errSourceCode;
            ErrMomType = type;
            UndefinedErrorText = "Неопределенная ошибка";
        }

        //Тип источника ошибок
        public ErrMomType ErrMomType { get; private set; }
        //Текст в случае неопределенной ошибки
        public string UndefinedErrorText { get; set; }

        //Код источника ошибок
        public string ErrSourceCode { get; private set; }

        //Получить описание ошибки по ее номеру
        public ErrDescr GetDescr(int number)
        {
            return _errDescrs.ContainsKey(number)
                ? _errDescrs[number]
                : new ErrDescr(number, UndefinedErrorText, ErrQuality.Error, ErrMomType);
        }

        //Словарь описаний ошибок, ключи - номера
        private readonly DicI<ErrDescr> _errDescrs = new DicI<ErrDescr>();
        
        //Добавляет описание ошибки с числовым ключом
        public void AddDescr(int number, string text, ErrQuality quality = ErrQuality.Error)
        {
            if (!_errDescrs.ContainsKey(number))
                _errDescrs.Add(number, new ErrDescr(number, text, quality, ErrMomType));
        }
        public void AddDescr(int number, string text, int quality)
        {
            if (!_errDescrs.ContainsKey(number))
            {
                var q = quality == 2 ? ErrQuality.Error : (quality == 1 ? ErrQuality.Warning : ErrQuality.Good);
                _errDescrs.Add(number, new ErrDescr(number, text, q, ErrMomType));    
            }
        }

        //Добавляет пустое описание с хорошим качеством
        public void AddGoodDescr(int number)
        {
            _errDescrs.Add(number, null);
        }
    }
}