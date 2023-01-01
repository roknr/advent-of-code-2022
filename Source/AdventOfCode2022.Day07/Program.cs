using System.Diagnostics.CodeAnalysis;
using AoCDirectory = AdventOfCode2022.Day07.Directory;
using AoCFile = AdventOfCode2022.Day07.File;

// Lord forgive me, for i have sinned

const int TotalDiskSpace = 70_000_000;
const int UpdateSize = 30_000_000;

var inputLines = File.ReadLines("./input.txt");

var fileSystem = BuildFileSystem(inputLines);

var result1 = SumDirectoriesWithMaxSize(fileSystem, 100_000);
Console.WriteLine(result1);


var unusedSpace = TotalDiskSpace - fileSystem.Size;
var requiredSpace = UpdateSize - unusedSpace;

var result2 = FindMostEfficientDirectoryToDelete(fileSystem, requiredSpace);
Console.WriteLine(result2!.Size);

static int SumDirectoriesWithMaxSize(AoCDirectory directory, int maxSize)
{
    var sum = 0;

    foreach (var subdirectory in directory.Subdirectories)
    {
        if (subdirectory.Size <= maxSize)
        {
            sum += subdirectory.Size;
        }

        sum += SumDirectoriesWithMaxSize(subdirectory, maxSize);
    }

    return sum;
}

static AoCDirectory? FindMostEfficientDirectoryToDelete(AoCDirectory directory, int requiredSpace)
{
    var currentMatch = directory.Size >= requiredSpace
        ? directory
        : null;

    foreach (var subdirectory in directory.Subdirectories)
    {
        var subdirectoryMatch = FindMostEfficientDirectoryToDelete(subdirectory, requiredSpace);

        if (subdirectoryMatch == null)
        {
            continue;
        }

        var acquiredSpaceNew = requiredSpace - subdirectoryMatch.Size;

        var acquiredSpaceCurrent = currentMatch != null
            ? requiredSpace - currentMatch.Size
            : int.MinValue;

        if (acquiredSpaceNew < 0 && acquiredSpaceNew > acquiredSpaceCurrent)
        {
            currentMatch = subdirectoryMatch;
        }
    }

    return currentMatch;
}

static AoCDirectory BuildFileSystem(IEnumerable<string> inputLines)
{
    using var lineEnumerator = inputLines.GetEnumerator();

    if (!lineEnumerator.MoveNext())
    {
        throw new ArgumentException("Invalid input.", nameof(inputLines));
    }

    var root = new AoCDirectory
    {
        Name = lineEnumerator.Current.Split(' ')[2],
        ParentDirectory = null
    };

    var currentDirectory = root;

    var enumerateNext = true;
    while (true)
    {
        if (string.IsNullOrWhiteSpace(lineEnumerator.Current) || (enumerateNext && !lineEnumerator.MoveNext()))
        {
            break;
        }

        enumerateNext = true;

        var (commandType, argument) = ParseCommand(lineEnumerator.Current);

        if (commandType == CommandType.ChangeDirectory)
        {
            if (argument == ".." && currentDirectory.ParentDirectory != null)
            {
                currentDirectory = currentDirectory.ParentDirectory;

                continue;
            }

            var existingDirectory = currentDirectory
                .Subdirectories
                .FirstOrDefault(dir => dir.Name == argument);

            currentDirectory = existingDirectory ?? new AoCDirectory
            {
                Name = argument!,
                ParentDirectory = currentDirectory
            };
        }
        else if (commandType == CommandType.List)
        {
            while (lineEnumerator.MoveNext() && !IsCommand(lineEnumerator.Current))
            {
                if (TryParseDirectory(lineEnumerator.Current, out var directoryName))
                {
                    currentDirectory.Items.Add(new AoCDirectory
                    {
                        Name = directoryName,
                        ParentDirectory = currentDirectory
                    });
                }
                else if (TryParseFile(lineEnumerator.Current, out var file))
                {
                    currentDirectory.Items.Add(file);
                }
            }

            enumerateNext = false;
        }
    }

    return root;
}

static bool IsCommand(string line)
{
    return line.StartsWith('$');
}

static (CommandType commandType, string? argument) ParseCommand(string line)
{
    if (!IsCommand(line))
    {
        throw new ArgumentException("Invalid type of command.", nameof(line));
    }

    var command = line.Split(' ');

    var commandType = command[1] switch
    {
        "cd" => CommandType.ChangeDirectory,
        "ls" => CommandType.List,
        _ => throw new ArgumentException("Invalid type of command.", nameof(line)),
    };

    var argument = command.Length > 2
        ? command[2]
        : null;

    return (commandType, argument);
}

static bool TryParseDirectory(string line, [NotNullWhen(true)] out string? directoryName)
{
    directoryName = null;

    if (!line.StartsWith("dir"))
    {
        return false;
    }

    directoryName = line.Split(' ')[1];

    return true;
}

static bool TryParseFile(string line, [NotNullWhen(true)] out AoCFile? file)
{
    file = null;

    var sizeAndFileName = line.Split(' ');

    if (sizeAndFileName.Length != 2 || !int.TryParse(sizeAndFileName[0], out var size))
    {
        return false;
    }

    file = new AoCFile(size)
    {
        Name = sizeAndFileName[1]
    };

    return true;
}

public enum CommandType
{
    ChangeDirectory,
    List
}