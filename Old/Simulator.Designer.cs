
namespace CORG_MT
{
    partial class Simulator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.source = new System.Windows.Forms.TabControl();
            this.data = new System.Windows.Forms.TabPage();
            this.text = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.RunButton = new FontAwesome.Sharp.IconButton();
            this.RunOneButton = new FontAwesome.Sharp.IconButton();
            this.PauseButton = new FontAwesome.Sharp.IconButton();
            this.UndoOneButton = new FontAwesome.Sharp.IconButton();
            this.StopButton = new FontAwesome.Sharp.IconButton();
            this.ResetButton = new FontAwesome.Sharp.IconButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.source.SuspendLayout();
            this.text.SuspendLayout();
            this.SuspendLayout();
            // 
            // source
            // 
            this.source.Controls.Add(this.data);
            this.source.Controls.Add(this.text);
            this.source.Location = new System.Drawing.Point(12, 70);
            this.source.Name = "source";
            this.source.SelectedIndex = 0;
            this.source.Size = new System.Drawing.Size(297, 368);
            this.source.TabIndex = 0;
            // 
            // data
            // 
            this.data.Location = new System.Drawing.Point(4, 24);
            this.data.Name = "data";
            this.data.Padding = new System.Windows.Forms.Padding(3);
            this.data.Size = new System.Drawing.Size(289, 340);
            this.data.TabIndex = 1;
            this.data.Text = "Data";
            this.data.UseVisualStyleBackColor = true;
            // 
            // text
            // 
            this.text.Controls.Add(this.richTextBox1);
            this.text.Location = new System.Drawing.Point(4, 24);
            this.text.Name = "text";
            this.text.Padding = new System.Windows.Forms.Padding(3);
            this.text.Size = new System.Drawing.Size(289, 340);
            this.text.TabIndex = 2;
            this.text.Text = "Text";
            this.text.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(283, 334);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "li $t2, 25\nlw $t3, value\nadd $t4, $t2, $t3\nsub $t5, $t2, $t3\nsw $t5, Z";
            // 
            // RunButton
            // 
            this.RunButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RunButton.IconChar = FontAwesome.Sharp.IconChar.AngleDoubleRight;
            this.RunButton.IconColor = System.Drawing.Color.Black;
            this.RunButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.RunButton.IconSize = 24;
            this.RunButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.RunButton.Location = new System.Drawing.Point(16, 12);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(29, 27);
            this.RunButton.TabIndex = 3;
            this.toolTip1.SetToolTip(this.RunButton, "Run the current program");
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.Run);
            // 
            // RunOneButton
            // 
            this.RunOneButton.IconChar = FontAwesome.Sharp.IconChar.AngleRight;
            this.RunOneButton.IconColor = System.Drawing.Color.Black;
            this.RunOneButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.RunOneButton.IconSize = 24;
            this.RunOneButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.RunOneButton.Location = new System.Drawing.Point(51, 12);
            this.RunOneButton.Name = "RunOneButton";
            this.RunOneButton.Size = new System.Drawing.Size(29, 27);
            this.RunOneButton.TabIndex = 4;
            this.toolTip1.SetToolTip(this.RunOneButton, "Run one step at a time");
            this.RunOneButton.UseVisualStyleBackColor = true;
            this.RunOneButton.Click += new System.EventHandler(this.RunOneStep);
            // 
            // PauseButton
            // 
            this.PauseButton.IconChar = FontAwesome.Sharp.IconChar.Pause;
            this.PauseButton.IconColor = System.Drawing.Color.Black;
            this.PauseButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.PauseButton.IconSize = 24;
            this.PauseButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.PauseButton.Location = new System.Drawing.Point(121, 12);
            this.PauseButton.Name = "PauseButton";
            this.PauseButton.Size = new System.Drawing.Size(29, 27);
            this.PauseButton.TabIndex = 5;
            this.toolTip1.SetToolTip(this.PauseButton, "Pause the currently running program");
            this.PauseButton.UseVisualStyleBackColor = true;
            this.PauseButton.Click += new System.EventHandler(this.Pause);
            // 
            // UndoOneButton
            // 
            this.UndoOneButton.IconChar = FontAwesome.Sharp.IconChar.AngleLeft;
            this.UndoOneButton.IconColor = System.Drawing.Color.Black;
            this.UndoOneButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.UndoOneButton.IconSize = 24;
            this.UndoOneButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.UndoOneButton.Location = new System.Drawing.Point(86, 12);
            this.UndoOneButton.Name = "UndoOneButton";
            this.UndoOneButton.Size = new System.Drawing.Size(29, 27);
            this.UndoOneButton.TabIndex = 6;
            this.toolTip1.SetToolTip(this.UndoOneButton, "Undo the last step");
            this.UndoOneButton.UseVisualStyleBackColor = true;
            // 
            // StopButton
            // 
            this.StopButton.IconChar = FontAwesome.Sharp.IconChar.Stop;
            this.StopButton.IconColor = System.Drawing.Color.Black;
            this.StopButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.StopButton.IconSize = 24;
            this.StopButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.StopButton.Location = new System.Drawing.Point(156, 12);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(29, 27);
            this.StopButton.TabIndex = 7;
            this.toolTip1.SetToolTip(this.StopButton, "Stop the currently running program");
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.Stop);
            // 
            // ResetButton
            // 
            this.ResetButton.IconChar = FontAwesome.Sharp.IconChar.AngleDoubleLeft;
            this.ResetButton.IconColor = System.Drawing.Color.Black;
            this.ResetButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.ResetButton.IconSize = 24;
            this.ResetButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.ResetButton.Location = new System.Drawing.Point(191, 12);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(29, 27);
            this.ResetButton.TabIndex = 8;
            this.toolTip1.SetToolTip(this.ResetButton, "Reset MIPS memory and registers");
            this.ResetButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(498, 244);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "label1";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Simulator_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.Simulator_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Simulator_RunWorkerCompleted);
            // 
            // Simulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1017);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.UndoOneButton);
            this.Controls.Add(this.PauseButton);
            this.Controls.Add(this.RunOneButton);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.source);
            this.MaximizeBox = false;
            this.Name = "Simulator";
            this.Text = "Simulator";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.source.ResumeLayout(false);
            this.text.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl source;
        private System.Windows.Forms.TabPage text;
        private System.Windows.Forms.TabPage data;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private FontAwesome.Sharp.IconButton RunButton;
        private FontAwesome.Sharp.IconButton RunOneButton;
        private FontAwesome.Sharp.IconButton PauseButton;
        private FontAwesome.Sharp.IconButton UndoOneButton;
        private FontAwesome.Sharp.IconButton StopButton;
        private FontAwesome.Sharp.IconButton ResetButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}