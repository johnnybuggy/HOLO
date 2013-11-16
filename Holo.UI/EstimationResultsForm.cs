using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Holo.Processing.Search;

namespace Holo.UI
{
    public partial class EstimationResultsForm : Form
    {
        public EstimationResultsForm()
        {
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = true;
        }

        public void SetResults(IList<EstimationResult> results)
        {
            dataGridView1.DataSource = new SortableBindingList<EstimationResult>(results);

            dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Descending);
        }
    }
}
