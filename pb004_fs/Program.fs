open System
open System.Runtime.InteropServices

let sample =
    """7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

22 13 17 11  0
 8  2 23  4 24
21  9 14 16  7
 6 10  3 18  5
 1 12 20 15 19

 3 15  0  2 22
 9 18 13 17  5
19  8  7 25 23
20 11 10 24  4
14 21 16 12  6

14 21 17 24  4
10 16 15  9 19
18  8 23 26 20
22 11 13  6  5
 2  0 12  3  7"""

let split (sep: string []) (thing: string) =
    thing.Split(
        sep,
        (StringSplitOptions.TrimEntries
         ||| StringSplitOptions.RemoveEmptyEntries)
    )

let lines =
    split [| "\r\n"; "\n" |]

//let split2 (seps:string[])(thing:string) =
let id_print thing =
    printfn $"(%A{thing})"
    thing


let parse (input: string) =
    let splat = split [| "\n\n" |] input

    let numbers =
        Array.head splat
        |> (fun a -> a.Split ",")
        |> Array.map int


    let boards =
        Array.skip 1 splat
        |> Array.map (
            lines
            >> Array.map (split [| " " |] >> Array.map (int >> Some))
        )

    (numbers, boards)

let mark (number: int) : int option [] [] -> int option [] [] =
    Array.map (
        Array.map
            (fun a ->
                match a with
                | Some n when n = number -> None
                | _ -> a)
    )

let bingo (board: int option [] []) : bool =
    let hasRow =
        board |> Array.exists (Array.forall ((=) None))

    let hasCol =
        board
        |> Array.transpose
        |> Array.exists (Array.forall ((=) None))

    hasRow || hasCol

let rec part1 (numbers: int [], boards: int option [] [] []) : int =
    let number = Array.head numbers
    let changed = Array.map (mark number) boards
    let bingoes = Array.filter bingo changed

    if not (Array.isEmpty bingoes) then
        bingoes
        |> Array.map (
            Array.map (Array.choose id)
            >> Array.concat
            >> Array.fold (+) 0
        )
        |> Array.max
        |> (*) number
    else
        part1 ((Array.tail numbers), changed)

let rec part2 (numbers: int [], boards: int option [] [] []) : int =
    let number = Array.head numbers
    let marked = Array.map (mark number) boards
    let losing = Array.filter (not << bingo) marked

    if Array.isEmpty losing && Array.length marked = 1 then
        let score =
            marked
            |> Array.exactlyOne
            |> Array.map (Array.choose id)
            |> Array.concat
            |> Array.fold (+) 0

        score * number
    else if Array.isEmpty marked then
        failwith "array empty"
    else
        part2 ((Array.tail numbers), losing)

assert (part1 (parse sample) |> (=) 4512)
assert (part2 (parse sample) |> (=) 1924)
printfn "tests passed"

open System.IO
let input = File.ReadAllText "input"
let result = part1 (parse input)
let result2 = part2 (parse input)

printfn "%A" result
printfn "%A" result2
