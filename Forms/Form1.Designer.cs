namespace c_lan
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TestButton = new Button();
            dataGridView1 = new DataGridView();
            ConnectionnameText = new TextBox();
            HostText = new TextBox();
            PortText = new TextBox();
            ConnectionnameLabel = new Label();
            HostLabel = new Label();
            PortLabel = new Label();
            UserText = new TextBox();
            PasswordText = new TextBox();
            UserLabel = new Label();
            PasswordLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // TestButton
            // 
            TestButton.Location = new Point(572, 61);
            TestButton.Name = "TestButton";
            TestButton.Size = new Size(78, 34);
            TestButton.TabIndex = 0;
            TestButton.Text = "测试连接";
            TestButton.Click += TestButton_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(561, 242);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(204, 108);
            dataGridView1.TabIndex = 1;
            // 
            // ConnectionnameText
            // 
            ConnectionnameText.Location = new Point(211, 24);
            ConnectionnameText.Name = "ConnectionnameText";
            ConnectionnameText.Size = new Size(244, 27);
            ConnectionnameText.TabIndex = 2;
            // 
            // HostText
            // 
            HostText.Location = new Point(211, 68);
            HostText.Name = "HostText";
            HostText.Size = new Size(244, 27);
            HostText.TabIndex = 3;
            // 
            // PortText
            // 
            PortText.Location = new Point(211, 115);
            PortText.Name = "PortText";
            PortText.Size = new Size(244, 27);
            PortText.TabIndex = 4;
            // 
            // ConnectionnameLabel
            // 
            ConnectionnameLabel.AutoSize = true;
            ConnectionnameLabel.Location = new Point(136, 27);
            ConnectionnameLabel.Name = "ConnectionnameLabel";
            ConnectionnameLabel.Size = new Size(69, 20);
            ConnectionnameLabel.TabIndex = 5;
            ConnectionnameLabel.Text = "连接名称";
            // 
            // HostLabel
            // 
            HostLabel.AutoSize = true;
            HostLabel.Location = new Point(142, 75);
            HostLabel.Name = "HostLabel";
            HostLabel.Size = new Size(39, 20);
            HostLabel.TabIndex = 6;
            HostLabel.Text = "主机";
            // 
            // PortLabel
            // 
            PortLabel.AutoSize = true;
            PortLabel.Location = new Point(142, 122);
            PortLabel.Name = "PortLabel";
            PortLabel.Size = new Size(39, 20);
            PortLabel.TabIndex = 7;
            PortLabel.Text = "端口";
            // 
            // UserText
            // 
            UserText.Location = new Point(211, 167);
            UserText.Name = "UserText";
            UserText.Size = new Size(244, 27);
            UserText.TabIndex = 8;
            // 
            // PasswordText
            // 
            PasswordText.Location = new Point(211, 221);
            PasswordText.Name = "PasswordText";
            PasswordText.Size = new Size(244, 27);
            PasswordText.TabIndex = 9;
            // 
            // UserLabel
            // 
            UserLabel.AutoSize = true;
            UserLabel.Location = new Point(142, 170);
            UserLabel.Name = "UserLabel";
            UserLabel.Size = new Size(54, 20);
            UserLabel.TabIndex = 10;
            UserLabel.Text = "用户名";
            // 
            // PasswordLabel
            // 
            PasswordLabel.AutoSize = true;
            PasswordLabel.Location = new Point(142, 224);
            PasswordLabel.Name = "PasswordLabel";
            PasswordLabel.Size = new Size(39, 20);
            PasswordLabel.TabIndex = 11;
            PasswordLabel.Text = "密码";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(827, 469);
            Controls.Add(PasswordLabel);
            Controls.Add(UserLabel);
            Controls.Add(PasswordText);
            Controls.Add(UserText);
            Controls.Add(PortLabel);
            Controls.Add(HostLabel);
            Controls.Add(ConnectionnameLabel);
            Controls.Add(PortText);
            Controls.Add(HostText);
            Controls.Add(ConnectionnameText);
            Controls.Add(dataGridView1);
            Controls.Add(TestButton);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button TestButton;
        private DataGridView dataGridView1;
        private TextBox ConnectionnameText;
        private TextBox HostText;
        private TextBox PortText;
        private Label ConnectionnameLabel;
        private Label HostLabel;
        private Label PortLabel;
        private TextBox UserText;
        private TextBox PasswordText;
        private Label UserLabel;
        private Label PasswordLabel;
    }
}
