open System
open System.Numerics

let sample = """be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce"""

let inline split (seps:string[]) (string:string) =
    string.Split(seps, StringSplitOptions.RemoveEmptyEntries ||| System.StringSplitOptions.TrimEntries)

let lines = split [| "\r\n"; "\n" |]

let twoOf arr = 
    match arr with 
    | [|a; b|] -> (a, b) 
    | _ -> failwith "not two"

let splitAtItem (item:'a) (arr:'a []) =
    let idx = Array.IndexOf(arr, item)
    Array.splitAt idx arr

type Entry = string[] * string[] 

let parse input: Entry[] =
    input
    |> lines
    |> Array.map (
        (split [|" "|])
        >> splitAtItem "|"
    )


let part1 = 
    Seq.map (
        snd
        >> Seq.map Seq.length
        >> Seq.filter (fun a -> 
            match a with 
            | 2 | 4 | 3 | 7 -> true
            | _ -> false
        )
        >> Seq.length
    )
    >> Seq.reduce (+)



let letterToInt (ch:char) =
    assert(System.Char.IsAscii(ch) && System.Char.IsLower(ch) && System.Char.IsLetter(ch))
    (int ch) - (int 'a')

assert(letterToInt 'a' = 0)
assert(letterToInt 'z' = 25)
assert((1 <<< 7) - 1 = 0b111_1111)

let zero    = 0b111_0111
let one     = 0b001_0010
let two     = 0b101_1101
let three   = 0b110_1101
let four    = 0b010_1110
let five    = 0b110_1011
let six     = 0b111_1011
let seven   = 0b010_0101
let eight   = 0b111_1111
let nine    = 0b110_1111

(*
  0:      1:      2:      3:      4:
 0000    ....    0000    0000    ....
1    2  .    2  .    2  .    2  1    2
1    2  .    2  .    2  .    2  1    2
 ....    ....    3333    3333    3333
4    5  .    5  4    .  .    5  .    5
4    5  .    5  4    .  .    5  .    5
 6666    ....    6666    6666    ....

  5:      6:      7:      8:      9:
 0000    0000    0000    0000    0000
1    .  1    .  .    2  1    2  1    2
1    .  1    .  .    2  1    2  1    2
 3333    3333    ....    3333    3333
.    5  4    5  .    5  4    5  .    5
.    5  4    5  .    5  4    5  .    5
 6666    6666    ....    6666    6666
*)

let outputs = [zero; one; two; three; four; five; six; seven; eight; nine]

let popcount (n:uint) = BitOperations.PopCount n

let sufficientToSolve (ltPos: int[]) (patterns: string[]) =
    let solveable (thing: int seq) = 
        let allPossibleSegmentCombinations = 
            thing 
            |> Seq.reduce (&&&)
        outputs
        |> Seq.filter (
            fun v -> v = (v &&& allPossibleSegmentCombinations)
        )
        |> Seq.length
        |> (=) 1

    patterns 
    |> Array.map (Seq.map letterToInt)
    |> Seq.forall solveable

let solve (unique, output) =
    let allPossibilities = 0b0111_1111

    // 0000101 -> this means segment 0 and segment 2 are possibilities
    //maps letters to all the possible segments
    let mutable ltPos = Array.create 7 allPossibilities

    let patterns = 
        Array.append unique output 
        |> Array.distinct 
        |> Array.map (Seq.map letterToInt >> Array.ofSeq)

    while not (sufficientToSolve ltPos output) do
        for signal in patterns do
            match signal.Length with
            | 2 -> for letter in signal do ltPos.[letter] <- ltPos.[letter] &&& one
            | 4 -> for letter in signal do ltPos.[letter] <- ltPos.[letter] &&& four
            | 3 -> for letter in signal do ltPos.[letter] <- ltPos.[letter] &&& seven
            | 7 -> for letter in signal do ltPos.[letter] <- ltPos.[letter] &&& eight
            | _ -> ()
            ()
        ()

    0

let part2:Entry seq -> int =
    Seq.map solve
    >> Seq.reduce (+)


assert (parse sample |> part1 |> (=) 26)

open System.IO

let parsed = parse (File.ReadAllText "input")

printfn "%A" (part1 parsed)