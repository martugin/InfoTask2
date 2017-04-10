using System;

namespace BaseLibrary
{
    //Класса для передачи информации индикатора из сервера в клиент
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

        public double Procent
        {
            get { throw new NotImplementedException();}
        }

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