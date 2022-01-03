module Repl

open System
open Parser

type Message =
    | DomainMessage of Domain.Message
    | HelpRequested
    | NotParsable of string

type Warehouse = Domain.Warehouse

let read (input : string) =
    match input with
    | EmptyWarehouse -> Domain.EmptyWarehouse |> DomainMessage
    | Help -> HelpRequested
    | ParseFailed  -> NotParsable input

open Microsoft.FSharp.Reflection

let createHelpText () : string =
    FSharpType.GetUnionCases typeof<Domain.Message>
    |> Array.map (fun case -> case.Name)
    |> Array.fold (fun prev curr -> prev + " " + curr) ""
    |> (fun s -> s.Trim() |> sprintf "Known commands are: %s")

let evaluate (update : Domain.Message -> Warehouse -> Warehouse) (warehouse : Warehouse) (msg : Message) =
    match msg with
    | DomainMessage msg ->
        let newWarehouse = update msg warehouse
        let message = sprintf "The message was %A. New warehouse is %A" msg newWarehouse
        (newWarehouse, message)
    | HelpRequested ->
        let message = createHelpText ()
        (warehouse, message)
    | NotParsable originalInput ->
        let message =
            sprintf """"%s" was not parsable. %s"""  originalInput "You can get information about known commands by typing \"Help\""
        (warehouse, message)

let print (warehouse : Warehouse, outputToPrint : string) =
    printfn "%s\n" outputToPrint
    printf "> "

    warehouse

let rec loop (warehouse : Warehouse) =
    Console.ReadLine()
    |> read
    |> evaluate WarehouseImplementation.update warehouse
    |> print
    |> loop
