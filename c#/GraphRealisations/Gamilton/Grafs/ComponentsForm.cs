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
    public partial class ComponentsForm : Form
    {
        public ComponentsForm(Vertex[][] components)
        {
            InitializeComponent();
            int y = 25;
            Label label;
            int x = 5;
            for (int i = 0; i < components.Length; i++)
            {
                label = new Label();
                label.Location = new Point(x, y);
                label.Text = (i + 1).ToString() + ": ";
                label.AutoSize = true;

                foreach (Vertex v in components[i])
                    label.Text += v.Name + "  ";

                Controls.Add(label);
                y += 23;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
