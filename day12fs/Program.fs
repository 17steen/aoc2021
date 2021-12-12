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

let solve input =
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
    
    let results = paths "start" [] connections

    //printfn "%A" results
    //printfn "%A" (List.length results)

    List.length results


assert(solve sample |> (=) 10)
assert(solve sample3 |> (=) 226)

open System.IO

let file = File.ReadAllText "input"
printfn "%A" <| solve file