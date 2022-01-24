module ConsumerFunctions

open Domain

let addConsumer
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    consumer
    =
    Warehouse
        { Materials = materialsInWarehouse
          Consumers = consumer :: consumersInWarehouse
          Consumptions = consumptionsInWarehouse }

let deleteConsumer
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
