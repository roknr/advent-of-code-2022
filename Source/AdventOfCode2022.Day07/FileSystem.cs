namespace AdventOfCode2022.Day07
{
    public abstract class FileSystemItem
    {
        public required string Name { get; init; }

        public abstract int Size { get; }
    }

    public class File : FileSystemItem
    {
        public override int Size { get; }

        public File(int size)
        {
            Size = size;
        }
    }

    public class Directory : FileSystemItem
    {
        public override int Size => CalculateSize();

        public List<FileSystemItem> Items { get; init; } = new List<FileSystemItem>();

        public required Directory? ParentDirectory { get; init; }

        public IEnumerable<Directory> Subdirectories => Items.OfType<Directory>();

        private int CalculateSize()
        {
            return Items.Sum(item => item.Size);
        }
    }
}
