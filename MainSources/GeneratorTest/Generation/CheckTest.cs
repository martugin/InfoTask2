using System;
using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorTest
{
    [TestClass]
    public class CheckTest
    {
        //Загрузка таблиц
        private TablsList Load()
        {
            var tabls = new TablsList();
            using (var db = new DaoDb(TestLib.CopyFile(@"Generator\GenData.accdb")))
            {
                tabls.AddStruct(db, "Tabl", "SubTabl", "SubSubTabl");
                tabls.AddDbStructs(db);    
                tabls.LoadValues(db);
            }
            return tabls;
        }


        //Разбор выражения GenRule таблицы и подтаблицы
        private TablStruct CheckRule(GenKeeper keeper, TablsList tabls, string formula)
        {
            keeper.Errors.Clear();
            var parsing = new RuleParsing(keeper, "поле", formula);
            if (parsing.ResultTree == null) return null;
            return ((INodeTabl) parsing.ResultTree).Check(tabls);
        }
        private TablStruct CheckSubRule(GenKeeper keeper, TablStruct tstruct, string formula)
        {
            keeper.Errors.Clear();
            var parsing = new SubRuleParsing(keeper, "поле", formula);
            if (parsing.ResultTree == null) return null;
            return ((NodeRSubTabl)parsing.ResultTree).Check(tstruct);
        }
        private DataType CheckField(GenKeeper keeper, TablStruct tstruct, string formula)
        {
            keeper.Errors.Clear();
            var parsing = new FieldsParsing(keeper, "поле", formula);
            if (parsing.ResultTree == null) return DataType.Error;
            return ((INodeExpr)parsing.ResultTree).Check(tstruct);
        }

        [TestMethod]
        public void CheckRule()
        {
            var tabls = Load();
            var keeper = new GenKeeper(new TablGenerator(new Logger(), null, null, null, null, null, null));
            var tstruct = CheckRule(keeper, tabls, "Tabl");
            Assert.AreEqual(0, tstruct.Level);
            Assert.AreEqual("Tabl", tstruct.TableName);
            Assert.IsNotNull(tstruct.Parent);
            Assert.IsNotNull(tstruct.Child);
            Assert.AreEqual(10, tstruct.Fields.Count);
            Assert.AreEqual(0, keeper.Errors.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckSubRule(keeper, tstruct, "SubTabl");
            Assert.AreEqual(1, tstruct.Level);
            Assert.AreEqual("SubTabl", tstruct.TableName);
            Assert.IsNotNull(tstruct.Parent);
            Assert.IsNotNull(tstruct.Child);
            Assert.AreEqual(11, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "Tabl('type')");
            Assert.AreEqual(0, tstruct.Level);
            Assert.AreEqual("Tabl", tstruct.TableName);
            Assert.AreEqual(10, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckSubRule(keeper, tstruct, "SubTabl(Code=='a')");
            Assert.AreEqual(1, tstruct.Level);
            Assert.AreEqual("SubTabl", tstruct.TableName);
            Assert.AreEqual(11, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "Tabl.SubTabl");
            Assert.AreEqual(1, tstruct.Level);
            Assert.AreEqual("SubTabl", tstruct.TableName);
            Assert.IsNotNull(tstruct.Parent);
            Assert.IsNotNull(tstruct.Child);
            Assert.AreEqual(11, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckSubRule(keeper, tstruct, "SubTabl");
            Assert.AreEqual(2, tstruct.Level);
            Assert.AreEqual("SubSubTabl", tstruct.TableName);
            Assert.IsNotNull(tstruct.Parent);
            Assert.IsNull(tstruct.Child);
            Assert.AreEqual(11, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckSubRule(keeper, tstruct, "SubTabl");
            Assert.IsNull(tstruct);
            Assert.AreEqual(1, keeper.Errors.Count);
            Assert.AreEqual("Подтаблица отстутствует, 'SubTabl' (поле, строка: 1, позиция: 1)", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "Tabl.OverTabl");
            Assert.AreEqual(-1, tstruct.Level);
            Assert.AreEqual("", tstruct.TableName);
            Assert.IsNull(tstruct.Parent);
            Assert.IsNotNull(tstruct.Child);
            Assert.AreEqual(0, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckSubRule(keeper, tstruct, "SubTabl(Code=='s1') /*И вааще*/");
            Assert.AreEqual(0, tstruct.Level);
            Assert.AreEqual("Tabl", tstruct.TableName);
            Assert.IsNotNull(tstruct.Parent);
            Assert.IsNotNull(tstruct.Child);
            Assert.AreEqual(10, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "Tabl(BoolField And RealField > 123 " + Environment.NewLine + " Or Not NameField <> 'JJJ')");
            Assert.AreEqual(0, tstruct.Level);
            Assert.AreEqual("Tabl", tstruct.TableName);
            Assert.IsNotNull(tstruct.Parent);
            Assert.IsNotNull(tstruct.Child);
            Assert.AreEqual(10, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckSubRule(keeper, tstruct, "SubTabl(NameSub Like 'ss*' Или -Cos(-RealSub) <= 0.2*IntSub ИсклИли StrLeft(NameSub;2) == 'ff')");
            Assert.AreEqual(1, tstruct.Level);
            Assert.AreEqual("SubTabl", tstruct.TableName);
            Assert.AreEqual(11, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "Tabl(1 Or True Or False)");
            Assert.AreEqual(0, tstruct.Level);
            Assert.AreEqual("Tabl", tstruct.TableName);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "Tabl(BoolField==True).SubTabl('a').SubTabl((NameName == 'qqq') or ( Num >= RealReal * Pi() * ((-IntInt + 2) Div Id)))");
            Assert.AreEqual(2, tstruct.Level);
            Assert.AreEqual("SubSubTabl", tstruct.TableName);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "Tabl(IntField)");
            Assert.AreEqual(1, keeper.Errors.Count);
            Assert.AreEqual("Недопустимый тип данных условия, 'Tabl' (поле, строка: 1, позиция: 1)", keeper.ErrMess);
            Assert.AreEqual(0, tstruct.Level);
            Assert.AreEqual("Tabl", tstruct.TableName);

            tstruct = CheckRule(keeper, tabls, "Tabl(RealField + IntField / 2.4)");
            Assert.AreEqual(1, keeper.Errors.Count);
            Assert.AreEqual("Недопустимый тип данных условия, 'Tabl' (поле, строка: 1, позиция: 1)", keeper.ErrMess);
            Assert.AreEqual(0, tstruct.Level);
            Assert.AreEqual("Tabl", tstruct.TableName);

            CheckRule(keeper, tabls, "Tabl(1+)");
            Assert.AreEqual("Недопустимое использование лексемы, ')' (поле, строка: 1, позиция: 8)", keeper.ErrMess);

            CheckRule(keeper, tabls, "Tabl(");
            Assert.AreEqual("Незакрытая скобка, '(' (поле, строка: 1, позиция: 5)", keeper.ErrMess);

            CheckRule(keeper, tabls, "Tabl(Field)");
            Assert.AreEqual("Поле не найдено в исходной таблице, 'Field' (поле, строка: 1, позиция: 6)", keeper.ErrMess);

            CheckRule(keeper, tabls, "Tabl(RealField + Exp(Field))");
            Assert.AreEqual("Поле не найдено в исходной таблице, 'Field' (поле, строка: 1, позиция: 22)", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "Tabl");

            CheckSubRule(keeper, tstruct, "SubTabl(2+3 log(1))");
            Assert.AreEqual("Недопустимое использование лексемы, 'log' (поле, строка: 1, позиция: 13)", keeper.ErrMess);

            CheckSubRule(keeper, tstruct, "SubTabl(Mid('s';1)==0)");
            Assert.AreEqual("Неизвестная функция, 'Mid' (поле, строка: 1, позиция: 9)", keeper.ErrMess);

            CheckSubRule(keeper, tstruct, "SubTabl(StrMid('s')==0)");
            Assert.AreEqual("Недопустимые типы данных параметров функции, 'StrMid' (поле, строка: 1, позиция: 9)", keeper.ErrMess);

            CheckSubRule(keeper, tstruct, "SubTabl(Sin(RealSub;3)==0)");
            Assert.AreEqual("Недопустимые типы данных параметров функции, 'Sin' (поле, строка: 1, позиция: 9)", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "VtzTz(Code=='ВТЗЗ00120101002')");
            Assert.AreEqual(0, tstruct.Level);
            Assert.AreEqual("Tbl_VTZTZ_Sub0", tstruct.TableName);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "VtzTz(NameVTZ Like '*ост*' And (PriorVTZ > 1))");
            Assert.AreEqual(0, tstruct.Level);
            Assert.AreEqual("Tbl_VTZTZ_Sub0", tstruct.TableName);
            Assert.AreEqual(8, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckSubRule(keeper, tstruct, "SubTabl((SysNumTZ >= 40000) And (ParentId < 500))");
            Assert.AreEqual(1, tstruct.Level);
            Assert.AreEqual("Tbl_VTZTZ_Sub1", tstruct.TableName);
            Assert.AreEqual(8, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "VtzTz.SubTabl(STRTRIM(STRUCASE(Code))== 'ТЗ_З00120101004')");
            Assert.AreEqual(1, tstruct.Level);
            Assert.AreEqual("Tbl_VTZTZ_Sub1", tstruct.TableName);
            Assert.AreEqual(8, tstruct.Fields.Count);
            Assert.AreEqual("", keeper.ErrMess);

            CheckRule(keeper, tabls, "VtzTz(UnitTypeVTZ * StrLen(NameVTZ;1) + Field)");
            Assert.AreEqual("Недопустимые типы данных параметров функции, 'StrLen' (поле, строка: 1, позиция: 21)", keeper.ErrMess);
        }

        [TestMethod]
        public void CheckFields()
        {
            var tabls = Load();
            var keeper = new GenKeeper(new TablGenerator(new Logger(), null, null, null, null, null, null));
            var tstruct = CheckRule(keeper, tabls, "Tabl");
            
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "aaa"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[IntField]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Boolean, CheckField(keeper, tstruct, "[BoolField]"));
            Assert.AreEqual("", keeper.ErrMess);

            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "aaa_[Code]_2"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[RealField]_"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[Id][IntField]"));
            Assert.AreEqual("", keeper.ErrMess);

            Assert.AreEqual(DataType.Boolean, CheckField(keeper, tstruct, "[1]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Real, CheckField(keeper, tstruct, "[1234.789]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[8][IntField][1]"));
            Assert.AreEqual("", keeper.ErrMess);

            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[a=5:a]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[a=5][a]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[a=5]sss[b=4]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Value, CheckField(keeper, tstruct, "[a=5]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Real, CheckField(keeper, tstruct, "[a=True][a=StrLen('eee'):a=a+0.2][a]"));
            Assert.AreEqual("", keeper.ErrMess);

            Assert.AreEqual(DataType.Real, CheckField(keeper, tstruct, "[2 * (3 + 1,5 + 2,0)]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[1+Cos((Pi+1)/2/*ooo*/)][BoolField]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[1 or True] "));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[Sign(Sqr(RealField + Log(2;IntField)))-1]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[StrReplace(StringField;'S';'uu')]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Boolean, CheckField(keeper, tstruct, "[NameField Like Code]"));
            Assert.AreEqual("", keeper.ErrMess);

            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[If(Code == 's1';IntField;3)]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Real, CheckField(keeper, tstruct, "[Если(True;IntField;False();3.1+1;BoolField And (IntField==5);56 div 3;7)]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Real, CheckField(keeper, tstruct, "[While(True;1;2,3)]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[i=0:s=0:While(i<IntField;s=s+3:s;0)]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[If(IntField < 10; a = 1; a = 2): a]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Real, CheckField(keeper, tstruct, "[n = 3: While(n > 0.001; n = n / 2): n]"));
            Assert.AreEqual("", keeper.ErrMess);

            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[[aaa]]"));
            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[StrLen([aaa])]"));
            Assert.AreEqual(DataType.Real, CheckField(keeper, tstruct, "[a=1:If(True;[[a=2][a]];[[a=4.5][a]]]"));
            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[a=[[IntField+1]]:a]"));

            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[OverTabl(a=2)]aaa"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Boolean, CheckField(keeper, tstruct, "[OverTabl(1)]"));
            Assert.AreEqual("", keeper.ErrMess);

            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[SubTabl(NameSub)]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[SubTablCond(True;NameSub)]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[SubTabl(NameSub;';')]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[SubTablCond('type';IntSub;[_])]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[SubTablCond('type';[[Code]_[NameSub]];[_])]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[n = 0: SubTabl(n=n+IntSub): n]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Real, CheckField(keeper, tstruct, "[n = 0: SubTabl(n=n+RealSub): n]"));
            Assert.AreEqual("", keeper.ErrMess);

            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[OverTabl(SubTabl(NameField;';'))]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[OverTabl(SubTabl(Num==11;NameField))]"));
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckSubRule(keeper, tstruct, "SubTabl");
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "bbb"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Real, CheckField(keeper, tstruct, "[RealSub]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[StrTrim(Code+NameSub)]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Real, CheckField(keeper, tstruct, "[If(BoolSub;IntSub;RealSub)]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[OverTabl(IntField)]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "[SubTablCond(IntInt==222;NameName;'_')]"));
            Assert.AreEqual("", keeper.ErrMess);

            tstruct = CheckRule(keeper, tabls, "VtzTz");
            Assert.AreEqual(DataType.Integer, CheckField(keeper, tstruct, "[SysNumVtz]"));
            Assert.AreEqual("", keeper.ErrMess);
            Assert.AreEqual(DataType.String, CheckField(keeper, tstruct, "ss_[SubTabl(NameTz;Id)]"));
            Assert.AreEqual("", keeper.ErrMess);
        }
    }
}