using System.Text.RegularExpressions;

var inputLines = File.ReadLines("./input.txt");

var instructions = inputLines
    .Skip(10)
    .Select(line => new Regex(@"\d+")
        .Matches(line))
    .Select(matches => new
    {
        NumberOfCratesToMove = int.Parse(matches[0].Value),
        FromStack = int.Parse(matches[1].Value) - 1,
        ToStack = int.Parse(matches[2].Value) - 1
    });

var part1Stacks = inputLines
    .Take(8)
    .Reverse()
    .Select(line => new Regex(@"\[(\w)\]")
        .Matches(line))
    .SelectMany(matches => matches
        .Select(match => new
        {
            StackIndex = match.Index / 4,
            Crate = match.Groups[1].Value
        }))
    .GroupBy(stackInfo => stackInfo.StackIndex)
    .Select(stackGroup => new Stack<string>(stackGroup.Select(g => g.Crate)))
    .ToArray();

var part2Stacks = part1Stacks
    .Select(stack => new Stack<string>(stack.Reverse()))
    .ToArray();

// TODO: find a better way?
foreach (var instruction in instructions)
{
    var part2Crates = new string[instruction.NumberOfCratesToMove];

    for (var i = 0; i < instruction.NumberOfCratesToMove; i++)
    {
        part1Stacks[instruction.ToStack].Push(part1Stacks[instruction.FromStack].Pop());

        part2Crates[instruction.NumberOfCratesToMove - 1 - i] = part2Stacks[instruction.FromStack].Pop();
    }

    foreach (var crate in part2Crates)
    {
        part2Stacks[instruction.ToStack].Push(crate);
    }
}

var part1Result = part1Stacks
    .Select(stack => stack.Peek())
    .Aggregate((current, next) => current + next);

var part2Result = part2Stacks
    .Select(stack => stack.Peek())
    .Aggregate((current, next) => current + next);

Console.WriteLine(part1Result);
Console.WriteLine(part2Result);