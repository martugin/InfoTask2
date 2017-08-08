using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    //Узел, задающий обращение к таблице
    internal class TablNode : TablikKeeperNode
    {
        public TablNode(TablikKeeper keeper, ITerminalNode fun, ITerminalNode tabl, ITerminalNode field, params IExprNode[] args) 
            : base(keeper, fun, args)
        {
            Fun = Keeper.FunsChecker.Funs[Token.Text];
            var t = tabl.Symbol.Text;
            if (Keeper.Module.Tabls.Structs.ContainsKey(t))
                Tabl = Keeper.Module.Tabls.Structs[t];
            else foreach (var m in Keeper.Module.LinkedModules)
                if (m.Tabls.Structs.ContainsKey(t))
                {
                    if (Tabl != null) AddError("Таблица присутствует сразу в нескольких модулях");
                    else Tabl = m.Tabls.Structs[t];
                }
            if (Tabl == null) AddError("Не найдена таблица");
            else
            {
                int tnum = Fun.Code == "Tabl" || Fun.Code == "TablContains" ? Args.Length - 1 : Args.Length;
                if (Tabl.Tabls.Count - 1 < tnum)
                    AddError("Недопустимое количество аргументов функции");
                else if (field != null) 
                {
                    Field = field.Symbol.Text;
                    var fields = Tabl.Tabls[tnum].Fields;
                    if (!fields.ContainsKey(Field))
                        AddError("В таблице не найдено указанное поле", field.Symbol);
                    else FieldType = fields[Field];
                }
            }
        }

        //Тип узла
        protected override string NodeType { get { return "Tabl"; } }

        //Функция обращения к таблице
        public FunCompile Fun { get; private set; }
        //Таблица
        public TablGroup Tabl { get; private set; }

        //Имя поля 
        public string Field { get; private set; }
        //Тип поля
        public DataType FieldType { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            switch (Fun.Code)
            {
                case "TablContains":
                    Type = new SimpleType(DataType.Boolean);        
                    break;
                case "Tabl":
                    Type = new SimpleType(FieldType);
                    break;
                case "TablList":
                    Type = new SimpleType(FieldType, ArrayType.List);
                    break;
                case "TablDicNumbers":
                    Type = new SimpleType(FieldType, ArrayType.DicNumbers);
                    break;
                case "TablDicStrings":
                    Type = new SimpleType(FieldType, ArrayType.DicStrings);
                    break;
            }

            if (Args.Select(x => !x.Type.DataType.LessOrEquals(DataType.String)).Any())
                AddError("Недопустимые типы аргументов функции");
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return Tabl.Code + (Field == null ? "" : "." + Field);
        }

        public override string ToTestString()
        {
            return ToTestWithChildren(Args);
        }
    }
}