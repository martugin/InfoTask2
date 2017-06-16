using BaseLibrary;

namespace Calculation
{
    //Базовый класс для расчетных и порожденных параметров
    public abstract class BaseParam
    {
        //Код
        public string Code { get; protected set; }
        //Имя
        public string Name { get; protected set; }

        //Задача
        public string Task { get; protected set; }
        //Тип накопления итоговых значений
        public SuperProcess SuperProcess { get; protected set; }
        //Код привязанного объекта
        public string ObjectCode { get; protected set; }
        //Единицы измерения
        public string Units { get; protected set; }
    }

    //---------------------------------------------------------------------------------------------
    //Базовый класс для пророжденных параметров расчета и компиляции
    public abstract class BaseDerivedParam : BaseParam
    {
        //Код старшего параметра
        public string ParamCode { get; protected set; }
        //Имя старшего параметра
        public string ParamName { get; protected set; }

        //Минимум и максимум шкалы
        public double? Min { get; protected set; }
        public double? Max { get; protected set; }
    }

    //---------------------------------------------------------------------------------------------
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