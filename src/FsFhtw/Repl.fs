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
    | AddMaterial v -> Domain.AddMaterial v |> DomainMessage
    | AddConsumer consumer -> Domain.AddConsumer consumer |> DomainMessage
    | DeleteMaterial name -> Domain.DeleteMaterial name |> DomainMessage
    | DeleteConsumer name -> Domain.DeleteConsumer name |> DomainMessage
    | AddConsumption v -> Domain.AddConsumption v |> DomainMessage
    | DeleteConsumption guid -> Domain.DeleteConsumption guid |> DomainMessage
    | InitWarehouse -> Domain.InitWarehouse |> DomainMessage
    | Help -> HelpRequested
    | ParseFailed  -> NotParsable input

open Microsoft.FSharp.Reflection

let createHelpText () : string =
    FSharpType.GetUnionCases typeof<Domain.Message>
    |> Array.map (fun case -> case.Name)
    |> Array.fold (fun prev curr -> prev + " " + curr) ""
    |> (fun s -> s.Trim() |> sprintf "Known commands are: %s")

let evaluate (update : Domain.Message -> Warehouse -> Domain.OperationResult) (warehouse : Warehouse) (msg : Message) =
    match msg with
    | DomainMessage msg ->
        let newWarehouse = update msg warehouse
        let message = sprintf "The message was %A. New warehouse is %A" msg newWarehouse
        (newWarehouse, warehouse, message)
    | HelpRequested ->
        let message = createHelpText ()
        (Domain.Warehouse warehouse, warehouse, message)
    | NotParsable originalInput ->
        let message =
            sprintf """"%s" was not parsable. %s"""  originalInput "You can get information about known commands by typing \"Help\""
        (Domain.Warehouse warehouse, warehouse, message)

let print (warehouse : Domain.OperationResult, oldWarehouse: Warehouse, outputToPrint : string) =
    match warehouse with
    | Domain.Warehouse w ->
        printfn "%s\n" outputToPrint
        printf "> "
        w
    | Domain.ConsumptionFailures f -> match f with
                                      | Domain.MaterialNotFoundFailure message -> printfn "%s" message
                                                                                  printf "> "
                                                                                  oldWarehouse
                                      | Domain.NotEnoughMaterialInStockFailure message -> printfn "%s" message
                                                                                          printf "> "
                                                                                          oldWarehouse
                                      | Domain.ConsumerNotFoundFailure message -> printfn "%s" message
                                                                                  printf "> "
                                                                                  oldWarehouse

let rec loop (warehouse : Warehouse) =
    Console.ReadLine()
    |> read
    |> evaluate WarehouseImplementation.update warehouse
    |> print
    |> loop
