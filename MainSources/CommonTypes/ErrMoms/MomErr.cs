using BaseLibrary;

namespace CommonTypes
{
    //Полная ошибка мгновенного значения
    public class MomErr
    {
        public MomErr(ErrDescr errDescr, IContextable addr)
        {
            ErrDescr = errDescr;
            AddressLink = addr;
        }

        public MomErr(string text, int quality = 2, int num = 0, MomErrType type = MomErrType.Calc)
        {
            ErrDescr = new ErrDescr(num, text, (ErrQuality)quality, type);
        }

        //Ссылка на описание ошибки
        public ErrDescr ErrDescr { get; private set; }
        //Ссылка на объект, породивший ошибку
        public IContextable AddressLink { get; private set; }

        //Строка - адрес происхождения ошибки
        public string Address { get { return AddressLink == null ? "" : AddressLink.Context; } }
        //Сообщение об ошибке
        public string Text { get { return ErrDescr.Text; } }
        //Качество ошибки
        public ErrQuality Quality { get { return ErrDescr.Quality; } }
        //Номер ошибки
        public int Number { get { return ErrDescr.Number; } }
        //Тип источника ошибки
        public MomErrType ErrType { get { return ErrDescr.ErrType; } }
    }
}