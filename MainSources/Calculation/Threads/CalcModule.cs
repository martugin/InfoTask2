using System.Collections.Generic;
using CommonTypes;
using ProvidersLibrary;

namespace Calculation
{
    //Модуль для расчета
    public class CalcModule : BaseModule
    {
        public CalcModule(CalcThread thread, string code)
            : base(thread, code)
        {
            Thread = thread;
        }

        //Поток расчета
        public CalcThread Thread { get; private set; }

        //Список связанных модулей
        private readonly List<CalcModule> _linkedModules = new List<CalcModule>();
        public List<CalcModule> LinkedModules { get { return _linkedModules; } }
        //Список связанных источников
        private readonly List<SourceConnect> _linkedSources = new List<SourceConnect>();
        public List<SourceConnect> LinkedSources { get { return _linkedSources; } }
        //Список связанных приемников
        private readonly List<ReceiverConnect> _linkedReceivers = new List<ReceiverConnect>();
        public List<ReceiverConnect> LinkedReceivers { get { return _linkedReceivers; } }

        //Добавить связанный модуль
        protected override void AddLinkedModule(string moduleCode)
        {
            LinkedModules.Add(Thread.Modules[moduleCode]);
        }

        //Добавить связанное соединение
        protected override void AddLinkedConnect(string connectCode)
        {
            if (Thread.Sources.ContainsKey(connectCode))
                LinkedSources.Add(Thread.Sources[connectCode]);
            else LinkedReceivers.Add(Thread.Receivers[connectCode]);
        }
    }
}