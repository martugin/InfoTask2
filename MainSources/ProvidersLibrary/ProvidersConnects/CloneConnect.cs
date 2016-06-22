using BaseLibrary;

namespace ProvidersLibrary
{
    //Соединение с клоном в формате Access
    public class CloneConnect : SourceConnect
    {
        public CloneConnect(string complect)
        {
            Complect = complect;
        }
        public CloneConnect(string name, Logger logger, string complect)
            : base(name, logger)
        {
            Complect = complect;
        }

        public override string Complect { get; protected set; }

        public override string Code
        {
            get { return "CloneSource"; }
        }

        protected override void ReadInf(DicS<string> dic)
        {
            throw new System.NotImplementedException();
        }
    }
}