module Parser

open System
open Domain

let safeEquals (it: string) (theOther: string) =
    String.Equals(it, theOther, StringComparison.OrdinalIgnoreCase)

[<Literal>]
let HelpLabel = "Help"

let (|EmptyWarehouse|AddMaterial|DeleteMaterial|AddConsumer|DeleteConsumer|Help|ParseFailed|) (input: string) =
    let tryParseInt (arg: string) valueConstructor =
        let (worked, arg') = Int32.TryParse arg

        if worked then
            valueConstructor arg'
        else
            ParseFailed

    let tryParseDouble (arg: string) valueConstructor =
        let (worked, arg') = Double.TryParse arg

        if worked then
            valueConstructor arg'
        else
            ParseFailed

    let parts = input.Split(' ') |> List.ofArray

    match parts with
    | [ verb ] when safeEquals verb (nameof Domain.EmptyWarehouse) -> EmptyWarehouse
    | [ verb; name; price; weight; stock; reportingStock ] when safeEquals verb (nameof Domain.AddMaterial) ->
        tryParseInt
            reportingStock
            (fun r ->
                tryParseInt
                    stock
                    (fun s ->
                        tryParseDouble
                            weight
                            (fun w ->
                                tryParseDouble
                                    price
                                    (fun p ->
                                        AddMaterial
                                            { Name = name
                                              Price = p
                                              Weight = w
                                              Stock = s
                                              ReportingStock = r }))))
    | [ verb; name; ] when safeEquals verb (nameof Domain.DeleteMaterial) -> DeleteMaterial name
    | [ verb; name; ] when safeEquals verb (nameof Domain.AddConsumer) -> AddConsumer { Name = name }
    | [ verb; name; ] when safeEquals verb (nameof Domain.DeleteConsumer) -> DeleteConsumer { Name = name }
    | [ verb ] when safeEquals verb HelpLabel -> Help
    | _ -> ParseFailed

let (|AddConsumption|ParseFailed|) (input: string) =
    let tryParseInt (arg: string) valueConstructor =
        let (worked, arg') = Int32.TryParse arg

        if worked then
            valueConstructor arg'
        else
            ParseFailed

    let tryParseDouble (arg: string) valueConstructor =
        let (worked, arg') = Double.TryParse arg

        if worked then
            valueConstructor arg'
        else
            ParseFailed

    let parts = input.Split(' ') |> List.ofArray

    match parts with
    | [ verb; consumer; material; amount; ] when safeEquals verb (nameof Domain.AddConsumption) -> tryParseInt amount (fun a ->  AddConsumption (consumer , material, a))
    | _ -> ParseFailed
