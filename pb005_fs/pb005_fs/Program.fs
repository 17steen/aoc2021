open System

let split (seps: string []) (string: string) =
    string.Split(
        seps,
        StringSplitOptions.RemoveEmptyEntries
        ||| StringSplitOptions.TrimEntries
    )

let lines = split [| "\r\n"; "\n" |]

let sample =
    """0,9 -> 5,9
8,0 -> 0,8
9,4 -> 3,4
2,2 -> 2,1
7,0 -> 7,4
6,4 -> 2,0
0,9 -> 2,9
3,4 -> 1,4
0,0 -> 8,8
5,5 -> 8,2"""

type Point = (int * int)
type Line = (Point * Point)

let twoOf arr =
    match arr with
    | [| a; b |] -> (a, b)
    | _ -> failwith "not two"

let parse =
    lines
    >> Array.map (
        split [| "->" |]
        >> Array.map (split [| "," |] >> Array.map int >> twoOf >> Point)
        >> twoOf
        >> Line
    )
let id_print thing =
    printfn "(%A)" thing
    thing
let minmax (a, b) =
    if (a < b) then (a, b) else (b, a)

let points (line: Line) : Point[] =
    match line with
    | (x1, y1), (x2, y2) when x1 = x2 ->
         let min, max = minmax (y1, y2)
         seq { min..max }
         |> Seq.map (fun a -> (x1, a))
         |> Seq.toArray
    | (x1, y1), (x2, y2) when y1 = y2 ->
         let min, max = minmax (x1, x2)
         seq { min..max }
         |> Seq.map (fun a -> (a, y1))
         |> Seq.toArray
    | _ -> [||]

let slope ((x1, y1), (x2, y2)) =
    (y2 - y1) / (x2 - x1)

let range a b =
    if a > b then
        seq { a..(-1)..b }
    else
        seq { a..b }
        
    
    
let points2 (line: Line) : Point seq =
    match line with
    | (x1, y1), (x2, y2) when x1 = x2 ->
         let min, max = minmax (y1, y2)
         seq { min..max }
         |> Seq.map (fun a -> (x1, a))
    | (x1, y1), (x2, y2) when y1 = y2 ->
         let min, max = minmax (x1, x2)
         seq { min..max }
         |> Seq.map (fun a -> (a, y1))
    | (x1, y1), (x2, y2) when slope line |> abs = 1->
        range x1 x2 |> Seq.zip <| range y1  y2
    | _ -> failwith "bruh?"
    
    
let cartography (map: Map<Point, int>) (point: Point) =
    Map.change point (fun a ->
        match a with
        | None -> Some(1)
        | Some(x) -> Some(x + 1)
    ) map
        
let part1 (lines: Line []) =
    lines
    |> Array.map points
    |> Array.concat
    |> Array.fold  cartography (Map [])
    |> Map.values
    |> Seq.filter ((<) 1)
    |> Seq.length
    
let part2 (lines: Line []) =
    lines
    |> Seq.map points2
    |> Seq.concat
    |> Seq.fold  cartography (Map [])
    |> Map.values
    |> Seq.filter ((<) 1)
    |> Seq.length


assert(parse sample |> part1 |> (=) 5)
assert(parse sample |> part2 |> (=) 12)

open System.IO
let file = File.ReadAllText "input"
let res = part1 <| parse file
let res2 = part2 <| parse file
printfn "%A" res
printfn "%A" res2
