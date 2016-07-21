using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using BaseLibrary;
using CommonTypes;
using Provider;
using ProvidersLibrary;

namespace Experiments
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void butXml_Click(object sender, EventArgs e)
        {
            var doc = XDocument.Load(DifferentIT.InfoTaskDir() + @"General\Config.xml");
            foreach (var c in doc.Element("Config").Element("Providers").Elements())
            {
                MessageBox.Show(c.Name + ", " + c.Attribute("DllFile").Value);
            }
            
        }

        private void butSiemens_Click(object sender, EventArgs e)
        {
            var source = new SimaticSource();
            source.AddMainConnect(@"SQLServer=OS21\WINCC");
            source.Name = "N";
            source.Logger = new Logger();
            var sig = source.AddInitialSignal("N", "N", DataType.Real, "Id=243");
            source.GetValues(new DateTime(2016, 07, 17, 2, 0, 0), new DateTime(2016, 07, 17, 5, 0, 0));
            using (var rec = new RecDao("SiemensValues.accdb", "ArchiveValues"))
                for (int i = 0; i < sig.MomList.Count; i++)
                {
                    rec.AddNew();
                    rec.Put("Time", sig.MomList.Time(i));
                    rec.Put("RealValue", sig.MomList.Real(i));
                    rec.Put("Quality", sig.MomList.Error(i) == null ? 0 : sig.MomList.Error(i).Number);
                }
            MessageBox.Show("Чтение завершено, " + sig.MomList.Count + " значений прочитано");
        }
    }
}
