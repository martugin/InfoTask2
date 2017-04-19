using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //���������� ��������� ������ � ��������� � �������� �������
    public class SourceController : ProviderController
    {
        public SourceController(SourceConnect connect) 
            :base(connect) { }

        //���������� 
        public SourceConnect SourceConnect { get { return (SourceConnect) Connect; } }

        //������ ��������, ���������� ������������ ��������
        protected readonly DicS<ControllerSourceSignal> ControllerSignals = new DicS<ControllerSourceSignal>();
        public IDicSForRead<ControllerSourceSignal> Signals { get { return ControllerSignals; } }

        //��������� ������ �� ���������
        protected override void RunCycle()
        {
            SourceConnect.GetValues();
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.BufferValue = (IReadMean)csig.SourceSignal.Value.Clone();
        }

        //��������� �������� �� ��������
        public void GetValues()
        {
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.Value = (IReadMean)csig.BufferValue.Clone();
        }
    }
}