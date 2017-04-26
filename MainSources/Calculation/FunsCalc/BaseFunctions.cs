using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Расчет скалярных функций, базовый класс для всех классов расчета функций
    public abstract partial class BaseFunctions
    {
        protected BaseFunctions()
        {
            var factory = new MomErrFactory(ErrSourceCode, MomErrType.Calc);
            factory.AddGoodDescr(0);
            ErrPool = new MomErrPool(factory);

            using (var rec = new AdoReader(ItStatic.InfoTaskDir() + @"General\General.accdb",
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
        protected MomErrPool ErrPool { get; private set; }
        //Код источника ошибок
        protected abstract string ErrSourceCode { get; }
        //Текущий контекст ошибки
        protected abstract IContextable Contextable { get; }

        //Текущая вычисляемая функция
        public CalcBaseFun CurFun { get; set; }
        
        //Добавить особую ошибку в заданный EditMom
        public void PutErr(EditMom mom, 
                                    string errMess, //Сообщение ошибки
                                    int quality = 2, //Качество ошибки
                                    int errNum = 0) //Номер ошибки среди ошибок для одной функции, число от 0 до 8
        {
            var number = CurFun.ErrorNumber + errNum + 1;
            ((MomErrFactory) ErrPool.Factory).AddDescr(number, errMess, quality);
            mom.AddError(ErrPool.MakeError(number, Contextable));
        }

        //Добавить обычную ошибку в заданный EditMom, номер ошибки задается для функции по умолчанию
        public void PutErr(EditMom mom)
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
        protected MomErr MaxErr(IEnumerable<IReadMean> par)
        {
            MomErr err = null;
            foreach (var mom in par)
                err = err.Add(mom.Error);
            return err;
        }
        //Вычисляет минимальную ошибку значений
        protected MomErr MinErr(IEnumerable<IReadMean> par)
        {
            MomErr err = null;
            foreach (var p in par)
            {
                if (p.Error == null) return null;
                err = err ?? p.Error;
            }
            return err;
        }

        //Текущий результат скалярной функции
        public EditMom ScalarRes { get; private set; }
        //Значения разного типа для результата
        private readonly Dictionary<DataType, EditMom> _scalarResTypes = new Dictionary<DataType, EditMom>()
            {
                {DataType.Boolean, new EditMom(DataType.Boolean)},
                {DataType.Integer, new EditMom(DataType.Integer)},
                {DataType.Real, new EditMom(DataType.Real)},
                {DataType.Time, new EditMom(DataType.Time)},
                {DataType.String, new EditMom(DataType.String)},
                {DataType.Value, new EditMom(DataType.Value)},
            };
        //Установить тип данных скалярного значения
        public void SetScalarDataType(DataType dtype)
        {
            ScalarRes = _scalarResTypes[dtype];
            ScalarRes.MakeDefault();
        }

        //Обрамление промежуточного вычисления скалярного значения
        public void CalcScalarFun(IReadMean[] par, //Параметры расчета
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