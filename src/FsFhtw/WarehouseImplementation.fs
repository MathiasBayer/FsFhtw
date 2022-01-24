module WarehouseImplementation

open Domain

let warehouseApi: WarehouseApi =
    { add = MaterialFunctions.addMaterial
      delete = MaterialFunctions.deleteMaterial
      addConsumer = ConsumerFunctions.addConsumer
      deleteConsumer = ConsumerFunctions.deleteConsumer
      addConsumption = ConsumptionFunctions.addConsumption
      deleteConsumption = ConsumptionFunctions.deleteConsumption
      updatePrice = MaterialFunctions.updatePrice
      updateStock = MaterialFunctions.updateStock
      getBelowReportingStock = WarehouseFunctions.getBelowReportingStock
      getWarehouse = WarehouseFunctions.getWarehouse
      empty = WarehouseFunctions.emptyWarehouse
      init = WarehouseFunctions.initialWarehouse }


let update (msg: Message) (model: Warehouse) : OperationResult =
    match msg with
    | EmptyWarehouse -> warehouseApi.empty
    | AddMaterial material -> warehouseApi.add model material
    | DeleteMaterial name -> warehouseApi.delete model name
    | AddConsumer consumer -> warehouseApi.addConsumer model consumer
    | DeleteConsumer consumer -> warehouseApi.deleteConsumer model consumer
    | AddConsumption request -> warehouseApi.addConsumption model request
    | DeleteConsumption guid -> warehouseApi.deleteConsumption model guid
    | UpdatePrice request -> warehouseApi.updatePrice model request
    | UpdateStock request -> warehouseApi.updateStock model request
    | GetBelowReportingStock -> warehouseApi.getBelowReportingStock model
    | GetWarehouse -> warehouseApi.getWarehouse model
    | InitWarehouse -> warehouseApi.init
