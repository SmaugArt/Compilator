namespace Tester
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.FilePath2Box = new System.Windows.Forms.TextBox();
            this.FilePath1Box = new System.Windows.Forms.TextBox();
            this.BackButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.CheckButton = new System.Windows.Forms.Button();
            this.SelectProgrammExeButton = new System.Windows.Forms.Button();
            this.SelectTestDirectoryButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ProgrammBox = new System.Windows.Forms.TextBox();
            this.AnswerBox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // FilePath2Box
            // 
            this.FilePath2Box.Location = new System.Drawing.Point(186, 38);
            this.FilePath2Box.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FilePath2Box.Name = "FilePath2Box";
            this.FilePath2Box.ReadOnly = true;
            this.FilePath2Box.Size = new System.Drawing.Size(1015, 22);
            this.FilePath2Box.TabIndex = 4;
            this.FilePath2Box.Text = "Path to a true answer .txt file";
            // 
            // FilePath1Box
            // 
            this.FilePath1Box.Location = new System.Drawing.Point(186, 12);
            this.FilePath1Box.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FilePath1Box.Name = "FilePath1Box";
            this.FilePath1Box.ReadOnly = true;
            this.FilePath1Box.Size = new System.Drawing.Size(1015, 22);
            this.FilePath1Box.TabIndex = 3;
            this.FilePath1Box.Text = "Path to a programm .txt file";
            // 
            // BackButton
            // 
            this.BackButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BackButton.Enabled = false;
            this.BackButton.Font = new System.Drawing.Font("Trebuchet MS", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BackButton.Location = new System.Drawing.Point(129, 12);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(50, 49);
            this.BackButton.TabIndex = 5;
            this.BackButton.Text = "←";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            this.BackButton.MouseHover += new System.EventHandler(this.BackButton_MouseHover);
            // 
            // NextButton
            // 
            this.NextButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NextButton.Enabled = false;
            this.NextButton.Font = new System.Drawing.Font("Trebuchet MS", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NextButton.Location = new System.Drawing.Point(1207, 11);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(50, 49);
            this.NextButton.TabIndex = 6;
            this.NextButton.Text = "→";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            this.NextButton.MouseHover += new System.EventHandler(this.NextButton_MouseHover);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Выберите директорию тестов для программы";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Испытуемая программа|*.exe";
            // 
            // CheckButton
            // 
            this.CheckButton.BackgroundImage = global::Tester.Properties.Resources._379_3791335_svg_royalty_free_the_process_editing_blog_tags_svg_royalty_free_the;
            this.CheckButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.CheckButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CheckButton.Enabled = false;
            this.CheckButton.Font = new System.Drawing.Font("Trebuchet MS", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CheckButton.Location = new System.Drawing.Point(1263, 11);
            this.CheckButton.Name = "CheckButton";
            this.CheckButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CheckButton.Size = new System.Drawing.Size(50, 50);
            this.CheckButton.TabIndex = 11;
            this.CheckButton.UseVisualStyleBackColor = true;
            this.CheckButton.Click += new System.EventHandler(this.CheckButton_Click);
            this.CheckButton.MouseHover += new System.EventHandler(this.CheckButton_MouseHover);
            // 
            // SelectProgrammExeButton
            // 
            this.SelectProgrammExeButton.BackgroundImage = global::Tester.Properties.Resources._61f959ee9d;
            this.SelectProgrammExeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SelectProgrammExeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SelectProgrammExeButton.Font = new System.Drawing.Font("Trebuchet MS", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SelectProgrammExeButton.Location = new System.Drawing.Point(12, 12);
            this.SelectProgrammExeButton.Name = "SelectProgrammExeButton";
            this.SelectProgrammExeButton.Size = new System.Drawing.Size(50, 49);
            this.SelectProgrammExeButton.TabIndex = 9;
            this.SelectProgrammExeButton.UseVisualStyleBackColor = true;
            this.SelectProgrammExeButton.Click += new System.EventHandler(this.SelectProgrammExeButton_Click);
            this.SelectProgrammExeButton.MouseHover += new System.EventHandler(this.SelectProgrammExeButton_MouseHover);
            // 
            // SelectTestDirectoryButton
            // 
            this.SelectTestDirectoryButton.BackgroundImage = global::Tester.Properties.Resources.magnifier_3170000_1280;
            this.SelectTestDirectoryButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SelectTestDirectoryButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SelectTestDirectoryButton.Font = new System.Drawing.Font("Trebuchet MS", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SelectTestDirectoryButton.Location = new System.Drawing.Point(68, 12);
            this.SelectTestDirectoryButton.Name = "SelectTestDirectoryButton";
            this.SelectTestDirectoryButton.Size = new System.Drawing.Size(50, 49);
            this.SelectTestDirectoryButton.TabIndex = 7;
            this.SelectTestDirectoryButton.UseVisualStyleBackColor = true;
            this.SelectTestDirectoryButton.Click += new System.EventHandler(this.SelectTestDirectoryButton_Click);
            this.SelectTestDirectoryButton.MouseHover += new System.EventHandler(this.SelectTestDirectoryButton_MouseHover);
            // 
            // ProgrammBox
            // 
            this.ProgrammBox.Font = new System.Drawing.Font("Calibri", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProgrammBox.Location = new System.Drawing.Point(12, 66);
            this.ProgrammBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ProgrammBox.Multiline = true;
            this.ProgrammBox.Name = "ProgrammBox";
            this.ProgrammBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ProgrammBox.Size = new System.Drawing.Size(429, 482);
            this.ProgrammBox.TabIndex = 12;
            // 
            // AnswerBox
            // 
            this.AnswerBox.Font = new System.Drawing.Font("Calibri", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AnswerBox.Location = new System.Drawing.Point(884, 66);
            this.AnswerBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.AnswerBox.Multiline = true;
            this.AnswerBox.Name = "AnswerBox";
            this.AnswerBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.AnswerBox.Size = new System.Drawing.Size(429, 481);
            this.AnswerBox.TabIndex = 13;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Calibri", 13.8F);
            this.textBox1.Location = new System.Drawing.Point(447, 66);
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(431, 481);
            this.textBox1.TabIndex = 14;
            this.textBox1.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1322, 559);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.AnswerBox);
            this.Controls.Add(this.ProgrammBox);
            this.Controls.Add(this.CheckButton);
            this.Controls.Add(this.SelectProgrammExeButton);
            this.Controls.Add(this.SelectTestDirectoryButton);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.FilePath2Box);
            this.Controls.Add(this.FilePath1Box);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.Text = "Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox FilePath2Box;
        private System.Windows.Forms.TextBox FilePath1Box;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.Button SelectTestDirectoryButton;
        private System.Windows.Forms.Button SelectProgrammExeButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button CheckButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox ProgrammBox;
        private System.Windows.Forms.TextBox AnswerBox;
        private System.Windows.Forms.RichTextBox textBox1;
    }
}

