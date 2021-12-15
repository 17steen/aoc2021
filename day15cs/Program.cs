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
var bade = Parse(File.ReadAllLines("input_bade"));
Console.WriteLine(Solve(sampleGrid)); 
Console.WriteLine(Solve(file)); 
Console.WriteLine(Solve(bade)); 


(int, int) Solve(byte[,] grid, int repeat = 5)
{
    var xLen = grid.GetLength(0);
    var yLen = grid.GetLength(1);

    var grid2 = new byte[xLen * repeat, yLen * repeat];

    for (var y = 0; y < yLen * repeat; ++y)
    {
        for (var x = 0; x < xLen * repeat; ++x)
        {
            var addition = x / xLen + y / yLen;
            int now = grid[x % xLen, y % yLen] + addition;

            now = (now - 1) % 9 + 1;

            grid2[x, y] = (byte)now;
        }
    }

    var part1 = MinPathSteen(new Grid<byte>(grid), out var path);
    Print(new Grid<byte>(grid), path);

    var part2 = MinPathSteen(new VirtuallyRepeatedGrid(grid, repeat), out var _dontcare);

    var part2unbased = MinPathSteen(new Grid<byte>(PhysicallyRepeatGrid(grid, repeat)), out var _dontcareaswell);
    return (part1, part2);
}

byte[,] PhysicallyRepeatGrid(byte[,] grid, int repeat) {
    var xLen = grid.GetLength(0);
    var yLen = grid.GetLength(1);

    var grid2 = new byte[xLen * repeat, yLen * repeat];

    for (var y = 0; y < yLen * repeat; ++y)
    {
        for (var x = 0; x < xLen * repeat; ++x)
        {
            var addition = x / xLen + y / yLen;
            int now = grid[x % xLen, y % yLen] + addition;

            now = (now - 1) % 9 + 1;

            grid2[x, y] = (byte)now;
        }
    }

    return grid2;
}

void Print<T>(I2DReadOnlyIndexable<T> xyGrid, IEnumerable<(int, int)>? points = null)
{
    var xLen = xyGrid.GetLength(0);
    var yLen = xyGrid.GetLength(1);
    Console.WriteLine("Grid:");
    for (var y = 0; y < yLen; ++y)
    {
        for (var x = 0; x < xLen; ++x)
        {
            if (points?.Contains((x, y)) ?? false)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.Write(xyGrid[x, y]);
            Console.ResetColor();
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

//doo doo
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
        (1, 0),
        (0, 1),
     }.Select(dir => (dir.Item1 + x, dir.Item2 + y));


    var min = int.MaxValue - 100000; //overflow maybe?

    alreadyVisited.Push((x, y));
    foreach (var (newX, newY) in directions)
    {
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
int MinPathSteen(I2DReadOnlyIndexable<byte> grid, out List<(int x, int y)> path)  
{
    var queue = new PriorityQueue<(int x, int y), int>();

    var xLen = grid.GetLength(0);
    var yLen = grid.GetLength(1);

    var distances = new int[xLen, yLen];
    var prev = new (int x, int y)?[xLen, yLen];

    for (var x = 0; x < xLen; ++x)
    {
        for (var y = 0; y < yLen; ++y)
        {
            distances[x, y] = int.MaxValue;
            prev[x, y] = null;
            queue.Enqueue((x, y), distances[x,y]);
        }
    }
    distances[0, 0] = 0;

    while (queue.Count > 0)
    {
        var min = queue.Dequeue();
        var neighbours = new(int x, int y)[] { (1, 0), (0, 1), (-1, 0), (0, -1) }
            .Select(d => 
                (x: min.x + d.x, y: min.y + d.y)
            )
            .Where(d => 
                d.x >= 0 && d.x < xLen && d.y >= 0 && d.y < yLen
            );
        
        foreach(var nb in neighbours)
        {
            var dist = distances[min.x, min.y];
            var risk = grid[nb.x, nb.y];

            var alt = dist + risk;

            if (alt < distances[nb.x, nb.y]) {
                distances[nb.x, nb.y] = alt;
                prev[nb.x, nb.y] = min;
                // i don't need prev[]
                queue.Enqueue(nb, alt);
            }
        }
    }

    // shortest path
    path = new List<(int x, int y)>{};

    (int x, int y)? current = (xLen - 1, yLen - 1);
    while (current is (int, int) pos)
    {
        path.Add(pos);
        current = prev[pos.x, pos.y];
    }
    path.Reverse();

    return distances[xLen - 1, yLen - 1];
}


byte[,] Parse(string[] input)
{
    var xLen = input[0].Length;
    var yLen = input.Length;

    var grid = new byte[xLen, yLen];

    for (var y = 0; y < yLen; ++y)
    {
        for (var x = 0; x < xLen; ++x)
        {
            grid[x, y] = (byte)char.GetNumericValue(input[y][x]);
        }
    }

    return grid;
}

class VirtuallyRepeatedGrid : I2DReadOnlyIndexable<byte>
{
    byte [,] grid;
    int repeat;
    public VirtuallyRepeatedGrid(byte [,] grid, int repeat = 5)
    {
        this.grid = grid;
        this.repeat = repeat;
    }

    public byte this[int x, int y] 
    {
        get {
            var xLen = grid.GetLength(0);
            var yLen = grid.GetLength(1);

            var addition = x / xLen + y / yLen;

            int now = grid[x % xLen, y % yLen] + addition;

            now = (now - 1) % 9 + 1;

            return (byte) now;
        } 
    }

    public int GetLength(int dim) => grid.GetLength(dim) * repeat;
    
}

public class Grid<T> : I2DReadOnlyIndexable<T>
{
    private T[,] grid;
    public Grid(T[,] grid)
    {
        this.grid = grid;
    }
    public T this[int i, int j] { get => grid[i, j]; }
    public int GetLength(int dim)  => grid.GetLength(dim);
}

public interface I2DReadOnlyIndexable<T>
{
    T this[int i, int j] { get; }
    int GetLength(int dim);
}