using System;
using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Расчет скалярных функций, базовый класс для всех классов расчета функций
    public abstract partial class FunctionsBase
    {
        protected FunctionsBase()
        {
            var factory = new ErrMomFactory(ErrSourceCode, ErrMomType.Calc);
            factory.AddGoodDescr(0);
            ErrPool = new ErrMomPool(factory);

            using (var rec = new ReaderAdo(DifferentIt.InfoTaskDir() + @"General\General.accdb",
                                           "SELECT FunctionsOverloads.*, Functions.Name, Functions.Synonym, Functions.Code, Functions.CalcType FROM Functions INNER JOIN FunctionsOverloads ON Functions.Id = FunctionsOverloads.FunctionId " +
                                           "WHERE " + FunsWhereCondition + " ORDER BY FunctionsOverloads.FunctionId, FunctionsOverloads.RunNumber"))
                while (rec.Read())
                {
                    string name = rec.GetString("Name");
                    string synonym = rec.GetString("Synonym");
                    if (!synonym.IsEmpty())
                        name += " (" + synonym + ")";
                    string code = (rec.GetString("Code") ?? (synonym ?? name)) + "_";
                    for (int i = 1; i <= 9 && rec.GetString("Operand" + i) != null; i++)
                        code += rec.GetString("Operand" + i).ToDataType().ToLetter();
                    for (int i = 1; i <= 2 && rec.GetString("More" + i) != null; i++)
                        code += rec.GetString("More" + i).ToDataType().ToLetter();
                    var errNum = rec.GetInt("IdOverload") * 10;
                    var ftype = rec.GetString("CalcType");
                    Funs.Add(code, CreateFun(code, ftype, errNum));
                    factory.AddDescr(errNum, "Недопустимые параметры функции " + name);
                }
        }

        //Условие выбора функций
        protected abstract string FunsWhereCondition { get; }

        //Создание расчетной функции
        protected abstract CalcBaseFun CreateFun(string code, //Код реализации функции
                                                                  string ftype, //Тип делегата функции
                                                                  int errNum); //Номер ошибки

        //Словарь перегрузок функций, ключи - коды реализаций
        private readonly DicS<CalcBaseFun> _funs = new DicS<CalcBaseFun>();
        public DicS<CalcBaseFun> Funs { get { return _funs; } }

        //Пул ошибок
        protected ErrMomPool ErrPool { get; private set; }
        //Код источника ошибок
        protected abstract string ErrSourceCode { get; }
        //Текущий контекст ошибки
        protected abstract IContextable Contextable { get; }

        //Текущая вычисляемая функция
        public CalcBaseFun CurFun { get; set; }
        
        //Добавить особую ошибку в заданный MomEdit
        public void PutErr(MomEdit mom, 
                                    string errMess, //Сообщение ошибки
                                    int quality = 2, //Качество ошибки
                                    int errNum = 0) //Номер ошибки среди ошибок для одной функции, число от 0 до 8
        {
            var number = CurFun.ErrorNumber + errNum + 1;
            ((ErrMomFactory) ErrPool.Factory).AddDescr(number, errMess, quality);
            mom.AddError(ErrPool.MakeError(number, Contextable));
        }

        //Добавить обычную ошибку в заданный MomEdit, номер ошибки задается для функции по умолчанию
        public void PutErr(MomEdit mom)
        {
            mom.AddError(ErrPool.MakeError(CurFun.ErrorNumber, Contextable));
        }

        //Добавить ошибку в текущее значение расчета (ScalarRes)
        public void PutErr(string errMess, int quality = 2, int errNum = 0) //Особая ошибка
        {
            PutErr(ScalarRes, errMess, quality, errNum);
        }
        public void PutErr() //По умолчанию для функции
        {
            PutErr(ScalarRes);
        }

        //Вычисляет суммарную ошибку значений
        protected ErrMom MaxErr(IEnumerable<IMean> par)
        {
            ErrMom err = null;
            foreach (var mom in par)
                err = err.Add(mom.Error);
            return err;
        }
        //Вычисляет минимальную ошибку значений
        protected ErrMom MinErr(IEnumerable<IMean> par)
        {
            ErrMom err = null;
            foreach (var p in par)
            {
                if (p.Error == null) return null;
                err = err ?? p.Error;
            }
            return err;
        }

        //Текущий результат скалярной функции
        public MomEdit ScalarRes { get; private set; }
        //Значения разного типа для результата
        private readonly Dictionary<DataType, MomEdit> _scalarResTypes = new Dictionary<DataType, MomEdit>()
            {
                {DataType.Boolean, new MomEdit(DataType.Boolean)},
                {DataType.Integer, new MomEdit(DataType.Integer)},
                {DataType.Real, new MomEdit(DataType.Real)},
                {DataType.Time, new MomEdit(DataType.Time)},
                {DataType.String, new MomEdit(DataType.String)},
                {DataType.Value, new MomEdit(DataType.Value)},
            };
        //Установить тип данных скалярного значения
        public void SetScalarDataType(DataType dtype)
        {
            ScalarRes = _scalarResTypes[dtype];
            ScalarRes.MakeDefault();
        }

        //Обрамление промежуточного вычисления скалярного значения
        public void CalcScalarFun(IMean[] par, //Параметры расчета
                                               Action action) //Действие, выполняющее расчет
        {
            try
            {
                ScalarRes.MakeDefault();
                foreach (var mean in par)
                    ScalarRes.AddError(mean.Error);
                action();
                if (double.IsNaN(ScalarRes.Real))
                    PutErr();
            }
            catch { PutErr(); }
        }
    }
}