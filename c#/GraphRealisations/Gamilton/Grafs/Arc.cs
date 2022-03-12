using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Grafs
{
    class Arc : Edge
    {
        SolidBrush _brush = new SolidBrush(Color.Black);
        public Arc(Vertex one, Vertex two, string name, Graph graph) : base(one.Centre, two.Centre)
        {
            _vertex_1 = one;
            _vertex_2 = two;
            _name = name;
            one.Connect(two);
            _connectedGraph = graph;
        }

        public Arc(Point one, Point two) : base(one, two)
        {
            _firstCoord = one;
            _secondCoord = two;
        }

        public override void Visualise(Graphics g)
        {
            Point vectorParallel = _firstCoord == _secondCoord ? new Point(0, Vertex.PresentationRadius) : VectorParallelToEdge();
            PointF vectorPerpendicular;
            PointF[] triangle;

            // Drawing loop
            if (Length() < Vertex.PresentationRadius)
            {
                triangle = new PointF[] { new PointF(_firstCoord.X + Vertex.PresentationRadius, _firstCoord.Y), 
                                         new PointF(_firstCoord.X + Vertex.PresentationRadius + 1, _firstCoord.Y - 10),
                                         new PointF(_firstCoord.X + Vertex.PresentationRadius + 7, _firstCoord.Y - 1) };

                g.DrawEllipse(_pen, _firstCoord.X + Vertex.PresentationRadius, _firstCoord.Y - 8, 20, 16);
                g.FillPolygon(_brush, triangle);
                if (_name != "")
                {
                    g.DrawString(_name, SystemFonts.DefaultFont, _pen.Brush,
                        _firstCoord.X + Vertex.PresentationRadius + 10 - g.MeasureString(_name, SystemFonts.DefaultFont).Width / 2,
                        _firstCoord.Y - SystemFonts.DefaultFont.Height / 2);
                }
            }
            else // Drawing line
            {
                vectorPerpendicular = Length() != 0 ? VectorPerpendicularToEdge() : new Point(-1, 0);
                triangle = new PointF[] { new PointF(_secondCoord.X - vectorParallel.X, _secondCoord.Y + vectorParallel.Y),
                                         new PointF(_secondCoord.X - vectorParallel.X * 2.5f - vectorPerpendicular.X * 4, _secondCoord.Y + vectorParallel.Y * 2.5f + vectorPerpendicular.Y * 4),
                                         new PointF(_secondCoord.X - vectorParallel.X * 2.5f + vectorPerpendicular.X * 4, _secondCoord.Y + vectorParallel.Y * 2.5f - vectorPerpendicular.Y * 4) };

                g.DrawLine(_pen, _firstCoord.X + vectorParallel.X,
                    _firstCoord.Y - vectorParallel.Y,
                    _secondCoord.X - vectorParallel.X,
                    _secondCoord.Y + vectorParallel.Y);
                g.FillPolygon(_brush, triangle);
                if (_name != "")
                {
                    g.DrawString(_name, SystemFonts.DefaultFont, _pen.Brush,
                                (_secondCoord.X - _firstCoord.X) / 2 + _firstCoord.X - g.MeasureString(_name, SystemFonts.DefaultFont).Width / 2 - vectorPerpendicular.X * 10,
                                (_secondCoord.Y - _firstCoord.Y) / 2 + _firstCoord.Y - SystemFonts.DefaultFont.Height / 2 + vectorPerpendicular.Y * 10);
                }

            }
        }
    }
}
