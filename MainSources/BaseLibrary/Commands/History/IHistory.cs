namespace BaseLibrary
{
    //Интерфейс для записи в историю
    public interface IHistory
    {
        //Ссылка на логгер
        Logger Logger { get; set; }

        //Записать начало выполнения надкомманды
        void WriteStartSuper(ProgressCommand command);
        //Записать начало выполнения комманды
        void WriteStart(LogCommand command);
        //Записать конец выполнения надкомманды
        void WriteFinishSuper(string results);
        //Записать конец выполнения комманды
        void WriteFinish(string results);

        //Записать событие
        void WriteEvent(string description, string pars);
        //Записать ошибку в историю
        void WriteError(CommandError error);

        //Записать ошибку в список ошибок
        void WriteErrorToList(CommandError error);
        //Очистить список ошибок
        void ClearErrorsList();

        //Сохранение старого файла истории и добавление нового, при достижении размера или при ошибке
        void UpdateHistory();

        //Закрыть файл истории
        void Close();
    }
}