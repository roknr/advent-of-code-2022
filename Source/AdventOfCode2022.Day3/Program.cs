var result1 = File.ReadLines("./input.txt")
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Select(line => line[..(line.Length / 2)]
        .Intersect(line[(line.Length / 2)..])
        .Single())
    .Select(c => c >= 'a'
        ? c - ('a' - 1)
        : c - ('A' - 1) + 'z' - 'a' + 1)
    .Sum();

Console.WriteLine(result1);


var result2 = File.ReadLines("./input.txt")
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Chunk(3)
    .Select(group => group[0]
        .Intersect(group[1])
        .Intersect(group[2])
        .Single())
    .Select(c => c >= 'a'
        ? c - ('a' - 1)
        : c - ('A' - 1) + 'z' - 'a' + 1)
    .Sum();

Console.WriteLine(result2);