using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Grafs
{
    class BipartiteGraph : Graph
    {
        public BipartiteGraph(int[,] adjacencyMatrix, Size fieldToDrawOn) : base(adjacencyMatrix, fieldToDrawOn) { }

        public List<Vertex> FirstSet { get { return _vertices.GetRange(0, _adjacencyMatrix.GetLength(0)); } }
        public List<Vertex> SecondSet { get { return _vertices.GetRange(_adjacencyMatrix.GetLength(0), _adjacencyMatrix.GetLength(1)); } }
        public override void CreateEdge(Vertex v1, Vertex v2)
        {
            if (v1.IsConnectedWith(v2) == false)
                _edges.Add(new Edge(v1, v2));
        }

        protected override void CreateByMatrix(int[,] adjacencyMatrix)
        {
            _vertNamingInt = 1;
            _vertNameLetter = 'A';
            _vertices = new List<Vertex>();
            _edges = new List<Edge>();
            _adjacencyMatrix = adjacencyMatrix;

            int widthBetween = Vertex.PresentationRadius * 2;
            int heightBetween = Vertex.PresentationRadius * 23;
            int firstSetCount = adjacencyMatrix.GetLength(0);
            int secondSetCount = adjacencyMatrix.GetLength(1);
            int firstSetStartX = _regionToDraw.Width / 2 - ((firstSetCount - 1) * widthBetween + firstSetCount * Vertex.PresentationRadius * 2) / 2; // центрируем ряд вершин, задавая левую крайнюю точку X
            int firstSetStartY = _regionToDraw.Height / 2 - (heightBetween + Vertex.PresentationRadius * 2 * 2) / 2; // крайнюю верхнюю Y
            int secondSetStartX = _regionToDraw.Width / 2 - ((secondSetCount - 1) * widthBetween + secondSetCount * Vertex.PresentationRadius * 2) / 2; // крайнюю левую X для второй доли графа
            for (int i = 0; i < firstSetCount; i++)
                CreateVertex(new Point(firstSetStartX + widthBetween * i + Vertex.PresentationRadius * 2 * i, firstSetStartY));

            for (int j = 0; j < secondSetCount; j++)
                CreateVertex(new Point(secondSetStartX + widthBetween * j + Vertex.PresentationRadius * 2 * j, firstSetStartY + heightBetween + Vertex.PresentationRadius * 2));

            for (int i = 0; i < firstSetCount; i++)
                for (int j = 0; j < secondSetCount; j++)
                {
                    if (adjacencyMatrix[i, j] == 1)
                        CreateEdge(_vertices[i], _vertices[firstSetCount + j]);
                    else if (adjacencyMatrix[i, j] != 0)
                        throw new Exception("Incorrect adjacency matrix");
                }
        }
    }
}
