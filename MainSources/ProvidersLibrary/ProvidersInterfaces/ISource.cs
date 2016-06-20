using System;
using BaseLibrary;

namespace CommonTypes
{
    //Стандартный интерфейс для соединений источников 
    public interface ISourceConnect : IProviderConnect
    {
        //Получение интервала времени архива, если не удалось определить - возвращает null
        TimeInterval GetTime();
    }

    //------------------------------------------------------------------------------------------------------------------------

    //Стандартный интерфейс для источников и имитаторов
    public interface IProviderSource : IProvider
    {
        //Список всех сигналов источника, сигналы можно только читать
        IDicSForRead<SourceSignal> Signals { get; }
        //Чтение значений за период
        void GetValues(DateTime beginRead, //Начало периода
                               DateTime endRead); //Конец периода
        //Создание ошибки 
        ErrMom MakeError(int number,  //Номер ошибки
                                      IContextable addr); //Контекст ошибки (объект)
    }

    //------------------------------------------------------------------------------------------------------------------------

    //Стандартный интерфейс для класса Source (источник исходных данных)
    public interface ISource : IProviderSource
    {
        //Добавить сигнал в список сигналов, принимает информацию по сигналу возвращает сам сигнал
        //Если сигнал уже есть, то его изменяет и возвращает и ничего не добавляет
        ISourceSignal AddSignal(string code, //Код сигнала
                                string context, //Контекст значений (код внутреннего объекта источника)
                                DataType dataType, //Тип данных
                                string signalInf, //Информация о сигнале для источника
                                bool skipRepeats = true, //Пропускать полностью повторяющиеся значения
                                string formula = null); //Расчетная формула если есть
        //Удаляет из источника все сигналы
        void ClearSignals();

        //Считать значения в клон, если задан cloneFile, cloneInf - настройки создания клона
        //Чтение значений за период от beginRead до endRead
        void MakeClone(DateTime beginRead, 
                       DateTime endRead, 
                       string cloneFile, string cloneInf);
    }

    //------------------------------------------------------------------------------------------------------------------------

    //Интерфейс сигнала источника
    public interface ISourceSignal : IContextable
    {
        //Список значений
        IMomListReadOnly MomList { get; }
        //Код
        string Code { get; }
        //Тип данных
        DataType DataType { get; }
    }
}