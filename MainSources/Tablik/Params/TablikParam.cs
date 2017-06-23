﻿using System;
using System.Collections.Generic;
using BaseLibrary;
using Calculation;
using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    //Один расчетный параметр для компиляции
    public class TablikParam : BaseCalcParam, ICalcParamNode, ITablikType
    {
        public TablikParam(TablikModule module, IRecordRead rec, bool isSubParam, bool isGenerated) 
            : base(rec, isSubParam)
        {
            _module = module;
            _keeper = new TablikKeeper(this);
            IsGenerated = isGenerated;
            if (!Code.IsCorrectCode())
            {
                ErrMess += "Указан недопустимый код расчетного параметра; ";
                IsFatalError = true;
            }

            CalcOn = rec.GetBool("CalcOn");
            UserExpr1 = rec.GetString("UserExpr1", "");
            if (UserExpr1 == "")
            {
                ErrMess += "Не заполнено расчетное выражение; ";
                IsFatalError = true;
            }
            UserExpr2 = rec.GetString("UserExpr2", "");
            if (UserExpr2 == "")
            {
                ErrMess += "Не заполнено управляющее выражение; ";
                IsFatalError = true;
            }
            InputsStr = rec.GetString("Inputs");
            
            JoinToParamTree();
        }

        //Модуль
        private readonly TablikModule _module;

        //Включен в расчет
        public bool CalcOn { get; private set; }
        //Строка с входными параметрами
        public string InputsStr { get; private set; }
        //Выражения
        public string UserExpr1 { get; private set; }
        public string UserExpr2 { get; private set; }

        //Является функцией
        public bool IsFun { get { return !InputsStr.IsEmpty(); } }
        //Является сгенерированным
        public bool IsGenerated { get; private set; }

        //Словарь расчетных подпараметров, ключи - коды, содержит только отмеченные и без грубых ошибок
        private readonly DicS<TablikParam> _params = new DicS<TablikParam>();
        public DicS<TablikParam> Params { get { return _params; } }
        //Словарь всех расчетных подпараметров, ключи - коды
        private readonly DicS<TablikParam> _paramsAll = new DicS<TablikParam>();
        public DicS<TablikParam> ParamsAll { get { return _paramsAll; } }
        //Словарь расчетных подпараметров, ключи - Id, содержит все подпараметров
        private readonly DicI<TablikParam> _paramsId = new DicI<TablikParam>();
        public DicI<TablikParam> ParamsId { get { return _paramsId; } }

        //Владелец
        internal TablikParam Owner { get; private set; }

        //Добавляет параметр в дерево параметров
        private void JoinToParamTree()
        {
            if (Code == null) return;
            if (OwnerId != 0)
            {
                Owner = _module.ParamsId[OwnerId];
                FullCode = Owner.FullCode + "." + Code;
                CalcOn &= Owner.CalcOn;
            }
            else FullCode = Code;

            var owner = OwnerId == 0 ? (ICalcParamNode)_module : Owner;
            owner.ParamsId.Add(ParamId, this);
            if (!owner.ParamsAll.ContainsKey(Code) || !owner.ParamsAll[Code].CalcOn)
                owner.ParamsAll.Add(Code, this, true);
            if (CalcOn)
            {
                if (owner.Params.ContainsKey(Code))
                {
                    ErrMess += "Повтор кода расчетного параметра (" + Code + "); ";
                    owner.Params[Code].ErrMess += "Повтор кода расчетного параметра (" + Code + "); ";
                }
                else owner.Params.Add(Code, this);
            }
        }

        //Случилась ошибка, после которой невозможно продлжать компиляцию параметра
        public bool IsFatalError { get; private set; }
        //Сообщения об ошибках 
        private string _errMess = "";
        public string ErrMess
        {
            get { return _errMess.Length < 255 ? _errMess : _errMess.Remove(254); }
            internal set { _errMess = value; }
        }

        #region Parse
        //Синтаксический анализ
        public void Parse()
        {
            _keeper.Errors.Clear();
            if (IsFatalError) return;
            if (IsFun) SemanticInputs(new InputsParsing(_keeper, "входы", InputsStr).ResultTree);
            SemanticFormula(new ExprParsing(_keeper, "расч", UserExpr1).ResultTree, true);
            SemanticFormula(new ExprParsing(_keeper, "упр", UserExpr2).ResultTree, false);
        }

        //Накопитель ошибок 
        private readonly ParsingKeeper _keeper;

        //Семантический разбор поля Inputs
        private void SemanticInputs(Node inputs)
        {
            Inputs.Clear();
            Vars.Clear();
            var pars = ((ListNode) inputs).Children;
            foreach (InputNode node in pars)
            {
                TablikVar v = null;
                var varCode = node.Token.Text;
                var typesList = node.TypeNode.Children;
                if (node.InputType == InputType.Simple)
                {
                    DataType dt = typesList.Count == 0 ? DataType.Real : typesList[0].Token.Text.ToDataType();
                    v = new TablikVar(varCode, new SimpleType(dt), node.ValueNode == null ? null : node.ValueNode.Mean);
                }

                else if (node.InputType == InputType.Param)
                {
                    if (_module.Params.ContainsKey(typesList[0].Token.Text))
                        v = new TablikVar(varCode, _module.Params[]);
                    else _keeper.AddError("Не найден расчетный параметр", typesList[0].Token);
                    
                }

                else if (node.InputType == InputType.Signal)
                {
                    
                }

                if (!Inputs.ContainsKey(v.Code))
                    Vars.Add(v.Code, Inputs.Add(v.Code, v));
                else _keeper.AddError("Два входа с одним кодом", typesList[0].Token);


            }
        }

        //Семантический разбор формулы
        private void SemanticFormula(Node formula, bool isCalc)//расчетное или управляющее выражение
        {
            throw new NotImplementedException();
        }

        //Флаг для построения графа зависимости параметров
        public DfsStatus DfsStatus { get; set; }

        #endregion

        #region DefineDataTypes

        //Тип данных параметра
        internal ITablikType Type { get; private set; }
        //Тип данных как сигнал
        public ITablikSignalType TablikSignalType { get { return Type.TablikSignalType; } }
        //Тип данных
        public DataType DataType { get { return Type.DataType; } }

        //Параметры, базовые для данного
        private readonly HashSet<TablikParam> _baseParams = new HashSet<TablikParam>();
        public HashSet<TablikParam> BaseParams { get { return _baseParams; } }

        //Выходы параметра
        private readonly DicS<TablikVar> _inputs = new DicS<TablikVar>();
        public DicS<TablikVar> Inputs { get { return _inputs; } }
        //Все переменные, включая выходы
        private readonly DicS<TablikVar> _vars = new DicS<TablikVar>();
        public DicS<TablikVar> Vars { get { return _vars; } }
        
        //Определение типов данных и формирование порожденных параметров
        public void DefineDataTypes()
        {
            throw new NotImplementedException();
        }

        #endregion
        
        //Запись результатов компиляции
        public void SaveCompileResults(IRecordAdd rec)
        {
            rec.Put("ErrMess", _errMess);
            rec.Put("CompiledExpr", CompiledExpr);
            rec.Put("ResiltType", "");
            rec.Update();
        }
    }
}