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
    
    //Общие функции для InfoTask и конвертеры 
    public static class DifferentProviders
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
    }
}