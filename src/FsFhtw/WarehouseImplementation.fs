module WarehouseImplementation

open Domain
let private addMaterial { Materials = materialsInWarehouse } material =
    { Materials = material :: materialsInWarehouse }

let private deleteMaterial { Materials = materialsInWarehouse } name =
    { Materials = materialsInWarehouse |> List.filter(fun material -> not <| material.Name.Equals(name)) }

let private emptyWarehosue = { Materials = [] }


let warehouseApi : WarehouseApi = {
    add = addMaterial
    delete = deleteMaterial
    empty = emptyWarehosue
}


let update (msg : Message) (model : Warehouse) : Warehouse =
    match msg with
    | EmptyWarehouse -> warehouseApi.empty
    | AddMaterial material -> warehouseApi.add model material
    | DeleteMaterial name -> warehouseApi.delete model name

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
