open System

let split (seps:string[]) (string:string) =
    string.Split(seps, StringSplitOptions.RemoveEmptyEntries ||| System.StringSplitOptions.TrimEntries)

let lines = split [| "\r\n"; "\n" |]

let parse =
    split [|","|] >> Array.map int

let sample = "16,1,2,0,4,2,7,1,2,14" |> parse

let median (arr:double[]) =
    let len = arr.Length
    if len = 0 then
        failwith "empty array"
    else if len % 2 = 1 then
        let half = len / 2
        let a = Array.item (half + 0) arr
        let b = Array.item (half + 1) arr
        (a + b) / 2.
    else
        Array.item (len / 2) arr


let part1 (input:int[]) =
    let median = 
        input
        |> Array.sort
        |> Array.map double
        |> median

    if median <> Math.Round median then
        failwith "median not an integer"

    let median = int median
    input
    |> Array.map ((-) median >> abs)
    |> Array.sum

let consume count = 
    Seq.init count ((+) 1) |> Seq.sum

let triangle number = 
    (number * number + number) / 2

let part2poop (input:int[]) =
    let average = 
        input
        |> Array.map double
        |> Array.average
        |> Math.Round
        |> int

    input
    |> Array.map ((-) average >> abs >> consume)
    |> Array.sum

let part2 (input:int[]) =
    let sorted =
        input
        |> Array.sort
    
    let min = Array.head sorted
    let max = Array.last sorted

    printfn $"max {min}"
    printfn $"max {max}"

    seq {
        for i in min .. max ->
            input
            |> Array.map ((-) i >> abs >> triangle)
            |> Array.sum
    }
    |>  Seq.min

open System.IO

let input = File.ReadLines "input" |> Seq.head |> parse

printfn "%A" (part1 sample)
printfn "%A" (part1 input)
printfn "%A" (part2 sample)
printfn "%A" (part2 input)
