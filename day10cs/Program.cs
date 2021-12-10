// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Numerics;
Console.WriteLine("Hello, World!");

var sample = @"[({(<(())[]>[[{[]{<()<>>
[(()[<>])]({[<{<<[]>>(
{([(<{}[<>[]}>{[]{[(<()>
(((({<>}<{<{<>}{[]{[]{}
[[<[([]))<([[{}[[()]]]
[{[{({}]{}}([{[{{{}}([]
{<[[]]>}<{[{[{[]{()[[[]
[<(<(<(<{}))><([]([]()
<{([([[(<>()){}]>(<<{{
<{([{{}}[<[[[<>{}]]]>[]]";

string[] Lines(string input) => input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

IEnumerable<char[]> Parse(IEnumerable<string> lines)
{
    return lines.Select(x => x.ToCharArray());
}


(int, BigInteger) GetCorruptCharacterScore(char[] line)
{
    var map = new Dictionary<char, char> { ['<'] = '>', ['('] = ')', ['{'] = '}', ['['] = ']', };
    var pointsMap = new Dictionary<char, int> { [')'] = 3, [']'] = 57, ['}'] = 1197, ['>'] = 25137, };

    var stack = new Stack<char>(line.Length / 2);

    foreach (var ch in line)
    {
        if (map.ContainsKey(ch))
        {
            stack.Push(ch);
        }
        else // not an opening tag
        {
            if (stack.Count == 0) 
                throw new Exception("bruh");

            var expectedClosing = map[stack.Pop()];

            if (expectedClosing != ch)
            {
                return (pointsMap[ch], 0);
            }
        }
    }
    // if we reach further than the loop, it isn’t corrupted, it is therefore part2

    var part2 = new BigInteger(0);
    var part2Map = new Dictionary<char, int> { [')'] = 1, [']'] = 2, ['}'] = 3, ['>'] = 4, };
    foreach(var remaining in stack)
    {
        Console.Write(map[remaining]);
        part2 *= 5;
        part2 += part2Map[map[remaining]];
    }

    //either invalid or uncorrupted
    return (0, part2);
}

(int, BigInteger) Solve(IEnumerable<char[]> input)
{
    var calculated = 
        input
        .Select(GetCorruptCharacterScore)
        .ToArray();

    var part1 = 
        calculated
        .Select(x => x.Item1)
        .Aggregate((a, c) => a + c);

    var part2 = 
        calculated
        .Select(x => x.Item2)
        .Where(x => x > 0)
        .OrderBy(x => x)
        .ToArray();

    Debug.Assert(part2.Length % 2 == 1);
    
    var middle = part2[part2.Length / 2];

    return (part1, middle);
}

Debug.Assert(Solve(Parse(Lines(sample))) == (26397, 288957));
Console.WriteLine(Solve(Parse(File.ReadLines("input"))));