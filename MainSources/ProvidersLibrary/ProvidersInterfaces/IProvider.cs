using System;
using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Стандартный интерфейс соединения с провайдером
    public interface IProviderConnect : IDisposable, IContextable
    {
        //В конструкторе провайдера в него должны загружаться имя, свойства и логгер

        //Имя соединения
        string Name { get; set; }
        //Тип провайдера
        ProviderType Type { get; }
        //Комплект провайдера
        string Complect { get; }
        //Код провайдера
        string Code { get; }
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

        //Проверка соединения в форме настроек возвращает true, если соединение успешное
        bool CheckConnection();
        //Cтрока для вывода сообщения о последней проверке соединения
        string CheckConnectionMessage { get; }
        //Проверка корректности настроек, возвращает строку с ошибками
        string CheckSettings(Dictionary<string, string> infDic); //словарь настроек
    }

    //------------------------------------------------------------------------------------------------------------------------
    //Стандартный интерфейс провайдера
    public interface IProvider
    {
        //Соединение с провайдером
        IProviderConnect ProviderConnect { get; }

        //Подготовка провайдера к работе (во время PrepareCalc)
        void Prepare();
        //Текущий период расчета
        DateTime PeriodBegin { get; }
        DateTime PeriodEnd { get; }
    }
}