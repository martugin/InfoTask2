using System;
using System.Text;

namespace BaseLibrary
{
    //Одна возвращаемая ошибка
    public class ErrorCommand
    {
        internal ErrorCommand(string text, Exception ex = null, string par = "", string context = "", CommandQuality quality = CommandQuality.Error)
        {
            Text = text;
            Params = par;
            Exeption = ex;
            Context = context;
            Quality = quality;
        }

        //Текст сообщения
        public string Text { get; private set; }
        //Ошибка или предупреждение
        public CommandQuality Quality { get; private set; }
        //Исключение, вызвавшее ошибку
        public Exception Exeption { get; private set; }
        //Параметры ошибки
        public string Params { get; private set; }
        //Контекст в котором выполняется комманда (проект, провайдер и т.п.)
        public string Context { get; private set; }

        //Строка для отображения ошибки в сообщениях
        public override string ToString()
        {
            return Text + (Params.IsEmpty() ? "" : (";" + Environment.NewLine + Params));
        }
        //Строка для записи в лог
        public string ToLog()
        {
            if (Exeption == null) return Params ?? "";
            var sb = new StringBuilder();
            if (!Params.IsEmpty())
                sb.Append(Params).Append(";").Append(Environment.NewLine);
            sb.Append(Exeption.Message).Append(";").Append(Environment.NewLine).Append(Exeption.StackTrace);
            return sb.ToString();
        }

        //Сравнение с другой ошибкой
        public bool EqualsTo(ErrorCommand err)
        {
            return Text == err.Text && Params == err.Params && Context == err.Context && Quality == err.Quality;
        }
    }
}
