using BaseLibrary;

namespace RealTimeServer
{
    //Поток получения данных из истоника в режиме реального времени
    public class RealTimeThread : Logger
    {
        public RealTimeThread(MainClass main,
                                           string appCode, //Код приложения
                                           string project) //Код проекта
        {
            MainClass = main;
            AppCode = appCode;
            Project = project;
            
        }

        protected MainClass MainClass { get; private set; }

        //Код вызывающего приложения
        protected internal string AppCode { get; private set; }
        //Код проекта
        protected internal string Project { get; private set; }
        
        //Частота считывания данных в мс
        public int ReadingFrequency { get; set; }

    }
}