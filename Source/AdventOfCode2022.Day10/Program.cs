const string FullPixel = "█";
const string EmptyPixel = " ";

var result1 = 0;
var cycle = 1;
var xRegister = 1;
var addInstructions = new Queue<int>();

using var instructionsEnumerator = File.ReadLines("./input.txt").GetEnumerator();

while (instructionsEnumerator.MoveNext() || addInstructions.Count > 0)
{
    if (instructionsEnumerator.Current != null)
    {
        if (instructionsEnumerator.Current[..4] == "noop")
        {
            addInstructions.Enqueue(0);
        }
        else
        {
            addInstructions.Enqueue(0);
            addInstructions.Enqueue(int.Parse(instructionsEnumerator.Current[5..]));
        }
    }

    var pixel = (cycle - 1) % 40;
    if (xRegister - 1 <= pixel && pixel <= xRegister + 1)
    {
        Console.Write(FullPixel);
    }
    else
    {
        Console.Write(EmptyPixel);
    }

    if (cycle % 40 == 0)
    {
        Console.WriteLine();
    }

    if ((cycle - 20) % 40 == 0)
    {
        result1 += cycle * xRegister;
    }

    xRegister += addInstructions.Dequeue();

    cycle++;
}

Console.WriteLine();
Console.WriteLine(result1);
