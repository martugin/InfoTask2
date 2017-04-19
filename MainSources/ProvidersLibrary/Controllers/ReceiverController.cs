using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //���������� �������� ������ � �������� � �������� �������
    public class ReceiverController : ProviderController
    {
        public ReceiverController(ReceiverConnect connect)
            : base(connect) { }

        //���������� 
        public ReceiverConnect ReceiverConnect { get { return (ReceiverConnect)Connect; } }

        //������ ��������, ���������� ������������ ��������
        protected readonly DicS<ControllerReceiverSignal> ControllerSignals = new DicS<ControllerReceiverSignal>();
        public IDicSForRead<ControllerReceiverSignal> Signals { get { return ControllerSignals; } }

        //������ ������ � ��������
        protected override void RunCycle()
        {
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.ReceiverSignal.Value = (IReadMean)csig.BufferValue.Clone();
            ReceiverConnect.WriteValues();
        }

        //��������� �������� �� ��������
        public void SetValues()
        {
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.BufferValue = (IReadMean)csig.Value.Clone();
        }
    }
}