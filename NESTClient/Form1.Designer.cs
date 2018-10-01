namespace NESTClient
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
            this.cmdExecute = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panelResults = new System.Windows.Forms.Panel();
            this.scintilla = new ScintillaNET.Scintilla();
            this.cmdSimple = new System.Windows.Forms.Button();
            this.cbCode = new System.Windows.Forms.ComboBox();
            this.cbTraceTraffic = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cmdExecute
            // 
            this.cmdExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdExecute.Location = new System.Drawing.Point(265, 707);
            this.cmdExecute.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.cmdExecute.Name = "cmdExecute";
            this.cmdExecute.Size = new System.Drawing.Size(429, 58);
            this.cmdExecute.TabIndex = 0;
            this.cmdExecute.Text = "Execute";
            this.cmdExecute.UseVisualStyleBackColor = true;
            this.cmdExecute.Click += new System.EventHandler(this.cmdExecute_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 37);
            this.label1.TabIndex = 2;
            this.label1.Text = "Source Code";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(714, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 37);
            this.label2.TabIndex = 3;
            this.label2.Text = "Results";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.SeaGreen;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(1232, 48);
            this.label3.TabIndex = 4;
            this.label3.Text = "NEST Library Demo";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelResults
            // 
            this.panelResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelResults.Location = new System.Drawing.Point(721, 118);
            this.panelResults.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelResults.Name = "panelResults";
            this.panelResults.Size = new System.Drawing.Size(495, 565);
            this.panelResults.TabIndex = 5;
            // 
            // scintilla
            // 
            this.scintilla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.scintilla.Lexer = ScintillaNET.Lexer.Cpp;
            this.scintilla.Location = new System.Drawing.Point(0, 118);
            this.scintilla.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.scintilla.Name = "scintilla";
            this.scintilla.Size = new System.Drawing.Size(694, 565);
            this.scintilla.TabIndex = 6;
            this.scintilla.Text = "var searchResponse = client.Search<post>(s => s\r\n    .Size(10)\r\n    .Query(q => q" +
    "\r\n         .Match(m => m\r\n            .Field(f => f.body)\r\n           .Query(\"un" +
    "icorn\")\r\n         )\r\n    )\r\n);\r\n";
            // 
            // cmdSimple
            // 
            this.cmdSimple.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdSimple.Location = new System.Drawing.Point(7, 707);
            this.cmdSimple.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.cmdSimple.Name = "cmdSimple";
            this.cmdSimple.Size = new System.Drawing.Size(238, 58);
            this.cmdSimple.TabIndex = 7;
            this.cmdSimple.Text = "Simple";
            this.cmdSimple.UseVisualStyleBackColor = true;
            this.cmdSimple.Click += new System.EventHandler(this.cmdSimple_Click);
            // 
            // cbCode
            // 
            this.cbCode.FormattingEnabled = true;
            this.cbCode.Location = new System.Drawing.Point(504, 82);
            this.cbCode.Name = "cbCode";
            this.cbCode.Size = new System.Drawing.Size(190, 36);
            this.cbCode.TabIndex = 8;
            this.cbCode.SelectedIndexChanged += new System.EventHandler(this.cbCode_SelectedIndexChanged);
            // 
            // cbTraceTraffic
            // 
            this.cbTraceTraffic.AutoSize = true;
            this.cbTraceTraffic.Location = new System.Drawing.Point(366, 82);
            this.cbTraceTraffic.Name = "cbTraceTraffic";
            this.cbTraceTraffic.Size = new System.Drawing.Size(132, 32);
            this.cbTraceTraffic.TabIndex = 16;
            this.cbTraceTraffic.Text = "Use Fiddler";
            this.cbTraceTraffic.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1232, 779);
            this.Controls.Add(this.cbTraceTraffic);
            this.Controls.Add(this.cbCode);
            this.Controls.Add(this.cmdSimple);
            this.Controls.Add(this.panelResults);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdExecute);
            this.Controls.Add(this.scintilla);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "Form1";
            this.Text = "Elastic Search - NEST Library";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdExecute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelResults;
        private ScintillaNET.Scintilla scintilla;
        private System.Windows.Forms.Button cmdSimple;
        private System.Windows.Forms.ComboBox cbCode;
        private System.Windows.Forms.CheckBox cbTraceTraffic;
    }
}

