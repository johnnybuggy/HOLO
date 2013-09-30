namespace HoloUI
{
    partial class AudiosPanel
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
            this.findSimilarByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miVolumeDistr = new System.Windows.Forms.ToolStripMenuItem();
            this.tempoDistributionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByTempoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmMain
            // 
            this.cmMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miPlay,
            this.findSimilarByToolStripMenuItem,
            this.sortByTempoToolStripMenuItem});
            this.cmMain.Name = "cmMain";
            this.cmMain.Size = new System.Drawing.Size(153, 92);
            // 
            // miPlay
            // 
            this.miPlay.Image = global::HoloUI.Resource.play;
            this.miPlay.Name = "miPlay";
            this.miPlay.Size = new System.Drawing.Size(152, 22);
            this.miPlay.Text = "Play";
            this.miPlay.Click += new System.EventHandler(this.miPlay_Click);
            // 
            // findSimilarByToolStripMenuItem
            // 
            this.findSimilarByToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miVolumeDistr,
            this.tempoDistributionToolStripMenuItem});
            this.findSimilarByToolStripMenuItem.Name = "findSimilarByToolStripMenuItem";
            this.findSimilarByToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.findSimilarByToolStripMenuItem.Text = "Find similar by";
            // 
            // miVolumeDistr
            // 
            this.miVolumeDistr.Name = "miVolumeDistr";
            this.miVolumeDistr.Size = new System.Drawing.Size(179, 22);
            this.miVolumeDistr.Text = "Volume distribution";
            this.miVolumeDistr.Click += new System.EventHandler(this.miVolumeDistr_Click);
            // 
            // tempoDistributionToolStripMenuItem
            // 
            this.tempoDistributionToolStripMenuItem.Name = "tempoDistributionToolStripMenuItem";
            this.tempoDistributionToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.tempoDistributionToolStripMenuItem.Text = "Tempo distribution";
            this.tempoDistributionToolStripMenuItem.Click += new System.EventHandler(this.tempoDistributionToolStripMenuItem_Click);
            // 
            // sortByTempoToolStripMenuItem
            // 
            this.sortByTempoToolStripMenuItem.Name = "sortByTempoToolStripMenuItem";
            this.sortByTempoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.sortByTempoToolStripMenuItem.Text = "Sort by Tempo";
            this.sortByTempoToolStripMenuItem.Click += new System.EventHandler(this.sortByTempoToolStripMenuItem_Click);
            // 
            // AudiosPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "AudiosPanel";
            this.Size = new System.Drawing.Size(461, 343);
            this.cmMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cmMain;
        private System.Windows.Forms.ToolStripMenuItem miPlay;
        private System.Windows.Forms.ToolStripMenuItem findSimilarByToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miVolumeDistr;
        private System.Windows.Forms.ToolStripMenuItem tempoDistributionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByTempoToolStripMenuItem;
    }
}
