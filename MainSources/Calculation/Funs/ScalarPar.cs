using System;
using CommonTypes;

namespace Calculation
{
    //Вспомогательный класс для хранения одного списка значений при вычислении скалярных функций
    internal class ScalarPar
    {
        public ScalarPar(IMomListRead list, int num)
        {
            _list = list;
            Num = num;
            Pos = -1;
        }

        //Список значений
        private readonly IMomListRead _list;
        //Номер в списке исходных параметров
        public int Num { get; private set; }

        //Текущий обрабатываемый индекс
        public int Pos { get; set; }
        //Время следующего индекса
        public DateTime NextTime
        {
            get
            {
                if (Pos + 1 >= _list.Count)
                    return DateTime.MaxValue;
                return _list.TimeI(Pos + 1);
            }
        }
    }
}