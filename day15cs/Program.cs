using System.Diagnostics;

var sampleGrid = Parse(
    @"1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581"
        .Split("\n") // has to be changed for windows encoding
);

var simple = Parse(
    @"1163
1381
3658
3694".Split("\n") // has to be changed for windows encoding
);

var test = Parse(
    @"00
00".Split("\n")
);

var file = Parse(File.ReadAllLines("input"));
//Console.WriteLine(Solve(test, 10));
Console.WriteLine(Solve(sampleGrid));
//Console.WriteLine(Solve(file)); // doesn't work, because i don't allow it to go up



(int, int) Solve(byte[,] grid1, int repeat = 5)
{
    var xLen = grid1.GetLength(0);
    var yLen = grid1.GetLength(1);

    var cache1 = new int?[xLen, yLen];

    var grid2   = new byte[xLen * repeat, yLen * repeat];
    var cache2  = new int?[xLen * repeat, yLen * repeat];

    for (var y = 0; y < yLen * repeat; ++y)
    {
        for (var x = 0; x < xLen * repeat; ++x)
        {
            var addition = x / xLen + y / yLen;
            int now =  grid1[x % xLen, y % yLen] + addition;
            if( now > 9)
            {
                now = (now - 1) % 9 + 1;
            }
            grid2[x, y] = (byte) now;
        }
    }

    Print(grid2);

    var part1 = MinPath(grid1, cache1, new()) - grid1[0, 0];
    var part2 = MinPath(grid2, cache2, new()) - grid2[0, 0];

    return (part1, part2);
}


void Print<T>(T[,] xyGrid, IEnumerable<(int, int)>? points = null)
{
    var xLen = xyGrid.GetLength(0);
    var yLen = xyGrid.GetLength(1);
    Console.WriteLine("Grid:");
    for (var y = 0; y < yLen; ++y)
    {
        for (var x = 0; x < xLen; ++x)
        {
            if (points?.Contains((x, y))??false)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(xyGrid[x, y]);
                Console.ResetColor();
            }
            else
            {
                Console.Write(xyGrid[x, y]);
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

int MinPath(byte[,] grid, int?[,] cache, Stack<(int, int)> alreadyVisited, int x = 0, int y = 0)
{
    var xLen = grid.GetLength(0);
    var yLen = grid.GetLength(1);


    if (Math.Min(x, y) < 0 || x >= xLen || y >= yLen)
        return int.MaxValue;


    int current = grid[x, y];

    if (cache[x, y] is int value)
    {
        return value;
    }

    if ((xLen, yLen) == (x + 1, y + 1)) // end cell
    {
        cache[x, y] = current;
        return current;
    }


    var directions = new[]
    {
        (x: 1, y: 0),
        (x: 0, y: 1),
        //(x: -1, y: 0), because i don't allow it to go upwards, can't find paths
        //(x: 0, y: -1),
     };


    var min = int.MaxValue - 100000; //overflow maybe?

    alreadyVisited.Push((x, y));
    foreach (var dir in directions)
    {
        var newX = x + dir.x;
        var newY = y + dir.y;

        if (alreadyVisited.Contains((newX, newY)))
            continue;

        //*/
        var next = MinPath(grid, cache, alreadyVisited, newX, newY);
        //*/

        min = Math.Min(min, next);
    }
    alreadyVisited.Pop();

    var result = current + min;

    cache[x, y] = result;

    return result;
}


byte[,] Parse(string[] input)
{
    var xLen = input[0].Length;
    var yLen = input.Length;

    var grid = new byte[xLen, yLen];

    var lol = char.GetNumericValue('¾');
    Debug.Assert(lol == 0.75);

    for (var y = 0; y < yLen; ++y)
    {
        for (var x = 0; x < xLen; ++x)
        {
            grid[x, y] = (byte)char.GetNumericValue(input[y][x]);
        }
    }

    return grid;
}