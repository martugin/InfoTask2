using System;

namespace BaseLibrary
{
    //Аргументы разных событий индикатора и табло

    //Показать индикатор
    public class ShowIndicatorEventArgs : EventArgs
    {
        public ShowIndicatorEventArgs(bool isTimed)
        {
            IsTimed = isTimed;
        }
        //Использовать индикатор со временем
        public bool IsTimed { get; private set; }
    }

    //Изменить уровень индикатора
    public class ChangeProcentEventArgs : EventArgs
    {
        public ChangeProcentEventArgs(double procent)
        {
            Procent = procent;
        }
        //Процент индикатора
        public double Procent { get; private set; }
    }

    //Изменить текст на табло (текст любого уровня)
    public class ChangeTabloTextEventArgs : EventArgs
    {
        //Три уровня текста на форме индикатора
        private readonly string[] _tablText = new string[3];
        public string[] TabloText { get { return _tablText; } }
    }

    //Изменить начало, конец периода обработки или режим
    public class ChangePeriodEventArgs : EventArgs
    {
        public ChangePeriodEventArgs(DateTime beginPeriod, DateTime endPeriod, string modePeriod)
        {
            BeginPeriod = beginPeriod;
            EndPeriod = endPeriod;
            ModePeriod = modePeriod;
        }

        //Начало и конец периода обработки
        public DateTime BeginPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        //Режим выполнения
        public string ModePeriod { get; set; }
    }
}