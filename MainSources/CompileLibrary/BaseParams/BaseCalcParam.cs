using BaseLibrary;

namespace CompileLibrary
{
    //Базовый класс для расчетных параметров расчета и компиляции
    public abstract class BaseCalcParam : BaseParam
    {
        //Формирование параметра из рекордсета
        protected BaseCalcParam(IRecordRead rec,
                                              bool isSubParam) //Является подпараметром
        {
            IsSubParam = isSubParam;
            if (!IsSubParam)
            {
                ParamId = rec.GetInt("ParamId");
                Code = rec.GetString("ParamCode");
                Name = rec.GetString("ParamName");
                Task = rec.GetString("Task");
            }
            else
            {
                OwnerId = rec.GetInt("ParamId");
                ParamId = rec.GetInt("SubParamId");
                Code = rec.GetString("SubParamCode");
                Name = rec.GetString("SubParamName"); 
            }
            Comment = rec.GetString("Comment");
            Units = rec.GetString("Units");
            Min = rec.GetString("Min");
            Max = rec.GetString("Max");

            ArchiveParamType = rec.GetString("ArchiveParamType").ToArchiveParamType();
            SuperProcess = rec.GetString("SuperProcessType").ToSuperProcess();
            ObjectCode = rec.GetString("ObjectCode");
        }

        //Полный код
        public string FullCode { get; protected set; }

        //Id в CalcParams или CalcSubParams
        public int ParamId { get; private set; }
        //Id вледельца
        public int OwnerId { get; private set; }
        //Является подпараметром
        public bool IsSubParam { get; private set; }
        
        //Способ формирования параметра для ведомости или архива результатов
        public ArchiveParamType ArchiveParamType { get; private set; }
        //Комментарий
        public string Comment { get; private set; }
        //Минимум и максимум шкалы
        public string Min { get; protected set; }
        public string Max { get; protected set; }

        //Скомпилированное выражение
        public string CompiledExpr { get; protected set; }
    }
}