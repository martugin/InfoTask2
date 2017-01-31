namespace BaseLibrary
{
    //Интерфейс для записи в историю
    public interface IHistory
    {
        //Ссылка на логгер
        Logger Logger { get; }

        //Начало выполнения надкомманды
        void WriteStartSuper(CommandProgress command);
        //Начало выполнения комманды
        void WriteStart(CommandLog command);
        //Конец выполнения надкомманды
        void WriteFinishSuper(CommandProgress command, string results);
        //Конец выполнения комманды
        void WriteFinish(CommandLog command, string results);

        //Записать событие
        void WriteEvent(string description, string pars);
        //Записать ошибку в историю
        void WriteError(ErrorCommand error);

        //Записать ошибку в список ошибок
        void WriteErrorToList(ErrorCommand error);
        //Очистить список ошибок
        void ClearErrorsList();

        //Сохранение старого файла истории и добавление нового, при достижении размера или при ошибке
        void UpdateHistory();

        //Закрыть файл истории
        void Close();
    }
}