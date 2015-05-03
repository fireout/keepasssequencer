namespace Sequencer.Forms
{
    partial class WordSequenceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WordSequenceForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtWordSelection = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboCapitalization = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboSubstitutionCount = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtExtras = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboExtraCount = new System.Windows.Forms.ComboBox();
            this.txtSubstitutionTo = new System.Windows.Forms.TextBox();
            this.txtSubstitutionFrom = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboWordCount = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.txtWordSelection);
            this.groupBox1.MinimumSize = new System.Drawing.Size(50, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // txtWordSelection
            // 
            resources.ApplyResources(this.txtWordSelection, "txtWordSelection");
            this.txtWordSelection.Name = "txtWordSelection";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cboCapitalization);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.cboSubstitutionCount);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtExtras);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cboExtraCount);
            this.groupBox2.Controls.Add(this.txtSubstitutionTo);
            this.groupBox2.Controls.Add(this.txtSubstitutionFrom);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cboWordCount);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // cboCapitalization
            // 
            resources.ApplyResources(this.cboCapitalization, "cboCapitalization");
            this.cboCapitalization.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCapitalization.FormattingEnabled = true;
            this.cboCapitalization.Name = "cboCapitalization";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // cboSubstitutionCount
            // 
            this.cboSubstitutionCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSubstitutionCount.FormattingEnabled = true;
            resources.ApplyResources(this.cboSubstitutionCount, "cboSubstitutionCount");
            this.cboSubstitutionCount.Name = "cboSubstitutionCount";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtExtras
            // 
            resources.ApplyResources(this.txtExtras, "txtExtras");
            this.txtExtras.Name = "txtExtras";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cboExtraCount
            // 
            resources.ApplyResources(this.cboExtraCount, "cboExtraCount");
            this.cboExtraCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExtraCount.FormattingEnabled = true;
            this.cboExtraCount.Name = "cboExtraCount";
            // 
            // txtSubstitutionTo
            // 
            resources.ApplyResources(this.txtSubstitutionTo, "txtSubstitutionTo");
            this.txtSubstitutionTo.Name = "txtSubstitutionTo";
            // 
            // txtSubstitutionFrom
            // 
            resources.ApplyResources(this.txtSubstitutionFrom, "txtSubstitutionFrom");
            this.txtSubstitutionFrom.Name = "txtSubstitutionFrom";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cboWordCount
            // 
            this.cboWordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWordCount.FormattingEnabled = true;
            resources.ApplyResources(this.cboWordCount, "cboWordCount");
            this.cboWordCount.Name = "cboWordCount";
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // WordSequenceForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WordSequenceForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtWordSelection;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboExtraCount;
        private System.Windows.Forms.TextBox txtSubstitutionTo;
        private System.Windows.Forms.TextBox txtSubstitutionFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboWordCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtExtras;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboCapitalization;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboSubstitutionCount;
    }
}