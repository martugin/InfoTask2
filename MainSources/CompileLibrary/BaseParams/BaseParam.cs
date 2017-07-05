namespace CompileLibrary
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
}