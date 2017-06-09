using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using BaseLibraryTest;
using ComLaunchers;
using CommonTypes;

namespace Experiments
{
    public partial class CalibratorForm : Form
    {
        public CalibratorForm()
        {
            InitializeComponent();
        }

        private ItLauncher _launcher;
        private ILauncherCalibratorProject _project;

        private void ButInit_Click(object sender, EventArgs e)
        {
            TestLib.CopyDir("Calibrator", "CalibratorProject");
            _launcher = new ItLauncher();
            _launcher.InitializeTest();
            _project = _launcher.LoadCalibratorProject(TestLib.TestRunDir + @"Calibrator\CalibratorProject\");
            _project.OpenThreads(PeriodLength.Text.ToDouble(), LateLength.Text.ToDouble());
        }

        private void AddSignal(ComboBox cb)
        {
            var s = cb.Text;
            if (!s.IsWhiteSpace())
                _project.AddSignal("Source1", s, s == "State" ? "Integer" : s, "List", "NumObject=1;ValuesInterval=1;Signal=" + s);
        }

        private void ButStart_Click(object sender, EventArgs e)
        {
            _project.ClearSignals();
            AddSignal(Signal1);
            AddSignal(Signal2);
            AddSignal(Signal3);
            _project.StartProcess();
        }

        private void ButStop_Click(object sender, EventArgs e)
        {
            _project.StopProcess();
        }
        
        private void ButClose_Click(object sender, EventArgs e)
        {
            _launcher.Close();
        }
    }
}
