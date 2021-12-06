let sample = "3,4,3,1,2"

open System

let split (seps:string[]) (string:string) =
    string.Split(seps, StringSplitOptions.RemoveEmptyEntries ||| System.StringSplitOptions.TrimEntries)

let lines = split [| "\r\n"; "\n" |]

let parse =
    split [|","|] >> Array.map int

let dayBad (lanternFishes: int list) =
    lanternFishes 
    |> List.map ((+) -1)
    |> List.fold (
        fun state i -> 
            if i < 0 then
                8 :: 6 :: state
            else
                i :: state
    ) []

let rec part1Bad days (input: int list) =
    printfn "%A" input
    if days <= 0 then
        List.length input
    else
        part1Bad (days - 1 ) (dayBad input)

let mapAdd key value = 
    Map.change key (
        fun a -> 
            match a with 
            | None -> Some value 
            | Some other -> Some (other + value)
    )

let day = 
    let folder state key v = 
        if key = 0 then
            state
            |> mapAdd 6 v
            |> mapAdd 8 v
        else
            mapAdd (key - 1) v state
    Map.fold folder (Map[])

let rec part1 days (input: Map<int, bigint>) = 
    if days <= 0 then
        input
        |> Map.values
        |> Seq.reduce (+)
    else
        part1 (days - 1 ) (day input)

/// <summary>
/// turns a list of lifetime,
/// into amount of entities bound to the lifetime
/// </summary>
/// <param name="input"></param>
let toMap (input: int seq) =
    input
    |> Seq.fold (fun state curr -> mapAdd curr 1I state) Map[]

let part2 = part1 256

let parsedSample = parse sample |> toMap

assert(part1 18 parsedSample |> (=) 26I)
assert(part1 80 parsedSample |> (=) 5934I)
assert(part2 parsedSample |> (=) 26984457539I)

open System.IO

let input =  File.ReadLines("input") |> Seq.head |> parse |> toMap

part1 80 input |> printfn "part1 %A"
part2 input |> printfn "%A"
