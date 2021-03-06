namespace MyAppGenerator
{
    partial class MainForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkWithTranslations = new System.Windows.Forms.CheckBox();
            this.chAddDapperToDbContext = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ddlKeyType = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.ddlArchitecture = new System.Windows.Forms.ComboBox();
            this.chkWithResourcesFile = new System.Windows.Forms.CheckBox();
            this.chklistCulutres = new System.Windows.Forms.CheckedListBox();
            this.txtProjectName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.schemaTreeView = new System.Windows.Forms.TreeView();
            this.btnGenerator = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.rbtnWindowsAuthentication = new System.Windows.Forms.RadioButton();
            this.txtLoginName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rbtnSQLServerAuthentication = new System.Windows.Forms.RadioButton();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.grpAuthentication = new System.Windows.Forms.GroupBox();
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.txtDataBase = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDomainCustomPath = new System.Windows.Forms.TextBox();
            this.txtApplicationCustomPath = new System.Windows.Forms.TextBox();
            this.txtInfrastructureCustomPath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtAPICustomPath = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.grpAuthentication.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkWithTranslations);
            this.groupBox1.Controls.Add(this.chAddDapperToDbContext);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.ddlKeyType);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.ddlArchitecture);
            this.groupBox1.Controls.Add(this.chkWithResourcesFile);
            this.groupBox1.Controls.Add(this.chklistCulutres);
            this.groupBox1.Controls.Add(this.txtProjectName);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(14, 314);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(349, 380);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "C#";
            // 
            // chkWithTranslations
            // 
            this.chkWithTranslations.AutoSize = true;
            this.chkWithTranslations.Location = new System.Drawing.Point(206, 204);
            this.chkWithTranslations.Name = "chkWithTranslations";
            this.chkWithTranslations.Size = new System.Drawing.Size(104, 17);
            this.chkWithTranslations.TabIndex = 29;
            this.chkWithTranslations.Text = "With Translation";
            this.chkWithTranslations.UseVisualStyleBackColor = true;
            // 
            // chAddDapperToDbContext
            // 
            this.chAddDapperToDbContext.AutoSize = true;
            this.chAddDapperToDbContext.Location = new System.Drawing.Point(111, 143);
            this.chAddDapperToDbContext.Name = "chAddDapperToDbContext";
            this.chAddDapperToDbContext.Size = new System.Drawing.Size(151, 17);
            this.chAddDapperToDbContext.TabIndex = 28;
            this.chAddDapperToDbContext.Text = "Add Dapper to DBContext";
            this.chAddDapperToDbContext.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 110);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Key Type:";
            // 
            // ddlKeyType
            // 
            this.ddlKeyType.FormattingEnabled = true;
            this.ddlKeyType.Items.AddRange(new object[] {
            "Int",
            "Uniqueidentifier"});
            this.ddlKeyType.Location = new System.Drawing.Point(111, 110);
            this.ddlKeyType.Name = "ddlKeyType";
            this.ddlKeyType.Size = new System.Drawing.Size(211, 21);
            this.ddlKeyType.TabIndex = 26;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 71);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "Architecture:";
            // 
            // ddlArchitecture
            // 
            this.ddlArchitecture.FormattingEnabled = true;
            this.ddlArchitecture.Items.AddRange(new object[] {
            "Union Architecture",
            "CQRS Architecture",
            "CQRS 2 Architecture",
            "DbContext Architecture",
            "Clean Arch Repository EF",
            "Clean Arch Repository Dapper"});
            this.ddlArchitecture.Location = new System.Drawing.Point(111, 71);
            this.ddlArchitecture.Name = "ddlArchitecture";
            this.ddlArchitecture.Size = new System.Drawing.Size(211, 21);
            this.ddlArchitecture.TabIndex = 24;
            // 
            // chkWithResourcesFile
            // 
            this.chkWithResourcesFile.AutoSize = true;
            this.chkWithResourcesFile.Location = new System.Drawing.Point(15, 204);
            this.chkWithResourcesFile.Name = "chkWithResourcesFile";
            this.chkWithResourcesFile.Size = new System.Drawing.Size(120, 17);
            this.chkWithResourcesFile.TabIndex = 18;
            this.chkWithResourcesFile.Text = "With Resources File";
            this.chkWithResourcesFile.UseVisualStyleBackColor = true;
            this.chkWithResourcesFile.CheckedChanged += new System.EventHandler(this.chkWithResourcesFile_CheckedChanged);
            // 
            // chklistCulutres
            // 
            this.chklistCulutres.Enabled = false;
            this.chklistCulutres.FormattingEnabled = true;
            this.chklistCulutres.Items.AddRange(new object[] {
            "ar Arabic",
            "de German",
            "en English",
            "es Spanish",
            "fr French"});
            this.chklistCulutres.Location = new System.Drawing.Point(16, 227);
            this.chklistCulutres.Name = "chklistCulutres";
            this.chklistCulutres.Size = new System.Drawing.Size(294, 94);
            this.chklistCulutres.TabIndex = 17;
            // 
            // txtProjectName
            // 
            this.txtProjectName.Location = new System.Drawing.Point(111, 32);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Size = new System.Drawing.Size(213, 20);
            this.txtProjectName.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Project Name:";
            // 
            // schemaTreeView
            // 
            this.schemaTreeView.CheckBoxes = true;
            this.schemaTreeView.Location = new System.Drawing.Point(404, 22);
            this.schemaTreeView.Name = "schemaTreeView";
            this.schemaTreeView.Size = new System.Drawing.Size(522, 327);
            this.schemaTreeView.TabIndex = 20;
            this.schemaTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.schemaTreeView_AfterCheck);
            // 
            // btnGenerator
            // 
            this.btnGenerator.Location = new System.Drawing.Point(404, 355);
            this.btnGenerator.Name = "btnGenerator";
            this.btnGenerator.Size = new System.Drawing.Size(141, 23);
            this.btnGenerator.TabIndex = 19;
            this.btnGenerator.Text = "Generate DAL";
            this.btnGenerator.UseVisualStyleBackColor = true;
            this.btnGenerator.Click += new System.EventHandler(this.btnGenerator_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Password:";
            // 
            // rbtnWindowsAuthentication
            // 
            this.rbtnWindowsAuthentication.AutoSize = true;
            this.rbtnWindowsAuthentication.Checked = true;
            this.rbtnWindowsAuthentication.Location = new System.Drawing.Point(15, 21);
            this.rbtnWindowsAuthentication.Name = "rbtnWindowsAuthentication";
            this.rbtnWindowsAuthentication.Size = new System.Drawing.Size(141, 17);
            this.rbtnWindowsAuthentication.TabIndex = 0;
            this.rbtnWindowsAuthentication.TabStop = true;
            this.rbtnWindowsAuthentication.Text = "Windows Authentication";
            this.rbtnWindowsAuthentication.UseVisualStyleBackColor = true;
            this.rbtnWindowsAuthentication.CheckedChanged += new System.EventHandler(this.rbtnWindowsAuthentication_CheckedChanged);
            // 
            // txtLoginName
            // 
            this.txtLoginName.Location = new System.Drawing.Point(97, 65);
            this.txtLoginName.Name = "txtLoginName";
            this.txtLoginName.Size = new System.Drawing.Size(167, 20);
            this.txtLoginName.TabIndex = 6;
            this.txtLoginName.TextChanged += new System.EventHandler(this.txtLoginName_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Login Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Server";
            // 
            // rbtnSQLServerAuthentication
            // 
            this.rbtnSQLServerAuthentication.AutoSize = true;
            this.rbtnSQLServerAuthentication.Location = new System.Drawing.Point(15, 44);
            this.rbtnSQLServerAuthentication.Name = "rbtnSQLServerAuthentication";
            this.rbtnSQLServerAuthentication.Size = new System.Drawing.Size(152, 17);
            this.rbtnSQLServerAuthentication.TabIndex = 1;
            this.rbtnSQLServerAuthentication.Text = "SQL Server Authentication";
            this.rbtnSQLServerAuthentication.UseVisualStyleBackColor = true;
            this.rbtnSQLServerAuthentication.CheckedChanged += new System.EventHandler(this.rbtnSQLServerAuthentication_CheckedChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(97, 91);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(167, 20);
            this.txtPassword.TabIndex = 8;
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(80, 23);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(275, 20);
            this.txtServer.TabIndex = 15;
            this.txtServer.TextChanged += new System.EventHandler(this.txtServer_TextChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(66, 129);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(141, 23);
            this.btnConnect.TabIndex = 9;
            this.btnConnect.Text = "Connect To Server";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // grpAuthentication
            // 
            this.grpAuthentication.Controls.Add(this.txtConnectionString);
            this.grpAuthentication.Controls.Add(this.btnConnect);
            this.grpAuthentication.Controls.Add(this.txtPassword);
            this.grpAuthentication.Controls.Add(this.rbtnSQLServerAuthentication);
            this.grpAuthentication.Controls.Add(this.label3);
            this.grpAuthentication.Controls.Add(this.rbtnWindowsAuthentication);
            this.grpAuthentication.Controls.Add(this.txtLoginName);
            this.grpAuthentication.Controls.Add(this.label4);
            this.grpAuthentication.Location = new System.Drawing.Point(14, 85);
            this.grpAuthentication.Name = "grpAuthentication";
            this.grpAuthentication.Size = new System.Drawing.Size(349, 223);
            this.grpAuthentication.TabIndex = 18;
            this.grpAuthentication.TabStop = false;
            this.grpAuthentication.Text = "Authentication";
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Location = new System.Drawing.Point(4, 183);
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(339, 20);
            this.txtConnectionString.TabIndex = 10;
            // 
            // txtDataBase
            // 
            this.txtDataBase.Location = new System.Drawing.Point(80, 49);
            this.txtDataBase.Name = "txtDataBase";
            this.txtDataBase.Size = new System.Drawing.Size(275, 20);
            this.txtDataBase.TabIndex = 17;
            this.txtDataBase.TextChanged += new System.EventHandler(this.txtDataBase_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "DataBase";
            // 
            // txtDomainCustomPath
            // 
            this.txtDomainCustomPath.Location = new System.Drawing.Point(554, 395);
            this.txtDomainCustomPath.Name = "txtDomainCustomPath";
            this.txtDomainCustomPath.Size = new System.Drawing.Size(372, 20);
            this.txtDomainCustomPath.TabIndex = 22;
            // 
            // txtApplicationCustomPath
            // 
            this.txtApplicationCustomPath.Location = new System.Drawing.Point(554, 424);
            this.txtApplicationCustomPath.Name = "txtApplicationCustomPath";
            this.txtApplicationCustomPath.Size = new System.Drawing.Size(372, 20);
            this.txtApplicationCustomPath.TabIndex = 23;
            // 
            // txtInfrastructureCustomPath
            // 
            this.txtInfrastructureCustomPath.Location = new System.Drawing.Point(554, 450);
            this.txtInfrastructureCustomPath.Name = "txtInfrastructureCustomPath";
            this.txtInfrastructureCustomPath.Size = new System.Drawing.Size(372, 20);
            this.txtInfrastructureCustomPath.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(401, 431);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 13);
            this.label6.TabIndex = 26;
            this.label6.Text = "Application Custom Path:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(401, 398);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(110, 13);
            this.label7.TabIndex = 27;
            this.label7.Text = "Domain Custom Path:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(401, 457);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(143, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Infrastructure Custom Path:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(401, 481);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(92, 13);
            this.label11.TabIndex = 30;
            this.label11.Text = "API Custom Path:";
            // 
            // txtAPICustomPath
            // 
            this.txtAPICustomPath.Location = new System.Drawing.Point(554, 474);
            this.txtAPICustomPath.Name = "txtAPICustomPath";
            this.txtAPICustomPath.Size = new System.Drawing.Size(372, 20);
            this.txtAPICustomPath.TabIndex = 29;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 609);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtAPICustomPath);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtInfrastructureCustomPath);
            this.Controls.Add(this.txtApplicationCustomPath);
            this.Controls.Add(this.txtDomainCustomPath);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.schemaTreeView);
            this.Controls.Add(this.btnGenerator);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.grpAuthentication);
            this.Controls.Add(this.txtDataBase);
            this.Controls.Add(this.label2);
            this.Name = "MainForm";
            this.Text = "My App Generator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpAuthentication.ResumeLayout(false);
            this.grpAuthentication.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox ddlArchitecture;
        private System.Windows.Forms.CheckBox chkWithResourcesFile;
        private System.Windows.Forms.CheckedListBox chklistCulutres;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TreeView schemaTreeView;
        private System.Windows.Forms.Button btnGenerator;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbtnWindowsAuthentication;
        private System.Windows.Forms.TextBox txtLoginName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbtnSQLServerAuthentication;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox grpAuthentication;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.TextBox txtDataBase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDomainCustomPath;
        private System.Windows.Forms.TextBox txtApplicationCustomPath;
        private System.Windows.Forms.TextBox txtInfrastructureCustomPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox ddlKeyType;
        private System.Windows.Forms.CheckBox chAddDapperToDbContext;
        private System.Windows.Forms.CheckBox chkWithTranslations;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtAPICustomPath;
    }
}

