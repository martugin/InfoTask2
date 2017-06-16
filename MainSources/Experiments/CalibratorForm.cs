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
            //_launcher.InitializeTest();
            _launcher.Initialize("Test");
            _project = _launcher.LoadCalibratorProject(TestLib.TestRunDir + @"Calibrator\CalibratorProject\");
            _project.OpenThreads(PeriodLength.Text.ToDouble(), LateLength.Text.ToDouble());
        }

        private LauncherRealTimeSignal AddSignal(ComboBox cb)
        {
            var s = cb.Text;
            if (s.IsWhiteSpace()) return null;
            return _project.AddSignal("Source1", s, s == "State" ? "Int" : s, "List", "NumObject=1;ValuesInterval=1;Signal=" + s);
        }

        private bool _isStarted;

        private LauncherRealTimeSignal _signal1;
        private LauncherRealTimeSignal _signal2;
        private LauncherRealTimeSignal _signal3;

        private void ButStart_Click(object sender, EventArgs e)
        {
            _project.ClearSignals();
            _signal1 = AddSignal(Signal1);
            _signal2 = AddSignal(Signal2);
            _signal3 = AddSignal(Signal3);
            _project.StartProcess();
            _isStarted = true;
            ValuesTimer.Interval = Convert.ToInt32(PeriodLength.Text.ToDouble() * 1000);
            ValuesTimer.Start();
        }

        private void ButStop_Click(object sender, EventArgs e)
        {
            ValuesTimer.Stop();
            _isStarted = false;
            _project.StopProcess();
        }
        
        private void ButClose_Click(object sender, EventArgs e)
        {
            _launcher.Close();
        }

        private void ValuesTimer_Tick(object sender, EventArgs e)
        {
            Value1.Text = _signal1 == null ? null : _signal1.String;
            Value2.Text = _signal2 == null ? null : _signal2.String;
            Value3.Text = _signal3 == null ? null : _signal3.String;
        }
    }
}
