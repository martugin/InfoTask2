using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
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
                tabls.LoadValues(db);
            }
            return tabls;
        }

        //Создать накопитель ошибок
        private static GenKeeper MakeKeeper()
        {
            return new GenKeeper(new TablGenerator(new Logger(), null, null, null, null, null, null));
        }

        //Выбрать ряды по условию генерации таблицы, возвращает ряды и, возможно, также структуру
        private TablRow[] SelectRows(GenKeeper keeper, TablsList tabls, string formula)
        {
            keeper.Errors.Clear();
            var parsing = new RuleParsing(keeper, "поле", formula);
            if (parsing.ResultTree == null) return null;
            var node = (INodeTabl) parsing.ResultTree;
            node.Check(tabls);
            return node.SelectRows(tabls).Cast<TablRow>().ToArray();
        }
        private IEnumerable<SubRows> SelectRowsStruct(GenKeeper keeper, TablsList tabls, string formula, out TablStruct tstruct)
        {
            tstruct = null;
            keeper.Errors.Clear();
            var parsing = new RuleParsing(keeper, "поле", formula);
            if (parsing.ResultTree == null) return null;
            var node = (INodeTabl)parsing.ResultTree;
            tstruct = node.Check(tabls);
            return node.SelectRows(tabls);
        }
        //Выбрать ряды по условию генерации подтаблицы
        private TablRow[] SelectSubRows(GenKeeper keeper, TablStruct tstruct, SubRows rows, string formula)
        {
            keeper.Errors.Clear();
            var parsing = new SubRuleParsing(keeper, "поле", formula);
            if (parsing.ResultTree == null) return null;
            var node = (NodeRSubTabl)parsing.ResultTree;
            node.Check(tstruct);
            return node.SelectRows(rows).Cast<TablRow>().ToArray();
        }

        [TestMethod]
        public void Simple()
        {
            var tabls = Load("Simple");
            var keeper = MakeKeeper();
            var rows = SelectRows(keeper, tabls, "Tabl");
            Assert.AreEqual(3, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);
            Assert.AreEqual(12, rows[1].Num);
            Assert.AreEqual("type", rows[1].Type);
            Assert.AreEqual("sSs", rows[2]["StringField"].String);
            Assert.AreEqual(777, rows[2]["IntField"].Integer);
            Assert.AreEqual(true, rows[2]["BoolField"].Boolean);

            rows = SelectRows(keeper, tabls, "Tabl('type')");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);
            Assert.AreEqual("s2", rows[1].Code);

            rows = SelectRows(keeper, tabls, "Tabl('ttype')");
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
            Assert.AreEqual("c", rows[2].Type);
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

            rows = SelectRows(keeper, tabls, "Tabl.SubTabl('a')");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("a", rows[0].Code);

            rows = SelectRows(keeper, tabls, "Tabl('type').SubTabl('b')");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("b", rows[0].Code);

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
        }

        [TestMethod]
        public void Complex()
        {
            var tabls = Load("Complex");
            var keeper = MakeKeeper();
            TablStruct tstruct;
            var t = SelectRowsStruct(keeper, tabls, "Tabl.OverTabl", out tstruct);
            Assert.AreEqual(1, t.Count());
            var tabl = t.First();
            Assert.IsNull(tabl.Parent);
            Assert.AreEqual(3, tabl.SubCodes.Count);
            Assert.AreEqual(2, tabl.SubTypes.Count);

            var rows = SelectSubRows(keeper, tstruct, tabl, "SubTabl");
            Assert.AreEqual(3, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);
            Assert.AreEqual("s2", rows[1].Code);
            Assert.AreEqual("s3", rows[2].Code);

            rows = SelectSubRows(keeper, tstruct, tabl, "SubTabl(IntField < 700 And BoolField)");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("s1", rows[0].Code);

            rows = SelectSubRows(keeper, tstruct, tabl, "SubTabl.SubTabl(RealSub==2)");
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("b", rows[0].Code);

            t = SelectRowsStruct(keeper, tabls, "Tabl(StrLCase(NameField)=='hhh')", out tstruct);
            Assert.AreEqual(1, t.Count());
            tabl = t.First();
            Assert.AreEqual(3, tabl.SubCodes.Count);
            Assert.AreEqual(3, tabl.SubNums.Count);

            rows = SelectSubRows(keeper, tstruct, tabl, "SubTabl");
            Assert.AreEqual(3, rows.Length);
            Assert.AreEqual("a", rows[0].Code);
            Assert.AreEqual("b", rows[1].Code);
            Assert.AreEqual("c", rows[2].Code);

            rows = SelectSubRows(keeper, tstruct, tabl, "SubTabl(BoolSub)");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("a", rows[0].Code);
            Assert.AreEqual("c", rows[1].Code);

            rows = SelectSubRows(keeper, tstruct, tabl, "SubTabl(BoolSub And StringSub=='' )");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("a", rows[0].Code);
            Assert.AreEqual("c", rows[1].Code);

            rows = SelectSubRows(keeper, tstruct, tabl, "SubTabl.SubTabl");
            Assert.AreEqual(2, rows.Length);
            Assert.AreEqual("qqq", rows[0].Code);
            Assert.AreEqual("ppp", rows[1].Code);

            t = SelectRowsStruct(keeper, tabls, "VTZTZ(SysNumVTZ==43014)", out tstruct);
            Assert.AreEqual(1, t.Count());
            tabl = t.First();
            Assert.AreEqual(5, tabl.SubCodes.Count);
            Assert.AreEqual(5, tabl.SubList.Count);

            rows = SelectSubRows(keeper, tstruct, tabl, "SubTabl");
            Assert.AreEqual(5, rows.Length);
            Assert.AreEqual("1NZ00Z803-1", rows[0].Code);
            Assert.AreEqual("1NZ00Z804-1", rows[1].Code);
            Assert.AreEqual("ТЗ_З00210110095", rows[2].Code);
            Assert.AreEqual("ТЗ_З00210110208", rows[3].Code);
            Assert.AreEqual("ТЗ_З00210110212", rows[4].Code);

            rows = SelectSubRows(keeper, tstruct, tabl, "SubTabl(Code Like 'ТЗ*')");
            Assert.AreEqual(3, rows.Length);
            Assert.AreEqual("ТЗ_З00210110095", rows[0].Code);
            Assert.AreEqual("ТЗ_З00210110208", rows[1].Code);
            Assert.AreEqual("ТЗ_З00210110212", rows[2].Code);

            rows = SelectSubRows(keeper, tstruct, tabl, "SubTabl(UnitTypeTz <> 'ТЗ' ИЛИ SysNumTz < 45000)");
            Assert.AreEqual(5, rows.Length);
            Assert.AreEqual("1NZ00Z803-1", rows[0].Code);
            Assert.AreEqual("1NZ00Z804-1", rows[1].Code);
            Assert.AreEqual("ТЗ_З00210110095", rows[2].Code);
            Assert.AreEqual("ТЗ_З00210110208", rows[3].Code);
            Assert.AreEqual("ТЗ_З00210110212", rows[4].Code);
        }
    }
}