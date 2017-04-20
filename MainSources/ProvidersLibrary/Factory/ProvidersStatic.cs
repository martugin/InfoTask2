namespace ProvidersLibrary
{
    //Тип значений сигналов
    public enum SignalType
    {
        Mom, //Сигнал истоника, значение - одно мгновенное значение
        List, //Сигнал истоника, значение - список мгновенных значений
        Uniform, //Сигнал истоника, значение - список мгновенных значений со срезом
        Calc, //Расчетный сигнал источника
        Clone, //Сигнал для записи в клон без срезов
        UniformClone, //Сигнал для записи в клон со срезами
        Receiver, //Сигнал приемника
        Error
    }

    //---------------------------------------------------------------------------------------------------------------------------
    //Общие функции для InfoTask и конвертеры 
    public static class ProvidersStatic
    {
        //Перевод из строки в SignalType
        public static SignalType ToSignalType(this string t)
        {
            if (t == null) return SignalType.Error;
            switch (t.ToLower())
            {
                case "mom":
                case "мгновенный":
                    return SignalType.Mom;
                case "list":
                case "список":
                    return SignalType.List;
                case "uniform":
                case "список со срезом":
                    return SignalType.Uniform;
                case "calc":
                case "расчетный":
                    return SignalType.Calc;
                case "receiver":
                case "приемник":
                    return SignalType.Receiver;
            }
            return SignalType.Error;
        }

        //Перевод из SignalType в английское имя
        public static string ToEnglish(this SignalType t)
        {
            switch (t)
            {
                case SignalType.Mom:
                    return "Mom";
                case SignalType.List:
                    return "List";
                case SignalType.Uniform:
                    return "Uniform";
                case SignalType.Calc:
                    return "Calc";
                case SignalType.Receiver:
                    return "Receiver";
            }
            return "Error";
        }

        //Перевод из SignalType в русское имя
        public static string ToRussian(this SignalType t)
        {
            switch (t)
            {
                case SignalType.Mom:
                    return "Мгновенный";
                case SignalType.List:
                    return "Список";
                case SignalType.Uniform:
                    return "Список со срезом";
                case SignalType.Calc:
                    return "Расчетный";
                case SignalType.Receiver:
                    return "Приемник";
            }
            return "Ошибка";
        }
    }
}