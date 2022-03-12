using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grafs
{
    public partial class AdjacencyMatrixForm : Form
    {
        public AdjacencyMatrixForm(Graph graph)
        {
            InitializeComponent();
            for (int i = 0; i < graph.Vertices.Count; i++)
            {
                dataGridView2.Columns.Add(i.ToString(), graph.Vertices[i].Name);
                dataGridView2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView2.Rows.Add();
                dataGridView2.Rows[i].HeaderCell.Value = graph.Vertices[i].Name;
            }

            int[,] matrix = graph.AdjacencyMatrix();
            for (int i = 0; i < graph.Vertices.Count; i++)
                for (int j = 0; j < graph.Vertices.Count; j++)
                    dataGridView2.Rows[i].Cells[j].Value = matrix[i, j];

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
