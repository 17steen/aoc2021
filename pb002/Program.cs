using System.Diagnostics;
using System.IO;

var sampleInput = @"forward 5
down 5
forward 8
up 3
down 8
forward 2";


var fileInput = File.ReadAllText("input");

Debug.Assert(Part1(sampleInput) == 150);
Debug.Assert(Part2(sampleInput) == 900);
Console.WriteLine(Part1(fileInput));
Console.WriteLine(Part2(fileInput));


int Part1(string input)
{
    var (horizontal, vertical) = input
        .Split("\n")
        .Select(
            s =>
            {
                var splat = s.Split(" ");
                var operation = splat[0];
                var amount = int.Parse(splat[1]);
                return operation switch
                {
                    "forward" => (amount, 0),
                    "down" => (0, amount),
                    "up" => (0, -amount),
                    _ => throw new ArgumentOutOfRangeException(operation),
                };
            }
        )
        .Aggregate((acc, curr) =>
            (acc.Item1 + curr.Item1, acc.Item2 + curr.Item2)
        );
    return horizontal * vertical;
}

int Part2(string input)
{
    (var horizontal, var vertical, _) = input
        .Split("\n")
        .Aggregate((Horizontal:0, Depth:0, Aim:0),
            (acc, s) =>
            {
                var splat = s.Split(" ");
                var operation = splat[0];
                var amount = int.Parse(splat[1]);
                var (horizontal, aim) = operation switch
                {
                    "forward" => (amount, 0),
                    "down" => (0, amount),
                    "up" => (0, -amount),
                    _ => throw new ArgumentOutOfRangeException(operation),
                };

                var totalAim = acc.Aim + aim;

                return (
                    acc.Horizontal + horizontal,
                    acc.Depth + horizontal * totalAim,
                    totalAim
                );
            }
        );
    return horizontal * vertical;
}