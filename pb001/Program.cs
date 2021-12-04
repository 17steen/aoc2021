using System.Diagnostics;

var sampleInput = @"199
200
208
210
200
207
240
269
260
263";
Debug.Assert(Part1(Parse(sampleInput)) == 7);
Debug.Assert(Part2(Parse(sampleInput)) == 5);

var inputText = File.ReadAllText("input");
Console.WriteLine(Part1(Parse(inputText)));
Console.WriteLine(Part2(Parse(inputText)));

int[] Parse(string input) =>
    input
        .Split("\n")
        .Select(int.Parse)
        .ToArray();

int Part1(int[] depths) =>
    depths
        .Zip(
            depths.Skip(1),
            (a, b) => b > a
        )
        .Count(x => x);

int Part2(int[] depths) =>
    Part1(
        depths
            .Zip(
                depths.Skip(1),
                depths.Skip(2)
            )
            .Select(t => t.First + t.Second + t.Third)
            .ToArray()
    );