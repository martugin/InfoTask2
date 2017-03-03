using System;
using System.Runtime.InteropServices;

namespace ComClients
{
    //Интерфейс для LoggerClient и свяызанных с ним классов
    public interface ILoggerClient
    {
        void AddEvent(string text, //Описание
                              string pars = ""); //Дополнительная информация
        
        void AddWarning(string text, //Описание
                                   string pars = ""); //Дополнительная информация
        
        void AddError(string text, //Описание
                               string pars = ""); //Дополнительная информация

        //Задать процент текущей команды
        void SetProcent(double procent);

        //Запуск простой комманды
        void Start(double startProcent, double finishProcent); //Процент индикатора относительно команды родителя
        //Завершение текущей команды
        void Finish(string results = "");

        //Запуск команды логирования
        void StartLog(double startProcent, double finishProcent, //Процент индикатора относительно команды родителя
                             string name, //Имя команды
                             string pars = "", //Дополнительная информация
                             string context = ""); //Контекст выполнения команды
        void StartLog( string name, string pars = "", string context = "");
        //Завершение текущей команды логирования
        void FinishLog(string results = null);

        //Запуск команды отображения индикатора
        void StartProgress(string name, //Имя команды
                                    string pars = "", //Дополнительная информация
                                    string text = ""); //Текст для отображения на индикаторе
        void StartProgress(string name, //Имя команды
                                    string pars, //Дополнительная информация
                                    DateTime beg, DateTime en, string mode = ""); //Период расчета
        //Звершение текущей команды отображения индикатора
        void FinishProgress();

        //Запуск команды отображения текста индикатора 2-ого уровня
        void StartIndicatorText(double startProcent, double finishProcent, string text);
        void StartIndicatorText(string text);
        //Завершение текущей команды отображения текста индикатора 2-ого уровня
        void FinishIndicatorText();

        //Прервать выполнение
        void Break();
    }

    //------------------------------------------------------------------------------------------------

    //Интерфейс событий для LoggerClient
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILoggerClientEvents
    {
        [DispId(1)]
        void Finished();
    }
}