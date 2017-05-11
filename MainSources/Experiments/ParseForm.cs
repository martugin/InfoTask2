using System;
using System.Windows.Forms;
using BaseLibrary;
using BaseLibraryTest;
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
            var logger = new Logger(new TestHistory(), new TestIndicator());
            var generator = new ModuleGenerator(logger, null, null, null);
            Result.Text = new RuleParsing(new GenKeeper(generator), "поле", Formula.Text).ToTestString();
        }

        private void ButSubCondition_Click(object sender, EventArgs e)
        {
            var logger = new Logger(new TestHistory(), new TestIndicator());
            var generator = new ModuleGenerator(logger, null, null, null);
            Result.Text = new SubRuleParsing(new GenKeeper(generator), "поле", Formula.Text).ToTestString();
        }

        private void ButGenField_Click(object sender, EventArgs e)
        {
            var logger = new Logger(new TestHistory(), new TestIndicator());
            var generator = new ModuleGenerator(logger, null, null, null);
            Result.Text = new FieldsParsing(new GenKeeper(generator), "поле", Formula.Text).ToTestString();
        }
    }
}
