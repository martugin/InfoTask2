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
    //Тип значений сигналов провайдера
    public enum SignalValueType
    {
        Mom, //Одно мгновенное значение
        List, //Список мгновенных значений
        Uniform, //Список мгновенных значений со срезом
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

        //Перевод из строки в SignalValueType
        public static SignalValueType ToSignalValueType(this string t)
        {
            if (t == null) return SignalValueType.Error;
            switch (t.ToLower())
            {
                case "мгновенный":
                case "moment":
                    return SignalValueType.Mom;
                case "архивный":
                case "arhive":
                    return SignalValueType.List;
                
            }
            return SignalValueType.Error;
        }

        //Перевод из SignalValueType в русское имя
        public static string ToRussian(this SignalValueType t)
        {
            switch (t)
            {
                case SignalValueType.Mom:
                    return "Мгновенный";
                case SignalValueType.List:
                    return "Архивный";
            }
            return "Ошибка";
        }

        //Перевод из SignalValueType в английское имя
        public static string ToEnglish(this SignalValueType t)
        {
            switch (t)
            {
                case SignalValueType.Mom:
                    return "Moment";
                case SignalValueType.List:
                    return "Archive";
            }
            return "Ошибка";
        }
    }
}