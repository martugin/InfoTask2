using System;

namespace BaseLibrary
{
    //Интерфейс для индикаторов
    public interface IIndicator
    {
        void ShowTextedIndicator();
        void ShowTimedIndicator();
        void HideIndicator();
        void ChangeProcent(double procent);
        double Procent { get; }
        void SetTimedProcess(DateTime endTime);
        void SetProcessUsual();
        void ChangeTabloText(int num, string text);
        void ChangePeriod(DateTime periodBegin, DateTime periodEnd, string periodMode);
    }
}