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
            this.findSimilarsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByTempoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByRhythmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.miDebug = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmMain
            // 
            this.cmMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miPlay,
            this.findSimilarsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.sortByTempoToolStripMenuItem,
            this.sortByRhythmToolStripMenuItem,
            this.sortByNameToolStripMenuItem,
            this.toolStripMenuItem1,
            this.miDebug});
            this.cmMain.Name = "cmMain";
            this.cmMain.Size = new System.Drawing.Size(160, 170);
            // 
            // miPlay
            // 
            this.miPlay.Image = global::HoloUI.Resource.play;
            this.miPlay.Name = "miPlay";
            this.miPlay.Size = new System.Drawing.Size(159, 22);
            this.miPlay.Text = "Play";
            this.miPlay.Click += new System.EventHandler(this.miPlay_Click);
            // 
            // findSimilarsToolStripMenuItem
            // 
            this.findSimilarsToolStripMenuItem.Name = "findSimilarsToolStripMenuItem";
            this.findSimilarsToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.findSimilarsToolStripMenuItem.Text = "Find similars";
            this.findSimilarsToolStripMenuItem.Click += new System.EventHandler(this.findSimilarsToolStripMenuItem_Click);
            // 
            // sortByTempoToolStripMenuItem
            // 
            this.sortByTempoToolStripMenuItem.Name = "sortByTempoToolStripMenuItem";
            this.sortByTempoToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.sortByTempoToolStripMenuItem.Text = "Sort by Intensity";
            this.sortByTempoToolStripMenuItem.Click += new System.EventHandler(this.sortByTempoToolStripMenuItem_Click);
            // 
            // sortByRhythmToolStripMenuItem
            // 
            this.sortByRhythmToolStripMenuItem.Name = "sortByRhythmToolStripMenuItem";
            this.sortByRhythmToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.sortByRhythmToolStripMenuItem.Text = "Sort by Rhythm";
            this.sortByRhythmToolStripMenuItem.Click += new System.EventHandler(this.sortByRhythmToolStripMenuItem_Click);
            // 
            // sortByNameToolStripMenuItem
            // 
            this.sortByNameToolStripMenuItem.Name = "sortByNameToolStripMenuItem";
            this.sortByNameToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.sortByNameToolStripMenuItem.Text = "Sort by Name";
            this.sortByNameToolStripMenuItem.Click += new System.EventHandler(this.sortByNameToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(156, 6);
            // 
            // miDebug
            // 
            this.miDebug.Name = "miDebug";
            this.miDebug.Size = new System.Drawing.Size(159, 22);
            this.miDebug.Text = "Debug";
            this.miDebug.Click += new System.EventHandler(this.miDebug_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(156, 6);
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
        private System.Windows.Forms.ToolStripMenuItem sortByTempoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem miDebug;
        private System.Windows.Forms.ToolStripMenuItem sortByRhythmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findSimilarsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    }
}
