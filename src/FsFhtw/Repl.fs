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
    | UpdatePrice v -> Domain.UpdatePrice v |> DomainMessage
    | GetBelowReportingStock -> Domain.GetBelowReportingStock |> DomainMessage
    | InitWarehouse -> Domain.InitWarehouse |> DomainMessage
    | Help -> HelpRequested
    | ParseFailed  -> NotParsable input

open Microsoft.FSharp.Reflection

let createHelpText () : string =
    FSharpType.GetUnionCases typeof<Domain.Message>
    |> Array.map (fun case -> case.Name)
    |> Array.fold (fun prev curr -> prev + " " + curr) ""
    |> (fun s -> s.Trim() |> sprintf "Known commands are: %s")


let private evaluateResult (msg: Domain.Message) (oldWarehouse: Warehouse) (result : Domain.OperationResult) : (Domain.Warehouse * string) =
    match result with
    | Domain.Warehouse w ->
        match msg with
        | Domain.GetBelowReportingStock -> let message = sprintf "The message was %A. All materials with stock below reporting stock are %A" msg (w.Materials |> List.map (fun m -> m.Name))
                                           (oldWarehouse, message)
        | _ -> let message = sprintf "The message was %A. New warehouse is %A" msg w
               (w, message)
    | Domain.ConsumptionFailures f -> match f with
                                        | Domain.MaterialNotFoundFailure message -> (oldWarehouse, message)
                                        | Domain.NotEnoughMaterialInStockFailure message -> (oldWarehouse, message)
                                        | Domain.ConsumerNotFoundFailure message -> (oldWarehouse, message)

let evaluate (update : Domain.Message -> Warehouse -> Domain.OperationResult) (warehouse : Warehouse) (msg : Message) =
    match msg with
    | DomainMessage msg ->
        let result = update msg warehouse
        let (warehouse, message) = evaluateResult msg warehouse result
        (warehouse, message)
    | HelpRequested ->
        let message = createHelpText ()
        (warehouse, message)
    | NotParsable originalInput ->
        let message =
            sprintf """"%s" was not parsable. %s"""  originalInput "You can get information about known commands by typing \"Help\""
        (warehouse, message)


        

let print (warehouse : Domain.Warehouse, outputToPrint : string) =
    printfn "%s\n" outputToPrint
    printf "> "
    warehouse
    

let rec loop (warehouse : Warehouse) =
    Console.ReadLine()
    |> read
    |> evaluate WarehouseImplementation.update warehouse
    |> print
    |> loop
