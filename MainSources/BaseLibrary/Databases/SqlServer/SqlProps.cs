namespace BaseLibrary
{
    //Свойства соединения с SQL
    public struct SqlProps
    {
        public SqlProps(string serverName, string databaseName, bool sqlIdent = false, string login = "", string password = "")
        {
            ServerName = serverName;
            DatabaseName = databaseName;
            SqlIdent = sqlIdent;
            Login = login;
            Password = password;
        }

        //Имя сервера
        public string ServerName;
        //Имя базы данных
        public string DatabaseName;
        //True, если идентификация SQL Server
        public bool SqlIdent;
        //Пользователь
        public string Login;
        //Пароль
        public string Password;

        public override string ToString()
        {
            return "Server=" + ServerName + ";Database=" + DatabaseName + ";SqlIdent=" + SqlIdent + ";Login=" + Login + ";Password=" + Password;
        }
    }
}