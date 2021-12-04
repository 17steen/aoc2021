open System.Diagnostics

let sampleInput =
    @"199
200
208
210
200
207
240
269
260
263"

let detuple func (a, b) = func a b

let id_print thing =
    printfn "%A" thing
    thing
    
let parse (input: string) =
    input.Split('\n') |> Seq.map int |> Seq.cache

let part1 (depths: int seq) =
    depths
    |> Seq.skip 1 
    |> Seq.zip depths 
    |> Seq.filter (detuple (<))
    |> Seq.length
    

let part2dumb (depths: int seq) =
    depths
    |> Seq.chunkBySize 3 
    |> Seq.skip 1
    |> Seq.map id_print
    |> Seq.filter (Seq.length >> ((=) 3))
    |> Seq.map Seq.head
    |> part1
    
let part2 (depths: int seq) =
    Seq.zip3 (depths) (Seq.skip 1 depths) (Seq.skip 2 depths)
    |> Seq.map (fun (a, b, c) -> a + b + c)
    |> part1

assert (parse sampleInput |> part1 |> (=) 7)
assert (parse sampleInput |> part2 |> (=) 5)

open System.IO
let input  =
       File.ReadAllText "input" |> parse
       
input |> part1 |> printfn "%d" 
input |> part2 |> printfn "%d" 
