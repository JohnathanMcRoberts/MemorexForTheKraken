namespace WindowsFormsApplicationForPanLearning
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
            this.buttonForTesting = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonForTesting
            // 
            this.buttonForTesting.Location = new System.Drawing.Point(51, 217);
            this.buttonForTesting.Name = "buttonForTesting";
            this.buttonForTesting.Size = new System.Drawing.Size(187, 23);
            this.buttonForTesting.TabIndex = 0;
            this.buttonForTesting.Text = "big red button";
            this.buttonForTesting.UseVisualStyleBackColor = true;
            this.buttonForTesting.Click += new System.EventHandler(this.buttonForTesting_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.buttonForTesting);
            this.Name = "Form1";
            this.Text = "Test Main Form";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonForTesting;
    }
}

