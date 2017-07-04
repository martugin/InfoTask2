using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Tablik
{
    //Параметр функции
    internal class FunParam
    {
        public FunParam(DataType dataType, ArrayType arrayType = ArrayType.Single, string defau = null)
        {
            SimpleType = new SimpleType(dataType, arrayType);
            Default = defau;
        }

        //Тип данных, или Error, если в таблице записан нестандартный тип
        public SimpleType SimpleType { get; private set; }
        //Значение по умолчанию или null
        public string Default { get; private set; }
        //True, если участвует в формировании типа результата
        public bool FormResult { get; set; }
    }

    //---------------------------------------------------------------------
    //Одна перегрузка функции
    internal class FunOverload
    {
        //Параметры
        private readonly List<FunParam> _inputs = new List<FunParam>();
        public List<FunParam> Inputs { get { return _inputs; } }
        //Дополнительные параметры
        private readonly List<FunParam> _inputsMore = new List<FunParam>();
        public List<FunParam> InputsMore { get { return _inputsMore; } }
        //Тип результата формируется из типов параметров
        public bool IsCombined { get; private set; }
        //Тип результата (если не Combined и не нестандартный тип)
        public DataType ResultType { get; private set; }
        public ArrayType ArratType { get; private set; }

        //Владелец 
        public FunClass Owner { get; private set; }
        //Номер в списке перегрузок
        public int Number { get; private set; }
        //Код для записи в скомпилированное выражение
        public string Code { get; private set; }

        //Конструктор, на входе рекордсет с таблицей FunctionsOverloads
        public FunOverload(IRecordRead reco, FunClass funClass)
        {
            Owner = funClass;
            Number = reco.GetInt("RunNumber");
            IsCombined = reco.GetBool("IsCombined");
            Code = Owner.Code;
            for (int i = 1; i <= 9; ++i)
            {
                var t = reco.GetString("Operand" + i);
                if (t.IsEmpty()) break;
                var dt = t.ToDataType();
                var at = ArrayType.Single;
                if (i == 1) at = reco.GetString("Operand1Array").ToArrayType();
                Inputs.Add(new FunParam(dt, at, reco.GetString("Default" + i)));
                Code += dt.ToLetter();
            }
            for (int i = 1; i <= 2; ++i)
            {
                var t = reco.GetString("More" + i);
                if (t.IsEmpty()) break;
                InputsMore.Add(new FunParam(t.ToDataType()));
            }

            ArratType = reco.GetString("ResultArray").ToArrayType();
            var s = reco.GetString("Result");
            if (!IsCombined) ResultType = s.ToDataType();
            else
            {
                var p = s.Split('+');
                foreach (string c in p)
                {
                    if (c == "M1") InputsMore[0].FormResult = true;
                    else if (c == "M2") InputsMore[1].FormResult = true;
                    else Inputs[int.Parse(c) - 1].FormResult = true;
                }
            }
        }

        //Проверка соответствия списка значений входным аргументам перегрузки 
        //Возвращает возвращаемый тип данных или null, если не подходит
        public ITablikType Check(ITablikType[] args, //Принимемые аргументы
                                                int startPos = 1) //С какой позиции наинать
        {
            throw new NotImplementedException();
        }
    }

    //---------------------------------------------------------------------
    //Встроенная функция
    internal class FunClass
    {
        //Код для записи в польскую запись
        public string Name { get; private set; }
        //Английский синоним
        public string Synonym { get; private set; }
        //Код для записи в скомпилированное выражение, маленькими буквами
        public string Code { get; private set; }
        //Перегрузки
        private readonly List<FunOverload> _overloads = new List<FunOverload>();
        public List<FunOverload> Overloads { get { return _overloads; } }

        //Конструктор, на входе рекордсет с таблицей Functions
        public FunClass(IRecordRead rec)
        {
            Name = rec.GetString("Name");
            Synonym = rec.GetString("Synonym");
            Code = rec.GetString("Code") ?? (Synonym ?? Name).ToLower();
        }

        //Определение подходящей перегрузки, возвращает перегрузку и тип данных
        public Tuple<FunOverload, ITablikType> DefineOverload(ITablikType[] args, //Принимемые аргументы
                                                                                             int startPos = 1) //С какой позиции наинать
        {
            foreach (var ov in Overloads)
            {
                var t = ov.Check(args, startPos);
                if (t != null) return new Tuple<FunOverload, ITablikType>(ov, t);
            }
            return new Tuple<FunOverload, ITablikType>(null, new SimpleType());
        }
    }
}