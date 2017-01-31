using System.Collections.Generic;
using System.Linq;

namespace BaseLibrary
{
    //Команда, которая копит ошибки, но не выдает из во вне (только в лог)
    public class CommandKeep : Command
    {
        internal CommandKeep(Logger logger, Command parent, double startProcent, double finishProcent) 
            : base(logger, parent, startProcent, finishProcent)
        {
        }

        //Список ошибок
        private readonly List<ErrorCommand> _errors = new List<ErrorCommand>();

        //Добавить ошибку 
        public override void AddError(ErrorCommand err)
        {
            if (Logger.History != null)
                Logger.History.WriteError(err);
            AddQuality(err.Quality);
            _errors.Add(err);
        }

        //Совокупное сообщение об ошибках
        public string ErrorMessage
        {
            get { return _errors.Aggregate("", (current, err) => current + err.ToString()); }
        }

        //Завершение команды
        internal protected override void FinishCommand(string results, bool isBreaked)
        {
            base.FinishCommand(results, isBreaked);
            Logger.CommandKeep = null;
        }
    }
}