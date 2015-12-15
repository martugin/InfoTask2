 using System;

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

    [Flags]
    public enum CommandFlags : byte
    {
        Simple = 0, //Простая комманда
        Progress = 1, //Команда для отображения индикатора
        Collect = 2, //Команда формирующая сообщение об ошибке
        Danger = 4 //Комманда, обрамляющая опасную операцию
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
