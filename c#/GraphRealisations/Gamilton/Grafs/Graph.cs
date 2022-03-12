using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Grafs
{
    public class Graph
    {
        protected List<Vertex> _vertexes;
        protected List<Edge> _edges;
        protected int _vertNamingInt;
        protected int _edgeNamingInt;
        protected char _vertNameLetter;

        public List<Vertex> Vertices { get { return _vertexes; } }
        public List<Edge> Edges { get { return _edges; } }

        public delegate void GraphEventHandler(bool increased);
        public event GraphEventHandler NumberVertexesChanged;
        public event GraphEventHandler NumberEdgesChanged;
        public event GraphEventHandler NumberLoopsChanged;
        public Graph()
        {
            _edgeNamingInt = 1;
            _vertNamingInt = 1;
            _vertNameLetter = 'A';
            _vertexes = new List<Vertex>();
            _edges = new List<Edge>();
        }
        public Graph(Graph sourceGraph)
        {
            _vertNamingInt = sourceGraph._vertNamingInt;
            _vertNameLetter = sourceGraph._vertNameLetter;
            _vertexes = sourceGraph._vertexes;
            ConnectByMatrix(sourceGraph.AdjacencyMatrix());
        }

        public void Visualise(Graphics g)
        {
            foreach (Vertex v in _vertexes)
                v.Visualise(g);
            foreach (Edge e in _edges)
                e.Visualise(g);
        }

        public void CreateVertex(Point coordinate)
        {
            foreach (Vertex v in _vertexes)
                if (v.CheckOverlay(coordinate))
                    return;

            if (_vertNameLetter > 'Z')
            {
                _vertNamingInt++;
                _vertNameLetter = 'A';
            }

            string name = string.Concat(_vertNameLetter++, _vertNamingInt);
            _vertexes.Add(new Vertex(coordinate, name, this));
            NumberVertexesChanged?.Invoke(true);
        }

        public virtual void CreateEdge(Vertex v1, Vertex v2)
        {
            if (v1.IsConnectedWith(v2) == false)
            {
                _edges.Add(new Edge(v1, v2, _edgeNamingInt++.ToString(), this));
                if (v1 == v2)
                    NumberLoopsChanged?.Invoke(true);
                NumberEdgesChanged?.Invoke(true);
            }
        }

        public Vertex GetVertexByCoord(Point coordinate)
        {
            foreach (Vertex v in _vertexes)
                if (v.IsMouseInVertex(coordinate))
                    return v;

            return null;
        }
       
        public void DeleteVertex(Vertex v)
        {
            _vertexes.Remove(v);

            foreach (Edge e in _edges.ToArray())
                if (e.Vertex_1 == v || e.Vertex_2 == v)
                    DeleteEdge(e);
            NumberVertexesChanged?.Invoke(false);
        }

        public void DeleteEdge(Edge e)
        {
            _edges.Remove(e);
            if (e.Vertex_2 == e.Vertex_1)
            {
                e.Vertex_1.Severe(e.Vertex_2);
                NumberLoopsChanged?.Invoke(false);
            }
            else
            {
                e.Vertex_1.Severe(e.Vertex_2);
                e.Vertex_2.Severe(e.Vertex_1);
            }
            NumberEdgesChanged?.Invoke(false);
        }

        public int GetMaxVertexDegree()
        {
            int deg = 0;
            foreach (var v in _vertexes)
                if (v.Degree() > deg)
                    deg = v.Degree();

            return deg;
        }

        public Dictionary<string, List<string>> AntecedentList()
        {
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            foreach (var v in _vertexes)
                dict.Add(v.Name, new List<string>());

            foreach (var v in _vertexes)
                foreach (var v2 in v.Adjacent)
                    if (dict.ContainsKey(v2.Name))
                        dict[v2.Name].Add(v.Name);

            return dict;
        }


        protected int ConnectedVerticesCount(List<Vertex> checkedVertices, Vertex currentVertex) // used for defining connectivity
        {
            int vertexesCount = 1;
            Vertex[] connectedVertices = currentVertex.Adjacent.ToArray();
            foreach (var v in connectedVertices)
                if (v != currentVertex && checkedVertices.Contains(v) == false)
                {
                    checkedVertices.Add(v);
                    vertexesCount += ConnectedVerticesCount(checkedVertices, v);
                }

            return vertexesCount;
        }

        public virtual ConnectivityCategory CheckConnectivity()
        {
            ConnectivityCategory category = ConnectivityCategory.Incoherent;
            if (_vertexes.Count > 0 && ConnectedVerticesCount(new List<Vertex>() {_vertexes[0] }, _vertexes[0]) == _vertexes.Count)
                category = ConnectivityCategory.Connected;

            int[,] matr = AdjacencyMatrix();
            if (category == ConnectivityCategory.Connected)
            {
                for (int i = 0; i < matr.GetLength(0); i++)
                    for (int j = 0; j < matr.GetLength(0); j++)
                        if (matr[i, j] == 0 && i != j)
                            return ConnectivityCategory.Connected;
                return ConnectivityCategory.FullyConected;
            }
            return ConnectivityCategory.Incoherent;
        }

        public virtual Vertex[][] GetComponents()
        {
            List<Vertex[]> components = new List<Vertex[]>();
            List<Vertex> component = new List<Vertex>();
            List<Vertex> checkedVertices = new List<Vertex>();
            int numChecked = 0;
            int i;
            if (_vertexes.Count > 0)
            {
                while (numChecked < _vertexes.Count)
                {
                    i = 0;
                    while (checkedVertices.Contains(_vertexes[i])) { i++; }

                    component.Add(_vertexes[i]);
                    numChecked += ConnectedVerticesCount(component, _vertexes[i]);
                    components.Add(component.ToArray());
                    checkedVertices.AddRange(component);
                    component = new List<Vertex>();
                }
            }

            return components.ToArray();
        }

        protected void ConnectByMatrix(int[,] adjacencyMatrix)
        {
            _edgeNamingInt = 1;
            _edges = new List<Edge>();
            foreach (Vertex v in _vertexes)
                v.Adjacent.Clear();
            for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < adjacencyMatrix.GetLength(0); j++)
                    if (adjacencyMatrix[i, j] == 1)
                        CreateEdge(_vertexes[i], _vertexes[j]);
            }
            NumberVertexesChanged?.Invoke(true);
        }

        public virtual int[,] AdjacencyMatrix()
        {
            int[,] matrix;
            matrix = new int[_vertexes.Count, _vertexes.Count];
            for (int i = 0; i < _vertexes.Count; i++)
                for (int j = i; j < _vertexes.Count; j++)
                    if (_vertexes[i].IsConnectedWith(_vertexes[j]))
                        matrix[i, j] = matrix[j, i] = 1;
                    else
                        matrix[i, j] = matrix[j, i] = 0;

            return matrix;
        }

        public HamiltonCycleResult CheckHamiltonCycles(out List<Vertex[]> ways)
        {
            ways = new List<Vertex[]>();
            if (CheckHamiltonNecessaryCondition() == false)
                return HamiltonCycleResult.NecessaryConditionViolation;

            HamiltonCycleStep(new List<Vertex>() {_vertexes[0] }, _vertexes[0], ways);

            if (ways.Count > 0)
                return HamiltonCycleResult.Found;
            else
                return HamiltonCycleResult.CyclesNotFound;
        }

        protected int HamiltonCycleStep(List<Vertex> checkedVertices, Vertex currentVertex, List<Vertex[]> ways)
        {
            Vertex[] connectedVertices = currentVertex.Adjacent.ToArray();
            foreach (var v in connectedVertices)
                if (v != currentVertex && checkedVertices.Contains(v) == false)
                {
                    checkedVertices.Add(v);
                    if (HamiltonCycleStep(checkedVertices, v, ways) == _vertexes.Count)
                        if (checkedVertices.Last().IsConnectedWith(checkedVertices[0]))
                            ways.Add((Vertex[])checkedVertices.ToArray().Clone());
                    checkedVertices.Remove(v);
                }

            return checkedVertices.Count;
        }

        protected virtual bool CheckHamiltonNecessaryCondition()
        {
            foreach (Vertex v in _vertexes)
                if (v.Adjacent.Count < 3 && v.IsConnectedWith(v) || !v.IsConnectedWith(v) && v.Adjacent.Count < 2)
                    return false;

            return true;
        }

        public int NumberLoops()
        {
            int n = 0;
            foreach (var e in _edges)
                if (e.Vertex_1 == e.Vertex_2)
                    n++;
            return n;
        }

        protected void NumberEdgesChangedWrap(bool increased)
        {
            NumberEdgesChanged?.Invoke(increased);
        }
        protected void NumberLoopsChangedWrap(bool increased)
        {
            NumberLoopsChanged?.Invoke(increased);
        }
        protected void NumberVertexesChangedWrap(bool increased)
        {
            NumberVertexesChanged?.Invoke(increased);
        }
    }

    public enum ConnectivityCategory
    {
        FullyConected,
        OneWay,
        Weak,
        Connected,
        Incoherent

    }

    public enum HamiltonCycleResult
    {
        NecessaryConditionViolation,
        CyclesNotFound,
        Found
    }


}