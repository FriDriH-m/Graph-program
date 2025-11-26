using System;
using System.Collections.Generic;

namespace MyApp
{
    public static class BfsIterator
    {
        public static IEnumerable<int> Bfs(Graph g, int start)
        {
            if (start < 0 || start >= g.Count) yield break;

            var seen = new bool[g.Count];
            var q = new Queue<int>();

            q.Enqueue(start);
            seen[start] = true;

            while (q.Count > 0)
            {
                int v = q.Dequeue();

                yield return v;

                for (int u = g.First(v); u != -1; u = g.Next(v, u))
                {
                    if (u < 0 || u >= g.Count) continue;
                    if (!seen[u])
                    {
                        seen[u] = true;
                        q.Enqueue(u);
                    }
                }
            }
        }
    }

    public class Graph
    {
        private List<List<int>> adjacencyMatrix;
        public int Count => adjacencyMatrix.Count;

        public Graph()
        {
            adjacencyMatrix = new List<List<int>>();
        }

        public int First(int v)
        {
            if (v < 0 || v >= adjacencyMatrix.Count) return -1;
            for (int j = 0; j < adjacencyMatrix.Count; j++)
            {
                if (adjacencyMatrix[v][j] == 1) return j;
            }
            return -1;
        }

        public int Next(int v, int i)
        {
            if (v < 0 || v >= adjacencyMatrix.Count) return -1;
            for (int j = i + 1; j < adjacencyMatrix.Count; j++)
                if (adjacencyMatrix[v][j] == 1) return j;
            return -1;
        }

        public int Vertex(int v, int k)
        {
            if (v < 0 || v >= adjacencyMatrix.Count) return -1;
            int c = 0;
            for (int j = 0; j < adjacencyMatrix.Count; j++)
            {
                if (adjacencyMatrix[v][j] == 1)
                {
                    if (c == k) return j;
                    c++;
                }
            }
            return -1;
        }

        public void Add_V()
        {
            for (int i = 0; i < adjacencyMatrix.Count; i++)
            {
                adjacencyMatrix[i].Add(0);
            }
            var list = new List<int>(adjacencyMatrix.Count + 1);
            for (int i = 0; i < adjacencyMatrix.Count + 1; i++)
            {
                list.Add(0);
            }
            adjacencyMatrix.Add(list);
        }

        public void Add_E(int fromId, int toId)
        {
            if (fromId < 0 || toId < 0) return;
            if (fromId >= adjacencyMatrix.Count) return;
            if (toId >= adjacencyMatrix[fromId].Count) return;

            adjacencyMatrix[fromId][toId] = 1;
            adjacencyMatrix[toId][fromId] = 1;
        }

        public void Del_V(int id)
        {
            if (id < 0 || id >= adjacencyMatrix.Count) return;

            for (int i = 0; i < adjacencyMatrix.Count; i++)
            {
                adjacencyMatrix[i].RemoveAt(id);
            }

            adjacencyMatrix.RemoveAt(id);
        }

        public void Del_E(int fromId, int toId)
        {
            if (fromId < 0 || toId < 0) return;
            if (fromId >= adjacencyMatrix.Count) return;
            if (toId >= adjacencyMatrix[fromId].Count) return;

            adjacencyMatrix[fromId][toId] = 0;
            adjacencyMatrix[toId][fromId] = 0;
        }

        public void Edit_V(int id, int newId)
        {
            if (id < 0 || newId < 0) return;
            if (id >= adjacencyMatrix.Count || newId >= adjacencyMatrix.Count) return;
            if (id == newId) return;

            var tmpRow = adjacencyMatrix[id];
            adjacencyMatrix[id] = adjacencyMatrix[newId];
            adjacencyMatrix[newId] = tmpRow;

            for (int i = 0; i < adjacencyMatrix.Count; i++)
            {
                int tmp = adjacencyMatrix[i][id];
                adjacencyMatrix[i][id] = adjacencyMatrix[i][newId];
                adjacencyMatrix[i][newId] = tmp;
            }
        }

