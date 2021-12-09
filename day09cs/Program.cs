const string sample = @"2199943210
3987894921
9856789892
8767896789
9899965678";

int[][] Parse(string input)
{

    var parsed = 
        input
            .Split(new[]{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => 
                x
                 .ToCharArray()
                 .Select(x => (int)char.GetNumericValue(x))
                 .ToArray()
            )
            .ToArray();
    return parsed;
}

(int, int) Solve(int[][] input)
{
    var yLen = input.Length;
    var xLen = input[0].Length;
    var sides = new [] { (0, 1), (1, 0), (-1, 0), (0, -1)};
    var bassinSizeList = new List<int>(){0,0,0};
    var sum = 0;
    for(int i = 0; i < yLen; ++i)
    {
        for(int j = 0; j < xLen; ++j)
        {
            var item = input[i][j];
            bool isSmallest = true;
            foreach(var (x, y) in sides)
            {
                var iPrime = i + y;
                var jPrime = j + x;
                if(iPrime >= yLen || iPrime < 0 || jPrime >= xLen || jPrime < 0)
                    continue;
                var thing = input[iPrime][jPrime];
                if(thing <= item)
                {
                    isSmallest = false;
                    break;
                }
            }
            if(isSmallest)
            {
                //start looking for the basin
                bassinSizeList.Add(BassinSize(CopyArray(input), (i,j)));
                sum += item + 1;
            }
        }
    }
    //Console.WriteLine(string.Join(", ", bassinSizeList.OrderByDescending(x=>x).Take(3)));
    var part1 = sum;
    var part2 = bassinSizeList.OrderByDescending(x => x).Take(3).Aggregate((a, c) => a * c);
    return (part1, part2);
}

int[][] CopyArray(int[][] array) => array.Select(x => x.Select(x => x).ToArray()).ToArray();

int BassinSize(int[][] area, (int, int) position)
{
    var (i,j) = position;
    var current = area[i][j];
    area[i][j] = 69;
    var yLen = area.Length;
    var xLen = area[0].Length;
    var sides = new [] { (0, 1), (1, 0), (-1, 0), (0, -1)};
    int sum = 1;
    foreach(var (x, y) in sides)
    {
        var iPrime = i + y;
        var jPrime = j + x;
        if(iPrime >= yLen || iPrime < 0 || jPrime >= xLen || jPrime < 0)
            continue;
        var thing = area[iPrime][jPrime];
        if(thing > current && thing < 9)
        {
            sum += BassinSize(area, (iPrime, jPrime));
        }
    }
    return sum;
}

var file = System.IO.File.ReadAllText("input");
var(part1, part2) = Solve(Parse(file));
Console.WriteLine($"{part1}, {part2}");