using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников и приемников

    public abstract class ProviderSignal : ISignal
    {
        protected ProviderSignal(string code)
        {
            Code = code;
        }

        protected ProviderSignal(string code, DataType dataType, string infObject, string infOut, string infProp)
        {
            Code = code;
            DataType = dataType;
            CodeOut = infObject + (infOut.IsEmpty() ? "" : ";" + infOut);
            Inf = infObject.ToPropertyDicS().AddDic(infOut.ToPropertyDicS()).AddDic(infProp.ToPropertyDicS());
            Inf.DefVal = "";
        }

        //Полный код сигнала
        public string Code { get; private set; }
        //Тип данных 
        public DataType DataType { get; protected set; }
        //InfObject + InfOut
        public string CodeOut { get; private set; }
        //Словарь свойств
        public DicS<string> Inf { get; private set; }

        //Список значений
        protected MomList MomList { get; set; }

        //Очистка списка значений
        internal virtual void ClearMoments(bool clearBegin)
        {
            MomList.Clear();
        }
    }
}