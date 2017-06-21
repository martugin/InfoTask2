using BaseLibrary;

namespace Calculation
{
    //Базовый класс для объектов расчета и компиляции
    public abstract class BaseObject
    {
        protected BaseObject(IRecordRead rec)
        {
            Id = rec.GetInt("ObjectId");
            Code = rec.GetString("CodeObject");
            Name = rec.GetString("NameObject");
            InfObject = rec.GetString("InfObject");
        }

        //Id
        public int Id { get; private set; }
        //Код 
        public string Code { get; private set; }
        //Имя
        public string Name { get; private set; }
        //Свойства для источника
        public string InfObject { get; private set; }

        //Другие свойства и их типы данных
        private readonly DicS<ObjectProp> _props = new DicS<ObjectProp>();
        public DicS<ObjectProp> Props { get { return _props; } }
    }
}