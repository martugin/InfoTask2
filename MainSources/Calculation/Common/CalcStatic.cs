using BaseLibrary;

namespace Calculation
{
    public static class CalcStatic
    {
        //Перевод типа функции в строку
        public static string ToString(this FunType f)
        {
            switch (f)
            {
                case FunType.Scalar:
                    return "Scalar";
                case FunType.CalcData:
                    return "CalcData";
                case FunType.Moments:
                    return "Moments";
                case FunType.Operator:
                    return "Operator";
                case FunType.Const:
                    return "Const";
                case FunType.ScalarObject:
                    return "ScalarObject";
                case FunType.Object:
                    return "Object";
                case FunType.ScalarComplex:
                    return "ScalarComplex";
                case FunType.Val:
                    return "Val";
                case FunType.Calc:
                    return "Calc";
            }
            return "Val";
        }

        //Перевод строки в тип функции
        public static FunType ToFunType(this string s)
        {
            if (s.IsEmpty()) return FunType.Val;
            switch (s.ToLower())
            {
                case "scalar":
                    return FunType.Scalar;
                case "calcdata":
                    return FunType.CalcData;
                case "moments":
                    return FunType.Moments;
                case "operator":
                    return FunType.Operator;
                case "const":
                    return FunType.Const;
                case "scalarobject":
                    return FunType.ScalarObject;
                case "object":
                    return FunType.Object;
                case "scalarcomplex":
                    return FunType.ScalarComplex;
                case "val":
                    return FunType.Val;
                case "calc":
                    return FunType.Calc;
            }
            return FunType.Val;
        }
    }
}