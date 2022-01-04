module Domain
open System

type Material =
    { Name: string
      Price: float
      Weight: float
      Stock: int
      ReportingStock: int }

type Consumer = { Name: string }

type Consumption =
    { Id: Guid
      Consumer: Consumer
      MaterialName: string
      Amount: int
      Price: float }

type ConsumptionRequest =
    { Consumer: string
      MaterialName: string
      Amount: int }

type UpdatePriceRequest =
    { MaterialName: string
      Price: float }

type Warehouse =
    { Materials: list<Material>
      Consumers: list<Consumer>
      Consumptions: list<Consumption> }

type ConsumptionFailures =
    | MaterialNotFoundFailure of string
    | NotEnoughMaterialInStockFailure of string
    | ConsumerNotFoundFailure of string

type OperationResult =
    | Warehouse of Warehouse
    | ConsumptionFailures of ConsumptionFailures

type AddMaterial = Warehouse -> Material -> OperationResult
type AddConsumer = Warehouse -> Consumer -> OperationResult
type DeleteMaterial = Warehouse -> string -> OperationResult
type DeleteConsumer = Warehouse -> Consumer -> OperationResult
type AddConsumption = Warehouse -> ConsumptionRequest -> OperationResult
type DeleteConsumption = Warehouse -> Guid -> OperationResult
type UpdatePrice = Warehouse -> UpdatePriceRequest -> OperationResult
type GetBelowReportingStock = Warehouse -> OperationResult
type GetWarehouse = Warehouse -> OperationResult

type Message =
    | EmptyWarehouse
    | AddMaterial of Material
    | DeleteMaterial of string
    | AddConsumer of Consumer
    | DeleteConsumer of Consumer
    | AddConsumption of ConsumptionRequest
    | DeleteConsumption of Guid
    | InitWarehouse
    | UpdatePrice of UpdatePriceRequest
    | GetBelowReportingStock
    | GetWarehouse

type WarehouseApi =
    { add: AddMaterial
      delete: DeleteMaterial
      addConsumer: AddConsumer
      deleteConsumer: DeleteConsumer
      addConsumption: AddConsumption
      deleteConsumption: DeleteConsumption
      updatePrice: UpdatePrice
      getBelowReportingStock: GetBelowReportingStock
      getWarehouse: GetWarehouse
      empty: OperationResult
      init: OperationResult }
