var treesGrid = TreesGrid.Parse("./input.txt");

var result1 = treesGrid.VisibleTrees;
Console.WriteLine(result1);


var result2 = treesGrid.ScenicScores.OfType<int>().Max();
Console.WriteLine(result2);


public class TreesGrid
{
    public int[,] Trees { get; }

    public int[,] ScenicScores => CalculateScenicScores();

    public int NumberOfRows { get; }

    public int NumberOfColumns { get; }

    public int VisibleTrees => CountVisibleTrees();

    public TreesGrid(int[,] treesGrid)
    {
        Trees = treesGrid;
        NumberOfRows = treesGrid.GetLength(0);
        NumberOfColumns = treesGrid.GetLength(1);
    }

    public static TreesGrid Parse(string inputFile)
    {
        var lines = File.ReadAllLines(inputFile);

        var grid = new int[lines[0].Length, lines.Length];

        for (var row = 0; row < grid.GetLength(0); row++)
        {
            for (var column = 0; column < grid.GetLength(1); column++)
            {
                grid[row, column] = lines[row][column] - '0';
            }
        }

        return new TreesGrid(grid);
    }

    private int CountVisibleTrees()
    {
        var edgeTrees = (NumberOfRows * 2) + (NumberOfColumns * 2) - 4;

        var interiorTrees = 0;

        for (var row = 1; row < NumberOfRows - 1; row++)
        {
            for (var column = 1; column < NumberOfColumns - 1; column++)
            {
                if (IsTreeAtCoordinatesVisibleFrom(row, column, Direction.Top)
                    || IsTreeAtCoordinatesVisibleFrom(row, column, Direction.Bottom)
                    || IsTreeAtCoordinatesVisibleFrom(row, column, Direction.Left)
                    || IsTreeAtCoordinatesVisibleFrom(row, column, Direction.Right))
                {
                    interiorTrees++;
                }
            }
        }

        return interiorTrees + edgeTrees;
    }

    private bool IsTreeAtCoordinatesVisibleFrom(int treeRow, int treeColumn, Direction direction)
    {
        return direction switch
        {
            Direction.Top => CheckIfTreeIsVisible(
                loopFromCoordinate: 0,
                loopToCoordinate: treeRow,
                atRow: loopCoordinate => loopCoordinate,
                atColumn: _ => treeColumn),

            Direction.Bottom => CheckIfTreeIsVisible(
                loopFromCoordinate: treeRow + 1,
                loopToCoordinate: NumberOfRows,
                atRow: loopCoordinate => loopCoordinate,
                atColumn: _ => treeColumn),

            Direction.Left => CheckIfTreeIsVisible(
                loopFromCoordinate: 0,
                loopToCoordinate: treeColumn,
                atRow: _ => treeRow,
                atColumn: loopCoordinate => loopCoordinate),

            Direction.Right => CheckIfTreeIsVisible(
                loopFromCoordinate: treeColumn + 1,
                loopToCoordinate: NumberOfColumns,
                atRow: _ => treeRow,
                atColumn: loopCoordinate => loopCoordinate),

            _ => throw new ArgumentException("Invalid direction.", nameof(direction))
        };

        bool CheckIfTreeIsVisible(int loopFromCoordinate, int loopToCoordinate, Func<int, int> atRow, Func<int, int> atColumn)
        {
            for (var coordinate = loopFromCoordinate; coordinate < loopToCoordinate; coordinate++)
            {
                if (Trees[treeRow, treeColumn] <= Trees[atRow(coordinate), atColumn(coordinate)])
                {
                    return false;
                }
            }

            return true;
        }
    }

    private int[,] CalculateScenicScores()
    {
        var scenicScores = new int[NumberOfRows, NumberOfColumns];

        for (var row = 0; row < NumberOfRows; row++)
        {
            for (var column = 0; column < NumberOfColumns; column++)
            {
                scenicScores[row, column] =
                    CalculateViewingDistance(row, column, Direction.Top)
                    * CalculateViewingDistance(row, column, Direction.Bottom)
                    * CalculateViewingDistance(row, column, Direction.Left)
                    * CalculateViewingDistance(row, column, Direction.Right);
            }
        }

        return scenicScores;
    }

    private int CalculateViewingDistance(int treeRow, int treeColumn, Direction direction)
    {
        if (treeRow == 0 || treeColumn == 0 || treeRow == NumberOfRows - 1 || treeColumn == NumberOfColumns)
        {
            return 0;
        }

        var row = treeRow;
        var rowIncrement = direction switch
        {
            Direction.Top => -1,
            Direction.Bottom => 1,
            _ => 0
        };

        var column = treeColumn;
        var columnIncrement = direction switch
        {
            Direction.Left => -1,
            Direction.Right => 1,
            _ => 0
        };

        var viewingDistance = 0;

        while (row > 0 && row < NumberOfRows - 1
            && column > 0 && column < NumberOfColumns - 1)
        {
            viewingDistance++;

            if (Trees[row + rowIncrement, column + columnIncrement] >= Trees[treeRow, treeColumn])
            {
                break;
            }

            row += rowIncrement;
            column += columnIncrement;
        }

        return viewingDistance;
    }

    private enum Direction
    {
        Top,
        Bottom,
        Left,
        Right
    }
}