using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using BaseLibrary;
using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    //Один расчетный параметр для компиляции 
    internal class TablikParam : BaseCalcParam, ISubParams, ITablikType
    {
        public TablikParam(TablikModule module, IRecordRead rec, bool isSubParam, bool isGenerated) 
            : base(rec, isSubParam)
        {
            Module = module;
            Keeper = new TablikKeeper(this);
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

        //Конструктор для тестов
        public TablikParam(TablikModule module, string code, string expr1, string expr2 = "Расчет")
            : base(false)
        {
            Module = module;
            Code = code;
            UserExpr1 = expr1;
            UserExpr2 = expr2;
            Keeper = new TablikKeeper(this);
        }

        //Модуль
        internal TablikModule Module { get; private set; }

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
                Owner = Module.ParamsId[OwnerId];
                FullCode = Owner.FullCode + "." + Code;
                CalcOn &= Owner.CalcOn;
            }
            else FullCode = Code;

            var owner = OwnerId == 0 ? (ISubParams)Module : Owner;
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
            Keeper.Errors.Clear();
            if (IsFatalError) return;
            if (IsFun) ParseInputs();
            ParseFormula();
        }

        //Накопитель ошибок 
        public TablikKeeper Keeper { get; private set; }

        //Входы параметра
        private readonly DicS<TablikVar> _inputs = new DicS<TablikVar>();
        public DicS<TablikVar> Inputs { get { return _inputs; } }
        //Входы параметра в списке по порядку
        private readonly List<TablikVar> _inputsList = new List<TablikVar>();
        public List<TablikVar> InputsList { get { return _inputsList; } }
        //Все переменные, включая входы
        private readonly DicS<TablikVar> _vars = new DicS<TablikVar>();
        public DicS<TablikVar> Vars { get { return _vars; } }

        //Параметры, используемые в данном
        private readonly Dictionary<TablikParam, IToken> _usedParams = new Dictionary<TablikParam, IToken>();
        public Dictionary<TablikParam, IToken> UsedParams { get { return _usedParams; } }
        
        //Семантический разбор поля Inputs
        private void ParseInputs()
        {
            Inputs.Clear();
            InputsList.Clear();
            Vars.Clear();
            var inputs = (ListNode<InputNode>) new InputsParsing(Keeper, InputsStr).ResultTree;
            TablikVar v = null;
            foreach (var node in inputs.Nodes)
            {
                var varCode = node.Token.Text;
                if (Inputs.ContainsKey(varCode))
                    Keeper.AddError("Два входа с одинаковыми именами", node);
                else if (varCode.ToLower() == "расчет" || varCode.ToLower() == "calc")
                    Keeper.AddError("Недопустимое имя входа", node);
                switch (node.InputType)
                {
                    case InputType.Simple:
                        var dt = node.TypeNode == null ? DataType.Real : node.TypeNode.Text.ToDataType();
                        var at = node.SubTypeNode == null ? ArrayType.Single : node.SubTypeNode.Token.Text.ToArrayType();
                        var val = node.ValueNode == null ? null : node.ValueNode.Mean;
                        v = new TablikVar(varCode, new SimpleType(dt, at), val);
                        break;

                    case InputType.Param:
                        if (!Module.Params.ContainsKey(node.TypeNode.Text))
                            Keeper.AddError("Не найден расчетный параметр", node.TypeNode);
                        else 
                        {
                            var par = Module.Params[node.TypeNode.Text];
                            if (node.SubTypeNode == null)
                            {
                                if (!par.IsFun) 
                                    Keeper.AddError("Параметр без входов не может быть типом данных входа", node.TypeNode);
                                else v = new TablikVar(varCode, par);
                            }
                            else if (!par.Params.ContainsKey(node.SubTypeNode.Text))
                                Keeper.AddError("Не найден расчетный подпараметр", node.SubTypeNode);
                            else
                            {
                                var spar = par.Params[node.SubTypeNode.Text];
                                if (!spar.IsFun) 
                                    Keeper.AddError("Подпараметр без входов не может быть типом данных входа", node.SubTypeNode);
                                else v = new TablikVar(varCode, spar);
                            }
                        }
                        break;

                    case InputType.Signal:
                        string scode = node.TypeNode.Text.Substring(1, node.TypeNode.Text.Length - 2);
                        ObjectTypeBase t = null;
                        foreach (var con in Module.LinkedSources)
                        {
                            if (con.ObjectsTypes.ContainsKey(scode))
                            {
                                if (t == null) t = con.ObjectsTypes[scode];
                                else Keeper.AddError("Одинаковый код типа объекта в двух разных источниках", node);
                            }
                            else if (con.BaseObjectsTypes.ContainsKey(scode))
                            {
                                if (t == null) t = con.BaseObjectsTypes[scode];
                                else Keeper.AddError("Одинаковый код типа объекта в двух разных источниках", node);
                            }
                        }
                        if (t != null) v = new TablikVar(varCode, t) { MetSignals = new SetS(), MetProps = new SetS() };
                        else Keeper.AddError("Не найден тип объекта или сигнала", node);
                        break;
                }
                if (v != null)
                {
                    if (Inputs.ContainsKey(v.Code))
                        Keeper.AddError("Два входа с одинаковыми именами", node);
                    else
                    {
                        if (v.DefaultValue == null && InputsList.Count > 0 && InputsList[InputsList.Count-1].DefaultValue != null)
                            Keeper.AddError("Входы со значениямия по умолчанию должны располагаться в конце списка входов", node);
                        InputsList.Add(Vars.Add(v.Code, Inputs.Add(v.Code, v)));
                    }
                }
            }
        }

        //Узлы расчетного и управляющего выражения
        internal TablikListNode Expr1 { get; private set; }
        internal TablikListNode Expr2 { get; private set; }

        //Семантический разбор формулы
        private void ParseFormula()//расчетное или управляющее выражение
        {
            Vars.Add("расчет", Vars.Add("calc", new TablikVar("расчет")));
            Expr1 = (TablikListNode)new ExprParsing(Keeper, "расч", UserExpr1).ResultTree;
            Expr2 = (TablikListNode)new ExprParsing(Keeper, "упр", UserExpr2).ResultTree;
        }

        //Флаг для построения графа зависимости параметров
        public DfsStatus DfsStatus { get; set; }

        public void Dfs(List<TablikParam> paramsOrder) //Параметры, упорядоченные по порядку обсчета
        {
            DfsStatus = DfsStatus.Process;
            foreach (var p in UsedParams.Keys)
            {
                if (p.DfsStatus == DfsStatus.Before)
                    p.Dfs(paramsOrder);
                else if (p.DfsStatus == DfsStatus.Process)
                    Keeper.AddError("Циклическая зависимость параметров", UsedParams[p]);
            }
            paramsOrder.Add(this);
            foreach (var p in Params.Values)
                p.Dfs(paramsOrder);
            DfsStatus = DfsStatus.After;
        }
        #endregion

        #region DefineDataTypes
        //Полный тип данных параметра
        internal ITablikType Type { get; private set; }
        //Тип данных
        public DataType DataType { get { return Type.DataType; } }
        //Тип данных - простой
        public SimpleType Simple { get { return Type.Simple; } }
        //Тип данных как сигнал
        public ITablikSignalType TablikSignalType { get { return Type.TablikSignalType; } }

        //Параметры, базовые для данного
        private readonly HashSet<TablikParam> _baseParams = new HashSet<TablikParam>();
        public HashSet<TablikParam> BaseParams { get { return _baseParams; } }

        //Списки используемых сигналов и колонок для переменных типа объекта
        public SetS MetSignals { get; set; }
        public SetS MetProps { get; set; }

        //Данный параметр является наследником указанного типа
        public bool LessOrEquals(ITablikType type)
        {
            if (type is TablikParam)
                return ((TablikParam)Type).LessOrEquals(type) || BaseParams.Any(bp => bp.LessOrEquals(type));
            if (type is ITablikSignalType)
                return TablikSignalType.LessOrEquals(type);
            return Simple.LessOrEquals(type);
        }

        //Определение типов данных и формирование порожденных параметров
        public void DefineDataTypes()
        {
            foreach (var node in Expr1.Nodes)
                node.DefineType();

            var last = Expr1.Nodes.Last();
            if (last.Type.DataType != DataType.Void)
            {
                var resVar = Vars["calc"];
                resVar.Type = resVar.Type.Add(last.Type);
            }
            foreach (var node in Expr2.Nodes)
                node.DefineType();
            last = Expr2.Nodes.Last();
            Type = last.Type;
            if (Type is ObjectType || Type is BaseObjectType)
            {
                MetSignals = Keeper.GetMetSignals(last);
                MetProps = Keeper.GetMetProps(last);
            }
        }

        //Создать попрожденные параметры
        public void AddDerivedParams(string prefix, //Префикс, задаваемый вызывающими параметрами
                                                      string prefixName, //Префикс для имени
                                                      string task, //Задача
                                                      bool isCaller) //Первый в цепочке вызовов
        {
            if (Keeper.Errors.Count > 0) return;
            foreach (var p in BaseParams)
                p.AddDerivedParams(prefix, prefixName, task, false);
            if (Type is TablikParam)
                ((TablikParam)Type).AddDerivedParams(prefix, prefixName, task, false);
            foreach (var p in Params.Values)
                p.AddDerivedParams(prefix + "." + p.Code, prefixName + "." + p.Name, task, true);
            if (isCaller)
            {
                var dp = new TablikDerivedParam(Code, prefix, Name, prefixName, task, SuperProcess, Units, Min.ToDouble(), Max.ToDouble(), ObjectCode);
                Module.DerivedParams.Add(dp);
            }
        }

        #endregion
        
        //Запись результатов компиляции
        public void SaveCompileResults(IRecordAdd rec)
        {
            rec.Put("ErrMess", _errMess);
            rec.Put("ResiltType", Type.ToResString());

            var sb = new StringBuilder("Inputs: ");
            foreach (var v in InputsList)
                sb.Append(v.Code).Append("; ");
            sb.Append("Vars: ");
            foreach (var v in Vars.Values)
                if (!Inputs.ContainsKey(v.Code))
                    sb.Append(v.Code).Append("; ");

            sb.Append("Expr: ");
            foreach (var node in Expr1.Nodes)
                node.SaveCompiled(sb);
            if (Vars["calc"].Type.DataType != DataType.Void)
                sb.Append("Assign!Calc!1; ");
            foreach (var node in Expr2.Nodes)
                node.SaveCompiled(sb);
            rec.Put("CompiledExpr", sb.ToString());
            rec.Update();
        }

        //Запись в строку
        public string ToResString()
        {
            return FullCode;
        }
    }
}