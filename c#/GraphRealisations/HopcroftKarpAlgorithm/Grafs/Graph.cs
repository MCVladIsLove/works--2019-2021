using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Grafs
{
    public abstract class Graph
    {
        protected List<Vertex> _vertices;
        protected List<Edge> _edges;
        protected int _vertNamingInt;
        protected char _vertNameLetter;
        protected int[,] _adjacencyMatrix;
        protected Random _rnd;
        protected Size _regionToDraw;
        public List<Vertex> Vertices { get { return _vertices; } }
        public List<Edge> Edges { get { return _edges; } }
        public int[,] AdjacencyMatix { get { return _adjacencyMatrix; } }
        public Graph(int[,] adjacencyMatrix, Size fieldToDrawOn)
        {
            _rnd = Randomiser.Random;
            _regionToDraw = fieldToDrawOn;
            CreateByMatrix(adjacencyMatrix);
        }
        public virtual void Visualise(Graphics g)
        {
            foreach (Vertex v in _vertices)
                v.Visualise(g);
            foreach (Edge e in _edges)
                e.Visualise(g);
        }

        public virtual bool CreateVertex(Point coordinate)
        {
            foreach (Vertex v in _vertices)
                if (v.CheckOverlay(coordinate))
                    return false;

            if (_vertNameLetter > 'Z')
            {
                _vertNamingInt++;
                _vertNameLetter = 'A';
            }

            string name = string.Concat(_vertNameLetter++, _vertNamingInt);
            _vertices.Add(new Vertex(coordinate, name, this));

            return true;
        }

        public abstract void CreateEdge(Vertex v1, Vertex v2);


        protected virtual void CreateByMatrix(int[,] adjacencyMatrix)
        {
            _vertNamingInt = 1;
            _vertNameLetter = 'A';
            _vertices = new List<Vertex>();
            _edges = new List<Edge>();
            _adjacencyMatrix = adjacencyMatrix;
            for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
            {
                while (CreateVertex(new Point(_rnd.Next(0, _regionToDraw.Width - Vertex.PresentationRadius),
                    _rnd.Next(0, _regionToDraw.Height - Vertex.PresentationRadius))) == false) { } 


                for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
                    if (adjacencyMatrix[i, j] == 1)
                        CreateEdge(_vertices[i], _vertices[j]);

            }
        }


    }

}