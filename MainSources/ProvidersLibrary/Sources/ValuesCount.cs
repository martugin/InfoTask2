namespace ProvidersLibrary
{
    //Состояние чтения значений из источника
    public enum ValuesCountStatus
    {
        Undefined, //Чтение не производилось
        Success, //Чтение прошло успешно
        Partial, //Часть объектов прочиталось успешно, часть нет
        Fail, //Слишком много объектов подряд не прочиталось
        Disconnect //Соединение не было установлено или разорвалось
    }

    //-------------------------------------------------------------------------------------------------------------

    //Результат чтения значений из источника
    public class ValuesCount
    {
        public ValuesCount(ValuesCountStatus status = ValuesCountStatus.Undefined)
        {
            Status = status;
        }
        public ValuesCount(int readCount, int writeCount, ValuesCountStatus status)
        {
            Status = status;
            ReadCount = readCount;
            WriteCount = writeCount;
        }
        
        //Состояние чтения значений
        public ValuesCountStatus Status { get; set; }
        //Количество прочитанных и сформированных значений
        public int ReadCount { get; set; }
        public int WriteCount { get; set; }

        //Не нужно продолжать чтение данных
        public bool IsBad { get { return Status == ValuesCountStatus.Disconnect || Status == ValuesCountStatus.Fail; } }

        //Приписывает значение статусу Disconnect
        public ValuesCount MakeBad()
        {
            Status = ValuesCountStatus.Disconnect;
            return this;
        }

        //Покомпонентное сложение
        public static ValuesCount operator +(ValuesCount vc1, ValuesCount vc2)
        {
            return new ValuesCount(vc1.ReadCount + vc2.ReadCount, vc1.WriteCount + vc2.WriteCount, Add(vc1.Status, vc2.Status));
        }

        //Сложение статусов
        private static ValuesCountStatus Add(ValuesCountStatus s1, ValuesCountStatus s2)
        {
            if (s1 == ValuesCountStatus.Success && s2 == ValuesCountStatus.Success)
                return ValuesCountStatus.Success;
            if (s1 == ValuesCountStatus.Undefined) return s2;
            if (s2 == ValuesCountStatus.Undefined) return s1;
            if (s1 == ValuesCountStatus.Disconnect || s2 == ValuesCountStatus.Disconnect)
                return ValuesCountStatus.Disconnect;
            if (s1 == ValuesCountStatus.Fail && s2 == ValuesCountStatus.Fail)
                return ValuesCountStatus.Fail;
            return ValuesCountStatus.Partial;
        }

        //Строка для записи в историю
        public override string ToString()
        {
            string s = IsBad ? "Неудачное чтение, " : "";
            return s + ReadCount + " значений прочитано, " + WriteCount + " значений сформировано";
        }
    }
}