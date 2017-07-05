using System;
using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using CompileLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculationTest
{
    [TestClass]
    public class TablsTest
    {
        //Открытие тестовых баз с копированием 
        private DaoDb CopyDb(string prefix)
        {
            return new DaoDb(TestLib.CopyFile("Libraries", "TablsData.accdb", "Tabls" + prefix + ".accdb"));
        }
        //Путь к файлу
        private string File(string prefix)
        {
            return TestLib.TestRunDir + @"Libraries\Tabls" + prefix + ".accdb";
        }
        
        [TestMethod]
        public void TablStruct()
        {
            using (var db = CopyDb("Struct"))
            {
                var tlist = new TablsList();
                Assert.AreEqual(0, tlist.Structs.Count);
                Assert.AreEqual(0, tlist.Tabls.Count);
                var gr = tlist.AddStruct(db, "Tabl", "SubTabl", "SubSubTabl");
                Assert.AreEqual(1, tlist.Structs.Count);
                Assert.AreEqual("Tabl", gr.Code);
                Assert.AreEqual(File("Struct"), gr.DbFile);
                Assert.AreEqual(4, gr.Tabls.Count);

                var tstruct = gr.Tabls[-1];
                Assert.AreEqual(-1, tstruct.Level);
                Assert.IsNull(tstruct.Parent);
                Assert.IsNotNull(tstruct.Child);
                Assert.AreEqual(0, tstruct.Fields.Count);

                tstruct = gr.Tabls[0];
                Assert.AreEqual("Tabl", tstruct.TableName);
                Assert.AreEqual(0, tstruct.Level);
                Assert.IsNotNull(tstruct.Parent);
                Assert.IsNotNull(tstruct.Child);
                Assert.AreEqual(11, tstruct.Fields.Count);
                Assert.AreEqual(DataType.String, tstruct.Fields["Code"]);
                Assert.AreEqual(DataType.Integer, tstruct.Fields["Num"]);
                Assert.AreEqual(DataType.String, tstruct.Fields["NameField"]);
                Assert.AreEqual(DataType.Boolean, tstruct.Fields["BoolField"]);

                tstruct = gr.Tabls[1];
                Assert.AreEqual("SubTabl", tstruct.TableName);
                Assert.AreEqual(1, tstruct.Level);
                Assert.IsNotNull(tstruct.Parent);
                Assert.IsNotNull(tstruct.Child);
                Assert.AreEqual(12, tstruct.Fields.Count);
                Assert.AreEqual(DataType.String, tstruct.Fields["GenType"]);
                Assert.AreEqual(DataType.Integer, tstruct.Fields["Num"]);
                Assert.AreEqual(DataType.String, tstruct.Fields["StringSub"]);
                Assert.AreEqual(DataType.Real, tstruct.Fields["RealSub"]);
                Assert.AreEqual(DataType.Integer, tstruct.Fields["IntSub"]);
                Assert.AreEqual(DataType.Time, tstruct.Fields["TimeSub"]);

                tstruct = gr.Tabls[2];
                Assert.AreEqual("SubSubTabl", tstruct.TableName);
                Assert.AreEqual(2, tstruct.Level);
                Assert.IsNotNull(tstruct.Parent);
                Assert.IsNull(tstruct.Child);
                Assert.AreEqual(12, tstruct.Fields.Count);
                Assert.AreEqual(DataType.String, tstruct.Fields["Code"]);
                Assert.AreEqual(DataType.Integer, tstruct.Fields["Id"]);
                Assert.AreEqual(DataType.String, tstruct.Fields["StringString"]);
                Assert.AreEqual(DataType.Real, tstruct.Fields["RealReal"]);
                Assert.AreEqual(DataType.Boolean, tstruct.Fields["BoolBool"]);

                tlist.AddDbStructs(db);
                Assert.AreEqual(2, tlist.Structs.Count);
                Assert.AreEqual(0, tlist.Tabls.Count);
                gr = tlist.Structs["VTZTZ"];
                Assert.AreEqual("VTZTZ", gr.Code);
                Assert.AreEqual(File("Struct"), gr.DbFile);
                Assert.AreEqual(3, gr.Tabls.Count);

                tstruct = gr.Tabls[-1];
                Assert.AreEqual(-1, tstruct.Level);
                Assert.IsNull(tstruct.Parent);
                Assert.IsNotNull(tstruct.Child);
                Assert.AreEqual(0, tstruct.Fields.Count);

                tstruct = gr.Tabls[0];
                Assert.AreEqual("Tbl_VTZTZ_Sub0", tstruct.TableName);
                Assert.AreEqual(0, tstruct.Level);
                Assert.IsNotNull(tstruct.Parent);
                Assert.IsNotNull(tstruct.Child);
                Assert.AreEqual(9, tstruct.Fields.Count);
                Assert.AreEqual(DataType.String, tstruct.Fields["Code"]);
                Assert.AreEqual(DataType.Integer, tstruct.Fields["Num"]);
                Assert.AreEqual(DataType.String, tstruct.Fields["NameVTZ"]);
                Assert.AreEqual(DataType.Integer, tstruct.Fields["SysNumVTZ"]);
                Assert.AreEqual(DataType.Integer, tstruct.Fields["IdUnitVTZ"]);

                tstruct = gr.Tabls[1];
                Assert.AreEqual("Tbl_VTZTZ_Sub1", tstruct.TableName);
                Assert.AreEqual(1, tstruct.Level);
                Assert.IsNotNull(tstruct.Parent);
                Assert.IsNull(tstruct.Child);
                Assert.AreEqual(9, tstruct.Fields.Count);
                Assert.AreEqual(DataType.String, tstruct.Fields["Code"]);
                Assert.AreEqual(DataType.Integer, tstruct.Fields["ParentId"]);
                Assert.AreEqual(DataType.String, tstruct.Fields["NameTZ"]);
                Assert.AreEqual(DataType.Integer, tstruct.Fields["SysNumTZ"]);
                Assert.AreEqual(DataType.String, tstruct.Fields["UnitTypeTZ"]);
            }
        }
                
        [TestMethod]
        public void TablValues()
        {
            using (var db = CopyDb("Values"))
            {
                var tlist = new TablsList();
                Assert.AreEqual(0, tlist.Tabls.Count);
                tlist.AddStruct(db, "Tabl", "SubTabl", "SubSubTabl");
                Assert.AreEqual(1, tlist.Structs.Count);
                tlist.LoadValues(db, false);
                Assert.AreEqual(1, tlist.Tabls.Count);
                var tabl = tlist.Tabls["Tabl"];
                Assert.AreEqual(3, tabl.Rows[0].Count);
                Assert.AreEqual(4, tabl.Rows[1].Count);
                Assert.AreEqual(0, tabl.Rows[2].Count);
                Assert.AreEqual(3, tabl.SubCodes.Count);
                Assert.AreEqual(2, tabl.SubNums.Count);
                Assert.AreEqual(3, tabl.SubList.Count);

                Assert.IsTrue(tabl.SubCodes.ContainsKey("s1"));
                Assert.IsTrue(tabl.SubCodes.ContainsKey("s2"));
                Assert.IsTrue(tabl.SubCodes.ContainsKey("s3"));
                Assert.IsTrue(tabl.SubNums.ContainsKey(11));
                Assert.IsTrue(tabl.SubNums.ContainsKey(12));

                var row = tabl.SubCodes["s1"];
                Assert.AreEqual(3, row.SubCodes.Count);
                Assert.AreEqual(3, row.SubNums.Count);
                Assert.AreEqual(3, row.SubList.Count);
                Assert.AreEqual(1, row.Id);
                Assert.AreEqual(11, row.Num);
                Assert.AreEqual("s1", row.Code);

                Assert.AreEqual(DataType.Integer, row["Num"].DataType);
                Assert.AreEqual(11, row["Num"].Integer);
                Assert.IsNull(row["Num"].Error);
                Assert.AreEqual(DataType.String, row["StringField"].DataType);
                Assert.AreEqual("SSS", row["StringField"].String);
                Assert.AreEqual(DataType.Boolean, row["BoolField"].DataType);
                Assert.AreEqual(true, row["StringField"].Boolean);
                Assert.AreEqual(DataType.Time, row["TimeField"].DataType);
                Assert.AreEqual(new DateTime(2016, 9 ,26), row["TimeField"].Date);

                row = row.SubNums[1];
                Assert.AreEqual(2, row.SubCodes.Count);
                Assert.AreEqual(2, row.SubNums.Count);
                Assert.AreEqual(2, row.SubList.Count);
                Assert.AreEqual(1, row.Id);
                Assert.AreEqual(1, row.ParentId);
                Assert.AreEqual(1, row.Num);
                Assert.AreEqual("a", row.Code);

                Assert.AreEqual(DataType.Integer, row["Num"].DataType);
                Assert.AreEqual(1, row["Num"].Integer);
                Assert.AreEqual(DataType.String, row["Code"].DataType);
                Assert.AreEqual("a", row["Code"].String);
                Assert.AreEqual(DataType.String, row["StringSub"].DataType);
                Assert.AreEqual("", row["StringSub"].String);
                Assert.AreEqual(DataType.Boolean, row["BoolSub"].DataType);
                Assert.AreEqual(true, row["StringSub"].Boolean);
                Assert.AreEqual(DataType.Real, row["RealSub"].DataType);
                Assert.AreEqual(1, row["RealSub"].Real);

                row = row.SubNums[2000];
                Assert.AreEqual(0, row.SubCodes.Count);
                Assert.AreEqual(0, row.SubNums.Count);
                Assert.AreEqual(0, row.SubList.Count);
                Assert.AreEqual(2, row.Id);
                Assert.AreEqual(1, row.ParentId);
                Assert.AreEqual(2000, row.Num);
                Assert.AreEqual("ppp", row.Code);

                Assert.AreEqual(DataType.String, row["Code"].DataType);
                Assert.AreEqual("ppp", row["Code"].String);
                Assert.AreEqual(DataType.String, row["StringString"].DataType);
                Assert.AreEqual("ppp", row["StringString"].String);
                Assert.AreEqual(DataType.Boolean, row["BoolBool"].DataType);
                Assert.AreEqual(true, row["BoolBool"].Boolean);

                row = tabl.SubNums[12];
                Assert.AreEqual(1, row.SubCodes.Count);
                Assert.AreEqual(1, row.SubNums.Count);
                Assert.AreEqual(1, row.SubList.Count);
                Assert.AreEqual(2, row.Id);
                Assert.AreEqual(12, row.Num);
                Assert.AreEqual("s2", row.Code);

                Assert.AreEqual(DataType.Integer, row["Num"].DataType);
                Assert.AreEqual(12, row["Num"].Integer);
                Assert.AreEqual(DataType.String, row["StringField"].DataType);
                Assert.AreEqual("SSS", row["StringField"].String);
                Assert.AreEqual(DataType.Boolean, row["BoolField"].DataType);
                Assert.AreEqual(true, row["StringField"].Boolean);
                Assert.AreEqual(DataType.Real, row["RealField"].DataType);
                Assert.AreEqual(123.5, row["RealField"].Real);

                tlist.AddDbStructs(db);
                tlist.LoadValues(db, true);
                Assert.AreEqual(2, tlist.Tabls.Count);
                Assert.AreEqual(2, tlist.Structs.Count);
                tabl = tlist.Tabls["VTZTZ"];
                Assert.AreEqual(6, tabl.SubCodes.Count);
                Assert.AreEqual(1, tabl.SubNums.Count);
                Assert.AreEqual(6, tabl.SubList.Count);

                row = tabl.SubCodes["ВТЗЗ00120101002"];
                Assert.AreEqual(1, row.SubCodes.Count);
                Assert.AreEqual(1, row.SubNums.Count);
                Assert.AreEqual(1, row.SubList.Count);
                Assert.AreEqual(386, row.Id);
                Assert.AreEqual(0, row.Num);
                Assert.AreEqual("ВТЗЗ00120101002", row.Code);

                Assert.AreEqual(DataType.String, row["Code"].DataType);
                Assert.AreEqual("ВТЗЗ00120101002", row["Code"].String);
                Assert.AreEqual(DataType.String, row["NameVTZ"].DataType);
                Assert.AreEqual("Выходные цепи отключения ДВ-А", row["NameVTZ"].String);
                Assert.AreEqual(DataType.Integer, row["SysNumVTZ"].DataType);
                Assert.AreEqual(43000, row["SysNumVTZ"].Integer);
                Assert.AreEqual(DataType.Integer, row["PriorVTZ"].DataType);
                Assert.AreEqual(1, row["PriorVTZ"].Integer);

                row = row.SubCodes["ТЗ_З00120101004"];
                Assert.IsNotNull(row.Parent);
                Assert.AreEqual(0, row.SubCodes.Count);
                Assert.AreEqual(0, row.SubNums.Count);
                Assert.AreEqual(0, row.SubList.Count);
                Assert.AreEqual(1, row.Id);
                Assert.AreEqual(386, row.ParentId);
                Assert.AreEqual(0, row.Num);
                Assert.AreEqual("ТЗ_З00120101004", row.Code);

                Assert.AreEqual(DataType.String, row["Code"].DataType);
                Assert.AreEqual("ТЗ_З00120101004", row["Code"].String);
                Assert.AreEqual(DataType.String, row["NameTZ"].DataType);
                Assert.AreEqual("Понижение Pм на смазку ДВ-А", row["NameTZ"].String);
                Assert.AreEqual(DataType.Integer, row["SysNumTZ"].DataType);
                Assert.AreEqual(41000, row["SysNumTZ"].Integer);
                Assert.AreEqual(DataType.Integer, row["IdUnitTZ"].DataType);
                Assert.AreEqual(10299, row["IdUnitTZ"].Integer);
            }
        }
    }
}
