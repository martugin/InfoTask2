using System;
using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Стандартный интерфейс для провайдера (Source, Archive или Receiver)
    public interface IProvider : IDisposable, IContextable
    {
        //В конструкторе провайдера в него должны загружаться имя, свойства и логгер

        //Тип провайдера
        ProviderType Type { get; }
        //Код провайдера
        string Code { get; }
        //Имя экземпляра провайдера
        string Name { get; set; }
        //Cвойства провайдера
        string Inf { get; set; }
        //Кэш для идентификации соединения
        string Hash { get; }
        //Каждому провайдеру передается логгер (поток)
        Logger Logger { get; set; }

        //Проверка соединения с провайдером, вызывается когда уже произошла ошибка для повторной проверки соединения
        //Возвращает true, если соединение установлено
        bool Check();

        //Настройка провайдера (через форму), возвращает строку с новыми настройками
        string Setup();
        //True, пока идет настройка
        bool IsSetup { get; set; }
        //Проверка соединения в форме настроек возвращает true, если соединение успешное
        bool CheckConnection();
        //Cтрока для вывода сообщения о последней проверке соединения
        string CheckConnectionMessage { get; }
        //Проверка корректности настроек, возвращает строку с ошибками
        string CheckSettings(Dictionary<string, string> infDic, //словарь настроек
                                        Dictionary<string, string> nameDic); //словарь имен свойств
        //Словарь комманд открытия дилогов, ключи - имена свойств, вторые ключи - названия пунктов меню
        DicS<Dictionary<string, IMenuCommand>> MenuCommands { get; } 
        //Возвращает выпадающий список для поля настройки
        List<string> ComboBoxList(Dictionary<string, string> props, //словарь значений свойств
                                                string propname); //имя свойства для ячейки со списком

        //Подготовка провайдера к работе (во время PrepareCalc)
        void Prepare();
        //Текущий период расчета
        DateTime PeriodBegin { get; }
        DateTime PeriodEnd { get; }
    }

    //--------------------------------------------------------------------
    
    //Стандартный интерфейс для источников и имитаторов
    public interface IProviderSource : IProvider
    {
        //Список всех сигналов источника, сигналы можно только читать
        IDicSForRead<ISourceSignal> Signals { get; }
        //Чтение значений за период
        void GetValues(DateTime beginRead, //Начало периода
                               DateTime endRead); //Конец периода
    }

    //--------------------------------------------------------------------

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

        //Получение интервала времени архива, если не удалось определить - возвращает null
        TimeInterval GetTime();

        //Считать значения в клон, если задан cloneFile, cloneInf - настройки создания клона
        //Чтение значений за период от beginRead до endRead
        void MakeClone(DateTime beginRead, DateTime endRead, string cloneFile, string cloneInf);
    }

    //--------------------------------------------------------------------

    //Интерфейс сигнала источника
    public interface ISourceSignal
    {
        //Список значений
        IMomListReadOnly MomList { get; }
        //Код
        string Code { get; }
        //Тип данных
        DataType DataType { get; }
    }

    //--------------------------------------------------------------------

    //Стандартный интерфейс для класса Receiver
    public interface IReceiver : IProvider
    {
        //Список сигналов приемника с мгновенными значениями
        IDicSForRead<ReceiverSignal> Signals { get; }
        //Добавить сигнал приемника
        ReceiverSignal AddSignal(string signalInf, //Информация о сигнале для приемника
                                               string code, //Код сигнала
                                               DataType dataType); //Тип данных
        //Отправить значения параметров на приемник
        void WriteValues();
        //Допускается передача списка мгновенных значений за один раз
        bool AllowListValues { get; }
    }
}