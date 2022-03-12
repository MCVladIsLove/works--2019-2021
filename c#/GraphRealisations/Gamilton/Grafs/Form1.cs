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
    public partial class Form1 : Form
    {
        Graph _graph;
        bool _isMousePressed = false;
        Vertex _caughtVertex1;
        Vertex _caughtVertex2;
        Edge _tempEdge;
        int _numLoops = 0;
        int _numEdges = 0;
        int _numVertices = 0;
        public Form1()
        {
            InitializeComponent();
            _graph = new Graph();
            _graph.NumberVertexesChanged += NumberVertexesChanged;
            _graph.NumberEdgesChanged += NumberEdgesChanged;
            _graph.NumberLoopsChanged += NumberLoopsChanged;
        }

        private void NumberLoopsChanged(bool increased)
        {
            _numLoops = _graph.NumberLoops();
            label10.Text = _numLoops.ToString();
            FillMaxDegrees();
            SetConnectivityCategory(label11);
        }

        private void NumberEdgesChanged(bool increased)
        {
            _numEdges = _graph.Edges.Count;
            label9.Text = _numEdges.ToString();
            FillMaxDegrees();
            SetConnectivityCategory(label11);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            _graph.Visualise(e.Graphics);
            _tempEdge?.Visualise(e.Graphics);
        }
            
        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _caughtVertex1 = _graph.GetVertexByCoord(e.Location);
            if (e.Button == MouseButtons.Left)
            {
                _isMousePressed = true;
                if (_caughtVertex1 != null)
                    if (_graph.GetType().Name == "Graph")
                        _tempEdge = new Edge(_caughtVertex1.Centre, e.Location);
                    else
                        _tempEdge = new Arc(_caughtVertex1.Centre, e.Location);
                _graph.CreateVertex(e.Location);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isMousePressed = false;
                _caughtVertex2 = _graph.GetVertexByCoord(e.Location);
                if (_caughtVertex1 != null && _caughtVertex2 != null)
                    _graph.CreateEdge(_caughtVertex1, _caughtVertex2);
                _caughtVertex1 = null;
                _caughtVertex2 = null;
                _tempEdge = null;
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (_caughtVertex1 != null)
                    _graph.DeleteVertex(_caughtVertex1);
            }
        }

        private void NumberVertexesChanged(bool increased)
        {
            _numVertices = _graph.Vertices.Count;
            label8.Text = _numVertices.ToString();
            FillMaxDegrees();
            SetConnectivityCategory(label11);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMousePressed && _caughtVertex1 != null)
                _tempEdge.ChangeCoordinates(_caughtVertex1.Centre, e.Location);
        }

        private void SetConnectivityCategory(Label label)
        {
            string text = "";
            switch (_graph.CheckConnectivity())
            {
                case ConnectivityCategory.FullyConected:
                    text = "полносвязный";
                    break;
                case ConnectivityCategory.Incoherent:
                    text = "несвязный";
                    break;
                case ConnectivityCategory.OneWay:
                    text = "односторонне связный";
                    break;
                case ConnectivityCategory.Weak:
                    text = "слабосвязный";
                    break;
                case ConnectivityCategory.Connected:
                    text = "связный";
                    break;
            }
            label.Text = text;
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                _graph = new OrientedGraph(_graph);
                label13.Text = "Максимальная степень по заходам:";
                label7.Text = "Максимальная степень по исходам:";
                label12.Location = new Point(label12.Location.X + 62, label12.Location.Y);
            }
            else
            {
                label13.Text = "";
                label7.Text = "Максимальная степень:";
                label12.Location = new Point(label12.Location.X - 62, label12.Location.Y);
                label14.Text = "";
                _graph = new Graph(_graph);
            }

            FillMaxDegrees();

            SetConnectivityCategory(label11);

            _graph.NumberVertexesChanged += NumberVertexesChanged;
            _graph.NumberEdgesChanged += NumberEdgesChanged;
            _graph.NumberLoopsChanged += NumberLoopsChanged;

            _numLoops = _graph.NumberLoops();
            _numEdges = _graph.Edges.Count + _numLoops;

            label9.Text = _numEdges.ToString();
            label10.Text = _numLoops.ToString();
        }

        private void FillMaxDegrees()
        {
            if (_graph.GetType().Name == "OrientedGraph")
            {
                OrientedGraph unpackedGraph = (OrientedGraph)_graph;
                label14.Text = unpackedGraph.GetMaxVertexIncomingDegree().ToString();
            }
            label12.Text = _graph.GetMaxVertexDegree().ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new AdjacencyMatrixForm(_graph).ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new AntecedentForm(_graph.AntecedentList()).ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new InfoForm().ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new ComponentsForm(_graph.GetComponents()).ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<Vertex[]> cycles = null;
            HamiltonCycleResult res;
            if (_graph.Vertices.Count > 0)
                res = _graph.CheckHamiltonCycles(out cycles);
            else
                res = HamiltonCycleResult.NecessaryConditionViolation;


            if (res == HamiltonCycleResult.Found)
                new HamiltonCyclesForm(cycles).ShowDialog();
            else if (res == HamiltonCycleResult.CyclesNotFound)
                new HamiltonCyclesForm("Не найдено циклов").ShowDialog();
            else if (res == HamiltonCycleResult.NecessaryConditionViolation)
                new HamiltonCyclesForm("Не соблюдается необходимое условие\n" +
                    "(для неориентированного графа - степень \n" +
                    "каждой вершины >= 2\n" +
                    "для ориентировнного - степень \n" +
                    "по исходам >= 1 &&\n" +
                    "степень по заходам >= 1)\n" +
                    "для графа с кол-вом вершин > 2").ShowDialog();
        }
    }
}
