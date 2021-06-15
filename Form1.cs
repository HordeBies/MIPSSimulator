using MetroSet_UI.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MIPS.Sim;
using MetroSet_UI.Controls;

namespace MIPS
{
    public partial class Form1 : MetroSetForm
    {
        public bool mode;
        public MIPSSimulator simulator;
        public SimStateMachine state = SimStateMachine.Ready;
        public Form1()
        {
            InitializeComponent();
            UpdateButtons();
            simulator = new MIPSSimulator(this);
            InitDGVbindings();
        }


        private void CompileButton_Click(object sender, EventArgs e)
        {
            if (BackgroundCompiler.IsBusy)
                return;
            BackgroundCompiler.RunWorkerAsync(richTextBox1.Lines);
        }
        private void Compiler_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] input = e.Argument as string[];
            e.Result = simulator.Compile(input);
        }
        private void Compiler_Finished(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(bool)e.Result)
                return;
            richTextBox2.Lines = simulator.iMemoryText.ToArray();
            RefreshControl(richTextBox2);
            RefreshControl(DGVim);
            SelectTab(metroSetTabControl1, 1);
            SelectTab(MetroSetTabControl2, 0);
        }

        delegate void SelectTabCallback(MetroSetTabControl control, int index);
        public void SelectTab(MetroSetTabControl control, int index)
        {
            if (!metroSetRadioButton1.Checked)
                return;
            if (control.InvokeRequired)
            {
                SelectTabCallback d = new SelectTabCallback(SelectTab);
                this.Invoke(d, new object[] { control, index });
            }
            else
            {
                control.SelectTab(index);
            }
        }
        delegate void RefreshControlCallback(Control control);
        public void RefreshControl(Control control)
        {
            if (control.InvokeRequired)
            {
                RefreshControlCallback d = new RefreshControlCallback(RefreshControl);
                this.Invoke(d, new object[] { control });
            }
            else
            {
                control.Refresh();
            }
        }
        public void ClearLog()
        {
            metroSetListBox1.Clear();
            RefreshControl(metroSetListBox1);
            SelectTab(MetroSetTabControl2, 3);
        }
        public void SendLog(string msg)
        {
            metroSetListBox1.AddItem(msg);
            RefreshControl(metroSetListBox1);
            SelectTab(MetroSetTabControl2, 3);
        }
        public void ReportError(string err)
        {
            SendLog(err);
        }
        public void updateState()
        {
            //metroSetTabControl2.SelectedTab.Controls[0].Refresh();
        }
        private void InitDGVbindings()
        {
            BindingSourceRegs.DataSource = simulator.Registers;
            DGVregs.DataSource = BindingSourceRegs;
            DGVregs.Update();

            BindingSourceIM.DataSource = simulator.iMemory;
            DGVim.DataSource = BindingSourceIM;
            DGVim.Update();

            BindingSourceDM.DataSource = simulator.dMemory;
            DGVdm.DataSource = BindingSourceDM;
            DGVdm.Update();

        }
        private int index = 0;

        private bool RunFlag = false;
        private void Enable(Button button) => button.Enabled = true;
        private void Disable(Button button) => button.Enabled = false;

        private void UpdateButtons()
        {
            switch (state)
            {
                case SimStateMachine.Ready:
                    Enable(RunButton);
                    Enable(RunOneButton);
                    Disable(PauseButton);
                    Disable(StopButton);
                    Enable(ResetButton);
                    break;
                case SimStateMachine.Running:
                    Disable(RunButton);
                    Disable(RunOneButton);
                    Enable(PauseButton);
                    Enable(StopButton);
                    Disable(ResetButton);
                    break;
                case SimStateMachine.Paused:
                    Enable(RunButton);
                    Enable(RunOneButton);
                    Disable(PauseButton);
                    Enable(StopButton);
                    Enable(ResetButton);
                    break;
                case SimStateMachine.Stopped:
                    Enable(RunButton);
                    Enable(RunOneButton);
                    Disable(PauseButton);
                    Disable(StopButton);
                    Enable(ResetButton);
                    break;
                case SimStateMachine.Finished:
                    Disable(RunButton);
                    Disable(RunOneButton);
                    Disable(PauseButton);
                    Enable(StopButton);
                    Enable(ResetButton);
                    break;
                default:
                    break;
            }
        }
        private void Run(object sender, EventArgs e)
        {

        }
        private void RunOneStep(object sender, EventArgs e)
        {

        }
        private void Pause(object sender, EventArgs e)
        {

        }
        private void Stop(object sender, EventArgs e)
        {

        }

        private void Simulator_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker args
            bool runFlag = (bool)e.Argument;
            int LineIndex;
            if (runFlag)
            {
                while (simulator.iMemory[simulator.CurrentLine].Value != 0)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    //Thread.Sleep(new TimeSpan(0, 0, 2)); //test
                    LineIndex = simulator.Execute();

                }
            }
            else
            {
                //Thread.Sleep(new TimeSpan(0, 0, 2)); //test
                LineIndex = simulator.Execute();
                
            }

        }
        private void EvaluateSelection(int index)
        {
            if (index >= richTextBox2.Lines.Length)
                return;

            richTextBox2.SelectAll();
            richTextBox2.SelectionBackColor = Color.White;

            int indexPos = 0;

            for(int i = 0; i< index; i++)
            {
                indexPos += (richTextBox2.Lines[i].Length + 1);

            }

            richTextBox2.Select(indexPos, richTextBox2.Lines[index].Length);
            richTextBox2.SelectionBackColor = Color.FromArgb(95, 207, 255);
        }
        private void Simulator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            index = e.ProgressPercentage;

            //var test = new BindingSource();
            //dataGridView6.DataSource = test;
            //dataGridView6.Update();
            EvaluateSelection(index);
            index++;
            MetroSetTabControl2.SelectedTab.Controls[0].Refresh();
            AutoSwitch();
            UpdateButtons();
        }

        private void AutoSwitch()
        {
        }

        private void Simulator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RunFlag = false;
            state = SimStateMachine.Paused;
            if (e.Cancelled)
            {
                SendLog("Cancelled");
                return;
            }
            var res = (e.Result as Tuple<int, bool>);

            index = res.Item1;
            if (res.Item2)
            {
                state = SimStateMachine.Finished;
            }
            if (false)
            {
                Stop(null, null);
                return;
            }
            UpdateButtons();

        }

        private void InputChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double temp;
            double.TryParse("13.13", out temp);
            SendLog(temp.ToString());
        }

        private void ıconButton2_Click(object sender, EventArgs e)
        {
            using (var form = new Credits())
            {
                form.ShowDialog();
            }   
        }

        private void ıconButton1_Click(object sender, EventArgs e)
        {
            using (var form = new Popup())
            {
                form.ShowDialog();
            }
        }

        private void Reset(object sender, EventArgs e)
        {
            //simulator.Reset(richTextBox1.Lines, richTextBox2.Lines);
            MetroSetTabControl2.SelectedTab.Controls[0].Refresh();
        }

    }
}
