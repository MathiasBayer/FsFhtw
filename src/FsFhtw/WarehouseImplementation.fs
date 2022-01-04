module WarehouseImplementation

open Domain
open System

let private addMaterial
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    material
    =
    Warehouse
        { Materials = material :: materialsInWarehouse
          Consumers = consumersInWarehouse
          Consumptions = consumptionsInWarehouse }

let private addConsumer
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    consumer
    =
    Warehouse
        { Materials = materialsInWarehouse
          Consumers = consumer :: consumersInWarehouse
          Consumptions = consumptionsInWarehouse }

let private deleteMaterial
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    name
    =
    Warehouse
        { Materials =
              materialsInWarehouse
              |> List.filter (fun material -> not <| material.Name.Equals(name))
          Consumers = consumersInWarehouse
          Consumptions = consumptionsInWarehouse }

let private deleteConsumer
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    consumer
    =
    Warehouse
        { Materials = materialsInWarehouse
          Consumers =
              consumersInWarehouse
              |> List.filter (fun c -> not <| c.Equals(consumer))
          Consumptions = consumptionsInWarehouse }

let private calculatePrice (price: float, amount: int) = price * float amount

let private addConsumption
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    (consumer, material, amount)
    =
    let mat =
        materialsInWarehouse
        |> List.tryFind (fun m -> m.Name.Equals(material))

    match mat with
    | Some material ->
        if material.Stock < amount then
            ConsumptionFailures(
                NotEnoughMaterialInStockFailure(
                    "Not enough "
                    + material.Name
                    + " in stock. Request amount was "
                    + string amount
                    + ". Available amount is "
                    + string material.Stock
                    + "."
                )
            )
        else
            let con =
                consumersInWarehouse
                |> List.tryFind (fun c -> c.Name.Equals(consumer))

            match con with
            | Some consumer ->
                let updatedMaterial =
                    { material with
                          Stock = material.Stock - amount }

                let consumption =
                    { Id = Guid.NewGuid()
                      Consumer = consumer
                      MaterialName = material.Name
                      Amount = amount
                      Price = calculatePrice (material.Price, amount) }

                Warehouse
                    { Materials =
                          updatedMaterial :: materialsInWarehouse
                          |> List.filter (fun material -> not <| material.Equals(mat))
                      Consumers = consumersInWarehouse
                      Consumptions = consumption :: consumptionsInWarehouse }
            | None ->
                ConsumptionFailures(
                    ConsumerNotFoundFailure(
                        "Consumer with name \""
                        + consumer
                        + "\" not found."
                    )
                )
    | None -> ConsumptionFailures(MaterialNotFoundFailure("Material " + material + " not found."))

let private deleteConsumption
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    guid
    =
    let consumption =
        consumptionsInWarehouse
        |> List.find (fun c -> c.Id.Equals(guid))

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


let private updatePrice
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    (name, price)
    =
    let mat =
        materialsInWarehouse
        |> List.tryFind (fun m -> m.Name.Equals(name))

    match mat with
    | None -> ConsumptionFailures(MaterialNotFoundFailure("Material " + name + " not found."))
    | Some material ->
        let updatedMaterial = { material with Price = price }

        Warehouse
            { Materials =
                  updatedMaterial :: materialsInWarehouse
                  |> List.filter (fun mat -> not <| material.Equals(mat))
              Consumers = consumersInWarehouse
              Consumptions = consumptionsInWarehouse }

let private getBelowReportingStock
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    =
    Warehouse
       { Materials =
           materialsInWarehouse
           |> List.filter (fun mat -> mat.Stock < mat.ReportingStock)
         Consumers = consumersInWarehouse
         Consumptions = consumptionsInWarehouse } 


let private emptyWarehosue =
    Warehouse
        { Materials = []
          Consumers = []
          Consumptions = [] }

let private initWarehouse =
    Warehouse
        { Materials =
              [ { Name = "Test"
                  Price = 1
                  Weight = 1
                  Stock = 10
                  ReportingStock = 0 }
                { Name = "Test2"
                  Price = 1
                  Weight = 1
                  Stock = 10
                  ReportingStock = 0 } ]
          Consumers =
              [ { Name = "Mathias" }
                { Name = "Reinhard" } ]
          Consumptions =
              [ { Id = Guid.NewGuid()
                  Consumer = { Name = "Mathias" }
                  MaterialName = "Test"
                  Amount = 4
                  Price = 4 } ] }


let warehouseApi: WarehouseApi =
    { add = addMaterial
      delete = deleteMaterial
      addConsumer = addConsumer
      deleteConsumer = deleteConsumer
      addConsumption = addConsumption
      deleteConsumption = deleteConsumption
      updatePrice = updatePrice
      getBelowReportingStock = getBelowReportingStock
      empty = emptyWarehosue
      init = initWarehouse }


let update (msg: Message) (model: Warehouse) : OperationResult =
    match msg with
    | EmptyWarehouse -> warehouseApi.empty
    | AddMaterial material -> warehouseApi.add model material
    | DeleteMaterial name -> warehouseApi.delete model name
    | AddConsumer consumer -> warehouseApi.addConsumer model consumer
    | DeleteConsumer consumer -> warehouseApi.deleteConsumer model consumer
    | AddConsumption (consumer, material, amount) -> warehouseApi.addConsumption model (consumer, material, amount)
    | DeleteConsumption guid -> warehouseApi.deleteConsumption model guid
    | UpdatePrice (name, price) -> warehouseApi.updatePrice model (name, price)
    | GetBelowReportingStock -> warehouseApi.getBelowReportingStock model 
    | InitWarehouse -> warehouseApi.init
