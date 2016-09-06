using System;
using System.Reflection;
using BaseLibrary;

namespace CommonTypes
{
    //Одна перегрузка функции
    public class FunCalc
    {
        //Загрузка из рекордсета перегрузок функций
        protected FunCalc(Functions funs, MethodInfo met, string name)
        {
            Code = met.Name;
            Name = name;
            SaveData = false;
            Functions = funs;
        }

        //Ссылка на Functions
        protected Functions Functions { get; private set; }
        
        //Код функции с буквами типов данных параметров
        public string Code { get; private set; }
        //Имя функции, для текста ошибок
        protected string Name { get; private set; }

        //Стандартный номер ошибки
        public int ErrorNumber { get; private set; }

        //Сохранять данные для следующих периодов
        public bool SaveData { get; protected set; }

        
        

        //Обрамление для вычисления значения функции
        public IVal CalculateFun(IVal[] par, FunData data, DataType dataType)
        {
            Functions.CurFun = this;
            try { return Calculate(par, data, dataType); }
            finally { Functions.CurFun = null; }
        }

        //Само вычисление значения функции
        protected abstract IVal Calculate(IVal[] par, FunData data, DataType dataType);

        //Создает функцию нужного типа из рекордсета FunctionsOverload
        public static FunCalc Create(IRecordRead rec, Functions funs)
        {
            string name = rec.GetString("Name");
            string synonym = rec.GetString("Synonym");
            string code = rec.GetString("Code") ?? (synonym ?? name).ToLower();
            for (int i = 1; i < 10 && rec.GetString("Operand" + i) != null; i++)
                code += rec.GetString("Operand" + i).ToDataType().ToLetter();
            FunType ftype = rec.GetString("FunType").ToFunType();
            MethodInfo met = typeof(Functions).GetMethod(code);
            if (met == null) return null;
            FunCalc fun = null;
            switch (ftype)
            {
                case FunType.Scalar:
                    fun = new FunScalar(met, funs);
                    break;
                case FunType.CalcData:
                    fun = new FunCalcData(met, funs);
                    break;
                case FunType.Moments:
                    fun = new FunMoments(met, funs);
                    break;
                case FunType.Const:
                    fun = new FunConst(met, funs);
                    break;
                case FunType.ScalarObject:
                    fun = new FunScalarObject(met, funs);
                    break;
                case FunType.Object:
                    fun = new FunObject(met, funs);
                    break;
                case FunType.Val:
                    fun = new FunVal(met, funs);
                    break;
                case FunType.Calc:
                    fun = new FunCalc(met, funs);
                    break;
                case FunType.ScalarComplex:
                    fun = new FunScalarComplex(met, funs);
                    break;
            }
            if (fun != null)
                fun.ErrorNumber = rec.GetInt("Id");
            return fun;
        }

        //Подготовка списков значений приведенных типов
        protected ICalcVal[] MakeCalcVals(IVal[] par)
        {
            var cpar = new ICalcVal[par.Length];
            for (int i = 0; i < par.Length; i++)
                cpar[i] = par[i].CalcValue;
            return cpar;
        }

        protected IMomentsVal[] MakeMomentsVals(IVal[] par, bool skipFirst = false)
        {
            var cpar = new IMomentsVal[par.Length];
            for (int i = (skipFirst ? 1 : 0); i < par.Length; i++)
                cpar[i] = (IMomentsVal)par[i].CalcValue;
            return cpar;
        }

        protected IMom[] MakeMomVals(IVal[] par, bool skipFirst = false)
        {
            var cpar = new IMom[par.Length];
            for (int i = (skipFirst ? 1 : 0); i < par.Length; i++)
                cpar[i] = ((IMomentsVal)par[i].CalcValue).ToMom;
            return cpar;
        }
    }

    //-----------------------------------------------------------------------------------------------------------
    //Скалярная функция
    internal class FunScalar : FunCalc
    {
        private readonly Functions.ScalarDelegate _scalarDelegate;

        public FunScalar(MethodInfo met, Functions funs)
            : base(met, funs)
        {
            _scalarDelegate = (Functions.ScalarDelegate)Delegate.CreateDelegate(typeof(Functions.ScalarDelegate), funs, met);
        }

        protected override IVal Calculate(IVal[] par, FunData data, DataType dataType)
        {
            return Functions.CalcScalar(dataType, _scalarDelegate, MakeMomentsVals(par));
        }
    }
}