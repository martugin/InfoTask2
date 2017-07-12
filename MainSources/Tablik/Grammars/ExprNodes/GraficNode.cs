using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    //����, �������� ��������� � �������
    internal class GraficNode : TablikKeeperNode
    {
        public GraficNode(TablikKeeper keeper, ITerminalNode terminal, params IExprNode[] args) 
            : base(keeper, terminal, args)
        {
            string code = Token.Text;
            if (Keeper.Module.Grafics.ContainsKey(code))
                Grafic = Keeper.Module.Grafics[code];
            foreach (var m in Keeper.Module.LinkedModules)
                if (m.Grafics.ContainsKey(code))
                    Grafic = m.Grafics[code];
            if (Grafic == null)
                AddError("�� ������ ������");
            else
            {
                if (args.Length != Grafic.Dim)
                    AddError("���������� ���������� ������� �� ��������� � ����������� �������");
                else if (args.Any(arg => !arg.Type.DataType.LessOrEquals(DataType.Real)))
                    AddError("������������ ��������� ������� ������");
            }
        }

        //������
        public Grafic Grafic { get; private set; }
    }
}