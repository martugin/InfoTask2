using CommonTypes;

namespace Tablik
{
    //Базовый класс для BaseObjectType и ObjectType
    internal abstract class ObjectTypeBase : ITablikSignalType
    {
        protected ObjectTypeBase(TablikConnect connect, int id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
            Connect = connect;
        }

        //Id в таблице ObjectTypes или ObjectTypesCalc
        public int Id { get; protected set; }
        //Код типа объекта
        public string Code { get; protected set; }
        //Имя
        public string Name { get; protected set; }
        //Источник
        public TablikConnect Connect { get; private set; }

        //Тип данных
        public DataType DataType { get { return Simple.DataType; } }
        //Тип данных - простой
        public SimpleType Simple { get; protected set; }

        //Запись в строку
        public string ToResString()
        {
            return "{" + Code + "}";
        }

        //Тип данных как сигнал
        public ITablikSignalType TablikSignalType { get { return this; } }

        public abstract bool LessOrEquals(ITablikType type);
    }
}