using System.Drawing;

var rockCoordinates = GetRockCoordinates("./input.txt");

var cave1 = new Cave(rockCoordinates);

var result1 = 0;
while (cave1.ProduceSand())
{
    result1++;
}

Console.WriteLine(result1);


var cave2 = new Cave(rockCoordinates, hasFloor: true);

var result2 = 0;
while (cave2.ProduceSand())
{
    result2++;
}

Console.WriteLine(result2);


static IEnumerable<Point> GetRockCoordinates(string inputFile)
{
    var rockCoordinates = File.ReadLines(inputFile)
        .Select(line => line.Split(" -> "))
        .SelectMany(rockPathCoordinates =>
        {
            using var enumerator = rockPathCoordinates
                .Select(coordinatesString => coordinatesString.Split(","))
                .Select(coordinates => new Point(int.Parse(coordinates[0]), int.Parse(coordinates[1])))
                .GetEnumerator();

            if (!enumerator.MoveNext())
            {
                return Array.Empty<Point>().AsEnumerable();
            }

            var previousPoint = enumerator.Current;

            var path = new List<Point>
            {
                new Point(previousPoint.X, previousPoint.Y)
            };

            while (enumerator.MoveNext())
            {
                var currentPoint = enumerator.Current;

                var x = previousPoint.X;
                var y = previousPoint.Y;

                while (!(x == currentPoint.X && y == currentPoint.Y))
                {
                    x += Math.Sign(currentPoint.X - previousPoint.X);
                    y += Math.Sign(currentPoint.Y - previousPoint.Y);

                    path.Add(new Point(x, y));
                }

                previousPoint = currentPoint;
            }

            return path;
        });

    return rockCoordinates;
}

public class Cave
{
    private readonly Point _sandSource = new(500, 0);
    private readonly HashSet<Point> _obstacles;
    private readonly int _abyssY;
    private readonly int? _floorY;

    public Cave(IEnumerable<Point> rockCoordinates, bool hasFloor = false)
    {
        _obstacles = rockCoordinates.ToHashSet() ?? throw new ArgumentNullException(nameof(rockCoordinates));

        _abyssY = _obstacles.MaxBy(point => point.Y).Y;
        _floorY = hasFloor
            ? _abyssY + 2
            : null;
    }

    public bool ProduceSand()
    {
        return DropSand(_sandSource);
    }

    private bool DropSand(Point fromPoint)
    {
        if (_obstacles.Contains(fromPoint) || IsAbyssReached(fromPoint))
        {
            return false;
        }

        Point? nextPoint = null;
        foreach (var point in GetNextPoints(fromPoint))
        {
            if (CanFallTo(point))
            {
                nextPoint = point;
                break;
            }
        }

        if (nextPoint == null)
        {
            _obstacles.Add(fromPoint);

            return true;
        }

        return DropSand(nextPoint.Value);
    }

    private static IEnumerable<Point> GetNextPoints(Point source)
    {
        source.Y++;
        yield return source;

        source.X--;
        yield return source;

        source.X += 2;
        yield return source;
    }

    private bool CanFallTo(Point destination) => !_obstacles.Contains(destination) && (!_floorY.HasValue || destination.Y < _floorY.Value);

    private bool IsAbyssReached(Point source) => !_floorY.HasValue && source.Y >= _abyssY;
}