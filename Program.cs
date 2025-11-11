using System;
using System.Numerics;
using System.Reflection;
using System.Collections;

namespace MyApp
{
    public class Vertex 
    {
        private List<Vertex> neighbours;
        public int id;
        
        public Vertex(int id)
        {
            this.id = id;
            neighbours = new List<Vertex>();
        }
        public List<Vertex> GetNeighboursList() => neighbours;
        public void AddNeighbour(Vertex id) => neighbours.Add(id);
        public void RemoveNeighbour(Vertex id) => neighbours.Remove(id);
        public Vertex NeighborAt(int index)
        {
            if (index < 0 || index >= neighbours.Count) return null;
            return neighbours[index];
        }
        public bool HasNeighbor(int id) => neighbours.Any(n => n.id == id);
        public void SeeNeighbors()
        {
            Console.Write($"Vertex {id} neighbors: ");
            foreach (var neighbor in neighbours)
            {
                Console.Write($"{neighbor.id} ");
            }
            Console.WriteLine();
        }
    }

    class Graph
    {
        private int countOfVertices;
        private Dictionary<int, Vertex> vertices;
        private List<List<int>> adjacencyMatrix;

        public IEnumerable<Vertex> Bfs(Graph graph, int startId)
        {
            if (!vertices.ContainsKey(startId)) yield break;
            Queue<Vertex> _queue = new Queue<Vertex>();
            HashSet<int> _seen = new HashSet<int>();

            _queue.Enqueue(graph.Vertex(startId));
            _seen.Add(graph.Vertex(startId).id);

            while (_queue.Count > 0)
            {
                Vertex currentVertex = _queue.Dequeue();
                yield return currentVertex;

                foreach(Vertex neighbour in currentVertex.GetNeighboursList())
                {
                    if (_seen.Contains(neighbour.id)) continue;
                    _seen.Add(neighbour.id);
                    _queue.Enqueue(neighbour);
                }
            }
        }

        public Graph()
        {
            countOfVertices = 0;
            vertices = new Dictionary<int, Vertex>();
        }
        public Vertex First(int vertexId)
        {
            if (!vertices.ContainsKey(vertexId)) return null;
            if (vertices[vertexId].NeighborAt(0) != null) return vertices[vertexId].NeighborAt(0);
            else return null;
        }
        public Vertex Next(int vertexId, int i)
        {
            if (vertices[vertexId].NeighborAt(i) != null) return vertices[vertexId].NeighborAt(i);
            else return null;
        }
        public Vertex Vertex(int vertexId)
        {
            return vertices[vertexId];
        }
        public void Add_V(int id) 
        {
            vertices.TryAdd(id, new Vertex(id));
        }
        public void Add_E(int fromId, int toId) 
        {
            Vertex vertexFrom = vertices[fromId];
            Vertex vertexTo = vertices[toId];
            if (!vertexFrom.HasNeighbor(toId)) vertexFrom.AddNeighbour(vertices[toId]);
            if (!vertexTo.HasNeighbor(fromId)) vertexTo.AddNeighbour(vertices[fromId]);
        }
        public void Del_V(int id) 
        {            
            foreach (var vertex in vertices.Values)
            {
                vertex.RemoveNeighbour(vertices[id]);
            }
            vertices.Remove(id);
        }
        public void Del_E(int fromId, int toId) 
        {
            Vertex vertexFrom = vertices[fromId];
            Vertex vertexTo = vertices[toId];

            if (vertexFrom.HasNeighbor(toId)) vertexFrom.RemoveNeighbour(vertexTo);
            if (vertexTo.HasNeighbor(fromId)) vertexTo.RemoveNeighbour(vertexFrom);
        }
        public void Edit_V(int id, int newId) 
        {            
            if (vertices.ContainsKey(id))
            {
                Vertex vertex = vertices[id];
                vertices.Remove(id);
                vertex.id = newId;
                vertices.Add(newId, vertex);
            }
        }
        public void CreateAdjencencyMatrix()
        {
            int n = vertices.Count;
            adjacencyMatrix = new List<List<int>>(n);
            for (int i = 0; i < n; i++)
            {
                adjacencyMatrix.Add(new List<int>(new int[n]));
            }
            var vertexIds = new List<int>(vertices.Keys);
            for (int i = 0; i < vertexIds.Count; i++)
            {
                var vertex = vertices[vertexIds[i]];
                for (int j = 0; j < vertexIds.Count; j++)
                {
                    if (vertex.HasNeighbor(vertexIds[j]))
                    {
                        adjacencyMatrix[i][j] = 1;
                    }
                    else
                    {
                        adjacencyMatrix[i][j] = 0;
                    }
                }
            }
        }
        public void PrintAdjacencyMatrix()
        {
            if (adjacencyMatrix == null || adjacencyMatrix.Count == 0)
            {
                Console.WriteLine("Adjacency matrix is empty. Call CreateAdjencencyMatrix() first.");
                return;
            }

            var vertexIds = new List<int>(vertices.Keys);
            int n = adjacencyMatrix.Count;

            if (vertexIds.Count != n)
            {
                Console.WriteLine("Warning: vertex set changed after building matrix.");
                vertexIds = Enumerable.Range(0, n).ToList();
            }

            // шапка
            Console.Write("    ");
            for (int j = 0; j < n; j++) Console.Write($"{vertexIds[j],3}");
            Console.WriteLine();

            Console.Write("    ");
            for (int j = 0; j < n; j++) Console.Write("---");
            Console.WriteLine();

            // строки
            for (int i = 0; i < n; i++)
            {
                Console.Write($"{vertexIds[i],3}|");
                for (int j = 0; j < n; j++)
                    Console.Write($"{adjacencyMatrix[i][j],3}");
                Console.WriteLine();
            }
        }
        public void Debug()
        {
            foreach (var vertex in vertices)
            {
                Console.Write(vertex.Key + "-> ");
                vertex.Value.SeeNeighbors();
            }
        }
    }
    

    internal class Program
    {
        static void Main(string[] args)
        {
            var graf = new Graph();
            graf.Add_V(1);
            graf.Add_V(2);
            graf.Add_V(3);
            graf.Add_V(4);
            graf.Add_V(5);

            graf.Add_E(1, 2);
            graf.Add_E(1, 3);
            graf.Add_E(5, 1);
            graf.Add_E(5, 4);
            graf.Add_E(3, 4);

            foreach(Vertex v in graf.Bfs(graf, 1))
            {
                Console.WriteLine(v.id);
            }

            //graf.Debug();
            //graf.CreateAdjencencyMatrix();
            //graf.PrintAdjacencyMatrix();

            //Console.WriteLine(graf.Next(1, 2).id);
            //Console.WriteLine(graf.First(1).id);
            //Console.WriteLine(graf.First(2).id);
        }
    }
}