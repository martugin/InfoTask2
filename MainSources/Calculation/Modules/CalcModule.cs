using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Модуль для расчета
    public class CalcModule : DataModule, IReadConnect
    {
        public CalcModule(DataProject project, string code)
            : base(project, code)
        {
            Project = project;
        }

        //Проект
        public DataProject Project { get; private set; }

        //Список связанных модулей
        private readonly List<CalcModule> _linkedModules = new List<CalcModule>();
        public List<CalcModule> LinkedModules { get { return _linkedModules; } }
        //Список связанных источников
        private readonly List<IReadConnect> _linkedSources = new List<IReadConnect>();
        public List<IReadConnect> LinkedSources { get { return _linkedSources; } }
        //Список связанных приемников
        private readonly List<IWriteConnect> _linkedReceivers = new List<IWriteConnect>();
        public List<IWriteConnect> LinkedReceivers { get { return _linkedReceivers; } }

        //Выходные сигналы
        public IDicSForRead<IReadSignal> ReadingSignals
        {
            get { throw new NotImplementedException(); }
        }
    }
}