var part1Result = File.ReadLines("./input.txt")
    .Select(line => line[..(line.Length / 2)]
        .Intersect(line[(line.Length / 2)..])
        .Single())
    .Select(c => c >= 'a'
        ? c - ('a' - 1)
        : c - ('A' - 1) + 'z' - 'a' + 1)
    .Sum();

Console.WriteLine(part1Result);


var part2Result = File.ReadLines("./input.txt")
    .Chunk(3)
    .Select(group => group[0]
        .Intersect(group[1])
        .Intersect(group[2])
        .Single())
    .Select(c => c >= 'a'
        ? c - ('a' - 1)
        : c - ('A' - 1) + 'z' - 'a' + 1)
    .Sum();

Console.WriteLine(part2Result);