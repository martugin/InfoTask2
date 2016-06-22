using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    public class OvationSourceConnect : OleDbSourceConnect
    {
        public OvationSourceConnect(string name, Logger logger) 
            : base(name, logger) { }

        //Код провайдера
        public override string Code { get { return "OvationSource"; } }
        //Комплект провайдера
        public override string Complect { get { return "Ovation"; } protected set {}}

        //Настройки провайдера
        protected override void ReadInf(DicS<string> dic)
        {
            _dataSource = dic["DataSource"];
            Hash = "OvationHistorian=" + _dataSource;
        }

        //Имя дропа
        private string _dataSource;

        //Строка соединения с Historian
        protected override string ConnectinString
        {
            get
            {
                return "Provider=OvHOleDbProv.OvHOleDbProv.1;Persist Security Info=True;User ID='';" +
                          "Data Source=" + _dataSource + ";Location='';Mode=ReadWrite;Extended Properties=''";
            }
        }

        //Проверка соединения с провайдером, вызывается в настройках, или когда уже произошла ошибка для повторной проверки соединения
        public override bool Check()
        {
            return Danger(Connect, 2, 500, "Не удалось соединиться с Historian");
        }

        //Проверка соединения в настройке
        public override bool CheckConnection()
        {
            if (Check())
            {
                CheckConnectionMessage = "Успешное соединение с Historian";
                return true;
            }
            AddError(CheckConnectionMessage = "Ошибка соединения с Historian");
            return false;
        }

    }
}