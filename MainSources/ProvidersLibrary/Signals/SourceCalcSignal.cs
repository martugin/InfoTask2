using System.Collections.Generic;
using CommonTypes;

namespace ProvidersLibrary
{
    //Функции для расчетных сигналов
    internal enum SignalFunction
    {
        Bit, //Взятие бита
        BitOr, //Взятие битов и сложение по Or
        BitAnd, //Взятие битов и сложение по And
        Average, //Среднее по сегментам
        Summ, //Сумма по сегментам
        First, //Первое по сегментам
        Last, //Последнее по сегментам
        Max, //Максимум по сегментам
        Min //Минимум по сегментам
    }
    
    //---------------------------------------------------------------------------------------------
    
    //Расчетный сигнал
    public class SourceCalcSignal : SourceSignal
    {
        public SourceCalcSignal(Source source, SourceInitSignal initSignal, string code, DataType dataType, string formula) 
            : base(source, code, dataType)
        {
            _initSignal = initSignal;
            ParseFormula(formula);
        }

        //Сигнал, на основе которого вычисляется значение
        private readonly SourceInitSignal _initSignal;

        //Вычисляемая функция
        private SignalFunction _function;
        //Дополнительные параметры
        private readonly List<Mean> _pars = new List<Mean>();

        //Todo Сделать через делегат

        //Разбор формулы заданной прямой польской записью
        private void ParseFormula(string formula)
        {
            var lexemes = formula.Split(';');
            switch (lexemes[0])
            {
                case "Bit":
                    _function = SignalFunction.Bit;
                    break;
                case "BitOr":
                    _function = SignalFunction.BitOr;
                    break;
                case "BitAnd":
                    _function = SignalFunction.BitAnd;
                    break;
                case "Average":
                    _function = SignalFunction.Average;
                    break;
                case "Summ":
                    _function = SignalFunction.Summ;
                    break;
                case "First":
                    _function = SignalFunction.First;
                    break;
                case "Last":
                    _function = SignalFunction.Last;
                    break;
                case "Min":
                    _function = SignalFunction.Min;
                    break;
                case "Max":
                    _function = SignalFunction.Max;
                    break;
            }
            if (_function == SignalFunction.Bit || _function == SignalFunction.BitAnd || _function == SignalFunction.BitOr)
                for (int i = 1; i < lexemes.Length; i++ )
                    _pars.Add(MFactory.NewMean(DataType.Integer, lexemes[i]));
            else _pars.Add(MFactory.NewMean(DataType.Real, lexemes[1]));
        }
    }
}