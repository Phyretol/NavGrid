using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class NavGrid {
    public const float resolution = 1;

    public Vector2[][] points;
    public bool[][] walkable;

    private float width;
    private float height;
    private int w;
    private int h;
    
    public NavGrid(float width, float height) {
        this.width = width;
        this.height = height;
        w = (int)(width / resolution);
        h = (int)(height / resolution);
        Init();
    }

    private void Init() {
        float y = height / 2;
        points = new Vector2[h][];
        walkable = new bool[h][];
        for(int i=0; i<h; i++) {
            float x = -width / 2;
            points[i] = new Vector2[w];
            walkable[i] = new bool[w];
            for(int j=0; j<w; j++) {
                points[i][j] = new Vector2(x, y);
                walkable[i][j] = true;
                x += resolution;
            }
            y -= resolution;
        }
    }

    public void Clear() {
        for(int i=0; i<h; i++) {
            for (int j = 0; j < w; j++)
                walkable[i][j] = true;
        }
    }

    public void MarkObstacle(Shape shape) {
        Rectangle rect = shape.BoundingBox();
        Vector2 leftTop = new Vector2(rect.left, rect.top);
        Vector2 rightBottom = new Vector2(rect.right, rect.bottom);
        int l = GetClosestNode(leftTop).j;
        int r = GetClosestNode(rightBottom).j;
        int b = GetClosestNode(rightBottom).i;
        int t = GetClosestNode(leftTop).i;

        for(int i=t; i<=b; i++) {
            for(int j=l; j<=r; j++) {
                if (shape.Contains(points[i][j]))
                    walkable[i][j] = false;
            }
        }
    }

    private GridCoords GetClosestNode(Vector2 postion) {
        int x = (int)Math.Round((postion.x + width / 2) / resolution);
        int y = (int)Math.Round((postion.y) / resolution);
        y = h / 2 - y;
        GridCoords coords = new GridCoords(y, x);
        return coords;
    }

    private struct GridCoords {
        public int i;
        public int j;

        public GridCoords(int i, int j) {
            this.i = i;
            this.j = j;
        }

        public Vector2 Point(Vector2[][] points) {
            return points[i][j];
        }

        public bool Walkable(bool[][] walkable) {
            return walkable[i][j];
        }
    }

    private class PathNode : IComparable {
        public PathNode previous;
        public GridCoords coords;
        public float g;
        public float f;

        public PathNode(GridCoords coords, float f) {
            this.coords = coords;
            this.f = f;
            previous = null;
            g = 0;
        }

        public PathNode(GridCoords coords, float f, float g, PathNode previous) {
            this.coords = coords;
            this.previous = previous;
            this.f = f;
            this.g = g;
        }

        public int CompareTo(object obj) {
            PathNode pathNode = (PathNode)obj;
            return -(f.CompareTo(pathNode.f));
        }

        public static bool operator ==(PathNode n1, PathNode n2) {
            if (ReferenceEquals(n1, null)) {
                return ReferenceEquals(n2, null);
            }
            if (ReferenceEquals(n2, null))
                return false;
            return n1.coords.Equals(n2.coords);
        }

        public static bool operator !=(PathNode n1, PathNode n2) {
            if(ReferenceEquals(n1, null)) {
                return !ReferenceEquals(n2, null);
            }
            if (ReferenceEquals(n2, null)) 
                return true;
            return !n1.coords.Equals(n2.coords);
        }

        public override int GetHashCode() {
            return coords.GetHashCode();
        }

        public override bool Equals(object obj) {
            PathNode node = (PathNode)obj;
            return coords.Equals(node.coords);
        }
    }

    public List<Vector2> Path(Vector2 position, Vector2 destination) {
        GridCoords s = GetClosestNode(position);
        GridCoords e = GetClosestNode(destination);

        PriorityQueue<PathNode> queue = new PriorityQueue<PathNode>();
        HashSet<PathNode> closedSet = new HashSet<PathNode>();

        queue.Add(new PathNode(s, points[s.i][s.j].Distance(destination)));
        PathNode node;
        do {
            node = queue.Poll();
            List<PathNode> nodes = Discover(node, destination);
            foreach (PathNode n in nodes) {
                if (closedSet.Contains(n))
                    continue;
                if (queue.TryShow(n, out PathNode current)) {
                    if (n.g < current.g)
                        queue.Update(n);
                } else
                    queue.Add(n);
            }
            closedSet.Add(node);
        } while (!node.coords.Equals(e) && queue.Count() > 0);
        bool reachable = node.coords.Equals(e);
        List<Vector2> path = new List<Vector2>();
        while(node != null) {
            int i = node.coords.i;
            int j = node.coords.j;
            path.Add(points[i][j]);
            node = node.previous;
        }
        path.Reverse();
        if (reachable)
            path.Add(destination);
        return path;
    }

    private List<PathNode> Discover(PathNode node, Vector2 destination) {
        int i = node.coords.i;
        int j = node.coords.j;
        List<PathNode> nodes = new List<PathNode>();

        TestAdd(new GridCoords(i - 1, j), nodes, destination, node);
        TestAdd(new GridCoords(i + 1, j), nodes, destination, node);
        TestAdd(new GridCoords(i, j - 1), nodes, destination, node);
        TestAdd(new GridCoords(i, j + 1), nodes, destination, node);
        TestAdd(new GridCoords(i + 1, j + 1), nodes, destination, node);
        TestAdd(new GridCoords(i - 1, j - 1), nodes, destination, node);
        TestAdd(new GridCoords(i + 1, j - 1), nodes, destination, node);
        TestAdd(new GridCoords(i - 1, j + 1), nodes, destination, node);
        return nodes;
    }

    private void TestAdd(GridCoords coords, List<PathNode> nodes, Vector2 destination, PathNode node) {
        int i = coords.i;
        int j = coords.j;
        if (i < 0 || i >= h || j < 0 || j >= w)
            return;
        if (node.previous != null && coords.Equals(node.previous.coords))
            return;
        if (walkable[i][j]) {
            int ni = node.coords.i;
            int nj = node.coords.j;
            float g = node.g + points[i][j].Distance(points[ni][nj]);
            float h = points[i][j].Distance(destination);
            float f = g + h;
            nodes.Add(new PathNode(coords, f, g, node));
        }
    }
}
    