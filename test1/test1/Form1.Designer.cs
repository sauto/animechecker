namespace test1
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Check = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RestTime = new System.Windows.Forms.TextBox();
            this.RestTimeText = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.メニュー = new System.Windows.Forms.ToolStripMenuItem();
            this.保存CtrlSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.オプションToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.レイアウトリセットToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.Azure;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Check,
            this.Title,
            this.Time});
            this.dataGridView1.Location = new System.Drawing.Point(32, 58);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(268, 218);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentDoubleClick);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            this.dataGridView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseMove);
            this.dataGridView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseUp);
            // 
            // Check
            // 
            this.Check.FalseValue = "false";
            this.Check.HeaderText = "";
            this.Check.Name = "Check";
            this.Check.TrueValue = "true";
            this.Check.Width = 25;
            // 
            // Title
            // 
            this.Title.HeaderText = "タイトル";
            this.Title.Name = "Title";
            // 
            // Time
            // 
            this.Time.HeaderText = "視聴時間(分)";
            this.Time.Name = "Time";
            // 
            // RestTime
            // 
            this.RestTime.Location = new System.Drawing.Point(64, 11);
            this.RestTime.Name = "RestTime";
            this.RestTime.ReadOnly = true;
            this.RestTime.Size = new System.Drawing.Size(100, 19);
            this.RestTime.TabIndex = 5;
            this.RestTime.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RestTime_MouseDown);
            this.RestTime.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RestTime_MouseMove);
            this.RestTime.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RestTime_MouseUp);
            // 
            // RestTimeText
            // 
            this.RestTimeText.AutoSize = true;
            this.RestTimeText.Location = new System.Drawing.Point(9, 14);
            this.RestTimeText.Name = "RestTimeText";
            this.RestTimeText.Size = new System.Drawing.Size(49, 12);
            this.RestTimeText.TabIndex = 6;
            this.RestTimeText.Text = "残り時間";
            this.RestTimeText.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.RestTimeText.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RestTimeText_MouseDown);
            this.RestTimeText.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RestTimeText_MouseMove);
            this.RestTimeText.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RestTimeText_MouseUp);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(354, 367);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(100, 16);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "testcheckBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.testcheckBox1_CheckedChanged);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(225, 290);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 8;
            this.SaveButton.Text = "保存";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            this.SaveButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SaveButton_MouseDown);
            this.SaveButton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SaveButton_MouseMove);
            this.SaveButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SaveButton_MouseUp);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(32, 29);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(75, 23);
            this.AddButton.TabIndex = 9;
            this.AddButton.Text = "行の追加";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            this.AddButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AddButton_MouseDown);
            this.AddButton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AddButton_MouseMove);
            this.AddButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AddButton_MouseUp);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.メニュー,
            this.オプションToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(491, 26);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // メニュー
            // 
            this.メニュー.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.メニュー.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.保存CtrlSToolStripMenuItem});
            this.メニュー.Name = "メニュー";
            this.メニュー.Size = new System.Drawing.Size(68, 22);
            this.メニュー.Text = "メニュー";
            // 
            // 保存CtrlSToolStripMenuItem
            // 
            this.保存CtrlSToolStripMenuItem.Name = "保存CtrlSToolStripMenuItem";
            this.保存CtrlSToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.保存CtrlSToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.保存CtrlSToolStripMenuItem.Text = "保存";
            this.保存CtrlSToolStripMenuItem.Click += new System.EventHandler(this.CtrlSToolStripMenuItem_Click);
            // 
            // オプションToolStripMenuItem
            // 
            this.オプションToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.レイアウトリセットToolStripMenuItem});
            this.オプションToolStripMenuItem.Name = "オプションToolStripMenuItem";
            this.オプションToolStripMenuItem.Size = new System.Drawing.Size(80, 22);
            this.オプションToolStripMenuItem.Text = "オプション";
            // 
            // レイアウトリセットToolStripMenuItem
            // 
            this.レイアウトリセットToolStripMenuItem.Name = "レイアウトリセットToolStripMenuItem";
            this.レイアウトリセットToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.レイアウトリセットToolStripMenuItem.Text = "レイアウトリセット";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(491, 393);
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RestTimeText);
            this.panel1.Controls.Add(this.RestTime);
            this.panel1.Location = new System.Drawing.Point(19, 283);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 12;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(491, 393);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "アニメ視聴チェックリスト";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox RestTime;
        private System.Windows.Forms.Label RestTimeText;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem メニュー;
        private System.Windows.Forms.ToolStripMenuItem 保存CtrlSToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem オプションToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem レイアウトリセットToolStripMenuItem;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Check;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.Panel panel1;

    }
}

