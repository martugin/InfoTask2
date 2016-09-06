using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Расчет скалярных функций, базовый класс для всех классов расчета функций
    public abstract partial class Functions
    {
        protected Functions()
        {
            var factory = new ErrMomFactory(ErrSourceCode, ErrMomType.Calc);
            ErrPool = new ErrMomPool(factory);
        }


        //Пул ошибок
        protected ErrMomPool ErrPool { get; private set; }
        //Код источника ошибок
        protected abstract string ErrSourceCode { get; }
        //Текущий контекст ошибки
        protected abstract IContextable Contextable { get; }

        //Текущая вычисляемая функция
        public FunCalc CurFun { private get; set; }

        //Делегат скалярных функций
        public delegate void ScalarDelegate(IMom[] par);
        //Текущий результат скалярной функции
        protected MomEdit ScalarRes { get; set; }

        //Вычисление одного значения скалярной функции, результат в ScalarRes
        protected void CalcScalarValue( IMean[] par)
        {
            var fun = (FunScalar)    
        }

        //Добавить особую ошибку в заданный MomEdit
        private void PutErr(MomEdit mom, 
                                     string errMess, //Сообщение ошибки
                                     int quality = 2, //Качество ошибки
                                     int errNum = 0) //Номер ошибки среди ошибок для одной функции, число от 0 до 8
        {
            var number = CurFun.ErrorNumber + errNum + 1;
            ((ErrMomFactory) ErrPool.Factory).AddDescr(number, errMess, quality);
            mom.AddError(ErrPool.MakeError(number, Contextable));
        }

        //Добавить обычную ошибку в заданный MomEdit, номер ошибки задается для функции по умолчанию
        private void PutErr(MomEdit mom)
        {
            mom.AddError(ErrPool.MakeError(CurFun.ErrorNumber, Contextable));
        }

        //Добавить ошибку в текущее значение расчета (ScalarRes)
        protected void PutErr(string errMess, int quality = 2, int errNum = 0) //Особая ошибка
        {
            PutErr(ScalarRes, errMess, errNum);
        }
        protected void PutErr() //По умолчанию для функции
        {
            PutErr(ScalarRes);
        }

        //Вычисляет суммарную ошибку значений
        private ErrMom MaxErr(IEnumerable<IMean> par)
        {
            ErrMom err = null;
            foreach (var mom in par)
                err = err.Add(mom.Error);
            return err;
        }
        //Вычисляет минимальную ошибку значений
        private ErrMom MinErr(IEnumerable<IMean> par)
        {
            ErrMom err = null;
            foreach (var p in par)
            {
                if (p.Error == null) return null;
                err = err ?? p.Error;
            }
            return err;
        }


    }
}