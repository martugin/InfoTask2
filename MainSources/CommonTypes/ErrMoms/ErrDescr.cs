using BaseLibrary;

namespace CommonTypes
{
    //Типы источников ошибок
    public enum ErrMomType
    {
        Source, //Ошибка значения исходного сигнала
        Calc //Ошибка при вычислениях
    }

    //-------------------------------------------------------------------------------------------

    //Описание ошибки
    public class ErrDescr
    {
        public ErrDescr(int number, string text, ErrorQuality quality, ErrMomType type)
        {
            Text = text;
            Quality = quality;
            Number = number;
            ErrType = type;
        }

        //Из рекордсета клона и т.п.
        public ErrDescr(IRecordRead rec, ErrMomType type) 
            : this(rec.GetInt("NumError"), rec.GetString("TextError"), (ErrorQuality)rec.GetInt("Quality"), type) { }

        //Сообщение об ошибке
        public string Text { get; private set; }
        //Номер ошибки
        public int Number { get; private set; }
        //Качество ошибки
        public ErrorQuality Quality { get; private set; }
        //Тип источника ошибки
        public ErrMomType ErrType { get; private set; }

        //Запись в рекордсет
        public void ToRecordset(RecDao rec, bool addNew = true)
        {
            if (addNew) rec.AddNew();
            rec.Put("NumError", Number);
            rec.Put("TextError", Text);
            rec.Put("Quality", (int)Quality);
        }
    }
}