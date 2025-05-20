using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class SelectableMesh : MonoBehaviour
{
    [SerializeField] MeshRenderer rend;
    [SerializeField] MeshFilter meshFilter;
    public Material selectedMaterial;
    protected Material originalMaterial;

    public MeshRenderer MeshRenderer => rend;
    public MeshFilter MeshFilter => meshFilter;

    protected virtual void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        if (rend != null)
            originalMaterial = rend.material;

        // GetAllWorldSpaceVertices();
        GetHoleCenters();
    }

    public virtual void Select()
    {
        if (rend != null && selectedMaterial != null)
            rend.material = selectedMaterial;
    }

    public virtual void Deselect()
    {
        if (rend != null && originalMaterial != null)
            rend.material = originalMaterial;
    }

    public abstract void OnClickAction();

    //public void GetAllWorldSpaceVertices()
    //{
    //    Mesh mesh = meshFilter.sharedMesh;
    //    Vector3[] localVertices = mesh.vertices;

    //    foreach (var v in localVertices)
    //        GameManager.Instance.AllVectices.Add(transform.TransformPoint(v));
    //}


    Vector3 GetCenter(List<Vector3> vertices)
    {
        if (vertices == null || vertices.Count == 0)
            return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (var v in vertices)
            sum += v;

        return sum / vertices.Count;
    }

    // 📌 Call this to get hole centers
    public void GetHoleCenters()
    {
        Mesh mesh = meshFilter.sharedMesh;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Dictionary<Edge, int> edgeCount = new Dictionary<Edge, int>();

        // Step 1: Count edges
        for (int i = 0; i < triangles.Length; i += 3)
        {
            AddEdge(edgeCount, triangles[i], triangles[i + 1]);
            AddEdge(edgeCount, triangles[i + 1], triangles[i + 2]);
            AddEdge(edgeCount, triangles[i + 2], triangles[i]);
        }

        // Step 2: Extract boundary edges
        List<Edge> boundaryEdges = edgeCount.Where(e => e.Value == 1).Select(e => e.Key).ToList();

        // Step 3: Group into loops
        List<List<int>> loops = BuildLoops(boundaryEdges);

        // Step 4: Compute centers
        foreach (var loop in loops)
        {
            List<Vector3> worldVerts = loop.Select(i => transform.TransformPoint(vertices[i])).ToList();
            GameManager.Instance.CircleVectices.Add(GetCenter(worldVerts));
        }
    }

    void AddEdge(Dictionary<Edge, int> dict, int a, int b)
    {
        Edge edge = new Edge(Mathf.Min(a, b), Mathf.Max(a, b));
        if (dict.ContainsKey(edge))
            dict[edge]++;
        else
            dict[edge] = 1;
    }

    List<List<int>> BuildLoops(List<Edge> boundaryEdges)
    {
        List<List<int>> loops = new List<List<int>>();
        Dictionary<int, List<int>> adjacency = new Dictionary<int, List<int>>();

        foreach (Edge edge in boundaryEdges)
        {
            if (!adjacency.ContainsKey(edge.a)) adjacency[edge.a] = new List<int>();
            if (!adjacency.ContainsKey(edge.b)) adjacency[edge.b] = new List<int>();

            adjacency[edge.a].Add(edge.b);
            adjacency[edge.b].Add(edge.a);
        }

        HashSet<int> visited = new HashSet<int>();

        foreach (int start in adjacency.Keys)
        {
            if (visited.Contains(start)) continue;

            List<int> loop = new List<int>();
            int current = start;
            int prev = -1;

            while (true)
            {
                loop.Add(current);
                visited.Add(current);

                int next = adjacency[current].FirstOrDefault(n => n != prev);
                if (next == start || next == 0 || visited.Contains(next)) break;

                prev = current;
                current = next;
            }

            if (loop.Count > 2)
                loops.Add(loop);
        }

        return loops;
    }

    struct Edge
    {
        public int a, b;
        public Edge(int a, int b) { this.a = a; this.b = b; }

        public override int GetHashCode() => a.GetHashCode() ^ b.GetHashCode();
        public override bool Equals(object obj)
        {
            if (!(obj is Edge)) return false;
            Edge e = (Edge)obj;
            return a == e.a && b == e.b;
        }
    }
}
