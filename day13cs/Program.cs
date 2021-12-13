// See https://aka.ms/new-console-template for more information
var sample = @"6,10
0,14
9,10
0,3
10,4
4,11
6,0
6,12
4,1
0,13
10,12
3,4
3,0
8,4
1,10
2,14
8,10
9,0

fold along y=7
fold along x=5";

int Solve(string input)
{
    var splat =
        input
        .Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        .Select(str => str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));

    var dots = splat.ElementAt(0).Select(str =>
    {
        var nums = str.Split(",").Select(int.Parse);
        return (x: nums.ElementAt(0), y: nums.ElementAt(1));
    });

    var maxX = dots.Select(thing => thing.x).Max();
    var maxY = dots.Select(thing => thing.y).Max();

    var grid = new bool[maxX + 1, maxY + 1];

    Console.WriteLine($"max: {maxX},{maxY}");

    foreach (var (x, y) in dots)
    {
        Console.WriteLine($"{x},{y}");
        grid[x, y] = true;
    }

    //Print(grid);

    var folds = splat
        .ElementAt(1)
        .Select(
            fold =>
            {
                var splat = fold.Substring("fold along ".Length).Split("=");
                var axis = splat[0];
                var idx = int.Parse(splat[1]);
                return (axis, idx);
            }
    );

    int? part1 = null;

    foreach (var (axis, idx) in folds)
    {

        if (axis == "y")
        {
            maxY = idx - 1;
        }
        else
        {
            maxX = idx - 1;
        }
        var folded = new bool[maxX + 1, maxY + 1];

        // copy
        for (var x = 0; x < folded.GetLength(0); ++x)
        {
            for (var y = 0; y < folded.GetLength(1); ++y)
            {
                folded[x, y] = grid[x, y];
            }
        }

        if (axis == "y")
        {
            for (var x = 0; x < folded.GetLength(0); ++x)
            {
                for (var y = 0; y < folded.GetLength(1); ++y)
                {

                    var opposite = (idx * 2) - y;
                    folded[x, y] |= grid[x, opposite];
                }
            }
        }
        else
        {
            for (var x = 0; x < folded.GetLength(0); ++x)
            {
                for (var y = 0; y < folded.GetLength(1); ++y)
                {
                    var opposite = (idx * 2) - x;
                    folded[x, y] |= grid[opposite, y];
                }
            }
        }
        //part1
        if(part1 is null)
        {
            part1 = 0;
            foreach(var dot in folded)
            {
                part1 += dot ? 1 : 0;
            }
        }

        grid = folded;
    }
    //part2 has to be interpreted by a human
    Print(grid);
    return part1.GetValueOrDefault();
}

void Print(bool[,] grid)
{
    for (int y = 0; y < grid.GetLength(1); ++y)
    {
        for (int x = 0; x < grid.GetLength(0); ++x)
        {
            if (grid[x, y])
            {
                Console.Write("#");
            }
            else
            {
                Console.Write(".");
            }
        }
        Console.WriteLine();
    }
}

var file = System.IO.File.ReadAllText("input");

Console.WriteLine(Solve(sample));
Console.WriteLine(Solve(file));