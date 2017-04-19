using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех источников
    public abstract class Source : Provider
    {
        //Ссылка на соединение
        public SourceConnect SourceConnect
        {
            get { return (SourceConnect) ProviderConnect; }
        }

        //Подготовка выходов
        protected override void PrepareOuts()
        {
            foreach (var sig in SourceConnect.Signals.Values)
                if (sig.Type != SignalType.Calc)
                {
                    var ob = AddOut(sig);
                    ob.Context = sig.ContextOut;
                    ob.AddSignal(sig);
                }
        }

        //Создать пул ошибок
        protected override void MakeErrPool()
        {
            if (ErrPool == null)
                ErrPool = new MomErrPool(MakeErrFactory());
        }

        //Создание фабрики ошибок
        protected virtual IMomErrFactory MakeErrFactory()
        {
            var factory = new MomErrFactory(ProviderConnect.Name, MomErrType.Source);
            factory.AddGoodDescr(0);
            return factory;
        }
        //Хранилище ошибок 
        internal MomErrPool ErrPool { get; private set; }

        //Создание ошибки 
        internal MomErr MakeError(int number, IContextable addr)
        {
            return ErrPool.MakeError(number, addr);
        }

        //Конец предыдущего периода чтения значений
        internal DateTime PrevPeriodEnd { get; set; }

        //Очистка значений сигналов
        protected virtual void ClearSignalsValues() {}

        //Чтение значений из провайдера
        protected abstract ValuesCount ReadProviderValues();

        //Чтение значений из источника
        internal ValuesCount ReadValues()
        {
            var vcount = new ValuesCount();
            try
            {
                ClearSignalsValues();
                using (Start(5, 10))
                    if (!Connect() || !Prepare())
                        return new ValuesCount(VcStatus.Fail);

                vcount = ReadProviderValues();
               
                //Вычисление значений расчетных сигналов
                if (SourceConnect.CalcSignals.Count > 0)
                {
                    AddEvent("Вычисление значений расчетных сигналов");
                    int calc = 0;
                    foreach (var sig in SourceConnect.CalcSignals.Values)
                    {
                        sig.Calculate();
                        calc += sig.Value.Count;
                    }
                    AddEvent("Значения расчетных сигналов прочитаны", calc + " значений сформировано");
                    vcount.WriteCount += calc;
                }
                AddEvent("Значения из источника прочитаны", vcount.ToString());
            }
            catch (Exception ex)
            {
                AddError("Ошибка при чтении значений из источника", ex);
                return vcount.AddStatus(VcStatus.Fail);
            }
            finally
            {
                AddErrorOutsWarning();
                PrevPeriodEnd = PeriodEnd;
            }
            return vcount;
        }

        //Словарь ошибочных объектов, ключи - коды объектов
        private readonly DicS<string> _errorOuts = new DicS<string>();

        //Добавляет объект в ErrorsObjects
        protected void AddErrorOut(string codeObject,  //Код сигнала
                                                string errText,        //Сообщение об ошибке
                                                Exception ex = null)  //Исключение
        {
            if (!_errorOuts.ContainsKey(codeObject))
            {
                var err = errText + (ex == null ? "" : ". " + ex.Message + ". " + ex.StackTrace);
                _errorOuts.Add(codeObject, err);
                if (SourceConnect.CloneErrorsRec != null)
                {
                    var rec = SourceConnect.CloneErrorsRec;
                    rec.AddNew();
                    rec.Put("OutContext", codeObject);
                    rec.Put("ErrorDescription", err);
                    rec.Update();
                }
            }
        }

        //Запись списка непрочитанных объектов в лог
        private void AddErrorOutsWarning()
        {
            if (_errorOuts.Count > 0)
            {
                int i = 0;
                string s = "";
                foreach (var ob in _errorOuts.Keys)
                    if (++i < 10) s += ob + ", ";
                AddWarning("Не удалось прочитать значения по некоторым сигналам", null,
                           s + (_errorOuts.Count > 10 ? " и др." : "") + "всего " + _errorOuts.Count + " выходов не удалось прочитать");
            }
        }
    }
}