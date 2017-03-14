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
using ComClients;
using CommonTypes;
using InfoTaskClientTest;
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

        private void button1_Click(object sender, EventArgs e)
        {
            var client = new TestItClient(false);
            client.RunTestForm();
        }
    }
}
