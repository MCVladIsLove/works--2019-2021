using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafs
{
    class OrientedGraph : Graph
    {
        public OrientedGraph()
        {
            _edgeNamingInt = 1;
            _vertNamingInt = 1;
            _vertNameLetter = 'A';
            _vertexes = new List<Vertex>();
            _edges = new List<Edge>();
        }

        public OrientedGraph(Graph sourceGraph) : base(sourceGraph) { }

        public override int[,] AdjacencyMatrix()
        {
            int[,] matrix;
            matrix = new int[_vertexes.Count, _vertexes.Count];
            for (int i = 0; i < _vertexes.Count; i++)
                for (int j = 0; j < _vertexes.Count; j++)
                    if (_vertexes[i].IsConnectedWith(_vertexes[j]))
                        matrix[i, j] = 1;
                    else
                        matrix[i, j] = 0;

            return matrix;
        }
        public override void CreateEdge(Vertex v1, Vertex v2)
        {
            if (v1.IsConnectedWith(v2) == false)
            {
                _edges.Add(new Arc(v1, v2, _edgeNamingInt++.ToString(), this));
                if (v1 == v2)
                    NumberLoopsChangedWrap(true);
                NumberEdgesChangedWrap(true);
            }
        }

        public int GetMaxVertexIncomingDegree()
        {
            int deg = 0;

            Dictionary<string, List<string>> antecedent = AntecedentList();

            foreach (string key in antecedent.Keys)
                if (antecedent[key].Count > deg)
                {
                    deg = antecedent[key].Count;
                }

            return deg;
        }

        protected override bool CheckHamiltonNecessaryCondition()
        {
            bool found;
            bool hasLoop;
            if (_vertexes.Count > 2)
            {

                foreach (Vertex v in _vertexes)
                {
                    found = false;
                    if (v.IsConnectedWith(v))
                        hasLoop = true;
                    else
                        hasLoop = false;       // петля тоже считается в adjacent каждого объекта Vertex

                    if (v.Adjacent.Count > 1 && hasLoop || !hasLoop && v.Adjacent.Count > 0)
                        for (int i = 0; i < _vertexes.Count && found == false; i++)
                            if (_vertexes[i].IsConnectedWith(v) && _vertexes[i] != v)
                                if (v.IsConnectedWith(_vertexes[i]))
                                {
                                    if (hasLoop)
                                    {
                                        if (v.Adjacent.Count > 2)             // если исходящих дуг больше одной, то достаточно иметь хоть 
                                            found = true;                     // входящую дугу
                                    }                                         // если исходит только одна дуга, то входящая дуга должна
                                    else if (v.Adjacent.Count > 1)            // иметь не совпадающую с концом исходящей вершину
                                        found = true;                         // иначе получается проход по уже пройденным вершинам
                                }
                                else
                                    found = true;

                    if (found == false)
                        return false;
                }

                return true;
            }
            else if (_vertexes.Count == 2)
                return _vertexes[0].IsConnectedWith(_vertexes[1]) && _vertexes[1].IsConnectedWith(_vertexes[0]);

            return false;
        }

        public override Vertex[][] GetComponents()
        {
            List<Vertex[]> components = new List<Vertex[]>();
            List<Vertex> component = new List<Vertex>();
            int[][] matrix = ReachabilityMatrix();
            int[] checkedVertices = new int[_vertexes.Count];
            bool isEqual = true;
            
            for (int i = 0; i < _vertexes.Count; i++)
            {
                if (checkedVertices[i] == 1)
                    continue;

                for (int i2 = i; i2 < _vertexes.Count; i2++)
                {
                    for (int j = 0; j < _vertexes.Count; j++)
                        if (matrix[i][j] != matrix[i2][j])
                        {
                            isEqual = false;
                            break;
                        }

                    if (isEqual && checkedVertices[i2] != 1)
                    {
                        component.Add(_vertexes[i2]);
                        checkedVertices[i2] = 1;
                    }
                    isEqual = true;
                }
                components.Add(component.ToArray());
                component = new List<Vertex>();
            }

            return components.ToArray();    
        }
        public override ConnectivityCategory CheckConnectivity()
        {
            int[][] reachabilityMatrix = ReachabilityMatrix();

            ConnectivityCategory category = ConnectivityCategory.FullyConected;

            for (int i = 0; i < _vertexes.Count; i++)
                for (int j = 0; j < _vertexes.Count; j++)
                    if (reachabilityMatrix[i][j] == 0)
                    {
                        if (reachabilityMatrix[j][i] == 1)
                        {
                            if (category < ConnectivityCategory.Weak)
                                category = ConnectivityCategory.OneWay;
                        }
                        else
                        {
                            category = ConnectivityCategory.Weak;
                            i = _vertexes.Count;
                            break;
                        }
                    }

            if (category == ConnectivityCategory.Weak)
            {
                for (int i = 0; i < _vertexes.Count; i++)
                    for (int j = i; j < _vertexes.Count; j++)
                    {
                        reachabilityMatrix[i][j] |= reachabilityMatrix[j][i];
                        reachabilityMatrix[j][i] |= reachabilityMatrix[i][j];
                    }
                int[] tmpCheckedVertices = new int[_vertexes.Count];
                tmpCheckedVertices[0] = 1;
                if (CheckIncoherence(tmpCheckedVertices, 0, reachabilityMatrix) != _vertexes.Count)
                    category = ConnectivityCategory.Incoherent;
            }

            return category;
        }

        private int CheckIncoherence(int[] checkedVertices, int currentVertex, int[][] reachabilityMatrixSimmetrised)
        {
            int vertexesCount = 1;
            int[] connectedVertices;
            connectedVertices = reachabilityMatrixSimmetrised[currentVertex];
            for (int j = 0; j < _vertexes.Count; j++)
                if (j != currentVertex && checkedVertices[j] == 0 && connectedVertices[j] == 1)
                {
                    checkedVertices[j] = 1;
                    vertexesCount += CheckIncoherence(checkedVertices, j, reachabilityMatrixSimmetrised);
                }

            return vertexesCount;
        }

        public int[][] ReachabilityMatrix()
        {
            int[][] matrix = new int[_vertexes.Count][];
            int[,] adjacencyMatrix = AdjacencyMatrix();
            for (int i = 0; i < _vertexes.Count; i++)
            {
                matrix[i] = new int[_vertexes.Count];
                Reachability(i, adjacencyMatrix, matrix[i]);
            }

            return matrix;
        }
        private void Reachability(int row, int[,] adjacencyMatrix, int[] checkedVertices)
        {
            checkedVertices[row] = 1;
            for (int j = 0; j < adjacencyMatrix.GetLength(0); j++)
                if (adjacencyMatrix[row, j] == 1 && checkedVertices[j] == 0)
                {
                    checkedVertices[j] = 1;
                    Reachability(j, adjacencyMatrix, checkedVertices);
                }        
        }
    }
}
