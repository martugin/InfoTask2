﻿using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Узел, задающий расчетный параметр
    internal class ParamNode : TablikKeeperNode
    {
        public ParamNode(TablikKeeper keeper, ITerminalNode terminal, TablikCalcParam param, IEnumerable<IExprNode> args = null) 
            : base(keeper, terminal)
        {
            Param = param;
            Args = args != null ? args.ToArray() : new IExprNode[0];
            if (Keeper.Module == param.Module && !Keeper.Param.UsedParams.ContainsKey(param))
                Keeper.Param.UsedParams.Add(param, terminal.Symbol);
        }

        //Тип узла
        protected override string NodeType
        {
            get { return Param.Owner == null ? "Param" : "SubParam"; }
        }

        //Параметр
        public TablikCalcParam Param { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            Type = Param.Type;
            var inputs = Param.InputsList;
            if (Args.Length > inputs.Count ||
                Args.Length < inputs.Count && inputs[Args.Length].DefaultValue == null)
                AddError("Количество входов расчетного параметра не совпадает с количеством аргументов");
            else if (Args.Where((t, i) => !t.Type.LessOrEquals(inputs[i].Type)).Any())
                AddError("Недопустимые типы аргументов расчетного параметра");
            else MakeUsedMetSignals();
        }

        //Сформировать используемые сигналы и свойства по входам типа объекта
        private void MakeUsedMetSignals()
        {
            for (int i = 0; i < Args.Length; i++)
            {
                var imet = Param.InputsList[i].MetSignals;
                if (imet != null)
                {
                    var amet = Keeper.GetMetSignals(Args[i]);
                    if (amet != null)
                        foreach (var m in imet.Values)
                            amet.Add(m);
                    if (Args[i].Type is TablikObject)
                    {
                        var ob = (TablikObject)Args[i].Type;
                        var sigs = ob.ObjectType.Signals;
                        foreach (var sigCode in imet.Values)
                            if (ob.UsedSignals.Contains(sigs[sigCode]))
                                ob.UsedSignals.Add(sigs[sigCode]);
                    }
                }

                var iprop = Param.InputsList[i].MetProps;
                if (iprop != null)
                {
                    var aprop = Keeper.GetMetProps(Args[i]);
                    if (aprop != null)
                        foreach (var p in iprop.Values)
                            aprop.Add(p);
                    if (Args[i].Type is TablikObject)
                    {
                        var ob = (TablikObject)Args[i].Type;
                        foreach (var propCode in iprop.Values)
                            if (ob.UsedProps.ContainsKey(propCode))
                                ob.UsedProps.Add(propCode,  ob.Props[propCode]);
                    }
                }
            }
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return Param.Code;
        }

        public override string ToTestString()
        {
            if (Args.Length == 0) return ToTestWithChildren();
            return ToTestWithChildren(Args);
        }
    }
}