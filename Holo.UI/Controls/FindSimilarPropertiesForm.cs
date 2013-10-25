using System;
using System.Windows.Forms;

namespace Holo.UI.Controls
{
    public partial class FindSimilarPropertiesForm : Form
    {
        public FindSimilarPropertiesForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
