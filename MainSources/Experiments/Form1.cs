﻿using System;
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
using InfoTaskLauncherTest;
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
            var launcher = new ExperimentsItLauncher();
            launcher.RunTestForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var it = new ItLauncher();
            it.Initialize("Experiments");
            it.Finished += OnFinished;
            var cloner = it.LoadCloner("FictiveSimpleSource", "Label=fic");
            cloner.MakeCloneSync(ItStatic.InfoTaskDir() + @"LocalData\RunItLauncher\FictiveSimpleClone");
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
            var launcher = new ExperimentsItLauncher();
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var it = new ItLauncher();
            it.Initialize("Experiments");
            it.Finished += OnFinished;
            var cloner = it.LoadCloner("LogikaSource", @"DbFile=D:\InfoTask2\Debug\TestsRun\Providers\Logika\CloneProlog.mdb");
            cloner.MakeCloneAsync(ItStatic.InfoTaskDir() + @"TestsRun\Providers\Logika\Hand\Clone");
        }
    }
}
