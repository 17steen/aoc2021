using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Security;
using System.Threading.Channels;

const string sample = "target area: x=20..30, y=-10..-5";
const string input = "target area: x=244..303, y=-91..-54";

var throwsSample = @"23,-10  25,-9   27,-5   29,-6   22,-6   21,-7   9,0     27,-7   24,-5
25,-7   26,-6   25,-5   6,8     11,-2   20,-5   29,-10  6,3     28,-7
8,0     30,-6   29,-8   20,-10  6,7     6,4     6,1     14,-4   21,-6
26,-10  7,-1    7,7     8,-1    21,-9   6,2     20,-7   30,-10  14,-3
20,-8   13,-2   7,3     28,-8   29,-9   15,-3   22,-5   26,-8   25,-8
25,-6   15,-4   9,-2    15,-2   12,-2   28,-9   12,-3   24,-6   23,-7
25,-10  7,8     11,-3   26,-7   7,1     23,-9   6,0     22,-10  27,-6
8,1     22,-8   13,-4   7,6     28,-6   11,-4   12,-4   26,-9   7,4
24,-10  23,-8   30,-8   7,0     9,-1    10,-1   26,-5   22,-9   6,5
7,5     23,-6   28,-10  10,-2   11,-1   20,-9   14,-2   29,-7   13,-3
23,-5   24,-8   27,-9   30,-7   28,-5   21,-10  7,9     6,6     21,-5
27,-10  7,2     30,-9   21,-8   22,-7   24,-9   20,-6   6,9     29,-5
8,-2    27,-8   30,-5   24,-7"
    .Split(new[]{" ", "\n"}, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
    .Select(str =>
    {
        var nums =
            str
                .Split(",")
                .Select(int.Parse)
                .ToArray();
        return (nums[0], nums[1]);
    }).ToArray();

var inputRec = new Rectangle(244, -91, 303 - 244 + 1, -54 - -91 + 1);

var sampleRec = new Rectangle(20, -10, 30 - 20 + 1, -5 - -10 + 1);

Debug.Assert(sampleRec.Contains(30, -5));
Debug.Assert(sampleRec.Contains(20, -10));

var retval = Solve(inputRec);
Console.WriteLine(retval);


(int, int) Solve(Rectangle target)
{
    var validThrows = new List<(int, int)>();
    var currentMaxY = int.MinValue;
    
    // TODO: make this loop not infinite thanks
    // there must be a way to be intelligent about this
    for (var startY = target.Top; startY < 999; ++startY)
    {
        // use this  : S = n(n + 1)/2  to get minimal starting value of x

        for (var startX = 0; startX <= target.Right; ++startX)
        {
            var current = new Point(0, 0);
            var xVelocity = startX;
            var yVelocity = startY;

            var maxY = 0;

            var path = new List<Point>();

            var i = 0;

            while (true)
            {
                ++i;
                current.Offset(xVelocity, yVelocity);

                path.Add(current);

                maxY = Math.Max(maxY, current.Y);


                if (target.Contains(current))
                {
                    currentMaxY = Math.Max(maxY, currentMaxY);
                    validThrows.Add((startX, startY));
                    break;
                }
                else if (current.X > target.Right || current.Y < target.Top)
                {
                    if (startX == 6 && startY == 0)
                    {
                        Console.WriteLine("mamma mia");
                    }
                    break;
                }

                xVelocity -= 1 * Math.Sign(xVelocity);
                yVelocity -= 1;
            }
        }
    }


    /*  (this was for debugging the sample
    var missingThrows = throwsSample.Where(
        x => !validThrows.Contains(x)
    );
    var excessThrows = validThrows.Where(
        x => !throwsSample.Contains(x)
    );
    
    Console.WriteLine("excess throws");
    foreach (var throw_ in excessThrows)
    {
        Console.WriteLine($"{throw_}");
    }
    
    Console.WriteLine("missing throws");
    foreach (var throw_ in missingThrows)
    {
        Console.WriteLine($"{throw_}");
    }
    Console.WriteLine("done");
    */

    return (currentMaxY, validThrows.Count);
}

