using System;
using System.Collections.Generic;
using System.IO;
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
        BipartiteGraph _graph;
        int[,] _adjacencyMatrix;
        Dictionary<Vertex, Vertex> _pairs;
        HopcroftKarp _hopcroftKarp;
        float _connectivityPercent;
        public Form1()
        {
            InitializeComponent();
            numericUpDown2.Maximum = numericUpDown4.Maximum - 2; 
            numericUpDown4.Minimum = numericUpDown2.Value + 2;
            HopcroftKarp.WorkCompleted += HopcroftKarp_WorkCompleted;
            HopcroftKarp.WorkStarted += HopcroftKarp_WorkStarted;
        }

        private void HopcroftKarp_WorkStarted()
        {
            button5.Enabled = false;
        }

        private void HopcroftKarp_WorkCompleted()
        {
            button5.Enabled = true;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            _graph?.Visualise(e.Graphics);
        }
            
        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _adjacencyMatrix = CreateRandomBinaryMatrix((int)numericUpDown1.Value, (int)numericUpDown3.Value);
            CorrectMatrix();
            _graph = new BipartiteGraph(_adjacencyMatrix, pictureBox1.Size);
            button3.Enabled = true;
            button4.Enabled = true;
            button2.Enabled = false;
            button5.Enabled = false;
        }

        int[,] CreateRandomBinaryMatrix(int rows, int columns)
        {
            int[,] matrix = new int[rows, columns];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    matrix[i, j] = Randomiser.Random.Next(0, 2);

            return matrix;
        }

        void CorrectMatrix()
        {
            int edges = 0;
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
                for (int j = 0; j < _adjacencyMatrix.GetLength(1); j++)
                    if (_adjacencyMatrix[i, j] == 1)
                        edges++;

            int vertices = _adjacencyMatrix.GetLength(0);
            _connectivityPercent = 0;
            int randFirst;
            int randSecond;
            while (true)
            {
                _connectivityPercent = 2f * edges / (vertices * (vertices - 1f)) * 100f;
                randFirst = Randomiser.Random.Next(0, _adjacencyMatrix.GetLength(0));
                randSecond = Randomiser.Random.Next(0, _adjacencyMatrix.GetLength(1));
                if (_connectivityPercent > (int)numericUpDown4.Value)
                {
                    if (_adjacencyMatrix[randFirst, randSecond] != 0)
                    {
                        _adjacencyMatrix[randFirst, randSecond] = 0;
                        edges--;
                    }
                }
                else if (_connectivityPercent < (int)numericUpDown2.Value)
                {
                    if (_adjacencyMatrix[randFirst, randSecond] != 1)
                    {
                        _adjacencyMatrix[randFirst, randSecond] = 1;
                        edges++;
                    }
                }
                else
                    break;

            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            UnmarkEdges();
            if (_hopcroftKarp.Step() == false)
                button2.Enabled = false;
            MarkPairs();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown4.Minimum = numericUpDown2.Value + 2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UnmarkEdges();
            if (_hopcroftKarp == null || _hopcroftKarp.StepMode == false)
                _hopcroftKarp = new HopcroftKarp(_graph);
            else
                button2.Enabled = false;
            _hopcroftKarp.HopcroftKarpAlgorithm(out _pairs);
            MarkPairs();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UnmarkEdges();
            button2.Enabled = true;
            _hopcroftKarp = new HopcroftKarp(_graph);
            _hopcroftKarp.StepByStep(out _pairs);
        }

        private void MarkPairs()
        {
            if (_pairs != null)
            {
                Vertex v;
                foreach (Vertex u in _graph.FirstSet)
                    if (_pairs.TryGetValue(u, out v))
                        foreach (var edge in _graph.Edges)
                            if (edge.Vertex_1 == u && edge.Vertex_2 == v)
                                edge.ChangeColor(Color.Red);
            }
        }

        private void UnmarkEdges()
        {
            if (_pairs != null)
            {
                foreach (var e in _graph.Edges)
                    e.ChangeColor(Color.Black);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (StreamWriter w = new StreamWriter("res.txt", true))
                SaveInfo(w);

            button5.Enabled = false;
        }

        private void SaveInfo(StreamWriter sw)
        {
            sw.WriteLine("\t\tМатрица смежности");
            sw.Write("\t   ");
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
                sw.Write(_graph.FirstSet[i].Name + " ");
            sw.WriteLine();
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
            {
                sw.Write("\t" + _graph.SecondSet[i].Name + " ");
                for (int j = 0; j < _adjacencyMatrix.GetLength(1); j++)
                    sw.Write(_adjacencyMatrix[i, j] + "  ");
                sw.WriteLine();
            }
            sw.WriteLine();
            sw.WriteLine("Процент связности графа: " + _connectivityPercent + "%");
            sw.Write("Паросочетание: ");
            int n = 0;
            foreach (var e in _pairs)
                if (e.Value.Name != "")
                {
                    sw.Write(e.Key.Name + "-" + e.Value.Name + " ");
                    n++;
                }
            sw.WriteLine("\t" + n);
            sw.WriteLine("");
            sw.WriteLine("__________________________________________________________________________________________");
        }
    }
}
