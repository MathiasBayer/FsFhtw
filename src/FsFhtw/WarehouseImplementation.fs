module WarehouseImplementation

open Domain
let private addMaterial { Materials = materialsInWarehouse; Consumers = consumersInWarehouse; Consumptions = consumptionsInWarehouse } material =
    { Materials = material :: materialsInWarehouse; Consumers = consumersInWarehouse; Consumptions = consumptionsInWarehouse }

let private addConsumer{ Materials = materialsInWarehouse; Consumers = consumersInWarehouse; Consumptions = consumptionsInWarehouse } consumer =
    { Materials = materialsInWarehouse; Consumers = consumer :: consumersInWarehouse; Consumptions = consumptionsInWarehouse }

let private deleteMaterial { Materials = materialsInWarehouse; Consumers = consumersInWarehouse; Consumptions = consumptionsInWarehouse } name =
    { Materials = materialsInWarehouse |> List.filter(fun material -> not <| material.Name.Equals(name)); Consumers = consumersInWarehouse; Consumptions = consumptionsInWarehouse }

let private deleteConsumer { Materials = materialsInWarehouse; Consumers = consumersInWarehouse; Consumptions = consumptionsInWarehouse } consumer =
    { Materials = materialsInWarehouse; Consumers = consumersInWarehouse |> List.filter(fun c -> not <| c.Equals(consumer)); Consumptions = consumptionsInWarehouse }

let calculatePrice (price: float, amount: int) = price * float amount

let private addConsumption { Materials = materialsInWarehouse; Consumers = consumersInWarehouse; Consumptions = consumptionsInWarehouse } (consumer, material, amount) =
    let mat = materialsInWarehouse |> List.find(fun m -> m.Name.Equals(material))
    let updatedMaterial = { mat with Stock = mat.Stock - amount }
    let consumption = { Consumer = {Name = consumer}; MaterialName = mat.Name; Amount = amount; Price = calculatePrice(mat.Price, amount)}
    { Materials = updatedMaterial :: materialsInWarehouse |> List.filter(fun material -> not <| material.Equals(mat)); Consumers = consumersInWarehouse; Consumptions = consumption :: consumptionsInWarehouse }


let private emptyWarehosue = { Materials = []; Consumers = []; Consumptions = [] }


let warehouseApi : WarehouseApi = {
    add = addMaterial
    delete = deleteMaterial
    addConsumer = addConsumer
    deleteConsumer = deleteConsumer
    addConsumption = addConsumption
    empty = emptyWarehosue
}


let update (msg : Message) (model : Warehouse) : Warehouse =
    match msg with
    | EmptyWarehouse -> warehouseApi.empty
    | AddMaterial material -> warehouseApi.add model material
    | DeleteMaterial name -> warehouseApi.delete model name
    | AddConsumer consumer -> warehouseApi.addConsumer model consumer
    | DeleteConsumer consumer -> warehouseApi.deleteConsumer model consumer
    | AddConsumption (consumer, material, amount) -> warehouseApi.addConsumption model (consumer,material, amount)

//let private init () : Warehouse =
//    { Materials = [ { Name = "Test"
//        Price = 1
//        Weight = 1
//        Stock = 10
//        ReportingStock = 0 }
//      { Name = "Test2"
//        Price = 1
//        Weight = 1
//        Stock = 10
//        ReportingStock = 20 } ] }
