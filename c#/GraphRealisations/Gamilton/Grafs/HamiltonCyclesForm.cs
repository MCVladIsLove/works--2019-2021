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
    public partial class HamiltonCyclesForm : Form
    {
        public HamiltonCyclesForm(List<Vertex[]> cycles)
        {
            InitializeComponent();
            int y = 25;
            Label label;
            int x = 5;
            for (int i = 0; i < cycles.Count; i++)
            {
                label = new Label();
                label.Location = new Point(x, y);
                label.Text = (i + 1).ToString() + ": ";
                label.AutoSize = true;

                for (int j = 0; j < cycles[i].Length; j++)
                    label.Text += cycles[i][j].Name + " - ";

                label.Text += cycles[i][0].Name;

                Controls.Add(label);
                y += 23;
            }
        }

        public HamiltonCyclesForm(string error)
        {
            InitializeComponent();
            int y = 25;
            Label label;
            int x = 5;
            label = new Label();
            label.Location = new Point(x, y);
            label.Text = error;
            label.AutoSize = true;
            Controls.Add(label);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
