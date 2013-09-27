namespace HoloUI
{
    partial class AudioSourcesPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.cmMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmMain
            // 
            this.cmMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miPlay});
            this.cmMain.Name = "cmMain";
            this.cmMain.Size = new System.Drawing.Size(153, 48);
            // 
            // miPlay
            // 
            this.miPlay.Image = global::HoloUI.Resource.play;
            this.miPlay.Name = "miPlay";
            this.miPlay.Size = new System.Drawing.Size(152, 22);
            this.miPlay.Text = "Play";
            this.miPlay.Click += new System.EventHandler(this.miPlay_Click);
            // 
            // AudioSourcesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "AudioSourcesPanel";
            this.Size = new System.Drawing.Size(461, 343);
            this.cmMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cmMain;
        private System.Windows.Forms.ToolStripMenuItem miPlay;
    }
}
