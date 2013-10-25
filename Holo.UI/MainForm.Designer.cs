using Holo.UI.Controls;

namespace Holo.UI
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
            this.components = new System.ComponentModel.Container();
            this.ssMain = new System.Windows.Forms.StatusStrip();
            this.lbItemCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbAddedCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.pnTop = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.pbSettings = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btAddFolder = new System.Windows.Forms.PictureBox();
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.cmSettings = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miRemoveShowedItems = new System.Windows.Forms.ToolStripMenuItem();
            this.pnAudios = new AudiosPanel();
            this.ssMain.SuspendLayout();
            this.pnTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSettings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btAddFolder)).BeginInit();
            this.cmSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // ssMain
            // 
            this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbItemCount,
            this.lbAddedCount,
            this.lbProgress,
            this.pbProgress});
            this.ssMain.Location = new System.Drawing.Point(0, 456);
            this.ssMain.Name = "ssMain";
            this.ssMain.Size = new System.Drawing.Size(838, 22);
            this.ssMain.TabIndex = 2;
            this.ssMain.Text = "statusStrip1";
            // 
            // lbItemCount
            // 
            this.lbItemCount.Name = "lbItemCount";
            this.lbItemCount.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.lbItemCount.Size = new System.Drawing.Size(68, 17);
            this.lbItemCount.Text = "Items: 0";
            this.lbItemCount.ToolTipText = "Items count";
            // 
            // lbAddedCount
            // 
            this.lbAddedCount.Name = "lbAddedCount";
            this.lbAddedCount.Size = new System.Drawing.Size(54, 17);
            this.lbAddedCount.Text = "Added: 0";
            this.lbAddedCount.Visible = false;
            // 
            // lbProgress
            // 
            this.lbProgress.Name = "lbProgress";
            this.lbProgress.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.lbProgress.Size = new System.Drawing.Size(87, 17);
            this.lbProgress.Text = "Processing:";
            this.lbProgress.Visible = false;
            // 
            // pbProgress
            // 
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(100, 16);
            this.pbProgress.Visible = false;
            // 
            // pnTop
            // 
            this.pnTop.BackgroundImage = global::Holo.UI.Resource.bgTop;
            this.pnTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnTop.Controls.Add(this.pictureBox2);
            this.pnTop.Controls.Add(this.tbSearch);
            this.pnTop.Controls.Add(this.pbSettings);
            this.pnTop.Controls.Add(this.label1);
            this.pnTop.Controls.Add(this.pictureBox1);
            this.pnTop.Controls.Add(this.btAddFolder);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(838, 67);
            this.pnTop.TabIndex = 3;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = global::Holo.UI.Resource._1375954379_edit_find;
            this.pictureBox2.Location = new System.Drawing.Point(649, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // tbSearch
            // 
            this.tbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearch.BackColor = System.Drawing.Color.CornflowerBlue;
            this.tbSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSearch.ForeColor = System.Drawing.Color.White;
            this.tbSearch.Location = new System.Drawing.Point(671, 3);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(118, 13);
            this.tbSearch.TabIndex = 4;
            this.tbSearch.TextChanged += new System.EventHandler(this.tbSearch_TextChanged);
            // 
            // pbSettings
            // 
            this.pbSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbSettings.BackColor = System.Drawing.Color.Transparent;
            this.pbSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbSettings.Image = global::Holo.UI.Resource.settings;
            this.pbSettings.Location = new System.Drawing.Point(819, 3);
            this.pbSettings.Name = "pbSettings";
            this.pbSettings.Size = new System.Drawing.Size(16, 16);
            this.pbSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbSettings.TabIndex = 3;
            this.pbSettings.TabStop = false;
            this.ttMain.SetToolTip(this.pbSettings, "Settings ...");
            this.pbSettings.Click += new System.EventHandler(this.pbSettings_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Music collection";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::Holo.UI.Resource.HOLO1;
            this.pictureBox1.Location = new System.Drawing.Point(12, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(108, 28);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // btAddFolder
            // 
            this.btAddFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btAddFolder.BackColor = System.Drawing.Color.Transparent;
            this.btAddFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btAddFolder.Image = global::Holo.UI.Resource.folder_open_add;
            this.btAddFolder.Location = new System.Drawing.Point(794, 29);
            this.btAddFolder.Name = "btAddFolder";
            this.btAddFolder.Size = new System.Drawing.Size(32, 32);
            this.btAddFolder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btAddFolder.TabIndex = 1;
            this.btAddFolder.TabStop = false;
            this.ttMain.SetToolTip(this.btAddFolder, "Add folder to collection");
            this.btAddFolder.Click += new System.EventHandler(this.btAddFolder_Click);
            // 
            // cmSettings
            // 
            this.cmSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRemoveShowedItems});
            this.cmSettings.Name = "cmSettings";
            this.cmSettings.Size = new System.Drawing.Size(273, 26);
            // 
            // miRemoveShowedItems
            // 
            this.miRemoveShowedItems.Image = global::Holo.UI.Resource.delete;
            this.miRemoveShowedItems.Name = "miRemoveShowedItems";
            this.miRemoveShowedItems.Size = new System.Drawing.Size(272, 22);
            this.miRemoveShowedItems.Text = "Remove showed items from database";
            this.miRemoveShowedItems.Click += new System.EventHandler(this.miRemoveShowedItems_Click);
            // 
            // pnAudios
            // 
            this.pnAudios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnAudios.Location = new System.Drawing.Point(0, 67);
            this.pnAudios.Name = "pnAudios";
            this.pnAudios.Size = new System.Drawing.Size(838, 389);
            this.pnAudios.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 478);
            this.Controls.Add(this.pnAudios);
            this.Controls.Add(this.pnTop);
            this.Controls.Add(this.ssMain);
            this.Name = "MainForm";
            this.Text = "Holo";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.ssMain.ResumeLayout(false);
            this.ssMain.PerformLayout();
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btAddFolder)).EndInit();
            this.cmSettings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip ssMain;
        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox btAddFolder;
        private System.Windows.Forms.ToolTip ttMain;
        private System.Windows.Forms.PictureBox pictureBox1;
        private AudiosPanel pnAudios;
        private System.Windows.Forms.ToolStripStatusLabel lbItemCount;
        private System.Windows.Forms.ToolStripProgressBar pbProgress;
        private System.Windows.Forms.PictureBox pbSettings;
        private System.Windows.Forms.ContextMenuStrip cmSettings;
        private System.Windows.Forms.ToolStripMenuItem miRemoveShowedItems;
        private System.Windows.Forms.ToolStripStatusLabel lbProgress;
        private System.Windows.Forms.ToolStripStatusLabel lbAddedCount;
        private System.Windows.Forms.TextBox tbSearch;
        private System.Windows.Forms.PictureBox pictureBox2;

    }
}