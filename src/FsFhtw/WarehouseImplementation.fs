module WarehouseImplementation

open Domain
let private addMaterial { Materials = materialsInWarehouse } material =
    { Materials = material :: materialsInWarehouse }

let private emptyWarehosue = { Materials = [] }


let warehouseApi : WarehouseApi = {
    add = addMaterial
    empty = emptyWarehosue
}


let update (msg : Message) (model : Warehouse) : Warehouse =
    match msg with
    | EmptyWarehouse -> warehouseApi.empty
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
