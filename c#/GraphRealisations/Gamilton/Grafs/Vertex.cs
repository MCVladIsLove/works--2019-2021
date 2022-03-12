using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Grafs
{
    public class Vertex
    {
        Pen _pen = new Pen(Color.Black);
        Point _coordinates;
        string _name;
        static int _presentationRadius = 10;
        List<Vertex> _adjacent = new List<Vertex>();
        Graph _connectedGraph;

        public string Name { get { return _name; } }
        public List<Vertex> Adjacent { get { return _adjacent; } }
        public Graph Graph { get { return _connectedGraph; } }
        static public int PresentationRadius { get { return _presentationRadius; } set { _presentationRadius = value; } }
        public Point Centre { get { return new Point(_coordinates.X + _presentationRadius, _coordinates.Y + _presentationRadius); } }

        public Vertex(Point coordinates, string name, Graph graph)
        {
            _coordinates = new Point(coordinates.X - _presentationRadius, coordinates.Y - _presentationRadius);
            _name = name;
            _connectedGraph = graph;
        }

        public void Visualise(Graphics g)
        {
            g.DrawEllipse(_pen, _coordinates.X, _coordinates.Y, _presentationRadius * 2, _presentationRadius * 2);
            g.DrawString(_name, SystemFonts.DefaultFont, _pen.Brush, 
                        Centre.X - g.MeasureString(_name, SystemFonts.DefaultFont).Width / 2, 
                        Centre.Y - SystemFonts.DefaultFont.Height / 2);
        }

        public bool CheckOverlay(Point coordinate)
        {
            if (Math.Sqrt(Math.Pow(coordinate.X - Centre.X, 2) +
                          Math.Pow(coordinate.Y - Centre.Y, 2)) > _presentationRadius * 3)
                return false;

            return true;

        }

        public bool IsMouseInVertex(Point coordinate)
        {
            if (Math.Sqrt(Math.Pow(coordinate.X - Centre.X, 2) +
                          Math.Pow(coordinate.Y - Centre.Y, 2)) < _presentationRadius)
                return true;

            return false;
        }

        public bool IsConnectedWith(Vertex v)
        {
            return _adjacent.Contains(v);
        }

        public void Connect(Vertex v)
        {
            if (_adjacent.Contains(v) == false)
                _adjacent.Add(v);
        }

        public void Severe(Vertex v)
        {
            _adjacent.Remove(v);
        }
        public int Degree()
        {
            int deg = _adjacent.Count;

            return deg;
        }
    }
}
