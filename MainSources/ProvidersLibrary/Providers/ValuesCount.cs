namespace ProvidersLibrary
{
    //Состояние чтения значений из источника
    public enum VcStatus
    {
        Undefined, //Чтение не производилось
        Success, //Чтение прошло успешно
        Partial, //Часть объектов прочиталось успешно, часть нет
        NoSuccess, //Пока не удалось прочитать ничего
        Fail //Соединение не было установлено или не прочиталось слишком много блоков подряд
    }

    //-------------------------------------------------------------------------------------------------------------

    //Результат чтения значений из источника
    public class ValuesCount
    {
        public ValuesCount(VcStatus status = VcStatus.Undefined)
        {
            Status = status;
        }
        public ValuesCount(int readCount, int writeCount, VcStatus status)
        {
            Status = status;
            ReadCount = readCount;
            WriteCount = writeCount;
        }
        
        //Состояние чтения значений
        public VcStatus Status { get; set; }
        //Количество прочитанных и сформированных значений
        public int ReadCount { get; set; }
        public int WriteCount { get; set; }

        //Не нужно продолжать чтение данных
        public bool IsFail { get { return Status == VcStatus.Fail || Status == VcStatus.NoSuccess; } }

        //Добавить к статусу значение
        public ValuesCount AddStatus(VcStatus addStatus)
        {
            Status = Add(Status, addStatus);
            return this;
        }

        //Покомпонентное сложение
        public static ValuesCount operator +(ValuesCount vc1, ValuesCount vc2)
        {
            return new ValuesCount(vc1.ReadCount + vc2.ReadCount, vc1.WriteCount + vc2.WriteCount, Add(vc1.Status, vc2.Status));
        }

        //Сложение статусов
        private static VcStatus Add(VcStatus s1, VcStatus s2)
        {
            if (s1 == VcStatus.Success && s2 == VcStatus.Success)
                return VcStatus.Success;
            if (s1 == VcStatus.Undefined) return s2;
            if (s2 == VcStatus.Undefined) return s1;
            if (s1 == VcStatus.Fail || s2 == VcStatus.Fail)
                return VcStatus.Fail;
            if (s1 == VcStatus.NoSuccess && s2 == VcStatus.NoSuccess)
                return VcStatus.NoSuccess;
            return VcStatus.Partial;
        }

        //Строка для записи в историю
        public override string ToString()
        {
            string s = IsFail ? "Неудачное чтение, " : "";
            return s + ReadCount + " значений прочитано, " + WriteCount + " значений сформировано";
        }
    }
}