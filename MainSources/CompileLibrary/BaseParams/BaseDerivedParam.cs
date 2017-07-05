namespace CompileLibrary
{
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
}