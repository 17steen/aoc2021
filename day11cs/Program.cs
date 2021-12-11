using System.Diagnostics;

var sample = Parse(@"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526");

var input = Parse(@"8624818384
3725473343
6618341827
4573826616
8357322142
6846358317
7286886112
8138685117
6161124267
3848415383");

int[,] Parse(string input)
{
    var array = new int[10, 10];

    var parsed =
        input
        .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(a =>
            a
            .Select(v => (int)char.GetNumericValue(v))
            .ToArray()
        ).ToArray();

    var rows = parsed.Length;
    var cols = parsed[0].Length;

    var result = new int[rows, cols];
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; ++j)
        {
            result[i, j] = parsed[i][j];
        }
    }

    return result;
}

int Flash(int[,] grid, int i, int j)
{
    var directions = new[]
    {
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1),  (0, 1),
        (1, -1), (1, 0), (1, 1),
    };
    if (grid[i, j] == -1)
        return 0;

    if(grid[i, j] <= 9)
        return 0;

    // flag as flashed
    grid[i, j] = -1;

    var flashCount = 1;

    foreach (var dir in directions)
    {
        var iPrime = i + dir.Item1;
        var jPrime = j + dir.Item2;

        if (Math.Min(iPrime, jPrime) < 0 
            || iPrime >= grid.GetLength(0) 
            || jPrime >= grid.GetLength(1)
            || grid[iPrime, jPrime] == -1)
        {
            continue;
        }

        ++grid[iPrime, jPrime];

        flashCount += Flash(grid, iPrime, jPrime);
    }
    return flashCount;
}

int Step(int[,] grid, out bool allFlashed)
{
    var result = new int[grid.GetLength(0), grid.GetLength(1)];
    var flashCount = 0;


    Debug.Assert(result[0, 0] == 0);
    for (int i = 0; i < grid.GetLength(0); i++)
    {
        for (int j = 0; j < grid.GetLength(1); ++j)
        {
            ++grid[i, j];
        }
    }

    for (int i = 0; i < grid.GetLength(0); i++)
    {
        for (int j = 0; j < grid.GetLength(1); ++j)
        {
            var item = grid[i, j];
            //flaged as flashed, ignore
            if (item != -1)
            {
                flashCount += Flash(grid, i, j);
            }
        }
    }

    //part2
    allFlashed = true;
    for (int i = 0; i < grid.GetLength(0); i++)
    {
        for (int j = 0; j < grid.GetLength(1); ++j)
        {
            if (grid[i, j] == -1)
                grid[i, j] = 0;
            else
                allFlashed = false;
        }
    }

    return flashCount;
}

void Print(int[,] grid)
{
    for (int i = 0; i < grid.GetLength(0); i++)
    {
        Console.Write("[");
        for (int j = 0; j < grid.GetLength(1); ++j)
        {
            Console.Write($"{grid[i,j]}, ");
        }
        Console.WriteLine("]");
    }

}

(int, int) Solve(int[,] grid, int steps)
{
    var flashCount = 0;
    //Print(grid);
    for (int i = 0;; ++i)
    {
        //Console.WriteLine("\n");
        var res = Step(grid, out bool allFlashed);
        if (i < steps)
        {
            flashCount += res;
        }
        if (allFlashed)
        {
            return (flashCount, i + 1);
        }
        //Print(grid);
    }
}

Debug.Assert(Solve(sample, 100) == (1656, 195));
Console.WriteLine(Solve(input, 100));




