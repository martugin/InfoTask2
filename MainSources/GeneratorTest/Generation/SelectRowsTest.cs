using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using BaseLibraryTest;
using Calculation;
using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorTest
{
    [TestClass]
    public class SelectRowsTest
    {
        //Загрузка таблиц
        private TablsList Load(string prefix)
        {
            var tabls = new TablsList();
            using (var db = new DaoDb(TestLib.CopyFile("Generator", "GenData.accdb", "Rows" + prefix + ".accdb")))
            {
                tabls.AddStruct(db, "Tabl", "SubTabl", "SubSubTabl");
                tabls.AddDbStructs(db);
                tabls.LoadValues(db, true);
            }
            return tabls;
        }

        //Создать накопитель ошибок
        private static GenKeeper MakeKeeper()
        {
            var logger = new Logger(new AppIndicator());
            logger.History = new TestHistory(logger);
            return new GenKeeper(new ModuleGenerator(logger, null, null, null));
        }

        //Выбрать ряды по условию генерации таблицы, возвращает ряды и, возможно, также структуру
        private SubRows[] SelectRowsS(GenKeeper keeper, TablsList tabls, string formula)
        {
            keeper.Errors.Clear();
            var parsing = new RuleParsing(keeper, "поле", formula);
            if (parsing.ResultTree == null) return null;
            var node = (NodeRTabl)parsing.ResultTree;
            node.Check(tabls, null);
            return node.SelectRows(tabls, null).ToArray();
        }
        private TablRow[] SelectRows(GenKeeper keeper, TablsList tabls, string formula)
        {
            return SelectRowsS(keeper, tabls, formula).Cast<TablRow>().ToArray();
        }
        private IEnumerable<SubRows> SelectRowsStruct(GenKeeper keeper, TablsList tabls, string formula, out ITablStruct tstruct)
        {
            tstruct = null;
            keeper.Errors.Clear();
            var parsing = new RuleParsing(keeper, "поле", formula);
            if (parsing.ResultTree == null) return null;
            var node = (NodeRTabl)parsing.ResultTree;
            tstruct = node.Check(tabls, null);
            return node.SelectRows(tabls, null);
        }
        //Выбрать ряды по условию генерации подтаблицы
        private SubRows[] SelectSubRowsS(GenKeeper keeper, TablsList tabls, ITablStruct tstruct, SubRows rows, string formula)
        {
            keeper.Errors.Clear();
            var parsing = new SubRuleParsing(keeper, "поле", formula);
            if (parsing.ResultTree == null) return null;
            var node = (SubNodeR)parsing.ResultTree;
            node.Check(tabls, tstruct);
            return node.SelectRows(tabls, rows).ToArray();
        }
        private TablRow[] SelectSubRows(GenKeeper keeper, TablsList tabls, ITablStruct tstruct, SubRows rows, string formula)
        {
            return SelectSubRowsS(keeper, tabls, tstruct, rows, formula).Cast<TablRow>().ToArray();
        }


        [TestMethod]
        public void OneLevel()
        {
            var tabls = Load("Simple");
            var keeper = MakeKeeper();
            var rows = SelectRows(keeper, tabls, "Tabl");
            Assert.AreEqual(3, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);
            Assert.AreEqual(12, rows[1].Num);
            Assert.AreEqual("sSs", rows[2]["StringField"].String);
            Assert.AreEqual(777, rows[2]["IntField"].Integer);
            Assert.AreEqual(true, rows[2]["BoolField"].Boolean);

            rows = SelectRows(keeper, tabls, "Tabl(TypeRec=='type')");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);
            Assert.AreEqual("s2", rows[1].Code);

            rows = SelectRows(keeper, tabls, "Tabl(TypeRec=='ttype')");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("s3", rows[0].Code);

            rows = SelectRows(keeper, tabls, "Tabl(True)");
            Assert.AreEqual(3, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);
            Assert.AreEqual("s2", rows[1].Code);
            Assert.AreEqual("s3", rows[2].Code);

            rows = SelectRows(keeper, tabls, "Tabl(IntField==666)");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("s2", rows[0].Code);

            rows = SelectRows(keeper, tabls, "Tabl(RealField >= 123.5)");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("s2", rows[0].Code);
            Assert.AreEqual("s3", rows[1].Code);

            rows = SelectRows(keeper, tabls, "Tabl(StringFeld=='fff')");
            Assert.AreEqual(0, rows.Length);

            rows = SelectRows(keeper, tabls, "Tabl(StringField like '*S?')");
            Assert.AreEqual(3, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);
            Assert.AreEqual("s2", rows[1].Code);
            Assert.AreEqual("s3", rows[2].Code);

            rows = SelectRows(keeper, tabls, "Tabl(IntField < 700 And (TimeField > #26.09.2016 12:00:00# Or Not -RealField <> -123.4))");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);
            Assert.AreEqual("s2", rows[1].Code);

            rows = SelectRows(keeper, tabls, "Tabl(StrRight(StrTrim(StringField);2) == 'SS' and StrLen(NameField == 3))");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);
            Assert.AreEqual("s2", rows[1].Code);

            rows = SelectRows(keeper, tabls, "Tabl(IntField == 650+2*(3+1)-(-8*1))");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("s2", rows[0].Code);

            rows = SelectRows(keeper, tabls, "Tabl.SubTabl");
            Assert.AreEqual(4, rows.Length);
            Assert.AreEqual("a", rows[0].Code);
            Assert.AreEqual(2, rows[1].Num);
            Assert.AreEqual(1, rows[3]["RealSub"].Real);

            rows = SelectRows(keeper, tabls, "Tabl('type').SubTabl");
            Assert.AreEqual(4, rows.Length);
            Assert.AreEqual("a", rows[0].Code);
            Assert.AreEqual("b", rows[1].Code);
            Assert.AreEqual("c", rows[2].Code);
            Assert.AreEqual("1", rows[3].Code);

            rows = SelectRows(keeper, tabls, "Tabl(NameField == 'JJJ').SubTabl");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("1", rows[0].Code);

            rows = SelectRows(keeper, tabls, "Tabl.SubTabl(TypeRec=='a')");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("a", rows[0].Code);

            rows = SelectRows(keeper, tabls, "Tabl.SubTabl(Num==1)");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("a", rows[0].Code);
            Assert.AreEqual("1", rows[1].Code);

            rows = SelectRows(keeper, tabls, "Tabl(Code=='s3').SubTabl");
            Assert.AreEqual(0, rows.Length);

            rows = SelectRows(keeper, tabls, "Tabl(StrLen(NameField)==3).SubTabl(StrLen(NameSub) > 0 And RealSub < 1.2)");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("a", rows[0].Code);
            Assert.AreEqual("1", rows[1].Code);

            rows = SelectRows(keeper, tabls, "Tabl.SubTabl.SubTabl");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("qqq", rows[0].Code);
            Assert.AreEqual("ppp", rows[1].Code);

            rows = SelectRows(keeper, tabls, "Tabl(Code=='s1').SubTabl(Code=='a').SubTabl(Code=='ppp')");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("ppp", rows[0].Code);

            var grows = SelectRowsS(keeper, tabls, "Tabl.Group()");
            Assert.AreEqual(1, grows.Length);
            Assert.AreEqual(0, grows[0].Means.Count);

            grows = SelectRowsS(keeper, tabls, "Tabl.Group(TypeRec)");
            Assert.AreEqual(2, grows.Length);
            Assert.AreEqual(1, grows[0].Means.Count);
            Assert.AreEqual("type", grows[0]["TypeRec"].String);
            Assert.AreEqual("ttype", grows[1]["TypeRec"].String);

            grows = SelectRowsS(keeper, tabls, "Tabl.Group(IntField; NameField; Code)");
            Assert.AreEqual(3, grows.Length);
            Assert.AreEqual(3, grows[0].Means.Count);
            Assert.AreEqual("s1", grows[0]["Code"].String);
            Assert.AreEqual("HHH", grows[0]["NameField"].String);
            Assert.AreEqual(555, grows[0]["IntField"].Integer);
            Assert.AreEqual(1, grows[0].SubList.Count);
            Assert.AreEqual(11, grows[0].SubList[0].Num);
            Assert.AreEqual("s1", grows[0].SubList[0].Code);

            Assert.AreEqual("s2", grows[1]["Code"].String);
            Assert.AreEqual("JJJ", grows[1]["NameField"].String);
            Assert.AreEqual(666, grows[1]["IntField"].Integer);
            Assert.AreEqual(1, grows[1].SubList.Count);
            Assert.AreEqual(12, grows[1].SubList[0].Num);
            Assert.AreEqual("s2", grows[1].SubList[0].Code);

            Assert.AreEqual("s3", grows[2]["Code"].String);
            Assert.AreEqual("DDD", grows[2]["NameField"].String);
            Assert.AreEqual(777, grows[2]["IntField"].Integer);
            Assert.AreEqual(1, grows[2].SubList.Count);
            Assert.AreEqual(11, grows[2].SubList[0].Num);
            Assert.AreEqual("s3", grows[2].SubList[0].Code);

            grows = SelectRowsS(keeper, tabls, "Tabl.SubTabl(Num<>3).Group(Num)");
            Assert.AreEqual(2, grows.Length);
            Assert.AreEqual(1, grows[0].Means.Count);
            Assert.AreEqual(1, grows[0]["Num"].Integer);
            Assert.AreEqual(2, grows[0].SubList.Count);
            Assert.AreEqual("a", grows[0].SubList[0].Code);
            Assert.AreEqual("1", grows[0].SubList[1].Code);

            Assert.AreEqual(2, grows[1]["Num"].Integer);
            Assert.AreEqual(1, grows[1].SubList.Count);
            Assert.AreEqual("b", grows[1].SubList[0].Code);
        }

        [TestMethod]
        public void TwoLevel()
        {
            var tabls = Load("Complex");
            var keeper = MakeKeeper();
            ITablStruct tstruct;
            var t = SelectRowsStruct(keeper, tabls, "Tabl.Group", out tstruct);
            Assert.AreEqual(1, t.Count());
            var tabl = t.First();
            Assert.IsNull(tabl.Parent);
            Assert.AreEqual(3, tabl.SubList.Count);

            var rows = SelectSubRows(keeper, tabls, tstruct, tabl, "SubTabl");
            Assert.AreEqual(3, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);
            Assert.AreEqual("s2", rows[1].Code);
            Assert.AreEqual("s3", rows[2].Code);

            rows = SelectSubRows(keeper, tabls, tstruct, tabl, "SubTabl(IntField < 700 And BoolField)");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);

            rows = SelectSubRows(keeper, tabls, tstruct, tabl, "SubTabl.SubTabl(RealSub==2)");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("b", rows[0].Code);

            var grows = SelectSubRowsS(keeper, tabls, tstruct, tabl, "SubTabl.SubTabl.Group(TypeRec)");
            Assert.AreEqual(4, grows.Length);
            Assert.AreEqual("a", grows[0]["TypeRec"].String);
            Assert.AreEqual(1, grows[0].SubList.Count);
            Assert.AreEqual("b", grows[1]["TypeRec"].String);
            Assert.AreEqual(1, grows[1].SubList.Count);
            Assert.AreEqual("c", grows[2]["TypeRec"].String);
            Assert.AreEqual(1, grows[2].SubList.Count);
            Assert.AreEqual("", grows[3]["TypeRec"].String);
            Assert.AreEqual(1, grows[3].SubList.Count);

            t = SelectRowsStruct(keeper, tabls, "Tabl(StrLCase(NameField)=='hhh')", out tstruct);
            Assert.AreEqual(1, t.Count());
            tabl = t.First();
            Assert.AreEqual(3, tabl.SubCodes.Count);
            Assert.AreEqual(3, tabl.SubNums.Count);

            rows = SelectSubRows(keeper, tabls, tstruct, tabl, "SubTabl");
            Assert.AreEqual(3, rows.Length);
            Assert.AreEqual("a", rows[0].Code);
            Assert.AreEqual("b", rows[1].Code);
            Assert.AreEqual("c", rows[2].Code);

            rows = SelectSubRows(keeper, tabls, tstruct, tabl, "SubTabl(BoolSub)");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("a", rows[0].Code);
            Assert.AreEqual("c", rows[1].Code);

            rows = SelectSubRows(keeper, tabls, tstruct, tabl, "SubTabl(BoolSub And StringSub=='' )");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("a", rows[0].Code);
            Assert.AreEqual("c", rows[1].Code);

            rows = SelectSubRows(keeper, tabls, tstruct, tabl, "SubTabl.SubTabl");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("qqq", rows[0].Code);
            Assert.AreEqual("ppp", rows[1].Code);
            
            t = SelectRowsStruct(keeper, tabls, "VTZTZ(SysNumVTZ==43014)", out tstruct);
            Assert.AreEqual(1, t.Count());
            tabl = t.First();
            Assert.AreEqual(5, tabl.SubCodes.Count);
            Assert.AreEqual(5, tabl.SubList.Count);

            rows = SelectSubRows(keeper, tabls, tstruct, tabl, "SubTabl");
            Assert.AreEqual(5, rows.Length);
            Assert.AreEqual("1NZ00Z803-1", rows[0].Code);
            Assert.AreEqual("1NZ00Z804-1", rows[1].Code);
            Assert.AreEqual("ТЗ_З00210110095", rows[2].Code);
            Assert.AreEqual("ТЗ_З00210110208", rows[3].Code);
            Assert.AreEqual("ТЗ_З00210110212", rows[4].Code);

            rows = SelectSubRows(keeper, tabls, tstruct, tabl, "SubTabl(Code Like 'ТЗ*')");
            Assert.AreEqual(3, rows.Length);
            Assert.AreEqual("ТЗ_З00210110095", rows[0].Code);
            Assert.AreEqual("ТЗ_З00210110208", rows[1].Code);
            Assert.AreEqual("ТЗ_З00210110212", rows[2].Code);

            rows = SelectSubRows(keeper, tabls, tstruct, tabl, "SubTabl(UnitTypeTz <> 'ТЗ' ИЛИ SysNumTz < 45000)");
            Assert.AreEqual(5, rows.Length);
            Assert.AreEqual("1NZ00Z803-1", rows[0].Code);
            Assert.AreEqual("1NZ00Z804-1", rows[1].Code);
            Assert.AreEqual("ТЗ_З00210110095", rows[2].Code);
            Assert.AreEqual("ТЗ_З00210110208", rows[3].Code);
            Assert.AreEqual("ТЗ_З00210110212", rows[4].Code);
        }
    }
}
