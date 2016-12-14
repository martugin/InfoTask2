﻿namespace BaseLibrary
{
    //Интерфейс для записи в историю
    public interface IHistory
    {
        //Начало выполнения надкомманды
        void WriteStartSuper(CommProgress command);
        //Начало выполнения комманды
        void WriteStart(CommLog command);
        //Конец выполнения надкомманды
        void WriteFinishSuper(CommProgress command, string results);
        //Конец выполнения комманды
        void WriteFinish(CommLog command, string results);

        //Записать событие
        void WriteEvent(string description, string pars);
        //Записать ошибку в историю
        void WriteError(ErrorCommand error);

        //Записать ошибку в список ошибок
        void WriteErrorToList(ErrorCommand error);

        //Закрыть файл истории
        void Close();
    }
}