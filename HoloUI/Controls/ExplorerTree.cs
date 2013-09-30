/* *************************************************
 * Programmer: Rajesh Lal(connectrajesh@hotmail.com)
 * Date: 06/25/06
 * Company Info: www.irajesh.com
 * See EULA.txt and Copyright.txt for additional information
 * **************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
//For registry access
    //Used for .dll import

namespace HoloUI
{
	/// <summary>
	/// Summary description for ExplorerTree.
	/// </summary>
	/// 
	[ToolboxBitmap(typeof(ExplorerTree), "tree.gif"),DefaultEvent("PathChanged")	]
	public class ExplorerTree : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button btnRefresh;
		private System.Windows.Forms.TreeView tvwMain;
		private System.ComponentModel.IContainer components;

		private bool goflag = false ;
		private bool showMyDocuments = true;
		private bool showMyFavorites = true;
		private bool showMyNetwork = true;

		private bool showAddressbar = true;
		private bool showToolbar = true;



		private bool GoFlag
		{
			get
			{
				return goflag;
			}
			set
			{
				goflag=value;
			}
		}
		public bool ShowAddressbar
		{
			get
			{
				return showAddressbar;
			}
			set
			{
				showAddressbar=value;
			}
		}
		public bool ShowToolbar
		{
			get
			{
				return showToolbar;
			}
			set
			{
				showToolbar=value;
			}
		}
		public bool ShowMyDocuments
		{
			get
			{
				return showMyDocuments;
			}
			set
			{
				showMyDocuments=value;
				this.Refresh(); 
			}
		}

		public bool ShowMyFavorites
		{
			get
			{
				return showMyFavorites;
			}
			set
			{
				showMyFavorites=value;
				this.Refresh();
			}
		}

		public bool ShowMyNetwork
		{
			get
			{
				return showMyNetwork;
			}
			set
			{
				showMyNetwork=value;
				this.Refresh();
			}
		}

		
		TreeNode node;
		TreeNode TreeNodeMyComputer ;
		TreeNode TreeNodeRootNode ;


		//ListViewItem comunalItem;
		private System.Windows.Forms.Button btnGo;
		
		//SHFILEINFO [] iconList = new SHFILEINFO[1];	//used icons
		public delegate void PathChangedEventHandler(object sender, EventArgs e);
		private PathChangedEventHandler PathChangedEvent;
		public event PathChangedEventHandler PathChanged
		{
			add
			{
				PathChangedEvent = (PathChangedEventHandler) System.Delegate.Combine(PathChangedEvent, value);
			}
			remove
			{
				PathChangedEvent = (PathChangedEventHandler) System.Delegate.Remove(PathChangedEvent, value);
			}
		}
		private System.Windows.Forms.Button btnHome;
		private System.Windows.Forms.Button btnBack;
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader Path;
		private System.Windows.Forms.ColumnHeader Status;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ContextMenu cMShortcut;
		private System.Windows.Forms.MenuItem mnuShortcut;
		private System.Windows.Forms.TextBox txtPath;
		private System.Windows.Forms.GroupBox grptoolbar;
		private System.Windows.Forms.Button btnInfo;
		private string selectedPath ="home";
		

		
		[
		Category("Appearance"),
		Description("Selected Path of Image")
		]
		public string SelectedPath
		{
			get
			{
				return this.selectedPath;
			}
			set
			{
				this.selectedPath = value;
				this.Invalidate();
			}
		}

		
		public ExplorerTree()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExplorerTree));
            this.btnRefresh = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.tvwMain = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnHome = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cMShortcut = new System.Windows.Forms.ContextMenu();
            this.mnuShortcut = new System.Windows.Forms.MenuItem();
            this.grptoolbar = new System.Windows.Forms.GroupBox();
            this.grptoolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.White;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.ForeColor = System.Drawing.Color.Transparent;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(96, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(17, 17);
            this.btnRefresh.TabIndex = 62;
            this.toolTip1.SetToolTip(this.btnRefresh, "Refresh Explorer Tree");
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(0, 19);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(220, 20);
            this.txtPath.TabIndex = 61;
            this.toolTip1.SetToolTip(this.txtPath, "Current directory");
            this.txtPath.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
            this.txtPath.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPath_KeyPress);
            this.txtPath.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPath_KeyUp);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Image = ((System.Drawing.Image)(resources.GetObject("btnGo.Image")));
            this.btnGo.Location = new System.Drawing.Point(216, 16);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(24, 22);
            this.btnGo.TabIndex = 60;
            this.toolTip1.SetToolTip(this.btnGo, "Go to the directory");
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // tvwMain
            // 
            this.tvwMain.AllowDrop = true;
            this.tvwMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwMain.BackColor = System.Drawing.Color.White;
            this.tvwMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvwMain.ImageIndex = 0;
            this.tvwMain.ImageList = this.imageList1;
            this.tvwMain.Location = new System.Drawing.Point(0, 44);
            this.tvwMain.Name = "tvwMain";
            this.tvwMain.SelectedImageIndex = 2;
            this.tvwMain.ShowLines = false;
            this.tvwMain.ShowRootLines = false;
            this.tvwMain.Size = new System.Drawing.Size(240, 290);
            this.tvwMain.TabIndex = 59;
            this.tvwMain.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvwMain_AfterExpand);
            this.tvwMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwMain_AfterSelect);
            this.tvwMain.DoubleClick += new System.EventHandler(this.tvwMain_DoubleClick);
            this.tvwMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvwMain_MouseUp);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            this.imageList1.Images.SetKeyName(11, "");
            this.imageList1.Images.SetKeyName(12, "");
            this.imageList1.Images.SetKeyName(13, "");
            this.imageList1.Images.SetKeyName(14, "");
            this.imageList1.Images.SetKeyName(15, "");
            this.imageList1.Images.SetKeyName(16, "");
            this.imageList1.Images.SetKeyName(17, "");
            this.imageList1.Images.SetKeyName(18, "");
            this.imageList1.Images.SetKeyName(19, "");
            this.imageList1.Images.SetKeyName(20, "");
            this.imageList1.Images.SetKeyName(21, "");
            this.imageList1.Images.SetKeyName(22, "");
            this.imageList1.Images.SetKeyName(23, "");
            this.imageList1.Images.SetKeyName(24, "");
            this.imageList1.Images.SetKeyName(25, "");
            this.imageList1.Images.SetKeyName(26, "");
            this.imageList1.Images.SetKeyName(27, "");
            this.imageList1.Images.SetKeyName(28, "");
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.Color.White;
            this.btnHome.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.ForeColor = System.Drawing.Color.Transparent;
            this.btnHome.Image = ((System.Drawing.Image)(resources.GetObject("btnHome.Image")));
            this.btnHome.Location = new System.Drawing.Point(118, 8);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(17, 17);
            this.btnHome.TabIndex = 63;
            this.toolTip1.SetToolTip(this.btnHome, "Application Directory");
            this.btnHome.UseVisualStyleBackColor = false;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.White;
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.ForeColor = System.Drawing.Color.Transparent;
            this.btnBack.Image = ((System.Drawing.Image)(resources.GetObject("btnBack.Image")));
            this.btnBack.Location = new System.Drawing.Point(30, 8);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(17, 17);
            this.btnBack.TabIndex = 64;
            this.toolTip1.SetToolTip(this.btnBack, "Backward");
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.White;
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.ForeColor = System.Drawing.Color.Transparent;
            this.btnNext.Image = ((System.Drawing.Image)(resources.GetObject("btnNext.Image")));
            this.btnNext.Location = new System.Drawing.Point(52, 8);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(17, 17);
            this.btnNext.TabIndex = 65;
            this.toolTip1.SetToolTip(this.btnNext, "Forward");
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.Color.White;
            this.btnUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.ForeColor = System.Drawing.Color.Transparent;
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.Location = new System.Drawing.Point(74, 8);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(17, 17);
            this.btnUp.TabIndex = 67;
            this.toolTip1.SetToolTip(this.btnUp, "Parent Directory");
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.White;
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.Transparent;
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.Location = new System.Drawing.Point(8, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(17, 17);
            this.btnAdd.TabIndex = 70;
            this.toolTip1.SetToolTip(this.btnAdd, "Add shortcut to frequently used folders");
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.BackColor = System.Drawing.Color.White;
            this.btnInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInfo.ForeColor = System.Drawing.Color.Transparent;
            this.btnInfo.Image = ((System.Drawing.Image)(resources.GetObject("btnInfo.Image")));
            this.btnInfo.Location = new System.Drawing.Point(139, 8);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(17, 17);
            this.btnInfo.TabIndex = 71;
            this.toolTip1.SetToolTip(this.btnInfo, "Application Directory");
            this.btnInfo.UseVisualStyleBackColor = false;
            this.btnInfo.Visible = false;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Path,
            this.Status});
            this.listView1.Location = new System.Drawing.Point(8, 208);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(224, 48);
            this.listView1.TabIndex = 68;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Visible = false;
            // 
            // cMShortcut
            // 
            this.cMShortcut.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuShortcut});
            // 
            // mnuShortcut
            // 
            this.mnuShortcut.Index = 0;
            this.mnuShortcut.Text = "Remove Shortcut";
            this.mnuShortcut.Click += new System.EventHandler(this.mnuShortcut_Click);
            // 
            // grptoolbar
            // 
            this.grptoolbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grptoolbar.Controls.Add(this.btnInfo);
            this.grptoolbar.Controls.Add(this.btnRefresh);
            this.grptoolbar.Controls.Add(this.btnHome);
            this.grptoolbar.Controls.Add(this.btnBack);
            this.grptoolbar.Controls.Add(this.btnNext);
            this.grptoolbar.Controls.Add(this.btnUp);
            this.grptoolbar.Controls.Add(this.btnAdd);
            this.grptoolbar.Location = new System.Drawing.Point(-8, -8);
            this.grptoolbar.Name = "grptoolbar";
            this.grptoolbar.Size = new System.Drawing.Size(256, 32);
            this.grptoolbar.TabIndex = 71;
            this.grptoolbar.TabStop = false;
            this.grptoolbar.Enter += new System.EventHandler(this.grptoolbar_Enter);
            // 
            // ExplorerTree
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.tvwMain);
            this.Controls.Add(this.grptoolbar);
            this.Name = "ExplorerTree";
            this.Size = new System.Drawing.Size(240, 336);
            this.Load += new System.EventHandler(this.ExplorerTree_Load);
            this.grptoolbar.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void ExplorerTree_Load(object sender, System.EventArgs e)
		{
			GetDirectory();
			
			if (Directory.Exists(selectedPath))
			{
				setCurrentPath(selectedPath);
				
			}
			else
			{
				setCurrentPath("home");
			}
			btnGo_Click(this,e); 

			refreshView();

		}
		public void refreshView()
		{
			if ((!showAddressbar )&& (!showToolbar ))
			{
				tvwMain.Top = 0;
				txtPath.Visible = false;
				btnGo.Visible = false ; 
				grptoolbar.Visible = false; 
				tvwMain.Height = this.Height;
			}
			else
			{
				if (showToolbar&&(!showAddressbar))
				{
					tvwMain.Top = 20;
					txtPath.Visible = false;
					btnGo.Visible = false; 
					tvwMain.Height = this.Height- 20;
					grptoolbar.Visible = true; 
				}
				else if (showAddressbar&&(!showToolbar))
				{
					tvwMain.Top = 20;
					txtPath.Top = 1;
					btnGo.Top = -2;
					txtPath.Visible = true;
					btnGo.Visible = true ; 
					tvwMain.Height = this.Height - 20;
					grptoolbar.Visible = false; 
				}
				else 
				{
					tvwMain.Top = 40;
					txtPath.Visible = true;
					btnGo.Visible = true ; 
					txtPath.Top = 19;
					btnGo.Top = 16;
					grptoolbar.Visible = true; 
					tvwMain.Height = this.Height- 40;
				}
			}
		}

		public void GetDirectory()
		{
			tvwMain.Nodes.Clear();  
			
		
			string [] drives = Environment.GetLogicalDrives();
			TreeNode nodeD;
			//Environment.UserDomainName .GetFolderPath( 
			//Environment.GetFolderPath (Environment.SystemDirectory);

			TreeNode nodemd;
			TreeNode nodemf;
			TreeNode nodemyC;
			TreeNode nodemNc;

			TreeNode nodemyN;

			TreeNode nodeEN;
			TreeNode nodeNN;
			
			nodeD = new TreeNode();
			nodeD.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			nodeD.Text = "Desktop";
			nodeD.ImageIndex = 10;
			nodeD.SelectedImageIndex = 10;

			tvwMain.Nodes.Add(nodeD);
			TreeNodeRootNode = nodeD ;
			
			
			if (ShowMyDocuments) 
			{
				//Add My Documents and Desktop folder outside
				nodemd = new TreeNode();
				nodemd.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				nodemd.Text = "My Documents";
				nodemd.ImageIndex = 9;
				nodemd.SelectedImageIndex = 9;
				nodeD.Nodes.Add(nodemd);
				FillFilesandDirs(nodemd);
			}

			nodemyC = new TreeNode();
			nodemyC.Tag = "My Computer";
			nodemyC.Text = "My Computer";
			nodemyC.ImageIndex = 12;
			nodemyC.SelectedImageIndex = 12;
			nodeD.Nodes.Add(nodemyC);
			nodemyC.EnsureVisible(); 

			TreeNodeMyComputer = nodemyC ;

			nodemNc = new TreeNode();
							nodemNc.Tag = "my Node";
							nodemNc.Text = "my Node";//dir.Substring(dir.LastIndexOf(@"\") + 1);
							nodemNc.ImageIndex = 12;
							nodemNc.SelectedImageIndex = 12;
							nodemyC.Nodes.Add(nodemNc);
						

			
			if (ShowMyNetwork) 
			{
				
				nodemyN = new TreeNode();
				nodemyN.Tag = "My Network Places";
				nodemyN.Text = "My Network Places";
				nodemyN.ImageIndex = 13;
				nodemyN.SelectedImageIndex = 13;
				nodeD.Nodes.Add(nodemyN);
				nodemyN.EnsureVisible();

				nodeEN = new TreeNode();
				nodeEN.Tag = "Entire Network";
				nodeEN.Text = "Entire Network";
				nodeEN.ImageIndex = 14;
				nodeEN.SelectedImageIndex = 14;
				nodemyN.Nodes.Add(nodeEN);

				nodeNN = new TreeNode();
				nodeNN.Tag = "Network Node";
				nodeNN.Text = "Network Node";
				nodeNN.ImageIndex = 15;
				nodeNN.SelectedImageIndex = 15;
				nodeEN.Nodes.Add(nodeNN);
				
				nodeEN.EnsureVisible();
			}
			
			if (ShowMyFavorites) 
			{
				nodemf = new TreeNode();
				nodemf.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
				nodemf.Text = "My Favorites";
				nodemf.ImageIndex = 26;
				nodemf.SelectedImageIndex = 26;
				nodeD.Nodes.Add(nodemf);
				FillFilesandDirs(nodemf);
			}
			ExploreTreeNode(nodeD);
			
		}
		private void ExploreTreeNode(TreeNode n)
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				//get dirs
				FillFilesandDirs(n);
				
				//get dirs one more level deep in current dir so user can see there is
				//more dirs underneath current dir
				foreach(TreeNode node in n.Nodes)
				{
					if (String.Compare(n.Text,"Desktop")==0) 
					{
						if ((String.Compare(node.Text ,"My Documents")==0) ||(String.Compare(node.Text ,"My Computer")==0) ||(String.Compare(node.Text ,"Microsoft Windows Network")==0)|| (String.Compare(node.Text ,"My Network Places")==0))
						{}
						else
						{
							FillFilesandDirs(node);
						}
					}
					else
					{
						FillFilesandDirs(node);
					}
				}
			}
			
			catch
			{}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void GetDirectories(TreeNode parentNode)
		{
// added after suggestion
			string[] dirList;

			dirList = Directory.GetDirectories(parentNode.Tag.ToString());
			Array.Sort(dirList);

			//check if dir already exists in case click same dir twice
			if (dirList.Length == parentNode.Nodes.Count)
				return;
			//add each dir in selected dir
			for (int i = 0; i < dirList.Length; i++)
			{
				node = new TreeNode();
				node.Tag = dirList[i]; //store path in tag
				node.Text = dirList[i].Substring(dirList[i].LastIndexOf(@"\") + 1);
				node.ImageIndex = 1;
				parentNode.Nodes.Add(node);
			}

// old code
//			bool check = false;
//
//			//add each dir in selected dir
//			foreach(string dir in Directory.GetDirectories(parentNode.Tag.ToString()))
//			{
//				check = false;
//
//				//check if dir already exists in case click same dir twice
//				if(Directory.GetDirectories(parentNode.Tag.ToString()).Length == parentNode.Nodes.Count)
//					check = true;
//
//				if(!check)	//if not there add
//				{
//					node = new TreeNode();
//					node.Tag = dir;	//store path in tag
//					node.Text = dir.Substring(dir.LastIndexOf(@"\") + 1);
//					node.ImageIndex = 1;
//					parentNode.Nodes.Add(node);
//				}
//			}
			
		}

		
		private void FillFilesandDirs(TreeNode comunalNode)
		{
			try 
			{
				GetDirectories(comunalNode);
			}
			catch(Exception)
			{
				return;
			}
		}

		public void setCurrentPath(string strPath)
		{
			SelectedPath = strPath;
			
			if (String.Compare(strPath,"home")==0)
			{
				txtPath.Text = Application.StartupPath;
			}
			else
			{
				DirectoryInfo inf = new DirectoryInfo(strPath);
				if(inf.Exists)
				{
					txtPath.Text =  strPath;
				
				}
				else
					txtPath.Text = Application.StartupPath;
			}
			

		}

		private void tvwMain_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			string [] drives = Environment.GetLogicalDrives();
			string dir2 ="";

			Cursor.Current = Cursors.WaitCursor;   
			TreeNode n;
			TreeNode nodeNN;
			TreeNode nodemN;
			TreeNode nodemyC;
			TreeNode nodeNNode;
			TreeNode nodeDrive;
			nodemyC = e.Node;   

			n = e.Node;
			
			if (n.Text.IndexOf(":",1)>0)   
			{
				ExploreTreeNode(n);
			}
			else
			{//(String.Compare(n.Text ,"My Documents")==0) || (String.Compare(n.Text,"Desktop")==0) || 

				if ((String.Compare(n.Text,"Desktop" )==0)||(String.Compare(n.Text,"Microsoft Windows Network" )==0)||(String.Compare(n.Text ,"My Computer")==0) || (String.Compare(n.Text ,"My Network Places")==0)|| (String.Compare(n.Text ,"Entire Network")==0)||((n.Parent!=null)&&(String.Compare(n.Parent.Text,"Microsoft Windows Network")==0)) )
				{
					if((String.Compare(n.Text ,"My Computer")==0)&&(nodemyC.GetNodeCount(true) <2))
						///////////
						//add each drive and files and dirs
					{
						nodemyC.FirstNode.Remove();
 
					foreach(string drive in drives)
					{
				
						nodeDrive = new TreeNode();
						nodeDrive.Tag = drive;
					
						nodeDrive.Text = drive ;
					
						//Determine icon to display by drive
						switch(Win32.GetDriveType(drive))
						{
							case 2:
								nodeDrive.ImageIndex = 17;
								nodeDrive.SelectedImageIndex  = 17;
								break;
							case 3:
								nodeDrive.ImageIndex = 0;
								nodeDrive.SelectedImageIndex  = 0;
								break;
							case 4:
								nodeDrive.ImageIndex = 8;
								nodeDrive.SelectedImageIndex = 8;
								break;
							case 5:
								nodeDrive.ImageIndex = 7;
								nodeDrive.SelectedImageIndex = 7;
								break;
							default:
								nodeDrive.ImageIndex = 0;
								nodeDrive.SelectedImageIndex = 0;
								break;
						}
					
						nodemyC.Nodes.Add(nodeDrive);
						nodeDrive.EnsureVisible();
						tvwMain.Refresh(); 
						try
						{
							//add dirs under drive
							if (Directory.Exists (drive))
							{
								foreach(string dir in Directory.GetDirectories(drive))
								{
									dir2 = dir;
									node = new TreeNode();
									node.Tag = dir;
									node.Text = dir.Substring(dir.LastIndexOf(@"\") + 1);
									node.ImageIndex = 1;
									nodeDrive.Nodes.Add(node);
								}
							}
				
							//fill those dirs
							//					foreach(TreeNode curNode in 
							//						tvwMain.Nodes[tvwMain.Nodes.Count - 1].Nodes)
							//					{
							//						FillFilesandDirs(curNode);
							//					}
						}
						catch(Exception)	//error just add blank dir
						{
							// MessageBox.Show ("Error while Filling the Explorer:" + ex.Message );
							//					node = new TreeNode();
							//					node.Tag = dir2;
							//					node.Text = dir2.Substring(dir2.LastIndexOf(@"\") + 1);
							//					node.ImageIndex = 1;
							//					tvwMain.Nodes.Add(node);
						}
						nodemyC.Expand(); 
						}
					
					}				
					if((String.Compare(n.Text ,"Entire Network")==0))
					{
						if (n.FirstNode.Text == "Network Node")
						{
							n.FirstNode.Remove();
							//NETRESOURCE netRoot = new NETRESOURCE();
			
							ServerEnum servers = new ServerEnum(ResourceScope.RESOURCE_GLOBALNET, ResourceType.RESOURCETYPE_DISK, ResourceUsage.RESOURCEUSAGE_ALL, ResourceDisplayType.RESOURCEDISPLAYTYPE_NETWORK,"" );
							
							foreach	(string	s1 in servers)
							{
								string s2="";
								s2 = s1.Substring(0,s1.IndexOf("|",1));
									
								if(s1.IndexOf("NETWORK",1) > 0 ) 
								{
									nodeNN = new TreeNode();
									nodeNN.Tag =  s2;
									nodeNN.Text = s2;//dir.Substring(dir.LastIndexOf(@"\") + 1);
									nodeNN.ImageIndex = 15;
									nodeNN.SelectedImageIndex = 15;
									n.Nodes.Add(nodeNN);
								}
								else
								{
									TreeNode nodemNc;
									nodemN = new TreeNode();
									nodemN.Tag = s2;//"my Node";
									nodemN.Text = s2;//"my Node";//dir.Substring(dir.LastIndexOf(@"\") + 1);
									nodemN.ImageIndex = 16;
									nodemN.SelectedImageIndex = 16;
									n.LastNode.Nodes.Add(nodemN);

									nodemNc = new TreeNode();
									nodemNc.Tag = "my netNode";
									nodemNc.Text = "my netNode";//dir.Substring(dir.LastIndexOf(@"\") + 1);
									nodemNc.ImageIndex = 12;
									nodemNc.SelectedImageIndex = 12;
									nodemN.Nodes.Add(nodemNc);
								}
							}
						}
					}
					if ((n.Parent!=null)&&(String.Compare(n.Parent.Text,"Microsoft Windows Network")==0))

					{
						if (n.FirstNode.Text == "my netNode")
						{
							n.FirstNode.Remove();
							
							string pS=n.Text ;
							
							//NETRESOURCE netRoot = new NETRESOURCE();
			
							ServerEnum servers = new ServerEnum(ResourceScope.RESOURCE_GLOBALNET,
								ResourceType.RESOURCETYPE_DISK, 
								ResourceUsage.RESOURCEUSAGE_ALL, 
								ResourceDisplayType.RESOURCEDISPLAYTYPE_SERVER,pS);


							foreach	(string	s1 in servers)
							{
								string s2="";


								if((s1.Length <6)||(String.Compare(s1.Substring(s1.Length-6,6),"-share")!=0))
								{
									s2 = s1;//.Substring(s1.IndexOf("\\",2));
									nodeNN = new TreeNode();
									nodeNN.Tag =  s2;
									nodeNN.Text = s2.Substring(2) ;
									nodeNN.ImageIndex = 12;
									nodeNN.SelectedImageIndex = 12;
									n.Nodes.Add(nodeNN);
									foreach	(string	s1node in servers)
									{
										if (s1node.Length >6)
										{
											if(String.Compare(s1node.Substring(s1node.Length-6,6),"-share")==0)
											{
												if (s2.Length <=s1node.Length )
												{
													try
													{
														if (String.Compare(s1node.Substring(0,s2.Length+1),s2 + @"\")==0)  
														{
															nodeNNode = new TreeNode();
															nodeNNode.Tag =  s1node.Substring(0,s1node.Length -6);
															nodeNNode.Text = s1node.Substring(s2.Length+1,s1node.Length -s2.Length-7) ;
															nodeNNode.ImageIndex = 28;
															nodeNNode.SelectedImageIndex = 28;
															nodeNN.Nodes.Add(nodeNNode);
														}
													}
													catch(Exception)
													{}
												}
											}
										}

									}
								}

							}
						}
					}
				}
				else
				{	
					ExploreTreeNode(n); 
				}
			}
			Cursor.Current = Cursors.Default;
		}

		private void tvwMain_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			TreeNode n;
			n =  e.Node ;
				
			try
			{
				if ((String.Compare(n.Text ,"My Computer")==0) ||(String.Compare(n.Text ,"My Network Places")==0)||(String.Compare(n.Text ,"Entire Network")==0) )
				{
				}
				else
				{
					txtPath.Text = n.Tag.ToString() ; 
					
			
				}
			}
			catch{}
		}

		private void tvwMain_DoubleClick(object sender, System.EventArgs e)
		{
			
			TreeNode n;
			n = tvwMain.SelectedNode ;
			
			if (!tvwMain.SelectedNode.IsExpanded) 
				tvwMain.SelectedNode.Collapse();
			else
			{
					ExploreTreeNode(n);
			}
		}
		public void refreshFolders()
		{
			listView1.Items.Clear();   
			tvwMain.Nodes.Clear();
			setCurrentPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
			GetDirectory();
		}

		private void btnRefresh_Click(object sender, System.EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;   
			refreshView();
			
			try
			{
				refreshFolders();
			}
			catch(Exception e1)
			{
				MessageBox.Show ("Error: " + e1.Message); 
			}
			finally
			{
				setCurrentPath("home");
				Cursor.Current = Cursors.Default;
				ExploreMyComputer();
			}

		}
		private void ExploreMyComputer()
		{
			
			string [] drives = Environment.GetLogicalDrives();
			string dir2 ="";

			Cursor.Current = Cursors.WaitCursor;   
			TreeNode nodeDrive;

			if(TreeNodeMyComputer.GetNodeCount(true) <2)
			{
				TreeNodeMyComputer.FirstNode.Remove();
 
				foreach(string drive in drives)
				{
					nodeDrive = new TreeNode();
					nodeDrive.Tag = drive;
				
					nodeDrive.Text = drive ;
					
					switch(Win32.GetDriveType(drive))
					{
						case 2:
							nodeDrive.ImageIndex = 17;
							nodeDrive.SelectedImageIndex  = 17;
							break;
						case 3:
							nodeDrive.ImageIndex = 0;
							nodeDrive.SelectedImageIndex  = 0;
							break;
						case 4:
							nodeDrive.ImageIndex = 8;
							nodeDrive.SelectedImageIndex = 8;
							break;
						case 5:
							nodeDrive.ImageIndex = 7;
							nodeDrive.SelectedImageIndex = 7;
							break;
						default:
							nodeDrive.ImageIndex = 0;
							nodeDrive.SelectedImageIndex = 0;
							break;
					}
						
					TreeNodeMyComputer.Nodes.Add(nodeDrive);
					try
					{
						//add dirs under drive
						if (Directory.Exists (drive))
						{
							foreach(string dir in Directory.GetDirectories(drive))
							{
								dir2 = dir;
								node = new TreeNode();
								node.Tag = dir;
								node.Text = dir.Substring(dir.LastIndexOf(@"\") + 1);
								node.ImageIndex = 1;
								nodeDrive.Nodes.Add(node);
							}
						}
					
					
					}
					catch(Exception ex)	//error just add blank dir
					{
						 MessageBox.Show ("Error while Filling the Explorer:" + ex.Message );
					}
				}
			}
			
			TreeNodeMyComputer.Expand();
		}

		private void UpdateListAddCurrent()
		{
			int i =0;
			int j =0;
			
			int icount =0;
            icount = listView1.Items.Count + 1;

				for (i = 0;i< listView1.Items.Count-1;i++)
				{
					if (String.Compare(listView1.Items[i].SubItems[1].Text,"Selected")==0)
					{
						for (j = listView1.Items.Count-1;j> i + 1;j--)
     						listView1.Items[j].Remove();
						break;	
					}				  
					
				}		
		}
		private void UpdateListGoBack() 
		{	
			if ((listView1.Items.Count >0)&&(String.Compare(listView1.Items[0].SubItems[1].Text,"Selected")==0))
				return;
 			int i=0;
			for (i = 0;i< listView1.Items.Count;i++)
			{
				if (String.Compare(listView1.Items[i].SubItems[1].Text,"Selected")==0)
				{
					if (i != 0)
					{
						listView1.Items[i - 1].SubItems[1].Text = "Selected";
						txtPath.Text =listView1.Items[i - 1].Text;
					}
				}
				if (i != 0)
				{
					listView1.Items[i].SubItems[1].Text = " -/- ";
				}
			}
			}
		private void UpdateListGoFwd()
		{
			if ((listView1.Items.Count >0)&&(String.Compare(listView1.Items[listView1.Items.Count -1 ].SubItems[1].Text,"Selected")==0))
				return;
			int i=0;
			for (i = listView1.Items.Count-1;i >= 0;i--)
			{
				if (String.Compare(listView1.Items[i].SubItems[1].Text,"Selected")==0)
				{
					if (i != listView1.Items.Count) 
					{
						listView1.Items[i + 1].SubItems[1].Text = "Selected";
						txtPath.Text =listView1.Items[i + 1].Text;   
					}
				}

				if (i != listView1.Items.Count-1) listView1.Items[i].SubItems[1].Text = " -/- ";
			}
		}
		private void updateList(string f)
{
	int i=0;
	ListViewItem listviewitem;		// Used for creating listview items.

	int icount =0;
	UpdateListAddCurrent();
	icount = listView1.Items.Count + 1;
	try
	{
		if (listView1.Items.Count> 0)
		{    
			if (String.Compare(listView1.Items[listView1.Items.Count-1].Text, f)==0)
			{
				return;
			}
		}
	
		for (i = 0;i<listView1.Items.Count;i++)
		{
			listView1.Items[i].SubItems[1].Text = " -/- ";
		}
		listviewitem = new ListViewItem(f);
		listviewitem.SubItems.Add("Selected");
		listviewitem.Tag = f;
		this.listView1.Items.Add(listviewitem);
	}
	catch(Exception e)
	{
	MessageBox.Show(e.Message);   
	}
}
		public void btnGo_Click(object sender, System.EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;   
			try
			{
				ExploreMyComputer(); 
				string myString ="";
				int h=1;
				myString = txtPath.Text.ToLower()  ;
				//if (String.Compare(myString.Substring(myString.Length-1,1),@"\")==0)
				//{
				//	myString = myString.Substring(0,myString.Length-1);
				//	txtPath.Text = myString	;

				//}
				TreeNode tn = TreeNodeMyComputer  ;

			StartAgain:
			
				do
				{
					//Strom = (tvwMain.GetNodeCount(true)).ToString() ;	
					
					foreach(TreeNode t in tn.Nodes) 
					{
						string mypath =  t.Tag.ToString()  ;
						//mypath =  mypath.Replace("Desktop\\","") ;
						//mypath =  mypath.Replace("My Computer\\","") ;
						//mypath =  mypath.Replace(@"\\",@"\") ;

						//mypath =  mypath.Replace("My Documents\\",Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\") ;
						mypath=mypath.ToLower();
						string mypathf =mypath;
						if (!mypath.EndsWith(@"\"))  
						{
						if (myString.Length > mypathf.Length )	mypathf  =mypath + @"\";
						}

						if (myString.StartsWith(mypathf))
						{
							t.TreeView.Focus(); 
							t.TreeView.SelectedNode =  t; 
							t.EnsureVisible(); 
							t.Expand();
							if (t.Nodes.Count>=1)
							{
								t.Expand();
								tn = t;
							}
							else
							{
								if (String.Compare (myString,mypath)==0)
								{
									h = -1;
									break;
								}
								else
								{
									continue;  
								}
							}

							if (mypathf.StartsWith(myString))
							{
								h = -1;
								break;
							}
							else
							{
								goto  StartAgain;
								//return;
							}
						}
					}
				
					try
					{
						tn = tn.NextNode;
					}
					catch(Exception)
					{}
 
				}while(h>=0);

			}
			catch(Exception e1)
			{
				MessageBox.Show ("Error: " + e1.Message); 
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void btnHome_Click(object sender, System.EventArgs e)
		{
			setCurrentPath("home"); 
			ExploreMyComputer(); 
			btnGo_Click(sender,e); 
			
		}


		private void btnNext_Click(object sender, System.EventArgs e)
		{
			GoFlag = true;
			string cpath = txtPath.Text ; 
			UpdateListGoFwd();  
			
			if (String.Compare( cpath,txtPath.Text)==0)
			{}
			else
			{
				btnGo_Click(sender,e); 
			}
			GoFlag = false;
		}

		private void btnBack_Click(object sender, System.EventArgs e)
		{
			GoFlag = true;
			string cpath = txtPath.Text ; 
			UpdateListGoBack();  
			
			if (String.Compare( cpath,txtPath.Text)==0)
			{}
			else
			{
				btnGo_Click(sender,e); 
			}
			GoFlag = false;
		}

		private void tvwMain_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			updateList(txtPath.Text);   
			if (tvwMain.SelectedNode != null)
			{

				if ((tvwMain.SelectedNode.ImageIndex == 18)&&(e.Button==MouseButtons.Right))
					cMShortcut.Show(tvwMain ,new Point(e.X,e.Y)); 
			}
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			try
			{
				DirectoryInfo MYINFO = new DirectoryInfo(txtPath.Text);
				
				if (MYINFO.Parent.Exists)
					txtPath.Text = MYINFO.Parent.FullName;
				updateList( txtPath.Text);
				btnGo_Click(sender,e); 
			}
			catch(Exception)
			{
				//MessageBox.Show ("Parent directory does not exists","Directory Not Found",MessageBoxButtons.OK,MessageBoxIcon.Information ); 
			}
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			string myname="";
			string mypath="";


			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = "Add Folder in Explorer Tree";
			dialog.ShowNewFolderButton = true;
			dialog.SelectedPath = txtPath.Text  ;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				mypath = dialog.SelectedPath;
				myname = mypath.Substring(mypath.LastIndexOf("\\")+1);
				
				AddFolderNode(myname,mypath);
				
			}
		}
		private void AddFolderNode(string name, string path)
		{

			try
			{
				TreeNode nodemyC = new TreeNode();
			
				nodemyC.Tag = path;
				nodemyC.Text = name;

				nodemyC.ImageIndex = 18;
				nodemyC.SelectedImageIndex = 18;

				TreeNodeRootNode.Nodes.Add(nodemyC); 

				try
				{
					//add dirs under drive
					if (Directory.Exists (path))
					{
						foreach(string dir in Directory.GetDirectories(path))
						{
							TreeNode node = new TreeNode();
							node.Tag = dir;
							node.Text = dir.Substring(dir.LastIndexOf(@"\") + 1);
							node.ImageIndex = 1;
							nodemyC.Nodes.Add(node);
						}
					}
				}
				catch(Exception ex)	//error just add blank dir
				{
					MessageBox.Show ("Error while Filling the Explorer:" + ex.Message );
				}
			}
			catch(Exception e)
			{
				MessageBox.Show (e.Message);  
			}
		}

		private void mnuShortcut_Click(object sender, System.EventArgs e)
		{
			if (tvwMain.SelectedNode.ImageIndex ==18)  
				tvwMain.SelectedNode.Remove();
		}

		private void txtPath_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				if( Directory.Exists(txtPath.Text))
				{
					SelectedPath = txtPath.Text;
					PathChangedEvent(this,EventArgs.Empty); 
				}
			}
			catch(Exception)
			{}
		}

		private void txtPath_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			//MessageBox.Show(e.KeyChar.ToString());   
		}

		private void txtPath_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(	e.KeyValue ==13)
			{
				btnGo_Click(sender,e); 
				txtPath.Focus();
			}
  
		}

		private void grptoolbar_Enter(object sender, System.EventArgs e)
		{		
		}

	}
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
									
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct SHQUERYRBINFO
	{
		public uint cbSize;     
		public ulong i64Size;
		public ulong i64NumItems;
	};
								
	//Shell functions
	public class Win32
	{
		public const uint SHGFI_ICON = 0x100;
		//public const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
		public const uint SHGFI_SMALLICON = 0x1;    // 'Small icon
								
		[DllImport("shell32.dll")]
		public static extern IntPtr SHGetFileInfo(
			string pszPath,
			uint dwFileAttributes,
			ref SHFILEINFO psfi,
			uint cbSizeFileInfo,
			uint uFlags);
		
		[DllImport("kernel32")]
		public static extern uint GetDriveType(
			string lpRootPathName);

		[DllImport("shell32.dll")]
		public static extern bool SHGetDiskFreeSpaceEx(          
			string pszVolume,
			ref ulong pqwFreeCaller,
			ref ulong pqwTot,
			ref ulong pqwFree);

		[DllImport("shell32.Dll")]
		public static extern int SHQueryRecycleBin(          
			string pszRootPath,
			ref SHQUERYRBINFO pSHQueryRBInfo);

		[StructLayout(LayoutKind.Sequential)]
			public struct SHFILEINFO 
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

	

		[StructLayout( LayoutKind.Sequential )]
		public class BITMAPINFO
		{
			public Int32 biSize;
			public Int32 biWidth;
			public Int32 biHeight;
			public Int16 biPlanes;
			public Int16 biBitCount;
			public Int32 biCompression;
			public Int32 biSizeImage;
			public Int32 biXPelsPerMeter;
			public Int32 biYPelsPerMeter;
			public Int32 biClrUsed;
			public Int32 biClrImportant;
			public Int32 colors;
		};
		[DllImport("comctl32.dll")]
		public static extern bool ImageList_Add( IntPtr hImageList, IntPtr hBitmap, IntPtr hMask );
		[DllImport("kernel32.dll")]
		private static extern bool RtlMoveMemory( IntPtr dest, IntPtr source, int dwcount );
		[DllImport("shell32.dll")]
		public static extern IntPtr DestroyIcon( IntPtr hIcon );
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateDIBSection( IntPtr hdc, [In, MarshalAs(UnmanagedType.LPStruct)]BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset );

		
	}

	public enum ResourceScope
	{
		RESOURCE_CONNECTED = 1,
		RESOURCE_GLOBALNET,
		RESOURCE_REMEMBERED,
		RESOURCE_RECENT,
		RESOURCE_CONTEXT
	};

	public enum ResourceType
	{
		RESOURCETYPE_ANY,
		RESOURCETYPE_DISK,
		RESOURCETYPE_PRINT,
		RESOURCETYPE_RESERVED
	};

	public enum ResourceUsage
	{
		RESOURCEUSAGE_CONNECTABLE   = 0x00000001,
		RESOURCEUSAGE_CONTAINER     = 0x00000002,
		RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
		RESOURCEUSAGE_SIBLING       = 0x00000008,
		RESOURCEUSAGE_ATTACHED      = 0x00000010,
		RESOURCEUSAGE_ALL           = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
	};
	
	public enum ResourceDisplayType
	{
		RESOURCEDISPLAYTYPE_GENERIC,
		RESOURCEDISPLAYTYPE_DOMAIN,
		RESOURCEDISPLAYTYPE_SERVER,
		RESOURCEDISPLAYTYPE_SHARE,
		RESOURCEDISPLAYTYPE_FILE,
		RESOURCEDISPLAYTYPE_GROUP,
		RESOURCEDISPLAYTYPE_NETWORK,
		RESOURCEDISPLAYTYPE_ROOT,
		RESOURCEDISPLAYTYPE_SHAREADMIN,
		RESOURCEDISPLAYTYPE_DIRECTORY,
		RESOURCEDISPLAYTYPE_TREE,
		RESOURCEDISPLAYTYPE_NDSCONTAINER
	};

	public class ServerEnum : IEnumerable
	{
		enum ErrorCodes
		{
			NO_ERROR = 0,
			ERROR_NO_MORE_ITEMS = 259
		};

		[StructLayout(LayoutKind.Sequential)]
			private class NETRESOURCE 
		{
			public ResourceScope       dwScope = 0;
			public ResourceType        dwType = 0;
			public ResourceDisplayType dwDisplayType = 0;
			public ResourceUsage       dwUsage = 0;
			public string              lpLocalName = null;
			public string              lpRemoteName = null;
			public string              lpComment = null;
			public string              lpProvider = null;
		};
	

		private ArrayList aData = new ArrayList();
		

		public int Count
		{
			get { return aData.Count; }
		}
	
		[DllImport("Mpr.dll", EntryPoint="WNetOpenEnumA", CallingConvention=CallingConvention.Winapi)]
		private static extern ErrorCodes WNetOpenEnum(ResourceScope dwScope, ResourceType dwType, ResourceUsage dwUsage, NETRESOURCE p, out IntPtr lphEnum);

		[DllImport("Mpr.dll", EntryPoint="WNetCloseEnum", CallingConvention=CallingConvention.Winapi)]
		private static extern ErrorCodes WNetCloseEnum(IntPtr hEnum);

		[DllImport("Mpr.dll", EntryPoint="WNetEnumResourceA", CallingConvention=CallingConvention.Winapi)]
		private static extern ErrorCodes WNetEnumResource(IntPtr hEnum, ref uint lpcCount, IntPtr buffer, ref uint lpBufferSize);

	
		private	void EnumerateServers(NETRESOURCE pRsrc, ResourceScope scope, ResourceType type, ResourceUsage usage, ResourceDisplayType displayType,string kPath)
		{
		uint		bufferSize = 16384;
		IntPtr		buffer	= Marshal.AllocHGlobal((int) bufferSize);
		IntPtr		handle = IntPtr.Zero;
		ErrorCodes	result;
		uint		cEntries = 1;
		bool serverenum = false;

		result = WNetOpenEnum(scope, type, usage, pRsrc, out handle);

		if (result == ErrorCodes.NO_ERROR)
		{
			do
			{
				result = WNetEnumResource(handle, ref cEntries,	buffer,	ref	bufferSize);

				if ((result == ErrorCodes.NO_ERROR))
				{
					Marshal.PtrToStructure(buffer, pRsrc);

					if(String.Compare(kPath,"")==0)
					{
						if ((pRsrc.dwDisplayType	== displayType) || (pRsrc.dwDisplayType	== ResourceDisplayType.RESOURCEDISPLAYTYPE_DOMAIN))
							aData.Add(pRsrc.lpRemoteName + "|" + pRsrc.dwDisplayType );

						if ((pRsrc.dwUsage & ResourceUsage.RESOURCEUSAGE_CONTAINER )== ResourceUsage.RESOURCEUSAGE_CONTAINER )
						{	
							if ((pRsrc.dwDisplayType	== displayType))
							{
								EnumerateServers(pRsrc,	scope, type, usage,	displayType,kPath);
								
							}
								
						}
					}
					else
					{
						if (pRsrc.dwDisplayType	== displayType)
						{
							aData.Add(pRsrc.lpRemoteName);
							EnumerateServers(pRsrc,	scope, type, usage,	displayType,kPath);
							//return;
							serverenum = true;
						}
						if (!serverenum)
						{
							if (pRsrc.dwDisplayType	== ResourceDisplayType.RESOURCEDISPLAYTYPE_SHARE)
							{
								aData.Add(pRsrc.lpRemoteName + "-share");
							}
						}
						else
						{
							serverenum =false;
						}
						if((kPath.IndexOf(pRsrc.lpRemoteName)>=0)||(String.Compare(pRsrc.lpRemoteName,"Microsoft Windows Network")==0))
						{
							EnumerateServers(pRsrc,	scope, type, usage,	displayType,kPath);
							//return;
							
						}
						//}
					}
				
				}
				else if	(result	!= ErrorCodes.ERROR_NO_MORE_ITEMS)
					break;
			} while	(result	!= ErrorCodes.ERROR_NO_MORE_ITEMS);

			WNetCloseEnum(handle);
		}

		Marshal.FreeHGlobal((IntPtr) buffer);
		}

		public ServerEnum(ResourceScope scope, ResourceType type, ResourceUsage usage, ResourceDisplayType displayType,string kPath)
		{
			
			NETRESOURCE netRoot = new NETRESOURCE();
			EnumerateServers(netRoot, scope, type, usage, displayType,kPath);
		
		}
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return aData.GetEnumerator();
		}

		#endregion
	}
}
