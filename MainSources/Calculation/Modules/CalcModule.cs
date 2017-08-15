using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;
using CompileLibrary;

namespace Calculation
{
    //Модуль для расчета
    public class CalcModule : DataModule, IReadingConnect
    {
        public CalcModule(SchemeProject project, string code)
            : base(project, code) { }

        //Список связанных модулей
        private readonly List<CalcModule> _linkedModules = new List<CalcModule>();
        public List<CalcModule> LinkedModules { get { return _linkedModules; } }
        //Список связанных источников
        private readonly List<IReadingConnect> _linkedSources = new List<IReadingConnect>();
        public List<IReadingConnect> LinkedSources { get { return _linkedSources; } }
        //Список связанных приемников
        private readonly List<IWritingConnect> _linkedReceivers = new List<IWritingConnect>();
        public List<IWritingConnect> LinkedReceivers { get { return _linkedReceivers; } }

        //Выходные сигналы
        public IDicSForRead<IReadSignal> ReadingSignals
        {
            get { throw new NotImplementedException(); }
        }

        //Расчетные параметры
        private readonly DicS<CalcParam> _calcParams = new DicS<CalcParam>();
        public DicS<CalcParam> CalcParams { get { return _calcParams; } }

        //Закрузка расчетных формул
        public void Load()
        {
            throw new NotImplementedException();
        }

        //Произвести вычисления
        public void Calculate()
        {
            throw new NotImplementedException();
        }
    }
}