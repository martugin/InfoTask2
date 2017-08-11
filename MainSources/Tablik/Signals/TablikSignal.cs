﻿using BaseLibrary;
using CompileLibrary;

namespace Tablik
{
    //Сигнал из Signals
    internal class TablikSignal : BaseSignal, ITablikSignalType
    {
        public TablikSignal(IRecordRead rec) : base(rec)
        {
            Simple = new SimpleType(DataType);
        }

        //Тип данных - сигнал
        public ITablikSignalType TablikSignalType { get { return this; } }
        //Тип данных - простой
        public SimpleType Simple { get; private set; }

        //Базовые сигналы, ключи - коды
        private readonly DicS<BaseTablikSignal> _baseSignals = new DicS<BaseTablikSignal>();
        public DicS<BaseTablikSignal> BaseSignals { get { return _baseSignals; } }
        
        //Чвляется типом
        public bool LessOrEquals(ITablikType type)
        {
            return Simple.LessOrEquals(type);
        }

        //Запись в строку
        public string ToResString()
        {
            return "{" + Code + "}" + "(" + DataType + ")";
        }
    }
}