using CommonTypes;
using System;

namespace ClientLibrary
{
    //Представление соединения в клиенте
    public class ClientConnect : BaseConnect
    {
        public ClientConnect(ClientProject project, string code, string complect) 
            : base(project.App, code, complect, project.Code) { }

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