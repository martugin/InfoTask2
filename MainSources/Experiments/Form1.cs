using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using BaseLibrary;
using ComLaunchers;
using CommonTypes;
using InfoTaskLouncherTest;
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
            var doc = XDocument.Load(ItStatic.InfoTaskDir() + @"General\Config.xml");
            foreach (var c in doc.Element("Config").Element("Providers").Elements())
            {
                MessageBox.Show(c.Name + ", " + c.Attribute("DllFile").Value);
            }
        }

        private void butIndicator_Click(object sender, EventArgs e)
        {
            var launcher = new TestItLauncher();
            launcher.RunTestForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var it = new ItLauncher();
            it.Finished += OnFinished;
            it.Initialize("Experiments", "TestProject");
            var con = it.CreateSourConnect("Source", "Fictive");
            con.JoinProvider("FictiveSimpleSource", "Label=FictiveTest");
            con.MakeCloneAsync(new DateTime(2017, 1, 1), new DateTime(2017, 1, 1, 0, 10, 0), ItStatic.InfoTaskDir() + @"LocalData\RunItLauncher\Clone");
        }

        private void OnFinished()
        {
            MessageBox.Show("Finish");
        }

        private void butItDir_Click(object sender, EventArgs e)
        {
            MessageBox.Show(ItStatic.InfoTaskDir());
        }

        private void butJustIndicator_Click(object sender, EventArgs e)
        {
            var launcher = new TestItLauncher();
            launcher.RunJustIndicator();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            Thread.Sleep(1000);
            progressBar1.Value = 5;
            Thread.Sleep(100);
            progressBar1.Value = 6;
            Thread.Sleep(1000);
            progressBar1.Value = 30;
            Thread.Sleep(100);
            progressBar1.Value = 31;
            Application.DoEvents();
            Thread.Sleep(1000);
            Refresh();
            progressBar1.Value = 60;
            Thread.Sleep(100);
            progressBar1.Value = 61;
            Application.DoEvents();
            Thread.Sleep(1000);
            progressBar1.Value = 80;
            Thread.Sleep(100);
            progressBar1.Value = 81;
            Application.DoEvents();
            Thread.Sleep(1000);
            progressBar1.Value = 0;
            Application.DoEvents();
        }
    }
}
