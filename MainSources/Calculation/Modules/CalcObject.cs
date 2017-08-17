using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Объект для использования в расчете
    internal class CalcObject
    {
        public CalcObject(IRecordRead rec)
        {
            Code = rec.GetString("CodeObject");
            Name = rec.GetString("NameObject");
            Inf = rec.GetString("InfObject");
        }

        //Код и имя
        public string Code { get; private set; }
        public string Name { get; private set; }
        //Настройки объекта
        public string Inf { get; private set; }

        //Словарь сигналов
        private readonly DicS<IReadSignal> _signals = new DicS<IReadSignal>();
        public DicS<IReadSignal> Signals { get { return _signals; } }
        //Словарь значений свойств
        private readonly DicS<Mean> _props = new DicS<Mean>();
        public DicS<Mean> Props { get { return _props; } }
    }
}