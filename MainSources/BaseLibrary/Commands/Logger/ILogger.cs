using System;

namespace BaseLibrary
{
    //Интерфейс для Logger и ExternalLogger
    public interface ILogger
    {
        //Добавить событие в историю
        void AddEvent(string description, string pars = "");
        void AddEvent(string description, string pars,
            double procent); //Одновременно задает процент индикатора
        void AddEvent(string description, double procent);

        //Добавить ошибку в историю
        void AddError(string text, Exception ex = null, string pars = "", string context = null);
        //Добавить предупреждение в историю
        void AddWarning(string text, Exception ex = null, string pars = "", string context = null);

        //Процент текущей команды
        double Procent { get; set; }

        //Запуск простой комманды
        Command Start(double startProcent, double finishProcent);
        //Завершение текущей команды
        Command Finish(string results = "");

        //Запуск команды, обрамляющей вызов клиентских операций или запись в ErrorsList
        CollectCommand StartCollect(bool isWriteHistory, //Записывать ошибки в ErrorsList
                                                    bool isCollect); //Формировать общую ошибку
        CollectCommand FinishCollect(string results = null);

        //Запись результатов в команду Collect
        void AddCollectResult(string result);
        //Итоговая ошибка комманды Collect
        string CollectedErrorMessage { get; }
        //Результаты выполнения команды Collect
        string CollectedResults { get; }

        //Команда, задающая период обработки
        PeriodCommand StartPeriod(DateTime begin, DateTime end, string mode);
        //Звершение команды, задающей период обработки
        PeriodCommand FinishPeriod();
        
        //Начало, конец и режим периода обработки
        DateTime PeriodBegin { get; }
        DateTime PeriodEnd { get; }
        string PeriodMode { get; }

        //Запуск команды отображения индикатора
        ProgressCommand StartProgress(string name, //Имя комманды, оно же текст 0-го уровня для формы индикатора
                                                        string pars = "", //Параметры команды
                                                        DateTime? endTime = null); //Если не null, то время конца обратного отсчета
        //Завершение текущей команды отображения индикатора
        ProgressCommand FinishProgress();

        //Запуск команды логирования
        LogCommand StartLog(double startProcent, double finishProcent, string name, string context = "", string pars = "");
        LogCommand StartLog(string name, string context = "", string pars = "");
        //Завершение текущей команды логирования
        LogCommand FinishLog(string results = null);
        //Присвоение результата в команду логирования
        void SetLogResults(string results);
        
        //Запуск команды отображения текста индикатора 2-ого уровня
        IndicatorTextCommand StartIndicatorText(double startProcent, double finishProcent, string text);
        IndicatorTextCommand StartIndicatorText(string text);
        //Завершение текущей команды отображения текста индикатора 2-ого уровня
        IndicatorTextCommand FinishIndicatorText();

        //Запуск команды, накапливающей ошибку
        KeepCommand StartKeep(double startProcent, double finishProcent);
        KeepCommand StartKeep();
        KeepCommand FinishKeep();
        //Ошибка, накопленная KeepCommand
        string KeepedError { get; }

        //Запуск команды. обрамляющей опасную операцию
        DangerCommand StartDanger(double startProcent, double finishProcent,
                int repetitions, //Cколько раз повторять, если не удалась (вместе с первым)
                LoggerStability stability, //Минимальная LoggerStability, начиная с которой выполняется более одного повторения операции
                string errMess, //Сообщение об ошибке 
                string repeatMess, //Сообщение о повторе
                bool useThread = false, //Запускать опасную операцию в другом потоке, чтобы была возможность ее жестко прервать
                int errWaiting = 0);  //Cколько мс ждать при ошибке
        DangerCommand StartDanger(int repetitions, LoggerStability stability, string errMess, string repeatMess, bool useThread = false, int errWaiting = 0);

        //Прервать выполнение
        void Break();
    }
}