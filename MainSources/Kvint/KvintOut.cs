using System;
using CommonTypes;
using ProvidersLibrary;

namespace Kvint
{
    public class KvintOut : ListSourceOut
    {
        public KvintOut(ListSource source, string marka, string paramName, int cardId, int paramNo) : base(source)
        {
            _marka = marka;
            _paramName = paramName;
            _cardId = cardId;
            _paramNo = paramNo;
        }

        //Марка объекта
        private readonly string _marka;
        //Имя сигнала
        private readonly string _paramName;
        //Id объекта
        private readonly int _cardId;
        //Номер сигнала
        private readonly int _paramNo;
        //Id для получения данных
        private int _handler;

        //Получение Handler
        public void GetHandler(string serverName)
        {
            GetHandlerById();
            GetHandlerById();
        }

        private void GetHandlerById()
        {
            _handler = CsApi.OpenParamById(_cardId, _paramNo, 4117); //флаги 0x1015
        }

        private void GetHandlerByMarka()
        {
            _handler = CsApi.OpenParamByName(_marka, _paramName, 4117);
        }

        private void GetHandlerExternal(string serverName)
        {
            _handler = CsApi.OpenParamExternal(_cardId, _paramNo, serverName, 4117);
        }

        //Прочитать срез значений
        public ValuesCount ReadCut(DateTime beg)
        {
            CsData m = new CsData();
            var dbeg = beg.TimeToKvint();
            int n =  CsApi.FindFirst(_handler, ref m, dbeg, -1) ? AddValue(m) : 0;
            return new ValuesCount(n, n, VcStatus.Success);
        }

        //Прочитать значения за период, возвращает количество прочитанных значений
        public ValuesCount ReadChanges(DateTime beg, DateTime en)
        {
            int n = 0;
            try
            {
                CsData m = new CsData();
                var dbeg = beg.TimeToKvint();
                var den = en.TimeToKvint();
                if (CsApi.FindFirst(_handler, ref m, dbeg)) 
                {
                    n += AddValue(m);
                    while (true)
                    {
                        if (!CsApi.FindNext(_handler, ref m) || m.Time >= den) break;
                        n += AddValue(m);
                    }
                }
            }
            catch (Exception ex)
            {
                Provider.Logger.AddError("Ошибка при чтении значений", ex, _marka + "." + _paramName);
            }
            return new ValuesCount(n, n, VcStatus.Success);
        }

        //Добавить значение в список
        private int AddValue(CsData m)
        {
            DateTime t = m.Time.KvintToTime();
            double d = m.Value;
            int e = m.ErrorCode;
            return AddMom(ValueSignal, t, d, e == 0 ? null : new MomErr(e.ToString(), 2, e, MomErrType.Source));
        }
    }
}