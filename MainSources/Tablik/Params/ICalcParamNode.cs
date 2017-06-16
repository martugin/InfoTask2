using BaseLibrary;

namespace Tablik
{
    //Узел дерева расчетных параметров, реализуется CalcParam и Module
    internal interface ICalcParamNode
    {
        //Словарь расчетных параметров, ключи - коды, содержит только отмеченные и без грубых ошибок
        DicS<TablikParam> Params { get; }
        //Словарь всех расчетных параметров, ключи - коды
        DicS<TablikParam> ParamsAll { get; }
        //Словарь расчетных параметров, ключи - Id, содержит все параметры
        DicI<TablikParam> ParamsId { get; }
    }
}