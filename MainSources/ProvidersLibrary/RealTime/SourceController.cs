
namespace ProvidersLibrary
{
    //Постоянное получение данных с источника в реальном времени
    public class SourceController
    {
        public SourceController(SourceConnect connect)
        {
            Connect = connect;
        }

        //Соединение 
        public SourceConnect Connect { get; private set; }
            
        //Запуск процесса считывания
        public void Start()
        {
            
        }

        //Завершение процесса считывания
        public void Finish()
        {
            
        }
    }
}