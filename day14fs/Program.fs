open System
open System.Numerics
open System.Linq

let sample = """NNCB

CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C"""

let inline split (seps:string[]) (string:string) =
    string.Split(seps, StringSplitOptions.RemoveEmptyEntries ||| System.StringSplitOptions.TrimEntries)

let lines = split [| "\r\n"; "\n" |]

let twoOf arr = 
    match arr with 
    | [|a; b|] -> (a, b) 
    | _ -> failwith "not two"

let parse input =
    let lines = lines input
    let template = Array.head lines
    let pairInsertions = 
        Array.tail lines
        |> Array.map (
            split [|"->"|]
            >> twoOf
            // eg. NB -> C
            >> (fun (pair, b) -> (twoOf (pair.ToCharArray()), b.First()))
        )
        |> Map.ofArray

    (template, pairInsertions)


let performInsertions template pairInsertions =
    let paired = Seq.zip template (Seq.tail template)

    let result = 
        paired
        |> Seq.map (
            fun pair ->
                match Map.tryFind pair pairInsertions with
                | None -> [snd pair]
                | Some insertion -> [insertion; snd pair]
        )
        |> List.concat

    let result = (Seq.head template) :: result

    result

let rec solve count (template: char seq) pairInsertions =
    let mutable res = template |> List.ofSeq
    for _ in 1..count do
        res <- performInsertions res pairInsertions
        //printfn "%A" (res |> Seq.toArray |> String)

    let counted = 
        res
        |> List.countBy id 
        |> List.map snd // extract the count only
        |> List.sort

    let least   = List.head counted
    let most    = List.last counted

    (most - least)

let x = 2192039569602UL

assert(parse sample ||> solve 10 |> (=) 1588)
let file = System.IO.File.ReadAllText "input"

parse file ||> solve 10 |> printfn "%A"

    