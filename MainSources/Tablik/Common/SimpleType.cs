﻿using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    //Простой тип данных
    internal class SimpleType : ITablikType
    {
        public SimpleType(DataType dataType, ArrayType arrayType = ArrayType.Single)
        {
            DataType = dataType;
            ArrayType = arrayType;
        }
        public SimpleType() //Ошибка типа данных
        {
            DataType = DataType.Error;
            ArrayType = ArrayType.Error;
        }

        //Тип данных
        public DataType DataType { get; private set; }
        //Тип массива
        public ArrayType ArrayType { get; private set; }
        //Тип данных - простой
        public SimpleType Simple { get { return this; } }
        //Тип данных - сигнал
        public ITablikSignalType TablikSignalType { get { return null; } }

        //Является типом
        public bool LessOrEquals(ITablikType type)
        {
            if (!(type is SimpleType)) return false;
            return ArrayType == type.Simple.ArrayType && DataType.LessOrEquals(type.DataType);
        }

        //Запись в строку
        public string ToResString()
        {
            if (ArrayType == ArrayType.Single)
                return DataType.ToEnglish();
            return ArrayType.ToEnglish() + "(" + DataType.ToEnglish() + ")";
        }
    }
}