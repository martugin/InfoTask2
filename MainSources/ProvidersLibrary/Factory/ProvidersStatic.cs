namespace ProvidersLibrary
{
    //Тип провайдера
    public enum ProviderType
    {
        Source,
        Receiver,
        HandInput,
        Archive,
        Error
    }

    //---------------------------------------------------------------------------------------------------------------------------
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
        //Перевод из строки в ProviderType
        public static ProviderType ToProviderType(this string t)
        {
            if (t == null) return ProviderType.Error;
            switch (t.ToLower())
            {
                case "источник":
                case "source":
                    return ProviderType.Source;
                case "ручнойввод":
                case "handinput":
                    return ProviderType.HandInput;
                case "архив":
                case "archive":
                    return ProviderType.Archive;
                case "приемник":
                case "receiver":
                    return ProviderType.Receiver;
            }
            return ProviderType.Error;
        }

        //Перевод из ProviderType в русское имя
        public static string ToRussian(this ProviderType t)
        {
            switch (t)
            {
                case ProviderType.Source:
                    return "Источник";
                case ProviderType.HandInput:
                    return "РучнойВвод";
                case ProviderType.Archive:
                    return "Архив";
                case ProviderType.Receiver:
                    return "Приемник";
            }
            return "Ошибка";
        }

        //Перевод из ProviderType в английское имя
        public static string ToEnglish(this ProviderType t)
        {
            switch (t)
            {
                case ProviderType.Source:
                    return "Source";
                case ProviderType.HandInput:
                    return "HandInput";
                case ProviderType.Archive:
                    return "Archive";
                case ProviderType.Receiver:
                    return "Receiver";
            }
            return "Error";
        }

        //Перевод из строки в SignalType
        public static SignalType ToSignalType(this string t)
        {
            if (t == null) return SignalType.Error;
            switch (t.ToLower())
            {
                case "mom":
                    return SignalType.Mom;
                case "list":
                    return SignalType.List;
                case "uniform":
                    return SignalType.List;
                case "calc":
                    return SignalType.Calc;
                case "receiver":
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
    }
}