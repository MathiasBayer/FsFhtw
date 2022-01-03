module Parser

open System

let safeEquals (it : string) (theOther : string) =
    String.Equals(it, theOther, StringComparison.OrdinalIgnoreCase)

[<Literal>]
let HelpLabel = "Help"

let (|EmptyWarehouse|Help|ParseFailed|) (input : string) =
    let tryParseInt (arg : string) valueConstructor =
        let (worked, arg') = Int32.TryParse arg
        if worked then valueConstructor arg' else ParseFailed

    let parts = input.Split(' ') |> List.ofArray
    match parts with
    | [ verb ] when safeEquals verb (nameof Domain.EmptyWarehouse) -> EmptyWarehouse
    | [ verb ] when safeEquals verb HelpLabel -> Help
    | _ -> ParseFailed
