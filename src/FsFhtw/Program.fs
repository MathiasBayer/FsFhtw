[<EntryPoint>]
let main argv =
    printfn "Welcome to the FHTW Domain REPL!"
    printfn "Please enter your commands to interact with the system."
    printfn "Press CTRL+C to stop the program."
    printf "> "

    let initialState = WarehouseImplementation.warehouseApi.empty
    let warehouse = match initialState with
                    | Domain.Warehouse w ->  w
    Repl.loop warehouse
    0 // return an integer exit code
