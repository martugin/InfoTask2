using System;
using BaseLibrary;

namespace CommonTypes
{
    //Один сигнал для чтения по блокам
    public class SourceObject : IContextable
    {
        protected SourceObject(string code)
        {
            Code = code;
        }

        //Информация по объекту
        public string Inf { get; set; }

        //Код объекта
        public string Code { get; private set; }
        //Контекст
        public string Context { get { return Code + "(объект)"; } }

        //Для объекта опредлено значение среза на время time
        public virtual bool HasBegin { get { return false; } }

        //Проверяет, что хотя бы по одному сигналу из signals есть срез на время time
        protected bool SignalsHasBegin(params SourceSignal[] signals)
        {
            bool e = true;
            foreach (var sig in signals)
                if (sig != null)
                    e &= sig.HasBegin;
            return e;    
        }

        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        public virtual int AddBegin()
        {
            return 0;
        }

        //Добавляет во все сигналы из signals срез на время time, возвращает количество добавленных значений
        protected int SignalsAddBegin(params SourceSignal[] signals)
        {
            int n = 0;
            foreach (var sig in signals)
                if (sig != null)
                    n += sig.MakeBegin();
            return n;
        }
    }
}