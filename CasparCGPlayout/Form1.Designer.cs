using CasparCGPlayout.components;
using Svt.Caspar;

namespace CasparCGPlayout
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolstriplabelAMCPConnected = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolstriplabelOSCConnected = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolstriplabelDBConnected = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStrip1FileLabel = new System.Windows.Forms.ToolStripLabel();
            this.tslAbout = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsLabel_STOPALL = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDogToggle = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.lblPastDesc = new System.Windows.Forms.Label();
            this.lblCountUp = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pgbcountdown = new System.Windows.Forms.ProgressBar();
            this.lblRemainDesc = new System.Windows.Forms.Label();
            this.lblCountDown = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblLengthDesc = new System.Windows.Forms.Label();
            this.lblLength = new System.Windows.Forms.Label();
            this.TmrNoItemDuration = new System.Windows.Forms.Timer(this.components);
            this.tmrLastSeconds = new System.Windows.Forms.Timer(this.components);
            this.btnGoNext = new System.Windows.Forms.Button();
            this.tmrConnectionCheck = new System.Windows.Forms.Timer(this.components);
            this.tmrClockStarts = new System.Windows.Forms.Timer(this.components);
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnHold = new System.Windows.Forms.Button();
            this.tmrCGTiming = new System.Windows.Forms.Timer(this.components);
            this.tmrNTPUpdate = new System.Windows.Forms.Timer(this.components);
            this.ListRunningOrder = new CasparCGPlayout.components.ExListBox();
            this.digitalStudioClock2 = new CasparCGPlayout.Controls.DigitalStudioClock();
            this.digitalStudioClock1 = new CasparCGPlayout.Controls.DigitalStudioClock();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.AllowMerge = false;
            this.statusStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstriplabelAMCPConnected,
            this.toolstriplabelOSCConnected,
            this.toolstriplabelDBConnected});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.SizingGrip = false;
            // 
            // toolstriplabelAMCPConnected
            // 
            this.toolstriplabelAMCPConnected.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.toolstriplabelAMCPConnected, "toolstriplabelAMCPConnected");
            this.toolstriplabelAMCPConnected.Name = "toolstriplabelAMCPConnected";
            this.toolstriplabelAMCPConnected.Spring = true;
            // 
            // toolstriplabelOSCConnected
            // 
            this.toolstriplabelOSCConnected.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.toolstriplabelOSCConnected, "toolstriplabelOSCConnected");
            this.toolstriplabelOSCConnected.Name = "toolstriplabelOSCConnected";
            this.toolstriplabelOSCConnected.Spring = true;
            // 
            // toolstriplabelDBConnected
            // 
            this.toolstriplabelDBConnected.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.toolstriplabelDBConnected, "toolstriplabelDBConnected");
            this.toolstriplabelDBConnected.Name = "toolstriplabelDBConnected";
            this.toolstriplabelDBConnected.Spring = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip1FileLabel,
            this.tslAbout,
            this.toolStripSeparator1,
            this.tsLabel_STOPALL,
            this.toolStripSeparator2});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStrip1FileLabel
            // 
            this.toolStrip1FileLabel.Name = "toolStrip1FileLabel";
            resources.ApplyResources(this.toolStrip1FileLabel, "toolStrip1FileLabel");
            // 
            // tslAbout
            // 
            this.tslAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tslAbout.Name = "tslAbout";
            resources.ApplyResources(this.tslAbout, "tslAbout");
            this.tslAbout.Click += new System.EventHandler(this.TslAboutClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tsLabel_STOPALL
            // 
            this.tsLabel_STOPALL.Name = "tsLabel_STOPALL";
            resources.ApplyResources(this.tsLabel_STOPALL, "tsLabel_STOPALL");
            this.tsLabel_STOPALL.Click += new System.EventHandler(this.TsLabelStopallClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // btnDogToggle
            // 
            this.btnDogToggle.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            resources.ApplyResources(this.btnDogToggle, "btnDogToggle");
            this.btnDogToggle.Name = "btnDogToggle";
            this.btnDogToggle.UseVisualStyleBackColor = false;
            this.btnDogToggle.Click += new System.EventHandler(this.btnDogToggleClick);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel2.Controls.Add(this.progressBar2);
            this.panel2.Controls.Add(this.lblPastDesc);
            this.panel2.Controls.Add(this.lblCountUp);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // progressBar2
            // 
            resources.ApplyResources(this.progressBar2, "progressBar2");
            this.progressBar2.Name = "progressBar2";
            // 
            // lblPastDesc
            // 
            resources.ApplyResources(this.lblPastDesc, "lblPastDesc");
            this.lblPastDesc.Name = "lblPastDesc";
            // 
            // lblCountUp
            // 
            resources.ApplyResources(this.lblCountUp, "lblCountUp");
            this.lblCountUp.Name = "lblCountUp";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel3.Controls.Add(this.pgbcountdown);
            this.panel3.Controls.Add(this.lblRemainDesc);
            this.panel3.Controls.Add(this.lblCountDown);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // pgbcountdown
            // 
            resources.ApplyResources(this.pgbcountdown, "pgbcountdown");
            this.pgbcountdown.Name = "pgbcountdown";
            // 
            // lblRemainDesc
            // 
            resources.ApplyResources(this.lblRemainDesc, "lblRemainDesc");
            this.lblRemainDesc.Name = "lblRemainDesc";
            // 
            // lblCountDown
            // 
            resources.ApplyResources(this.lblCountDown, "lblCountDown");
            this.lblCountDown.Name = "lblCountDown";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel4.Controls.Add(this.lblLengthDesc);
            this.panel4.Controls.Add(this.lblLength);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // lblLengthDesc
            // 
            resources.ApplyResources(this.lblLengthDesc, "lblLengthDesc");
            this.lblLengthDesc.Name = "lblLengthDesc";
            // 
            // lblLength
            // 
            this.lblLength.BackColor = System.Drawing.SystemColors.ActiveCaption;
            resources.ApplyResources(this.lblLength, "lblLength");
            this.lblLength.Name = "lblLength";
            // 
            // TmrNoItemDuration
            // 
            this.TmrNoItemDuration.Interval = 500;
            this.TmrNoItemDuration.Tick += new System.EventHandler(this.tmrNoItemDurationTick);
            // 
            // tmrLastSeconds
            // 
            this.tmrLastSeconds.Interval = 500;
            this.tmrLastSeconds.Tick += new System.EventHandler(this.tmrLastSecondsTick);
            // 
            // btnGoNext
            // 
            this.btnGoNext.BackColor = System.Drawing.Color.Lime;
            resources.ApplyResources(this.btnGoNext, "btnGoNext");
            this.btnGoNext.Name = "btnGoNext";
            this.btnGoNext.UseVisualStyleBackColor = false;
            this.btnGoNext.Click += new System.EventHandler(this.btnGoNextClick);
            // 
            // tmrConnectionCheck
            // 
            this.tmrConnectionCheck.Tick += new System.EventHandler(this.tmrConnectionCheckTick);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            resources.ApplyResources(this.listBox1, "listBox1");
            this.listBox1.Name = "listBox1";
            this.listBox1.DoubleClick += new System.EventHandler(this.ListBox1DoubleClick);
            // 
            // btnHold
            // 
            this.btnHold.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.btnHold, "btnHold");
            this.btnHold.Name = "btnHold";
            this.btnHold.UseVisualStyleBackColor = false;
            this.btnHold.Click += new System.EventHandler(this.BtnHoldClick);
            // 
            // tmrCGTiming
            // 
            this.tmrCGTiming.Interval = 1000;
            // 
            // tmrNTPUpdate
            // 
            this.tmrNTPUpdate.Interval = 1024000;
            this.tmrNTPUpdate.Tick += new System.EventHandler(this.tmrNtpUpdateTick);
            // 
            // ListRunningOrder
            // 
            this.ListRunningOrder.BackColor = System.Drawing.Color.DarkKhaki;
            this.ListRunningOrder.FormattingEnabled = true;
            resources.ApplyResources(this.ListRunningOrder, "ListRunningOrder");
            this.ListRunningOrder.Name = "ListRunningOrder";
            // 
            // digitalStudioClock2
            // 
            this.digitalStudioClock2.BackColor = System.Drawing.Color.Black;
            this.digitalStudioClock2.DigitColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.digitalStudioClock2, "digitalStudioClock2");
            this.digitalStudioClock2.Name = "digitalStudioClock2";
            // 
            // digitalStudioClock1
            // 
            this.digitalStudioClock1.BackColor = System.Drawing.Color.Black;
            this.digitalStudioClock1.DigitColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.digitalStudioClock1, "digitalStudioClock1");
            this.digitalStudioClock1.Name = "digitalStudioClock1";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ListRunningOrder);
            this.Controls.Add(this.digitalStudioClock2);
            this.Controls.Add(this.btnHold);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnGoNext);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnDogToggle);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Tag = "";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.form1FormClosed);
            this.Load += new System.EventHandler(this.form1Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStrip1FileLabel;
        private System.Windows.Forms.Button btnDogToggle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblPastDesc;
        private System.Windows.Forms.Label lblCountUp;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblRemainDesc;
        private System.Windows.Forms.Label lblCountDown;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblLengthDesc;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.Timer TmrNoItemDuration;
        private System.Windows.Forms.Timer tmrLastSeconds;
        private System.Windows.Forms.Button btnGoNext;
        private System.Windows.Forms.ToolStripLabel tslAbout;
        private System.Windows.Forms.Timer tmrConnectionCheck;
        private System.Windows.Forms.Timer tmrClockStarts;
        private CasparCGPlayout.Controls.DigitalStudioClock digitalStudioClock1;
        private System.Windows.Forms.ToolStripStatusLabel toolstriplabelAMCPConnected;
        private System.Windows.Forms.ToolStripStatusLabel toolstriplabelOSCConnected;
        private System.Windows.Forms.ToolStripStatusLabel toolstriplabelDBConnected;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnHold;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel tsLabel_STOPALL;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Timer tmrCGTiming;
        private System.Windows.Forms.Timer tmrNTPUpdate;
        private System.Windows.Forms.ProgressBar pgbcountdown;
        private Controls.DigitalStudioClock digitalStudioClock2;
        private System.Windows.Forms.ProgressBar progressBar2;
        private ExListBox ListRunningOrder;
    }
}

