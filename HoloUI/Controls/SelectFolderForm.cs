using System;
using System.Windows.Forms;

namespace HoloUI
{
    public partial class SelectFolderForm : Form
    {
        public SelectFolderForm()
        {
            InitializeComponent();
        }

        public string SelectedFolder
        {
            get { return tv.SelectedPath; } 
            set { tv.SelectedPath = value; }
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
