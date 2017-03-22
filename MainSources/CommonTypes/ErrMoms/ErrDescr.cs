using BaseLibrary;

namespace CommonTypes
{
    //Типы источников ошибок
    public enum MomErrType
    {
        Source, //Ошибка значения исходного сигнала
        Calc //Ошибка при вычислениях
    }

    //-------------------------------------------------------------------------------------------

    //Описание ошибки
    public class ErrDescr
    {
        public ErrDescr(int number, string text, ErrQuality quality, MomErrType type)
        {
            Text = text;
            Quality = quality;
            Number = number;
            ErrType = type;
        }

        //Из рекордсета клона и т.п.
        public ErrDescr(IRecordRead rec, MomErrType type) 
            : this(rec.GetInt("NumError"), rec.GetString("TextError"), (ErrQuality)rec.GetInt("Quality"), type) { }

        //Сообщение об ошибке
        public string Text { get; private set; }
        //Номер ошибки
        public int Number { get; private set; }
        //Качество ошибки
        public ErrQuality Quality { get; private set; }
        //Тип источника ошибки
        public MomErrType ErrType { get; private set; }

        //Запись в рекордсет
        public void ToRecordset(DaoRec rec, bool addNew = true)
        {
            if (addNew) rec.AddNew();
            rec.Put("ErrNum", Number);
            rec.Put("ErrText", Text);
            rec.Put("Quality", (int)Quality);
        }
    }
}