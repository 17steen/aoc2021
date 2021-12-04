let sample = 
    """00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010""".Split "\r\n"

open System.IO
let input =
    File.ReadAllLines "input"
let boolsToNum: bool seq -> int =
    Seq.fold (fun acc b -> (acc <<< 1) + if b then 1 else 0) 0
let part1 (input: string seq) = 
    let commonBits =
        input
        |> Seq.map (Seq.map System.Char.GetNumericValue)
        |> Seq.transpose
        |> Seq.map (Seq.average >> (<) 0.5)
    
    let gamma = boolsToNum commonBits
    let epsilon = Seq.map (id >> not) commonBits |> boolsToNum
    
    gamma * epsilon
let part2 (input: string seq) =
    let parsed =
        input
        |> Seq.map (Seq.map System.Char.GetNumericValue)
        |> Seq.cache
    
    let rec criteria comp idx valid =
        if Seq.length valid = 1 then
            Seq.head valid
        else
            let avg = valid |> Seq.transpose |> Seq.map Seq.average |> Seq.item idx
            
            let toKeep = if comp avg then 1. else 0.
            
            valid
            |> Seq.filter (Seq.item idx >> (=) toKeep)
            |> criteria comp (idx + 1)
            
    let toNum =
         Seq.map ((=) 1.0) >> boolsToNum
            
    let findOxygenGen = criteria ((<=) 0.5) 0
    let findCo2Scrubber = criteria ((>) 0.5) 0
    
    let oxygenGen = findOxygenGen parsed |> toNum
    let CO2Scrubber = findCo2Scrubber parsed |> toNum
    
    oxygenGen * CO2Scrubber
    
assert (part1 sample |> (=) 198)
assert (part2 sample |> (=) 230)
printfn $"%A{part1 input}" 
printfn $"%A{part2 input}" 
