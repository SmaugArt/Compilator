namespace Compilator
{
    partial class Form1
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.FilePath1Box = new System.Windows.Forms.TextBox();
            this.SearchPathButton1 = new System.Windows.Forms.Button();
            this.SearchButton2 = new System.Windows.Forms.Button();
            this.FilePatch2Box = new System.Windows.Forms.TextBox();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Текстовый документ|*.txt";
            // 
            // FilePath1Box
            // 
            this.FilePath1Box.Location = new System.Drawing.Point(13, 14);
            this.FilePath1Box.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FilePath1Box.Name = "FilePath1Box";
            this.FilePath1Box.ReadOnly = true;
            this.FilePath1Box.Size = new System.Drawing.Size(373, 22);
            this.FilePath1Box.TabIndex = 0;
            // 
            // SearchPathButton1
            // 
            this.SearchPathButton1.Location = new System.Drawing.Point(393, 11);
            this.SearchPathButton1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SearchPathButton1.Name = "SearchPathButton1";
            this.SearchPathButton1.Size = new System.Drawing.Size(75, 23);
            this.SearchPathButton1.TabIndex = 1;
            this.SearchPathButton1.Text = "Обзор";
            this.SearchPathButton1.UseVisualStyleBackColor = true;
            this.SearchPathButton1.Click += new System.EventHandler(this.SearchPathButton1_Click);
            // 
            // SearchButton2
            // 
            this.SearchButton2.Location = new System.Drawing.Point(392, 39);
            this.SearchButton2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SearchButton2.Name = "SearchButton2";
            this.SearchButton2.Size = new System.Drawing.Size(75, 23);
            this.SearchButton2.TabIndex = 3;
            this.SearchButton2.Text = "Обзор";
            this.SearchButton2.UseVisualStyleBackColor = true;
            this.SearchButton2.Click += new System.EventHandler(this.SearchButton2_Click);
            // 
            // FilePatch2Box
            // 
            this.FilePatch2Box.Location = new System.Drawing.Point(12, 41);
            this.FilePatch2Box.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FilePatch2Box.Name = "FilePatch2Box";
            this.FilePatch2Box.ReadOnly = true;
            this.FilePatch2Box.Size = new System.Drawing.Size(373, 22);
            this.FilePatch2Box.TabIndex = 2;
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog1";
            this.openFileDialog2.Filter = "Текстовый документ|*.txt";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(485, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Файл программы";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(485, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Файл проверки";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Calibri", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(13, 98);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(593, 340);
            this.textBox1.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(245, 69);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Проверить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SearchButton2);
            this.Controls.Add(this.FilePatch2Box);
            this.Controls.Add(this.SearchPathButton1);
            this.Controls.Add(this.FilePath1Box);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox FilePath1Box;
        private System.Windows.Forms.Button SearchPathButton1;
        private System.Windows.Forms.Button SearchButton2;
        private System.Windows.Forms.TextBox FilePatch2Box;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
    }
}

