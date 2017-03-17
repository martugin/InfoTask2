using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoTaskClientTest
{
    [TestClass]
    public class ItClientTest
    {
        [TestMethod]
        public void Run()
        {
            var it = new TestItClient(true);
            it.RunTestIndicator();
            var ind = (TestIndicator)it.Logger.Indicator;
            Assert.AreEqual(82, ind.Events.Count);
            //ind.Compare(0, "ShowTimed");
            //ind.Compare(1, "PeriodBegin", "01.01.2017 10:00:00");
            //ind.Compare(2, "PeriodEnd", "01.01.2017 11:00:00");
            //ind.Compare(3, "PeriodMode");
            //ind.Compare(4, "ProcessUsual");
            //ind.Compare(5, "Text0", null);
            //ind.Compare(6, "Text1",  "Command (SSS)");
            //ind.Compare(7, "Text2", null);
            //ind.Compare(8, "Text2", "Text");
            //ind.Compare(9, "Procent", "10");
            //ind.Compare(10, "Text2");
            //ind.Compare(11, "Procent", "20");
            //ind.Compare(12, "Text1");
            //ind.Compare(13, "Text1", "Another");
            //ind.Compare(14, "Text2", "More");
            //ind.Compare(15, "Text2");
            //ind.Compare(16, "Procent", "30");
            //ind.Compare(17, "Text2", "More More");
            //ind.Compare(18, "Text2");
            //ind.Compare(19, "Procent", "40");
            //ind.Compare(20, "Text1");
            //ind.Compare(21, "Text1", "Third (3)");
            //ind.Compare(23, "Text2", "ThirdText");
            //ind.Compare(24, "Text2");
            //ind.Compare(26, "Text2", "ThirdMore");
            //ind.Compare(27, "Text2");
            //ind.Compare(28, "Procent", "70");
            //ind.Compare(29, "Text1");
            //ind.Compare(30, "Text1", "Last (UUU)");
            //ind.Compare(31, "Procent", "82");
            //ind.Compare(32, "Procent", "91");
            //ind.Compare(33, "Text2", "Last");
            //ind.Compare(34, "Text2");
            //ind.Compare(35, "Procent", "100");
            //ind.Compare(36, "Text1");
            //ind.Compare(38, "Hide");
            //ind.Compare(39, "Procent", "0");
            //ind.Compare(40, "ShowTimed");
            //ind.Compare(41, "PeriodBegin", "01.01.2017 11:00:00");
            //ind.Compare(42, "PeriodEnd", "01.01.2017 12:00:00");
            //ind.Compare(43, "PeriodMode", "Mode");
            //ind.Compare(44, "ProcessTimed", "01.01.2017 12:00:00");
            //ind.Compare(45, "Text1", "Log");
            //ind.Compare(46, "Text2", "ProgressText");
            //ind.Compare(47, "Procent", "25");
            //ind.Compare(48, "Text2");
            //ind.Compare(49, "Procent", "50");
            //ind.Compare(50, "Text2", "ProgressMore");
            //ind.Compare(51, "Procent", "75");
            //ind.Compare(52, "Text2");
            //ind.Compare(53, "Procent", "100");
            //ind.Compare(54, "Text1");
            //ind.Compare(55, "Hide");
            //ind.Compare(56, "Procent", "0");
            //ind.Compare(57, "ShowTexted");
            //ind.Compare(58, "Text0", "Заголовок");
            //ind.Compare(59, "ProcessUsual");
            //ind.Compare(60, "Text1", "Com (xxx)");
            //ind.Compare(61, "Text2", "Text");
            //ind.Compare(62, "Procent", "12,5");
            //ind.Compare(63, "Text2");
            //ind.Compare(64, "Procent", "25");
            //ind.Compare(65, "Text2", "TextText");
            //ind.Compare(66, "Procent", "37,5");
            //ind.Compare(67, "Text2");
            //ind.Compare(68, "Procent", "50");
            //ind.Compare(69, "Text1");
            //ind.Compare(70, "Text1", "Com (yyy)");
            //ind.Compare(71, "Text2", "TextTextText");
            //ind.Compare(72, "Procent", "62,5");
            //ind.Compare(73, "Text2");
            //ind.Compare(74, "Procent", "75");
            //ind.Compare(75, "Text2", "TextTextTextText");
            //ind.Compare(76, "Procent", "87,5");
            //ind.Compare(77, "Text2");
            //ind.Compare(78, "Procent", "100");
            //ind.Compare(79, "Text1");
            //ind.Compare(80, "Text0");
            //ind.Compare(81, "Hide");
        }

        [TestMethod]
        public void RunBreak()
        {
            var it = new TestItClient(true);
            it.RunBreak();
            var ind = (TestIndicator)it.Logger.Indicator;
            //Assert.AreEqual(13, ind.Events.Count);
            //ind.Compare(0, "ShowTexted");
            //ind.Compare(1, "Text0", "T");
            //ind.Compare(2, "Text1", null);
            //ind.Compare(3, "Text2", null);
            //ind.Compare(4, "ProcessUsual");
            //ind.Compare(5, "Procent", "20");
            //ind.Compare(6, "Text1", "Log");
            //ind.Compare(7, "Procent", "40");
            //ind.Compare(8, "Procent", "60");
            //ind.Compare(9, "Text1");
            //ind.Compare(10, "Procent", "100");
            //ind.Compare(11, "Text0");
            //ind.Compare(12, "Hide");
        }
    }
}