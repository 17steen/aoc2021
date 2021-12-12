open System
open System.Numerics

let sample = """start-A
start-b
A-c
A-b
b-d
A-end
b-end"""

let sample3 = """fs-end
he-DX
fs-he
start-DX
pj-DX
end-zg
zg-sl
zg-pj
pj-he
RW-he
fs-DX
pj-RW
zg-RW
start-pj
he-WI
zg-he
pj-fs
start-RW"""

let inline split (seps:string[]) (string:string) =
    string.Split(seps, StringSplitOptions.RemoveEmptyEntries ||| System.StringSplitOptions.TrimEntries)

let lines = split [| "\r\n"; "\n" |]

let twoOf arr = 
    match arr with 
    | [|a; b|] -> (a, b) 
    | _ -> failwith "not two"

let rec paths (current:string) (traversed: string list) (connectionsMap: Map<string, string Set>) = 
    if current.ToLower() = current
        && List.contains current traversed 
    then [] else

    let connections = Map.find current connectionsMap

    if current = "end" then [current :: traversed] else

    seq {
        for connection in connections ->
            paths connection (current :: traversed) connectionsMap
    }
    |> List.concat

let rec paths2 (current:string) (traversed: string list) (connectionsMap: Map<string, string Set>) = 
    let smallCaves = traversed |> List.where (fun a -> a = a.ToLower())
    let distinctLen = smallCaves |> List.distinct |> List.length
    let len = smallCaves |> List.length

    if (not traversed.IsEmpty && current = "start") || (len - distinctLen) = 2 then [] else

    if current = "end" then [current :: traversed] else

    let connections = Map.find current connectionsMap

    seq {
        for connection in connections ->
            paths2 connection (current :: traversed) connectionsMap
    }
    |> List.concat


let solve (input:string) =
    printfn "%A" input
    //failwith "wait"
    let array = 
        lines input
        |> Seq.map(
            split [|"-"|]
            >> twoOf
        )

    let folder state (left, right) =
        state
        |> Map.change left (fun v ->
            Some (
                match v with
                | None -> Set[right]
                | Some set -> Set.add right set
            )
        ) 
        |> Map.change right (fun v ->
            Some (
                match v with
                | None -> Set[left]
                | Some set -> Set.add left set
            )
        ) 

    let connections = 
        array
        |> Seq.fold folder Map[]
    
    let part1   = paths     "start" [] connections |> List.length
    let part2   = paths2    "start" [] connections |> List.length

    (part1, part2)


assert(solve sample |> (=) (10, 36))
assert(solve sample3 |> (=) (226, 3509))
printfn "%A" <| solve sample

open System.IO

let file = File.ReadAllText "input"
printfn "%A" <| solve file