var result1 = CalculateNumberOfTailVisitedPoints(2);
Console.WriteLine(result1);

var result2 = CalculateNumberOfTailVisitedPoints(10);
Console.WriteLine(result2);


static int CalculateNumberOfTailVisitedPoints(int numberOfKnots)
{
    var tailVisitedPoints = new HashSet<Point>
    {
        Point.Empty
    };

    var knotPoints = Enumerable
        .Range(0, numberOfKnots)
        .Select(_ => Point.Empty)
        .ToArray();

    foreach (var headPoint in GetHeadVisitedPoints())
    {
        knotPoints[0] = headPoint;

        for (var i = 1; i < knotPoints.Length; i++)
        {
            if (CalculatePointDistance(knotPoints[i - 1], knotPoints[i]) <= Constants.SQRT_2)
            {
                break;
            }

            var newX = (knotPoints[i - 1].X + knotPoints[i].X) / 2f;
            var newY = (knotPoints[i - 1].Y + knotPoints[i].Y) / 2f;

            knotPoints[i].X = Math.Sign(knotPoints[i - 1].X - knotPoints[i].X) == -1
                ? (int)Math.Floor(newX)
                : (int)Math.Ceiling(newX);

            knotPoints[i].Y = Math.Sign(knotPoints[i - 1].Y - knotPoints[i].Y) == -1
                ? (int)Math.Floor(newY)
                : (int)Math.Ceiling(newY);
        }

        tailVisitedPoints.Add(knotPoints.Last());
    }

    return tailVisitedPoints.Count;
}

static double CalculatePointDistance(Point p1, Point p2)
{
    return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
}

static IEnumerable<Point> GetHeadVisitedPoints()
{
    var previousPoint = Point.Empty;

    return File.ReadLines("./input.txt")
        .Select(line => new
        {
            Direction = line[0],
            Count = int.Parse(line[2..])
        })
        .SelectMany(step => Enumerable
            .Range(0, step.Count)
            .Select(_ =>
            {
                var xIncrement = step.Direction switch
                {
                    'L' => -1,
                    'R' => 1,
                    _ => 0
                };

                var yIncrement = step.Direction switch
                {
                    'U' => 1,
                    'D' => -1,
                    _ => 0
                };

                var point = new Point(previousPoint.X + xIncrement, previousPoint.Y + yIncrement);

                previousPoint = point;

                return point;
            }));
}

public record Point(int X, int Y)
{
    public int X { get; set; } = X;

    public int Y { get; set; } = Y;

    public static Point Empty => new(0, 0);
}

public static class Constants
{
    public static readonly double SQRT_2 = Math.Sqrt(2);
}