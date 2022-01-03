module WarehouseImplementation

open Domain
open System

let private addMaterial
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    material
    =
    { Materials = material :: materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }

let private addConsumer
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    consumer
    =
    { Materials = materialsInWarehouse
      Consumers = consumer :: consumersInWarehouse
      Consumptions = consumptionsInWarehouse }

let private deleteMaterial
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    name
    =
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
        |> List.find (fun m -> m.Name.Equals(material))

    let updatedMaterial = { mat with Stock = mat.Stock - amount }

    let consumption =
        { Id = Guid.NewGuid()
          Consumer = { Name = consumer }
          MaterialName = mat.Name
          Amount = amount
          Price = calculatePrice (mat.Price, amount) }

    { Materials =
          updatedMaterial :: materialsInWarehouse
          |> List.filter (fun material -> not <| material.Equals(mat))
      Consumers = consumersInWarehouse
      Consumptions = consumption :: consumptionsInWarehouse }

let private deleteConsumption
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    guid
    =
    let consumption = consumptionsInWarehouse |> List.find(fun c -> c.Id.Equals(guid))
    let mat =
        materialsInWarehouse
        |> List.find (fun m -> m.Name.Equals(consumption.MaterialName))

    let updatedMaterial = { mat with Stock = mat.Stock + consumption.Amount }

    { Materials =
          updatedMaterial :: materialsInWarehouse
          |> List.filter (fun material -> not <| material.Equals(mat))
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse |> List.filter (fun c -> not <| c.Equals(consumption)) }


let private emptyWarehosue =
    { Materials = []
      Consumers = []
      Consumptions = [] }

let private initWarehouse : Warehouse =
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
      empty = emptyWarehosue
      init = initWarehouse }


let update (msg: Message) (model: Warehouse) : Warehouse =
    match msg with
    | EmptyWarehouse -> warehouseApi.empty
    | AddMaterial material -> warehouseApi.add model material
    | DeleteMaterial name -> warehouseApi.delete model name
    | AddConsumer consumer -> warehouseApi.addConsumer model consumer
    | DeleteConsumer consumer -> warehouseApi.deleteConsumer model consumer
    | AddConsumption (consumer, material, amount) -> warehouseApi.addConsumption model (consumer, material, amount)
    | DeleteConsumption guid -> warehouseApi.deleteConsumption model guid
    | InitWarehouse -> warehouseApi.init
