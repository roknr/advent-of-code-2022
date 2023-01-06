using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

var result1 = File.ReadLines("./input.txt")
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Chunk(2)
    .Select((packetStrings, index) => new
    {
        PairIndex = index + 1,
        Left = Packet.Parse(packetStrings[0]),
        Right = Packet.Parse(packetStrings[1])
    })
    .Where(packets => packets.Left.CompareTo(packets.Right) == -1)
    .Sum(packets => packets.PairIndex);

Console.WriteLine(result1);


var result2 = File.ReadLines("./input.txt")
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Select(Packet.Parse)
    .Append(Packet.Divider(2))
    .Append(Packet.Divider(6))
    .OrderBy(packet => packet)
    .Select((packet, index) => new
    {
        Index = index + 1,
        Packet = packet
    })
    .Where(packetInfo => packetInfo.Packet.IsDivider)
    .Select(packetInfo => packetInfo.Index)
    .Aggregate((previous, current) => previous * current);

Console.WriteLine(result2);


public class Packet : IComparable<Packet>
{
    public PacketDataCollection Data { get; }

    public bool IsDivider { get; }

    private Packet(PacketDataCollection data, bool isDivider = false)
    {
        Data = data;
        IsDivider = isDivider;
    }

    public static Packet Parse(string packetString)
    {
        var packetData = PacketDataCollection.Parse(packetString);

        var packet = new Packet(packetData);

        return packet;
    }

    public static Packet Divider(int value) => new(
        data: new PacketDataCollection
        {
            new PacketDataCollection
            {
                new PacketData(value)
            }
        },
        isDivider: true);

    public int CompareTo(Packet? other)
    {
        return other == null
            ? -1
            : Data.CompareTo(other.Data);
    }
}

public interface IPacketData
    : IComparable<IPacketData>
{

}

public class PacketDataCollection
    : Collection<IPacketData>,
    IPacketData,
    IComparable<PacketDataCollection>,
    IComparable<PacketData>
{
    public PacketDataCollection() { }

    public PacketDataCollection(PacketData packetData) : base(new[] { packetData }) { }

    public static PacketDataCollection Parse(string packetDataCollectionString)
    {
        var packetDataJsonArray = JsonSerializer.Deserialize<JsonArray>(packetDataCollectionString);
        if (packetDataJsonArray == null)
        {
            throw new FormatException("Incorrect format of packet data collection string.");
        }

        var packetDataCollection = Parse(packetDataJsonArray);

        return packetDataCollection;


        static PacketDataCollection Parse(JsonArray packetDataJsonArray)
        {
            var result = new PacketDataCollection();

            foreach (var jsonNode in packetDataJsonArray)
            {
                IPacketData packetData = jsonNode switch
                {
                    JsonArray jsonArray => Parse(jsonArray),
                    JsonValue jsonValue => new PacketData(jsonValue.GetValue<int>()),
                    _ => throw new NotSupportedException("Unsupported type of packet data.")
                };

                result.Add(packetData);
            }

            return result;
        }
    }

    public int CompareTo(IPacketData? other)
    {
        return other switch
        {
            PacketDataCollection packetDataCollection => CompareTo(packetDataCollection),
            PacketData packetData => CompareTo(packetData),
            _ => throw new NotSupportedException("Unsupported type of packet data.")
        };
    }

    public int CompareTo(PacketDataCollection? other)
    {
        if (other == null)
        {
            return -1;
        }

        if (Count > 0 && other.Count == 0)
        {
            return 1;
        }

        for (var i = 0; i < Count; i++)
        {
            var comparison = this[i].CompareTo(other[i]);

            if (comparison == -1)
            {
                return -1;
            }

            if (comparison == 1 || (i == other.Count - 1 && other.Count < Count))
            {
                return 1;
            }
        }

        return Count == other.Count
            ? 0
            : -1;
    }

    public int CompareTo(PacketData? other)
    {
        if (other == null)
        {
            return -1;
        }

        var otherCollection = new PacketDataCollection(other);

        return CompareTo(otherCollection);
    }
}

[DebuggerDisplay("{Data}")]
public class PacketData
    : IPacketData,
    IComparable<PacketDataCollection>,
    IComparable<PacketData>
{
    public int Data { get; }

    public PacketData(int data)
    {
        Data = data;
    }

    public int CompareTo(IPacketData? other)
    {
        return other switch
        {
            PacketDataCollection packetDataCollection => CompareTo(packetDataCollection),
            PacketData packetData => CompareTo(packetData),
            _ => throw new NotSupportedException("Unsupported type of packet data.")
        };
    }

    public int CompareTo(PacketDataCollection? other)
    {
        if (other is null)
        {
            return -1;
        }

        var thisCollection = new PacketDataCollection(this);

        return thisCollection.CompareTo(other);
    }

    public int CompareTo(PacketData? other)
    {
        return other is null || Data < other.Data
            ? -1
            : Data == other.Data
                ? 0
                : 1;
    }
}