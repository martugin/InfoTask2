using BaseLibrary;

namespace CommonTypes
{
    //Соединение с клоном в формате Access
    public class CloneAccessConnect : SourceConnect
    {
        public override string Complect
        {
            get { throw new System.NotImplementedException(); }
        }

        public override string Code
        {
            get { return "CloneSource"; }
        }

        protected override void ReadDicS(DicS<string> dic)
        {
            throw new System.NotImplementedException();
        }
    }
}