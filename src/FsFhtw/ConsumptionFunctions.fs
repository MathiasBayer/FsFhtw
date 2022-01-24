module ConsumptionFunctions

open Domain
open MaterialFunctions
open System

let private calculatePrice (price: float, amount: int) = price * float amount

let addConsumption
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    (request: ConsumptionRequest)
    =
    let mat =
        findMaterialForName materialsInWarehouse request.MaterialName

    match mat with
    | Some material ->
        if material.Stock < request.Amount then
            ConsumptionFailures(
                NotEnoughMaterialInStockFailure(
                    "Not enough "
                    + material.Name
                    + " in stock. Request amount was "
                    + string request.Amount
                    + ". Available amount is "
                    + string material.Stock
                    + "."
                )
            )
        else
            let con =
                consumersInWarehouse
                |> List.tryFind (fun c -> c.Name.Equals(request.Consumer))

            match con with
            | Some consumer ->
                let updatedMaterial =
                    { material with
                          Stock = material.Stock - request.Amount }

                let consumption =
                    { Id = Guid.NewGuid()
                      Consumer = consumer
                      MaterialName = material.Name
                      Amount = request.Amount
                      Price = calculatePrice (material.Price, request.Amount) }

                Warehouse
                    { Materials =
                          updatedMaterial :: materialsInWarehouse
                          |> List.filter (fun mat -> not <| material.Equals(mat))
                      Consumers = consumersInWarehouse
                      Consumptions = consumption :: consumptionsInWarehouse }
            | None ->
                ConsumptionFailures(
                    ConsumerNotFoundFailure(
                        "Consumer with name \""
                        + request.Consumer
                        + "\" not found."
                    )
                )
    | None -> ConsumptionFailures(MaterialNotFoundFailure("Material " + request.MaterialName + " not found."))

let deleteConsumption
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    guid
    =
    let consumption =
        consumptionsInWarehouse
        |> List.tryFind (fun c -> c.Id.Equals(guid))

    match consumption with
    | Some consumption ->
        let mat =
            materialsInWarehouse
            |> List.find (fun m -> m.Name.Equals(consumption.MaterialName))

        let updatedMaterial =
            { mat with
                  Stock = mat.Stock + consumption.Amount }

        Warehouse
            { Materials =
                  updatedMaterial :: materialsInWarehouse
                  |> List.filter (fun material -> not <| material.Equals(mat))
              Consumers = consumersInWarehouse
              Consumptions =
                  consumptionsInWarehouse
                  |> List.filter (fun c -> not <| c.Equals(consumption)) }
    | None -> ConsumptionFailures(ConsumptionNotFoundFailure("Consumption with ID: " + string guid + " not found."))

