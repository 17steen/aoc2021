// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
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

string[] Lines(string input) => input.Split(new[]{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

IEnumerable<char[]> Parse(IEnumerable<string> lines)
{
    return lines.Select(x => x.ToCharArray());
}


int GetCorruptCharacterScore(char[] line)
{

    var map = new Dictionary<char,char>{
        ['<'] = '>',
        ['('] = ')', 
        ['{'] = '}', 
        ['['] = ']', 
    };

    var pointsMap = new Dictionary<char,int>{
        [')'] = 3, 
        [']'] = 57, 
        ['}'] = 1197, 
        ['>'] = 25137,
    };

    var corruptedList = new List<char>{};

    var stack = new Stack<char>(line.Length / 2);

    foreach(var ch in line)
    {
        if(map.ContainsKey(ch)) 
        {
            stack.Push(ch);
        }
        else // not an opening tag
        {
            if (stack.Count == 0) return 0; // invalid
            var expectedClosing = map[stack.Pop()];

            if (expectedClosing != ch)
            {
                return pointsMap[ch];
            }
        }
    }

    //either invalid or uncorrupted
    return 0;
}



int Part1(IEnumerable<char[]> input)
{
    return input.Select(GetCorruptCharacterScore).Aggregate((a, b) => a + b );
}

Debug.Assert(Part1(Parse(Lines(sample))) == 26397);

Console.WriteLine(Part1(Parse(File.ReadLines("input"))));