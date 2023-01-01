#region Main

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

var monkeys = File.ReadLines("./input.txt")
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Chunk(6)
    .Select(monkeyLines =>
    {
        var items = monkeyLines[1][18..]
            .Split(", ")
            .Select(x => long.Parse(x));

        var operation = ParseOperation(monkeyLines[2][19..]);
        var throwTo = ParseThrowTo(monkeyLines);
        var divisiblyBy = int.Parse(monkeyLines[3][21..]);

        return new Monkey(items, operation, throwTo, divisiblyBy);
    });

var monkeyKeepAway1 = new MonkeyKeepAway(monkeys);
monkeyKeepAway1.Play(20);

var result1 = monkeyKeepAway1.CalculateMonkeyBusiness();
Console.WriteLine(result1);


var monkeyKeepAway2 = new MonkeyKeepAway(monkeys);

Monkey.CommonDenominator = monkeyKeepAway2.Monkeys
    .Select(x => x.DivisibleBy)
    .Aggregate((previous, current) => previous * current);

monkeyKeepAway2.Play(10_000);

var result2 = monkeyKeepAway2.CalculateMonkeyBusiness();
Console.WriteLine(result2);

#endregion


static Func<long, long> ParseOperation(string operation)
{
    var operationParts = operation.Split(' ');

    var operationParameter = Expression.Parameter(typeof(long), "old");
    var operationResult = Expression.Parameter(typeof(long), "result");

    var leftOperand = ParseOperand(operationParts[0]);
    var rightOperand = ParseOperand(operationParts[2]);

    var binaryOperation = operationParts[1] switch
    {
        "+" => Expression.Add(leftOperand, rightOperand),
        "*" => Expression.Multiply(leftOperand, rightOperand),
        _ => throw new NotSupportedException($"Operation '{operationParts[1]}' is not supported.")
    };

    var resultExpression = Expression.Assign(operationResult, binaryOperation);

    var body = Expression.Block(
        new [] { operationResult },
        resultExpression);

    return Expression
        .Lambda<Func<long, long>>(body, operationParameter)
        .Compile();

    Expression ParseOperand(string operand)
    {
        Expression operandExpression;

        if (long.TryParse(operand, out var leftOperandConstant))
        {
            operandExpression = Expression.Constant(leftOperandConstant);
        }
        else if (operand == "old")
        {
            operandExpression = operationParameter;
        }
        else
        {
            throw new NotSupportedException($"The operand '{operand}' is not supported.");
        }

        return operandExpression;
    }
}

static Func<long, int> ParseThrowTo(string[] lines)
{
    var divisibleBy = int.Parse(lines[3][21..]);
    var onTrue = int.Parse(lines[4][29..]);
    var onFalse = int.Parse(lines[5][30..]);

    Expression <Func<long, int>> throwToExpression =
        item =>
            item % divisibleBy == 0
                ? onTrue
                : onFalse;

    return throwToExpression.Compile();
}

public class Monkey
{
    private readonly Queue<long> _items;
    private readonly Func<long, long> _operation;
    private readonly Func<long, int> _throwTo;

    public static long? CommonDenominator { get; set; }

    public int DivisibleBy { get; }

    public long NumberOfItemsInspected { get; private set; } = 0;

    public Monkey(
        IEnumerable<long> items,
        Func<long, long> operation,
        Func<long, int> throwTo,
        int divisibleBy)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        _items = new Queue<long>(items);

        _operation = operation ?? throw new ArgumentNullException(nameof(operation));
        _throwTo = throwTo ?? throw new ArgumentNullException(nameof(throwTo));
        DivisibleBy = divisibleBy;
    }

    public bool TryThrow([NotNullWhen(true)] out Throw? @throw)
    {
        @throw = null;

        if (!_items.TryDequeue(out var inspectingItem))
        {
            return false;
        }

        inspectingItem = _operation(inspectingItem);
        inspectingItem = ReduceWorryLevel(inspectingItem);

        NumberOfItemsInspected++;

        var throwTo = _throwTo(inspectingItem);

        @throw = new Throw(throwTo, inspectingItem);

        return true;
    }

    public void Catch(long item)
    {
        _items.Enqueue(item);
    }

    private static long ReduceWorryLevel(long item)
    {
        return CommonDenominator.HasValue
            ? item % CommonDenominator.Value
            : (long)Math.Floor(item / 3.0);
    }
}

public class MonkeyKeepAway
{
    public IReadOnlyList<Monkey> Monkeys { get; }

    public MonkeyKeepAway(IEnumerable<Monkey> monkeys)
    {
        if (monkeys == null)
        {
            throw new ArgumentNullException(nameof(monkeys));
        }

        Monkeys = new List<Monkey>(monkeys);
    }

    public void Play(int numberOfRounds)
    {
        for (var round = 1; round <= numberOfRounds; round++)
        {
            foreach (var monkey in Monkeys)
            {
                while (monkey.TryThrow(out var @throw))
                {
                    Monkeys[@throw.ToMonkey].Catch(@throw.Item);
                }
            }
        }
    }

    public long CalculateMonkeyBusiness()
    {
        var monkeyBusiness = Monkeys
            .OrderByDescending(x => x.NumberOfItemsInspected)
            .Select(x => x.NumberOfItemsInspected)
            .Take(2)
            .Aggregate((previous, current) => previous * current);

        return monkeyBusiness;
    }
}

public record Throw(int ToMonkey, long Item);
