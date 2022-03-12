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
        protected Point _firstCoord;
        protected Point _secondCoord;
        protected Vertex _vertex_1;
        protected Vertex _vertex_2;
        protected string _name;
        protected Pen _pen = new Pen(Color.Black);
        protected Graph _connectedGraph;

        public Vertex Vertex_1 { get { return _vertex_1; } }
        public Vertex Vertex_2 { get { return _vertex_2; } }
        public Graph Graph {get { return _connectedGraph; } }
        public Edge(Vertex v1, Vertex v2, string name, Graph graph)
        {
            _firstCoord = v1.Centre;
            _secondCoord = v2.Centre;
            _vertex_1 = v1;
            _vertex_2 = v2;
            _name = name;
            v1.Connect(v2);
            v2.Connect(v1);
            _connectedGraph = graph;
        }

        public Edge(Point coordinate_1, Point coordinate_2)
        {
            _firstCoord = coordinate_1;
            _secondCoord = coordinate_2;
        }

        public virtual void Visualise(Graphics g)
        {
            Point vectorParallel = _firstCoord == _secondCoord ? new Point(0, Vertex.PresentationRadius) : VectorParallelToEdge();
            PointF vectorPerpendicular;

            // Drawing loop
            if (Length() < Vertex.PresentationRadius)
            {
                g.DrawEllipse(_pen, _firstCoord.X + Vertex.PresentationRadius, _firstCoord.Y - 8, 20, 16);
                if (_name != "")
                {
                    g.DrawString(_name, SystemFonts.DefaultFont, _pen.Brush, 
                        _firstCoord.X + Vertex.PresentationRadius + 10 - g.MeasureString(_name, SystemFonts.DefaultFont).Width / 2, 
                        _firstCoord.Y - SystemFonts.DefaultFont.Height / 2);
                }
            }
            else // Drawing line
            {
                g.DrawLine(_pen, _firstCoord.X + vectorParallel.X, 
                    _firstCoord.Y - vectorParallel.Y, 
                    _secondCoord.X - vectorParallel.X, 
                    _secondCoord.Y + vectorParallel.Y);

                if (_name != "")
                {
                    vectorPerpendicular = Length() != 0 ? VectorPerpendicularToEdge() : new Point(-1, 0);
                    g.DrawString(_name, SystemFonts.DefaultFont, _pen.Brush,
                                (_secondCoord.X - _firstCoord.X) / 2 + _firstCoord.X - g.MeasureString(_name, SystemFonts.DefaultFont).Width / 2 - vectorPerpendicular.X * 10,
                                (_secondCoord.Y - _firstCoord.Y) / 2 + _firstCoord.Y - SystemFonts.DefaultFont.Height / 2 + vectorPerpendicular.Y * 10);
                }
            }


        }

        protected Point VectorParallelToEdge() //vector from centre of vertex parallel to edge and equals radius
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

        protected PointF VectorPerpendicularToEdge()
        {
            float x = (float)(_secondCoord.Y - _firstCoord.Y) / Length();
            float y = (float)(_secondCoord.X - _firstCoord.X) / Length();
            return new PointF(x, y);
        }
        public bool IsConnected()
        {
            return _vertex_1 != null && _vertex_2 != null;
        }

        public int Length()
        {
            return (int)Math.Sqrt(Math.Pow(_secondCoord.Y - _firstCoord.Y, 2) +
                            Math.Pow(_secondCoord.X - _firstCoord.X, 2));
        }

        public void ChangeCoordinates(Point coord_1, Point coord_2)
        {
            _firstCoord = coord_1;
            _secondCoord = coord_2;
        }
    }
}

