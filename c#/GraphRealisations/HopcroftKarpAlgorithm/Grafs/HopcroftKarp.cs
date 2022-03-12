using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafs
{
    class HopcroftKarp
    {
        Vertex _nullVertex;
        Dictionary<Vertex, int> _distances = new Dictionary<Vertex, int>();
        Dictionary<Vertex, Vertex> _pairV = new Dictionary<Vertex, Vertex>();
        Dictionary<Vertex, Vertex> _pairU = new Dictionary<Vertex, Vertex>();
        bool _stepModeEnabled = false;
        const int Infinity = int.MaxValue;
        List<Vertex> _set1;
        List<Vertex> _set2;
        Queue<Vertex> _queue = new Queue<Vertex>();
        public HopcroftKarp(BipartiteGraph graph)
        {
            _nullVertex = new Vertex();

            _set1 = graph.FirstSet;
            _set2 = graph.SecondSet;
        }

        public bool StepMode { get { return _stepModeEnabled; } }

        private bool Bfs()
        {
            foreach (Vertex u in _set1)
                if (_pairU[u] == _nullVertex)
                {
                    _distances[u] = 0;
                    _queue.Enqueue(u);
                }
                else
                    _distances[u] = Infinity;

            _distances[_nullVertex] = Infinity;
            while (_queue.Count > 0)
            {
                Vertex u = _queue.Dequeue();
                if (_distances[u] < _distances[_nullVertex])
                    foreach (Vertex v in u.Adjacent)
                        if (_distances[_pairV[v]] == Infinity)
                        {
                            _distances[_pairV[v]] = _distances[u] + 1;
                            _queue.Enqueue(_pairV[v]);
                        }    
            }
            return _distances[_nullVertex] != Infinity;
        }

        private bool Dfs(Vertex u)
        {
            if (u != _nullVertex)
            {
                foreach (Vertex v in u.Adjacent)
                    if (_distances[_pairV[v]] == _distances[u] + 1)
                        if (Dfs(_pairV[v]) == true)
                        {
                            _pairV[v] = u;
                            _pairU[u] = v;
                            return true;
                        }
                _distances[u] = Infinity;
                return false;
            }
            return true;
        }

        public void HopcroftKarpAlgorithm(out Dictionary<Vertex, Vertex> pairU)
        {
            if (_stepModeEnabled == false)
            {
                WorkStarted?.Invoke();
                foreach (Vertex u in _set1)
                    _pairU[u] = _nullVertex;
                foreach (Vertex v in _set2)
                    _pairV[v] = _nullVertex;
            }

            while (Bfs() == true)
                foreach (Vertex u in _set1)
                    if (_pairU[u] == _nullVertex)
                        Dfs(u);


            pairU = _pairU;
            _stepModeEnabled = false;

            WorkCompleted?.Invoke();
        }

        public void StepByStep(out Dictionary<Vertex, Vertex> pairU)
        {
            _stepModeEnabled = true;
            WorkStarted?.Invoke();
            pairU = _pairU;
            foreach (Vertex u in _set1)
                _pairU[u] = _nullVertex;
            foreach (Vertex v in _set2)
                _pairV[v] = _nullVertex;
        }

        public bool Step()
        {
            if (Bfs() == true)
            {
                foreach (Vertex u in _set1)
                    if (_pairU[u] == _nullVertex)
                        Dfs(u);
                return true;
            }
            else
            {
                _stepModeEnabled = false;
                WorkCompleted?.Invoke();
                return false;
            }
        }

        public delegate void AlgorithmEveventHandler();
        static public event AlgorithmEveventHandler WorkCompleted;
        static public event AlgorithmEveventHandler WorkStarted;


    }
}
