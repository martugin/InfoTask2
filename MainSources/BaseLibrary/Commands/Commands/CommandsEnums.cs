namespace BaseLibrary
{
    //Были ли ошибки, при выполнении комманды
    public enum CommandQuality : byte
    {
        Success = 0, //Без ошибок
        Warning = 1, //Команда содержит предупреждения
        Repeat = 2, //Команда содержит ошибки, которые удалось избежать при повторном выполнении
        Error = 3 //Команда содержит ошибки
    }

    //----------------------------------------------------------------------------------
    //Уровень важности безошибочности по отношению к быстроте логгера
    public enum LoggerDangerness : byte
    {
        RealTime = 0, //Поток реального времени
        Single = 1, //Поток выполняет разовую долгую операцию
        Periodic = 2 //Поток выполняет периодические долгие операции
    }

    //----------------------------------------------------------------------------------
    //Строка для записи в лог
    public static class CommandConverters
    {
        public static string ToRussian(this CommandQuality q)
        {
            switch (q)
            {
                case CommandQuality.Error:
                    return "Ошибка";
                case CommandQuality.Warning:
                    return "Предупреждение";
                case CommandQuality.Repeat:
                    return "Повтор";
            }
            return "Успешно";
        }    
    }
}
