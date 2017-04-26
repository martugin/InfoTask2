using CommonTypes;
using System;

namespace ClientLibrary
{
    //Представление соединения в клиенте
    public class ClientConnect : BaseConnect
    {
        public ClientConnect(BaseProject project, string code, string complect) 
            : base(project, code, complect) { }

        public override ProviderType Type
        {
            get { throw new NotImplementedException(); }
        }

        //Вызов окна настройки
        public string Setup()
        {
            throw new NotImplementedException();
            //if (MenuCommands == null)
            //{
            //    MenuCommands = new DicS<Dictionary<string, IMenuCommand>>();
            //    AddMenuCommands();
            //}
            //IsSetup = true;
            //new ProviderSetupForm { Conn = this }.ShowDialog();
            //while (IsSetup) Thread.Sleep(500);
            //return ProviderInf;
        }
    }
}