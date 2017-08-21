using BaseLibrary;

namespace Tablik
{
    //Узел дерева расчетных параметров, реализуется CalcParam и Module
    internal interface ISubParams
    {
        //Словарь расчетных параметров, ключи - коды, содержит только отмеченные и без грубых ошибок
        DicS<TablikCalcParam> Params { get; }
        //Словарь всех расчетных параметров, ключи - коды
        DicS<TablikCalcParam> ParamsAll { get; }
        //Словарь расчетных параметров, ключи - Id, содержит все параметры
        DicI<TablikCalcParam> ParamsId { get; }
    }
}