        public void PrintAdjacencyMatrix()
        {
            Console.Write("    ");
            for (int i = 0; i < adjacencyMatrix.Count; i++)
            {
                Console.Write((i) + " ");
            }
            Console.WriteLine();
            int j = 0;
            foreach (var vertex in adjacencyMatrix)
            {
                Console.Write(j + " | ");
                j++;
                if (vertex.Count > 0)
                {
                    foreach (var edge in vertex)
                    {
                        Console.Write(edge + " ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
    public static class CycleCounterBfs
    {
        public static List<List<int>> FindAllCycles(Graph g)
        {
            int n = g.Count;
            var cyclesSet = new HashSet<string>();
            var cycles = new List<List<int>>();

            for (int start = 0; start < n; start++)
            {
                var q = new Queue<List<int>>();
                q.Enqueue(new List<int> { start });

                while (q.Count > 0)
                {
                    var path = q.Dequeue();
                    int v = path[path.Count - 1]; 

                    for (int u = g.First(v); u != -1; u = g.Next(v, u))
                    {
                        if (u == start && path.Count >= 3)
                        {
                            AddCycle(path, cyclesSet, cycles);
                        }
                        else if (!path.Contains(u))
                        {
                            var newPath = new List<int>(path);
                            newPath.Add(u);
                            q.Enqueue(newPath);
                        }
                    }
                }
            }

            return cycles;
        }

        private static void AddCycle(List<int> path, HashSet<string> cyclesSet, List<List<int>> cycles)
        {
            int k = path.Count;
            var forward = path.ToArray();

            var backward = new int[k];
            for (int i = 0; i < k; i++)
                backward[i] = forward[k - 1 - i];

            var canonForward = MakeMinFirst(forward);
            var canonBackward = MakeMinFirst(backward);

            string keyForward = string.Join("-", canonForward);
            string keyBackward = string.Join("-", canonBackward);

            string key = string.CompareOrdinal(keyForward, keyBackward) <= 0
                ? keyForward
                : keyBackward;

            if (cyclesSet.Add(key))
            {
                cycles.Add(new List<int>(canonForward));
            }
        }

        private static int[] MakeMinFirst(int[] cycle)
        {
            int k = cycle.Length;
            int min = cycle[0];
            int minIndex = 0;

            for (int i = 1; i < k; i++)
            {
                if (cycle[i] < min)
                {
                    min = cycle[i];
                    minIndex = i;
                }
            }

            var res = new int[k];
            for (int i = 0; i < k; i++)
            {
                res[i] = cycle[(i + minIndex) % k];
            }
            return res;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var graf = new Graph();
            graf.Add_V();
            graf.Add_V();
            graf.Add_V();
            graf.Add_V();
            graf.Add_V();
            graf.Add_V();

            graf.Add_E(0, 1);
            graf.Add_E(0, 2);
            graf.Add_E(0, 3);
            graf.Add_E(1, 3);
            graf.Add_E(2, 4);
            graf.Add_E(3, 4);
            graf.Add_E(3, 5);
            graf.Add_E(4, 5);

            Console.WriteLine("Матрица смежности:");
            graf.PrintAdjacencyMatrix();
            Console.WriteLine("\n-----------------------------------------");
            Console.WriteLine("Обход графа в ширину (Вывод 1):");
            foreach (var v in BfsIterator.Bfs(graf, 0))
            {
                Console.Write(v + " ");
            }
            Console.WriteLine("\n-----------------------------------------");
            Console.WriteLine("Обход графа в ширину (Вывод 2):");
            foreach (var v in BfsIterator.Bfs(graf, 1))
            {
                Console.Write(v + " ");
            }
            Console.WriteLine("\n-----------------------------------------");
            Console.WriteLine("Обход графа в ширину (Вывод 3):");
            foreach (var v in BfsIterator.Bfs(graf, 3))
            {
                Console.Write(v + " ");
            }
            Console.WriteLine("\n-----------------------------------------");

            var cycles = CycleCounterBfs.FindAllCycles(graf);
            Console.WriteLine("Все циклы:");
            foreach (var cycle in cycles)
            {
                for (int i = 0; i < cycle.Count; i++)
                {
                    Console.Write(cycle[i] + " ");
                }
                Console.Write(cycle[0]);
                Console.WriteLine();
            }
            Console.WriteLine("Общее количество циклов: " + cycles.Count);
            Console.ReadLine();
        }
    }
}
