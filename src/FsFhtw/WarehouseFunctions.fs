module WarehouseFunctions

open Domain
open System

let emptyWarehouse =
    Warehouse
        { Materials = []
          Consumers = []
          Consumptions = [] }


let initialWarehouse =
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

let getWarehouse
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    =
    Warehouse
        { Materials = materialsInWarehouse
          Consumers = consumersInWarehouse
          Consumptions = consumptionsInWarehouse }

let getBelowReportingStock
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
