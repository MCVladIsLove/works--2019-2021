using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;


namespace Grafs
{
    public class Edge
    {
        Point _firstCoord;
        Point _secondCoord;
        Vertex _vertex_1;
        Vertex _vertex_2;
        Pen _pen = new Pen(Color.Black);

        public Vertex Vertex_1 { get { return _vertex_1; } }
        public Vertex Vertex_2 { get { return _vertex_2; } }
        public Edge(Vertex v1, Vertex v2)
        {
            _firstCoord = v1.Centre;
            _secondCoord = v2.Centre;
            _vertex_1 = v1;
            _vertex_2 = v2;
            v1.Connect(v2);
            v2.Connect(v1);
        }

        public Edge(Point coordinate_1, Point coordinate_2)
        {
            _firstCoord = coordinate_1;
            _secondCoord = coordinate_2;
        }

        public void Visualise(Graphics g)
        {
            Point vectorParallel = _firstCoord == _secondCoord ? new Point(0, Vertex.PresentationRadius) : VectorParallelToEdge();

            g.DrawLine(_pen, _firstCoord.X + vectorParallel.X,
                _firstCoord.Y - vectorParallel.Y,
                _secondCoord.X - vectorParallel.X,
                _secondCoord.Y + vectorParallel.Y);
        }

        private Point VectorParallelToEdge() //vector from centre of vertex parallel to edge and equals radius
        {
            int x = (int)(Vertex.PresentationRadius * (_secondCoord.X - _firstCoord.X) /
                            Length());

            int y;
            if (_secondCoord.Y > _firstCoord.Y)
                y = -(int)Math.Sqrt(Vertex.PresentationRadius * Vertex.PresentationRadius - x * x);
            else
                y = (int)Math.Sqrt(Vertex.PresentationRadius * Vertex.PresentationRadius - x * x);
            return new Point(x, y);
        }


        public int Length()
        {
            return (int)Math.Sqrt(Math.Pow(_secondCoord.Y - _firstCoord.Y, 2) +
                            Math.Pow(_secondCoord.X - _firstCoord.X, 2));
        }

        public void ChangeColor(Color col)
        {
            _pen.Color = col;
        }

    }
}

