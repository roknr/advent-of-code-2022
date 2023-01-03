using System.Drawing;

var input = ParseInput("./input.txt");

var pathfindDevice = new PathfindDevice(
    input.Heightmap,
    input.Start,
    input.Goal);

var goalNode1 = pathfindDevice.FindShortestPath();
Console.WriteLine(goalNode1?.Distance);


var goalNode2 = GetStartingPointsWithHeight(input.Heightmap, 0)
    .AsParallel()
    .WithDegreeOfParallelism(2)
    .Select(startPoint =>
    {
        var pathfindDevice = new PathfindDevice(
            input.Heightmap,
            startPoint,
            input.Goal);

        return pathfindDevice.FindShortestPath();
    })
    .Where(goalNode => goalNode != null)
    .OrderBy(goalNode => goalNode!.Distance)
    .First();

Console.WriteLine(goalNode2?.Distance);


static (int[,] Heightmap, Point Start, Point Goal) ParseInput(string inputFile)
{
    var lines = File.ReadAllLines(inputFile);

    var heightmap = new int[lines.Length, lines[0].Length];

    var start = Point.Empty;
    var goal = Point.Empty;

    for (var x = 0; x < heightmap.GetLength(0); x++)
    {
        for (var y = 0; y < heightmap.GetLength(1); y++)
        {
            if (lines[x][y] == 'S')
            {
                start.X = x;
                start.Y = y;
                heightmap[x, y] = 'a' - 'a';
            }
            else if (lines[x][y] == 'E')
            {
                goal.X = x;
                goal.Y = y;
                heightmap[x, y] = 'z' - 'a';
            }
            else
            {
                heightmap[x, y] = lines[x][y] - 'a';
            }
        }
    }

    return (heightmap, start, goal);
}

static IEnumerable<Point> GetStartingPointsWithHeight(int[,] heightmap, int height)
{
    for (var x = 0; x < heightmap.GetLength(0); x++)
    {
        for (var y = 0; y < heightmap.GetLength(1); y++)
        {
            if (heightmap[x, y] == height)
            {
                yield return new Point(x, y);
            }
        }
    }
}


public class PathfindDevice
{
    private readonly Node[,] _nodes;
    private readonly Node _goalNode;
    private readonly int _numberOfRows;
    private readonly int _numberOfColumns;
    private readonly PriorityQueue<Node, int> _nodesQueue = new();
    private readonly HashSet<Node> _nodesOpenSet = new();
    private readonly HashSet<Node> _nodesClosedSet = new();

    public PathfindDevice(int[,] heightmap, Point start, Point goal)
    {
        _numberOfRows = heightmap.GetLength(0);
        _numberOfColumns = heightmap.GetLength(1);
        _nodes = BuildNodes(heightmap);
        _goalNode = _nodes[goal.X, goal.Y];

        var startNode = _nodes[start.X, start.Y];
        startNode.Distance = 0;

        _nodesQueue.Enqueue(startNode, 0);
        _nodesOpenSet.Add(startNode);
    }

    public Node? FindShortestPath()
    {
        while (_nodesQueue.Count > 0)
        {
            var processingNode = _nodesQueue.Dequeue();
            if (processingNode == _goalNode)
            {
                return processingNode;
            }

            _nodesOpenSet.Remove(processingNode);
            _nodesClosedSet.Add(processingNode);

            foreach (var neighborNode in GetNeighborNodes(processingNode))
            {
                if (_nodesClosedSet.Contains(neighborNode))
                {
                    continue;
                }

                if (_nodesOpenSet.Contains(neighborNode))
                {
                    var distanceToNeighborNode = processingNode.Distance + 1;

                    if (distanceToNeighborNode < neighborNode.Distance)
                    {
                        neighborNode.Distance = distanceToNeighborNode;
                        neighborNode.Parent = processingNode;
                    }
                }
                else
                {
                    neighborNode.Distance = processingNode.Distance + 1;
                    neighborNode.Parent = processingNode;

                    _nodesOpenSet.Add(neighborNode);
                    _nodesQueue.Enqueue(neighborNode, neighborNode.Distance);
                }
            }
        }

        return null;
    }

    private IEnumerable<Node> GetNeighborNodes(Node node)
    {
        if (node.X > 0
            && _nodes[node.X - 1, node.Y].Height <= _nodes[node.X, node.Y].Height + 1)
        {
            yield return _nodes[node.X - 1, node.Y];
        }

        if (node.X < _numberOfRows - 1
            && _nodes[node.X + 1, node.Y].Height <= _nodes[node.X, node.Y].Height + 1)
        {
            yield return _nodes[node.X + 1, node.Y];
        }

        if (node.Y > 0
            && _nodes[node.X, node.Y - 1].Height <= _nodes[node.X, node.Y].Height + 1)
        {
            yield return _nodes[node.X, node.Y - 1];
        }

        if (node.Y < _numberOfColumns - 1
            && _nodes[node.X, node.Y + 1].Height <= _nodes[node.X, node.Y].Height + 1)
        {
            yield return _nodes[node.X, node.Y + 1];
        }
    }

    private Node[,] BuildNodes(int[,] heightmap)
    {
        var nodes = new Node[_numberOfRows, _numberOfColumns];

        for (var x = 0; x < _numberOfRows; x++)
        {
            for (var y = 0; y < _numberOfColumns; y++)
            {
                nodes[x, y] = new Node
                {
                    X = x,
                    Y = y,
                    Height = heightmap[x, y]
                };
            }
        }

        return nodes;
    }
}

public class Node
{
    public required int X { get; init; }

    public required int Y { get; init; }

    public required int Height { get; init; }

    public int Distance { get; set; } = int.MaxValue;

    public Node? Parent { get; set; }
}