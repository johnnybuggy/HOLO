namespace HoloUI.Controls
{
    partial class FindSimilarPropertiesForm
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
            this.cbVolumeDistr = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cbIntensity = new System.Windows.Forms.CheckBox();
            this.cbLongRhythm = new System.Windows.Forms.CheckBox();
            this.cbShortRhythm = new System.Windows.Forms.CheckBox();
            this.cbAmpEnvelope = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbVolumeDistr
            // 
            this.cbVolumeDistr.AutoSize = true;
            this.cbVolumeDistr.Checked = true;
            this.cbVolumeDistr.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVolumeDistr.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbVolumeDistr.Location = new System.Drawing.Point(91, 56);
            this.cbVolumeDistr.Name = "cbVolumeDistr";
            this.cbVolumeDistr.Size = new System.Drawing.Size(163, 24);
            this.cbVolumeDistr.TabIndex = 0;
            this.cbVolumeDistr.Text = "Volume distribution";
            this.cbVolumeDistr.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(193, 250);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(111, 34);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(346, 250);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(106, 34);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cbIntensity
            // 
            this.cbIntensity.AutoSize = true;
            this.cbIntensity.Checked = true;
            this.cbIntensity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIntensity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbIntensity.Location = new System.Drawing.Point(91, 86);
            this.cbIntensity.Name = "cbIntensity";
            this.cbIntensity.Size = new System.Drawing.Size(88, 24);
            this.cbIntensity.TabIndex = 3;
            this.cbIntensity.Text = "Intensity";
            this.cbIntensity.UseVisualStyleBackColor = true;
            // 
            // cbLongRhythm
            // 
            this.cbLongRhythm.AutoSize = true;
            this.cbLongRhythm.Checked = true;
            this.cbLongRhythm.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLongRhythm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbLongRhythm.Location = new System.Drawing.Point(91, 116);
            this.cbLongRhythm.Name = "cbLongRhythm";
            this.cbLongRhythm.Size = new System.Drawing.Size(201, 24);
            this.cbLongRhythm.TabIndex = 4;
            this.cbLongRhythm.Text = "Long rhythm tempogram";
            this.cbLongRhythm.UseVisualStyleBackColor = true;
            // 
            // cbShortRhythm
            // 
            this.cbShortRhythm.AutoSize = true;
            this.cbShortRhythm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbShortRhythm.Location = new System.Drawing.Point(91, 146);
            this.cbShortRhythm.Name = "cbShortRhythm";
            this.cbShortRhythm.Size = new System.Drawing.Size(204, 24);
            this.cbShortRhythm.TabIndex = 5;
            this.cbShortRhythm.Text = "Short rhythm tempogram";
            this.cbShortRhythm.UseVisualStyleBackColor = true;
            // 
            // cbAmpEnvelope
            // 
            this.cbAmpEnvelope.AutoSize = true;
            this.cbAmpEnvelope.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbAmpEnvelope.Location = new System.Drawing.Point(91, 176);
            this.cbAmpEnvelope.Name = "cbAmpEnvelope";
            this.cbAmpEnvelope.Size = new System.Drawing.Size(167, 24);
            this.cbAmpEnvelope.TabIndex = 6;
            this.cbAmpEnvelope.Text = "Amplitude envelope";
            this.cbAmpEnvelope.UseVisualStyleBackColor = true;
            this.cbAmpEnvelope.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(20, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Find similar audio by:";
            // 
            // FindSimilarPropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 300);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbAmpEnvelope);
            this.Controls.Add(this.cbShortRhythm);
            this.Controls.Add(this.cbLongRhythm);
            this.Controls.Add(this.cbIntensity);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbVolumeDistr);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindSimilarPropertiesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FindSimilarPropertiesForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox cbVolumeDistr;
        public System.Windows.Forms.CheckBox cbIntensity;
        public System.Windows.Forms.CheckBox cbLongRhythm;
        public System.Windows.Forms.CheckBox cbShortRhythm;
        public System.Windows.Forms.CheckBox cbAmpEnvelope;
    }
}