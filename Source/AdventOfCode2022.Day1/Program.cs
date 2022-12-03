using AdventOfCode2022.Day1;

var elves = File.ReadLines("./input.txt")
    .SplitBy(string.IsNullOrWhiteSpace)
    .Select((elfLines, iteration) => new
    {
        Index = iteration + 1,
        Calories = elfLines.Select(line => int.Parse(line)).Sum()
    })
    .MaxNBy(3, elf => elf!.Calories);

var totalCalories = 0;

foreach (var elf in elves)
{
    totalCalories += elf.Calories;

    Console.WriteLine($"Elf: {elf!.Index} | Calories: {elf.Calories}");
}

Console.WriteLine($"Total calories: {totalCalories}");
