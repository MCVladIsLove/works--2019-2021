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
    public partial class AntecedentForm : Form
    {
        public AntecedentForm(Dictionary<string, List<string>> antecedentsDict)
        {
            InitializeComponent();
            int y = 25;
            Label label;
            int x = 5;
            foreach (string key in antecedentsDict.Keys)
            {
                label = new Label();
                label.Location = new Point(x, y);
                label.Text = key + ": ";
                label.AutoSize = true;

                foreach (string val in antecedentsDict[key])
                    label.Text += val + " ";

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
