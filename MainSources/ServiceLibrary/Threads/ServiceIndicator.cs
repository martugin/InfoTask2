using System;
using BaseLibrary;

namespace ServiceLibrary
{
    //Индикатор для потока службы
    public class ServiceIndicator : IIndicator
    {
        public void ShowTextedIndicator()
        {
            throw new NotImplementedException();
        }

        public void ShowTimedIndicator()
        {
            throw new NotImplementedException();
        }

        public void HideIndicator()
        {
            throw new NotImplementedException();
        }

        public void ChangeProcent(double procent)
        {
            throw new NotImplementedException();
        }

        public double Procent { get; private set; }
        public void SetTimedProcess(DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public void SetProcessUsual()
        {
            throw new NotImplementedException();
        }

        public void ChangeTabloText(int num, string text)
        {
            throw new NotImplementedException();
        }

        public void ChangePeriod(DateTime periodBegin, DateTime periodEnd, string periodMode)
        {
            throw new NotImplementedException();
        }
    }
}