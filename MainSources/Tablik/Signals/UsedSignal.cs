using BaseLibrary;
using CommonTypes;

namespace Tablik
{
    //������������ ������
    internal class UsedSignal : ITablikSignalType
    {
        public UsedSignal(TablikSignal signal, TablikObject ob)
        {
            Signal = signal;
            Object = ob;
        }

        //������
        public TablikSignal Signal { get; private set; }
        //������
        public TablikObject Object { get; private set; }

        //��� � ��� �������
        public string Code { get { return Signal.Code; } }
        public string Name { get { return Signal.Name; } }

        //��� ������
        public DataType DataType { get { return Signal.DataType; } }
        public ITablikSignalType TablikSignalType { get { return this; } }
        public SimpleType Simple { get { return Signal.Simple; } }

        //�������� �����
        public bool LessOrEquals(ITablikType type)
        {
            if (type.TablikSignalType is TablikSignal)
                return Signal == type.TablikSignalType;
            return Simple.LessOrEquals(type);
        }

        //������ ���
        public string FullCode { get { return Object.Code + "." + Signal.Code; } }

        //������ � ���������
        public void ToRecordset(DaoRec rec, int objectId)
        {
            rec.AddNew();
            rec.Put("ObjectId", objectId);
            rec.Put("FullCode", FullCode);
            rec.Put("CodeSignal", Code);
            rec.Put("NameSignal", Name);
            rec.Put("DataType", DataType.ToRussian());
            rec.Put("SignalType", Signal.SignalType);
            rec.Put("InfOut", Signal.InfOut);
            rec.Put("InfProp", Signal.InfProp);
            rec.Put("InitialSignals", Signal.InitialSignals);
            rec.Put("Formula", Signal.Formula);
            rec.Update();
        }

        //������ � ������
        public string ToResString()
        {
            return "{" + Object.Connect.Code + "." + Object.Code + "." + Code + "}" + "(" + DataType + ")";
        }
    }
}