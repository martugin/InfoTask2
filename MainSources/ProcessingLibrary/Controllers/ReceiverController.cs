using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ProcessingLibrary
{
    //���������� �������� ������ � �������� � �������� �������
    public class ReceiverController : ProviderController
    {
        public ReceiverController(BaseProject project, ReceiverConnect connect)
            : base(project, connect) { }

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
                    csig.ReceiverSignal.InValue = (IReadMean)csig.BufferValue.Clone();
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