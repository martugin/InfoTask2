using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //���������� - ��������
    public class ReceiverConnect : ProviderConnect
    {
        public ReceiverConnect(Project project, string name, string complect) 
            : base(project, name, complect) { }

        //��� ����������
        public override ProviderType Type
        {
            get { return ProviderType.Receiver; }
        }
        //������� ��������� ���������
        internal Receiver Receiver
        {
            get { return (Receiver)Provider; }
        }
       
        //������ ��������, ���������� ������������ ��������
        private readonly DicS<ReceiverSignal> _signals = new DicS<ReceiverSignal>();
        public IDicSForRead<ReceiverSignal> Signals { get { return _signals; } }

        //�������� ������
        public ReceiverSignal AddSignal(string fullCode, //������ ��� �������
                                                         DataType dataType, //��� ������
                                                         SignalType signalType, //��� �������
                                                         string infObject, //�������� �������
                                                         string infOut = "", //�������� ������ ������������ �������
                                                         string infProp = "") //�������� ������� ������������ ������
        {
            if (_signals.ContainsKey(fullCode))
                return _signals[fullCode];
            Provider.IsPrepared = false;
            var contextOut = infObject + (infOut.IsEmpty() ? "" : ";" + infOut);
            var inf = infObject.ToPropertyDicS().AddDic(infOut.ToPropertyDicS()).AddDic(infProp.ToPropertyDicS());
            return _signals.Add(fullCode, new ReceiverSignal(this, fullCode, dataType, contextOut, inf));
        }

        //������ �������� � ��������
        public void WriteValues() 
        {
            if (PeriodIsUndefined()) return;
            if (Start(0, 80).Run(PutValues).IsSuccess) return;

            if (ChangeProvider())
                using (Start(80, 100))
                    PutValues();
        }

        //������ �������� � ��������
        protected void PutValues()
        {
            try
            {
                using (Start(0, 10))
                    if (!Receiver.Connect() || !Receiver.Prepare()) return;

                using (Start(10, 100))
                {
                    AddEvent("������ �������� � ��������");
                    Receiver.WriteValues();
                    AddEvent("�������� �������� � ��������");
                }
            }
            catch (Exception ex)
            {
                AddError("������ ��� ������ �������� � ��������", ex);
            }
        }
    }
}