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
using System.Threading;
using System.Diagnostics;

namespace MIPS
{
    public partial class Form1 : MetroSetForm
    {
        public bool mode;
        public MIPSSimulator simulator;
        private SimStateMachine state = SimStateMachine.Stopped;

        public Form1()
        {
            InitializeComponent();
            UpdateButtons();
            simulator = new MIPSSimulator(richTextBox1.Lines,richTextBox2.Lines);
            simulator.Registers.ForEach(i => Debug.WriteLine(i.Label));
            InitDGVbindings();
        }
        private void InitDGVbindings()
        {
            Registers.DataSource = simulator.Registers;
            var test = new BindingSource();
            test.DataSource = simulator.Registers.Where(i => i.Label.StartsWith("$s")).ToList();
            dataGridView2.DataSource = test;
            dataGridView2.Update();

            test = new BindingSource();
            test.DataSource = simulator.Registers.Where(i => i.Label.StartsWith("$v")).ToList();
            dataGridView3.DataSource = test;
            dataGridView3.Update();

            test = new BindingSource();
            test.DataSource = simulator.Registers.Where(i => i.Label.StartsWith("$a") && !i.Label.EndsWith("at")).ToList();
            dataGridView4.DataSource = test;
            dataGridView4.Update();

            test = new BindingSource();
            test.DataSource = simulator.Registers.Where(i => i.Label.StartsWith("$t"));
            dataGridView5.DataSource = test;
            dataGridView5.Update();
        }
        private int index = 0;
        private int indexPos = 0;

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
                default:
                    break;
            }
        }
        private void Run(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                return;
            if (index == richTextBox1.Lines.Length)
            {
                index = 0;
                indexPos = 0;
            }


            RunFlag = true;
            backgroundWorker1.RunWorkerAsync(new Tuple<bool, int, string[]>(RunFlag, index, richTextBox1.Lines));
            state = SimStateMachine.Running;
            UpdateButtons();
        }
        private void RunOneStep(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                return;
            if (index == richTextBox1.Lines.Length)
            {
                index = 0;
                indexPos = 0;
            }

            RunFlag = false;
            backgroundWorker1.RunWorkerAsync(new Tuple<bool, int, string[]>(RunFlag, index, richTextBox1.Lines));
            state = SimStateMachine.Running;
            UpdateButtons();
        }
        private void Pause(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            RunFlag = false;
            state = SimStateMachine.Paused;
            UpdateButtons();
        }
        private void Stop(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            state = SimStateMachine.Stopped;
            RunFlag = false;
            index = 0;
            indexPos = 0;
            UpdateButtons();
        }

        private void Simulator_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker args
            Tuple<bool, int, string[]> args = e.Argument as Tuple<bool, int, string[]>;//run flag,line number,input
            bool runFlag = args.Item1;
            int LineIndex = args.Item2;
            string[] Input = args.Item3;

            if (runFlag)
            {
                while (LineIndex < Input.Length)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    Thread.Sleep(new TimeSpan(0, 0, 2)); //test
                    //Simulate

                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    LineIndex++;
                    backgroundWorker1.ReportProgress(LineIndex);
                }
            }
            else
            {
                Thread.Sleep(new TimeSpan(0, 0, 2)); //test
                LineIndex++;
                backgroundWorker1.ReportProgress(LineIndex);
            }

            e.Result = new int?(LineIndex);
        }

        private void Simulator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            index = e.ProgressPercentage - 1;

            if (index >= richTextBox1.Lines.Length)
                return;

            indexPos += (index - 1 >= 0 ? richTextBox1.Lines[index - 1].Length + 1 : 0);

            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = Color.White;
            richTextBox1.Select(indexPos, richTextBox1.Lines[index].Length);

            richTextBox1.SelectionBackColor = Color.FromArgb(95, 207, 255);

            index++;
            UpdateButtons();
        }

        private void Simulator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RunFlag = false;
            state = SimStateMachine.Ready;
            if (e.Cancelled)
            {
                Debug.WriteLine("Cancelled");
                return;
            }

            index = (e.Result as int?).Value;
            Debug.WriteLine("Done");
            UpdateButtons();

        }

        private void metroSetButton1_Click(object sender, EventArgs e)
        {
            var test = simulator.Registers;
            test.ForEach(i => Debug.WriteLine(i.Value));
            test.ForEach(i => i.Value += 10);
            metroSetTabControl2.SelectedTab.Controls[0].Refresh();
        }
    }
}
