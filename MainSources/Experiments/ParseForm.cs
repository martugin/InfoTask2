using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using Generator;

namespace Experiments
{
    public partial class ParseForm : Form
    {
        public ParseForm()
        {
            InitializeComponent();
        }

        private void ButCondition_Click(object sender, EventArgs e)
        {
            var generator = new ModuleGenerator(new Logger(), null, null, null);
            Result.Text = new RuleParsing(new GenKeeper(generator), "поле", Formula.Text).ToTestString();
        }

        private void ButSubCondition_Click(object sender, EventArgs e)
        {
            var generator = new ModuleGenerator(new Logger(), null, null, null);
            Result.Text = new SubRuleParsing(new GenKeeper(generator), "поле", Formula.Text).ToTestString();
        }

        private void ButGenField_Click(object sender, EventArgs e)
        {
            var generator = new ModuleGenerator(new Logger(), null, null, null);
            Result.Text = new FieldsParsing(new GenKeeper(generator), "поле", Formula.Text).ToTestString();
        }
    }
}
