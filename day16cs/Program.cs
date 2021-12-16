using Bit = System.Boolean;
using Extensions;
using System.Diagnostics;

var samples = new[] { "C200B40A82", "04005AC33890", "880086C3E88112", "CE00C43D881120", "D8005AC2A8F0", "F600BC2D8F", "9C005AC2F8F0", "9C0141080250320F1802104A08", };
var file = File.ReadLines("input").First();

byte CharToHexValue(char ch) =>
    (byte)(char.ToUpper(ch) switch
    {
        >= '0' and <= '9' => ch - '0',
        >= 'A' and <= 'F' => ch - 'A' + 10,
        _ => throw new ArgumentOutOfRangeException(nameof(ch)),
    });

IEnumerable<Bit> Parse(string input)
{
    foreach (var ch in input)
    {
        var value = CharToHexValue(ch);
        for (int i = 0; i < 4; ++i)
        {
            var v = (value >> (4 - (i + 1))) & 1;
            yield return v == 1;
        }
    }
}

var zzz = string.Join(", ", samples.Select(Parse).Select(Solve));
Console.WriteLine(zzz);
Console.WriteLine(Solve(Parse(file)));


(ulong, ulong) Solve(IEnumerable<Bit> input)
{
    var iterator = input.GetEnumerator();

    ulong version = 0;
    ulong readLength = 0;

    var packetValue = ParsePacket(iterator, ref version, ref readLength);

    return (version, packetValue);
}

ulong ParsePacket(IEnumerator<Bit> iterator, ref ulong versionSum, ref ulong readLength)
{

    var version = iterator
                    .Eat(3)
                    .AsUnsignedInteger();
    versionSum += version;

    var typeId = iterator
                    .Eat(3)
                    .AsUnsignedInteger();
    readLength += 6;

    return typeId switch
    {
        4 => GetLiteralValue(iterator, ref readLength),
        _ => OperatorPacket(iterator, typeId, ref versionSum, ref readLength),
    };
}

ulong OperatorPacket(IEnumerator<Bit> enumerator, ulong type, ref ulong versionNumberSum, ref ulong readLength)
{
    List<ulong> packetValues = new();
    var lengthId = enumerator.Eat();
    ++readLength;
    if (!lengthId)
    {
        var length = enumerator.Eat(15).AsUnsignedInteger();
        readLength += 15;
        var then = readLength;

        while (true)
        {
            var res = ParsePacket(enumerator, ref versionNumberSum, ref readLength);
            packetValues.Add(res);

            if (readLength - then > length)
                throw new Exception($"read too much:len: {length} -> {readLength - then}");
            else if (readLength - then == length)
                break;
            else
                continue;
        }
    }
    else
    {
        var numberOfSubPackets = enumerator.Eat(11).AsUnsignedInteger();
        readLength += 11;
        for (var i = 0UL; i < numberOfSubPackets; ++i)
        {
            var res = ParsePacket(enumerator, ref versionNumberSum, ref readLength);
            packetValues.Add(res);
        }
    }

    switch (type)
    {
        // sum
        case 0:
            return packetValues.Aggregate((acc, c) => acc + c);
        // product
        case 1:
            return packetValues.Aggregate((acc, c) => acc * c);
        // min
        case 2:
            return packetValues.Min();
        // max
        case 3:
            return packetValues.Max();
        // gt
        case 5:
            {
                Debug.Assert(packetValues.Count == 2);
                var a = packetValues.ElementAt(0);
                var b = packetValues.ElementAt(1);
                return a > b ? 1u : 0u;
            }
        // lt
        case 6:
            {
                Debug.Assert(packetValues.Count == 2);

                var a = packetValues.ElementAt(0);
                var b = packetValues.ElementAt(1);
                return a < b ? 1u : 0u;
            }
        // eq
        case 7:
            {
                Debug.Assert(packetValues.Count == 2);

                var a = packetValues.ElementAt(0);
                var b = packetValues.ElementAt(1);
                return a == b ? 1u : 0u;
            }
        default:
            throw new ArgumentOutOfRangeException(type.ToString());
    }

}

ulong GetLiteralValue(IEnumerator<Bit> iterator, ref ulong readLength)
{
    bool continueReading = true;
    var literalValue = 0UL;
    do
    {
        continueReading = iterator.Eat();

        var value = iterator
                    .Eat(4)
                    .AsUnsignedInteger();

        readLength += 5;

        literalValue <<= 4;
        literalValue |= value;

    } while (continueReading);

    // not useful 

    return literalValue;
}

namespace Extensions
{
    public static class Extensions
    {
        public static T Eat<T>(this IEnumerator<T> enumerator)
        {
            if (!enumerator.MoveNext())
            {
                throw new ArgumentOutOfRangeException("nothing more to iterate over");
            }
            return enumerator.Current;
        }

        // careful, only get eaten once iterated over
        public static IEnumerable<T> Eat<T>(this IEnumerator<T> enumerator, ulong count)
        {
            for (var i = 0UL; i < count; ++i)
            {
                yield return enumerator.Eat();
            }
        }
        public static void NoOp<T>(this IEnumerable<T> enumerable)
        {
            foreach (var _ in enumerable) { }
        }

        public static ulong AsUnsignedInteger(this IEnumerable<Bit> bits)
        {
            var acc = 0u;
            foreach (var bit in bits)
            {
                var value = bit ? 1u : 0u;
                acc <<= 1;
                acc |= value;
            }

            return acc;
        }

    }

}

