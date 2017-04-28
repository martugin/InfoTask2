using System.Collections.Generic;
using CommonTypes;

namespace Calculation
{
    //Интерфейс для соединений и модулей для схемы проекта
    public interface ISchemeConnect
    {
        //Код соединения
        string Code { get; }
        //Тип провайдера или модуль
        ProviderType Type { get; }

        //Связанные входящие и исходящие соединения
        List<ISchemeConnect> InConnects { get; }
        List<ISchemeConnect> OutConnect { get; }
    }
}