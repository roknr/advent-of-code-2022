var part1Result = File.ReadLines("./input.txt")
    .Select(line => line
        .Split(',')
        .SelectMany(pair => pair.Split('-'))
        .Select(value => int.Parse(value))
        .ToArray())
    .Count(pair =>
        (pair[0] >= pair[2] && pair[1] <= pair[3])
        || (pair[2] >= pair[0] && pair[3] <= pair[1]));

Console.WriteLine(part1Result);


var part2Result = File.ReadLines("./input.txt")
    .Select(line => line
        .Split(',')
        .SelectMany(pair => pair.Split('-'))
        .Select(value => int.Parse(value))
        .ToArray())
    .Count(pair => !(pair[1] < pair[2] || pair[0] > pair[3]));

Console.WriteLine(part2Result);
