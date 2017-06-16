using System;
using BaseLibrary;
using Calculation;
using CommonTypes;

namespace Tablik
{
    //Один расчетный параметр для компиляции
    public class TablikParam : BaseCalcParam, ICalcParamNode
    {
        public TablikParam(TablikModule module, IRecordRead rec, bool isSubParam) 
            : base(rec, isSubParam)
        {
            _module = module;
            if (!Code.IsCorrectCode())
            {
                ErrMess += "Указан недопустимый код расчетного параметра; ";
                _isFatalError = true;
            }

            CalcOn = rec.GetBool("CalcOn");
            UserExpr1 = rec.GetString("UserExpr1", "");
            if (UserExpr1 == "")
            {
                ErrMess += "Не заполнено расчетное выражение; ";
                _isFatalError = true;
            }
            UserExpr2 = rec.GetString("UserExpr2", "");
            if (UserExpr2 == "")
            {
                ErrMess += "Не заполнено управляющее выражение; ";
                _isFatalError = true;
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

        //Словарь расчетных параметров, ключи - коды, содержит только отмеченные и без грубых ошибок
        private readonly DicS<TablikParam> _params = new DicS<TablikParam>();
        public DicS<TablikParam> Params { get { return _params; } }
        //Словарь всех расчетных параметров, ключи - коды
        private readonly DicS<TablikParam> _paramsAll = new DicS<TablikParam>();
        public DicS<TablikParam> ParamsAll { get { return _paramsAll; } }
        //Словарь расчетных параметров, ключи - Id, содержит все параметры
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
        private bool _isFatalError;
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
            if (_isFatalError) return;
            _curExpr = "выходы.";
            ParseInputs();
            _curExpr = "расч.";
            ParseFormula(UserExpr1);
            FinishCalcParsing();
            _curExpr = "упр.";
            ParseFormula(UserExpr2);
            FinishResultParsing();
        }

        //Какое выражение обрабатывается расчетное (расч.), управляющее (упр.). список выходов (выходы.)
        private string _curExpr;

        //Формирует строку, содержащую описание лексемы
        //private string LexInf(Lexeme lex)
        //{
        //    if (lex.Token == null) return " ";
        //    return " " + lex.Token.RealText + ", " + _curExpr + "стр." + lex.Token.Line + ", поз." + lex.Token.Column + "; ";
        //}
        //private string LexInf(Token tok)
        //{
        //    if (tok == null) return " ";
        //    return " " + tok.RealText + ", " + _curExpr + "стр." + tok.Line + ", поз." + tok.Column + "; ";
        //}

        //Разбор поля Inputs
        private void ParseInputs()
        {
            throw new NotImplementedException();
        }

        //Разбор формулы
        private void ParseFormula(string formula)
        {
            throw new NotImplementedException();
        }

        //Завершение разбора расчетного выражения
        private void FinishCalcParsing()
        {
            throw new NotImplementedException();
        }

        //Завершение разбора управляющего выражения
        private void FinishResultParsing()
        {
            throw new NotImplementedException();
        }

        //Семантический анализ
        private void Semantic()
        {
            throw new NotImplementedException();        
        }

        #endregion

        
        #region Compile
        
        //Состояние компиляции параметра
        internal CompileStage Stage { get; private set; }

        //Определение типов данных и формирование порожденных параметров
        public void Compile()
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