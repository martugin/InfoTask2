﻿using System;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using BaseLibrary;

namespace CommonTypes
{
    //Накопление данных в процессе разбора (ошибки и т.п.)
    public abstract class ParsingKeeper 
    {
        //Имя разбираемого поля
        public string FieldName { get; protected set; }
        //Установить разбираемое поле
        public virtual void SetFieldName(string fieldName)
        {
            FieldName = fieldName;
        }

        //Словарь ошибок по одному на поле таблицы, ключи - имена полей
        private readonly DicS<ParsingError> _errors = new DicS<ParsingError>();
        public DicS<ParsingError> Errors { get { return _errors; } }

        //Добавить ошибку в список
        public void AddError(string errMess, string lexeme, int line, int pos, IToken token = null)
        {
            if (!Errors.ContainsKey(FieldName))
                Errors.Add(FieldName, new ParsingError(FieldName, errMess, lexeme, line, pos, token));
        }
        public void AddError(string errMess, IToken token)
        {
            if (!Errors.ContainsKey(FieldName))
                Errors.Add(FieldName, new ParsingError(FieldName, errMess, token));    
        }
        public void AddError(string errMess, ITerminalNode terminal)
        {
            if (terminal == null)
                AddError(errMess, (IToken)null);
            else AddError(errMess, terminal.Symbol);
        }

        //Накапливаемое сообщение об ошибке
        public string ErrMess
        {
            get
            {
                var s = "";
                foreach (var err in Errors.Values)
                {
                    if (s != "") s += Environment.NewLine;
                    s += err.ToString();
                }
                return s;
            }
        }

        //Проверка на недостающие и лишние скобки, если нужно, то добавляется ошибка
        public void CheckParenths(ParserRuleContext context)
        {
            var childs = context.children;
            string last = childs[childs.Count - 1].GetText();
            string prev = childs.Count == 1 ? ")" : childs[childs.Count - 2].GetText();
            int num = childs.Count ==1 || childs[0].GetText() == "(" ? 0 : 1;
            if (last != ")") AddError("Незакрытая скобка", (ITerminalNode)childs[num]);
            if (last == ")" && prev == ")")
                AddError("Лишняя закрывающаяся скобка", (ITerminalNode)childs.Last());
        }
        public void CheckSquareParenths(ParserRuleContext context)
        {
            var childs = context.children;
            string last = childs[childs.Count - 1].GetText();
            string prev = childs.Count == 1 ? "]" : childs[childs.Count - 2].GetText();
            int num = childs.Count == 1 || childs[0].GetText() == "[" ? 0 : 1;
            if (last != "]") AddError("Незакрытая квадратная скобка", (ITerminalNode)childs[num]);
            if (last == "]" && prev == "]")
                AddError("Лишняя закрывающаяся квадратная скобка", (ITerminalNode)childs.Last());
        }
        public void CheckFigureParenths(ParserRuleContext context)
        {
            var childs = context.children;
            string last = childs[childs.Count - 1].GetText();
            string prev = childs.Count == 1 ? "}" : childs[childs.Count - 2].GetText();
            int num = childs.Count == 1 || childs[0].GetText() == "{" ? 0 : 1;
            if (last != "}") AddError("Незакрытая фигурная скобка", (ITerminalNode)childs[num]);
            if (last == "}" && prev == "}")
                AddError("Лишняя закрывающаяся фигурная скобка", (ITerminalNode)childs.Last());
        }

        //Методы создания узлов - констант разного типа
        protected abstract Node MakeNodeConst(ITerminalNode terminal, bool b);
        protected abstract Node MakeNodeConst(ITerminalNode terminal, int i);
        protected abstract Node MakeNodeConst(ITerminalNode terminal, double r);
        protected abstract Node MakeNodeConst(ITerminalNode terminal, DateTime d);
        protected abstract Node MakeNodeConst(ITerminalNode terminal, string s);

        //Обработка токена целого числа
        public Node GetIntConst(ITerminalNode terminal)
        {
            if (terminal == null || terminal.Symbol == null)
                return MakeNodeConst(null, 0);
            int res;
            if (!int.TryParse(terminal.Symbol.Text, out res))
                AddError("Недопустимое целое число", terminal);
            if (res == 1) return MakeNodeConst(terminal, true);
            if (res == 0) return MakeNodeConst(terminal, false);
            return MakeNodeConst(terminal, res);
        }
        
        //Обработка токена действительного числа 
        public Node GetRealConst(ITerminalNode terminal)
        {
            if (terminal == null || terminal.Symbol == null)
                return MakeNodeConst(null, 0.0);
            var token = terminal.Symbol;
            var d = token.Text.ToDouble();
            if (double.IsNaN(d))
            {
                AddError("Недопустимое число с плавающей точкой", token);
                d = 0;
            }
            return MakeNodeConst(terminal, d);
        }

        //Обработка токена временной константы
        public Node GetTimeConst(ITerminalNode terminal)
        {
            if (terminal == null || terminal.Symbol == null)
                return MakeNodeConst(null, Different.MinDate);
            var token = terminal.Symbol;
            var t = token.Text.ToDateTime();
            if (t == Different.MinDate)
                AddError("Недопустимое выражение времени", token);
            return MakeNodeConst(terminal, t);
        }

        //Обработка токена строковой константы
        public Node GetStringConst(ITerminalNode terminal, 
                                                  bool delAportrof) //Удалять кавычки
        {
            if (terminal == null || terminal.Symbol == null)
                return MakeNodeConst(null, "");
            var text = terminal.Symbol.Text;
            return MakeNodeConst(terminal, delAportrof ? text.Substring(1, text.Length - 2) : text);
        }
    }
}