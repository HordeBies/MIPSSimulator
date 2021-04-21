using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CORG_MT.MIPS;
using System.Diagnostics;
using System.Threading;

namespace CORG_MT
{
    public partial class Simulator : Form
    {
        public bool mode;
        public MIPSSimulator simulator;

        public Simulator()
        {
            InitializeComponent();
            UpdateButtons();
            
        }
        private int index = 0;
        private int indexPos = 0;

        private bool RunFlag = false;
        private bool PauseFlag = false;
        private void UpdateButtons()
        {
            label1.Text = index.ToString();
            if(index == richTextBox1.Lines.Length)
            {
                RunButton.Enabled = false;
                RunOneButton.Enabled = false;
            }
            else
            {
                if (!RunFlag)
                    RunButton.Enabled = true;
                else
                    RunButton.Enabled = false;
                RunOneButton.Enabled = true;
            }

            if(index <= 1)
            {

                UndoOneButton.Enabled = false;
            }
            else
            {
                UndoOneButton.Enabled = true;
            }

            if (index == 0)
            {
                PauseButton.Enabled = false;
                StopButton.Enabled = false;
            }
            else
            {
                if(!PauseFlag)
                    PauseButton.Enabled = true;
                else
                    PauseButton.Enabled = false;

                StopButton.Enabled = true;
            }

        }
        private void Run(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                return;

            richTextBox1.Enabled = false;
            RunFlag = true;
            PauseFlag = false;
            backgroundWorker1.RunWorkerAsync(new Tuple<bool, int, string[]>(RunFlag,index,richTextBox1.Lines));
            UpdateButtons();
        }
        private void RunOneStep(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                return;

            RunFlag = false;
            PauseFlag = true;
            backgroundWorker1.RunWorkerAsync(new Tuple<bool, int, string[]>(RunFlag, index, richTextBox1.Lines));
            UpdateButtons();

        }
        //private void UndoOneStep(object sender, EventArgs e)
        //{
        //    if (index - 2 < 0)
        //        return;

        //    index -= 2;
        //    indexPos -= (richTextBox1.Lines[index].Length+1);

        //    richTextBox1.SelectionBackColor = Color.White;
        //    richTextBox1.Select(indexPos, richTextBox1.Lines[index].Length);
        //    richTextBox1.SelectionBackColor = Color.FromArgb(125, 255, 0, 0);

        //    index++;
        //    UpdateButtons();
        //}
        private void Pause(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            RunFlag = false;
            PauseFlag = true;
            UpdateButtons();
        }
        private void Stop(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();

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
                while(LineIndex < Input.Length)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    Thread.Sleep(new TimeSpan(0,0,2)); //test
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
            index = e.ProgressPercentage-1;

            if (index >= richTextBox1.Lines.Length)
                return;

            indexPos += (index - 1 >= 0 ? richTextBox1.Lines[index - 1].Length + 1 : 0);

            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = Color.White;
            richTextBox1.Select(indexPos, richTextBox1.Lines[index].Length);

            richTextBox1.SelectionBackColor = Color.FromArgb(125, 255, 0, 0);

            index++;
            UpdateButtons();
        }

        private void Simulator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            richTextBox1.Enabled = true;
            RunFlag = false;
            PauseFlag = true;
            if (e.Cancelled)
            {
                Debug.WriteLine("Cancelled");
                return;
            }

            index = (e.Result as int?).Value;
            Debug.WriteLine("Done");
            UpdateButtons();
        }
    }
}
