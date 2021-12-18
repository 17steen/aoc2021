using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;

var sample1 = Node.Parse("[[[[4,3],4],4],[7,[[8,4],9]]]").Add(Node.Parse("[1,1]"));
sample1.Reduce();
Debug.Assert(sample1.ToString() == "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]");

var example2 = @"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]
[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]
[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]
[7,[5,[[3,8],[1,4]]]]
[[2,[2,2]],[8,[8,1]]]
[2,9]
[1,[[[9,3],9],[[9,0],[0,7]]]]
[[[5,[7,4]],7],1]
[[[[4,2],2],6],[8,7]]".Split("\n");
var sum = Sum(Parse(example2));
Debug.Assert(sum.ToString() == "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]");
Debug.Assert(sum.Magnitude() == 3488);

var example3 = @"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]".Split("\n");
Debug.Assert(Sum(Parse(example3)).Magnitude() == 4140);
Debug.Assert(Part2(Parse(example3).ToArray())== 3993);

var file = Parse(File.ReadLines("input")).ToArray();
Console.WriteLine(Sum(file).Magnitude());
Console.WriteLine(Part2(file));

IEnumerable<Pair> Parse(IEnumerable<string> input)
{
    return input.Select(Node.Parse).Select(n => (Pair) n);
}

uint Part2(Pair[] pairs)
{
    var max = uint.MinValue;
    for (var i = 0; i < pairs.Length; ++i)
    {
        for (var j = 0; j < pairs.Length; ++j)
        {
            if (i == j) continue;

            var result = pairs[i].Add(pairs[j]).Magnitude();

            max = Math.Max(max, result);
        }
    }
    return max;
}

Node Sum(IEnumerable<Pair> snailfishNumbers)
{
    var result = snailfishNumbers.Aggregate((a, b) => a + b);
    
    return result;
}


internal abstract class Node
{
    public Pair? Parent;

    public abstract Node Clone();
    public Pair Add(Node other)
    {
        var result = new Pair(this.Clone(), other.Clone());
        result.Reduce();
        return result;
    }

    public static Pair operator +(Node a, Node b) => a.Add(b);

    public static Node Parse(string input)
    {
        var enumerator = input.GetEnumerator();

        return GetNode(ref enumerator);

    }

    public abstract uint Magnitude();

    private static Node GetNode(ref CharEnumerator input)
    {
        input.MoveNext();
        if (input.Current == '[')
        {
            var left = GetNode(ref input);
            input.MoveNext();
            Debug.Assert(input.Current == ',');
            var right = GetNode(ref input);
            input.MoveNext(); //skip ']'
            Debug.Assert(input.Current == ']');
            return new Pair(left, right);
        }
        else if (char.IsDigit(input.Current))
        {
            return new Regular((uint) char.GetNumericValue(input.Current));
        }
        else
        {
            throw new ArgumentOutOfRangeException(input.Current.ToString());
        }
    }


    public void Reduce()
    {
        var goOn = true;
        do
        {
            var worked = ReduceImpl(this);
            if (!worked)
            {
                goOn = ReduceImpl(this, true);
            }

        } while (goOn);
    }

    private static bool ReduceImpl(Node current, bool allowSplit = false, int depth = 0)
    {
        if (depth == 4 && current is Pair p)
        {
            p.Explode();
            return true;
        }

        switch (current)
        {
            case Regular reg:
                if (reg.Value >= 10 && allowSplit)
                {
                    reg.Split();
                    return true;
                }

                break;
            case Pair pair:
                if (ReduceImpl(pair.Left, allowSplit, depth + 1)) return true;
                if (ReduceImpl(pair.Right, allowSplit, depth + 1)) return true;
                break;
        }

        return false;
    }

    // tries to get a regular number to the left of it
    protected Regular? Left()
    {
        var papa = Parent;
        if (papa is null)
            return null;

        if (papa.IsRight(this))
        {
            var current = papa.Left;
            while (current is Pair pair)
            {
                current = pair.Right;
            }

            return current as Regular;
        }
        else
        {
            return papa.Left();
        }
    }

    protected Regular? Right()
    {
        var papa = Parent;
        if (papa is null)
            return null;

        if (papa.IsLeft(this))
        {
            var current = papa.Right;
            while (current is Pair pair)
            {
                // papa[ us, [target, ]]
                current = pair.Left;
            }

            return current as Regular;
        }
        else
        {
            return papa.Right();
        }
    }

    public Node Root()
    {
        var curr = this;
        while (curr.Parent is not null)
        {
            curr = curr.Parent;
        }

        return curr;
    }
}

internal class Regular : Node
{
    public uint Value;

    public override Regular Clone()
    {
        return new Regular(Value);
    }

    public override uint Magnitude()
    {
        return Value;
    }

    public void Split()
    {
        var left = Value / 2;
        var right = Value / 2 + (Value % 2);

        //Console.WriteLine($"requested to split {this}");
        //Console.WriteLine($"parent: {Parent?.ToString() ?? "(null)"}");
        //Console.WriteLine($"root: {Root()}");

        var parent = this.Parent;
        if (parent is null)
            throw new Exception("can't split toplevel");

        var newPair = new Pair(new Regular(left), new Regular(right))
        {
            Parent = parent
        };


        if (parent.IsLeft(this))
            parent.Left = newPair;
        else if (parent.IsRight(this))
            parent.Right = newPair;
        else
            throw new Exception("impossible");
    }

    public Regular(uint value, Pair? parent = null)
    {
        this.Value = value;
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}

internal class Pair : Node
{
    public new Node Left;
    public new Node Right;

    public override Pair Clone()
    {
        return new Pair(Left.Clone(), Right.Clone());
    }

    public override uint Magnitude()
    {
        return 3 * Left.Magnitude() + 2 * Right.Magnitude();
    }

    public Pair(Node left, Node right)
    {
        Left = left;
        Right = right;
        Left.Parent = this;
        Right.Parent = this;
    }

    // modifies the whole tree
    public void Explode()
    {
        //Console.WriteLine($"requested to explode: {this}");
        //Console.WriteLine($"root: {Root()}");
        var parent = this.Parent;
        if (parent is null)
            throw new Exception("can't explode top level node");

        if (Left is Regular left && Right is Regular right)
        {
            //Console.WriteLine($">{Left()}<, >{Right()}<");
            if (Left() is { } l) l.Value += left.Value;
            if (Right() is { } r) r.Value += right.Value;


            if (parent.IsLeft(this))
            {
                parent.Left = new Regular(0);
                parent.Left.Parent = parent;
            }
            else if (parent.IsRight(this))
            {
                parent.Right = new Regular(0);
                parent.Right.Parent = parent;
            }
            else
            {
                throw new Exception("impossible");
            }
        }
        else
        {
            throw new Exception("both children of an exploded pair should be numbers");
        }
    }

    public bool IsLeft(Node child)
    {
        return ReferenceEquals(Left, child);
    }

    public bool IsRight(Node child)
    {
        return ReferenceEquals(Right, child);
    }

    public override string ToString()
    {
        return $"[{Left},{Right}]";
    }
}