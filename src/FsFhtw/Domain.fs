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
type AddConsumption = Warehouse -> string * string * int -> OperationResult
type DeleteConsumption = Warehouse -> Guid -> OperationResult

type Message =
    | EmptyWarehouse
    | AddMaterial of Material
    | DeleteMaterial of string
    | AddConsumer of Consumer
    | DeleteConsumer of Consumer
    | AddConsumption of string * string * int
    | DeleteConsumption of Guid
    | InitWarehouse

type WarehouseApi =
    { add: AddMaterial
      delete: DeleteMaterial
      addConsumer: AddConsumer
      deleteConsumer: DeleteConsumer
      addConsumption: AddConsumption
      deleteConsumption: DeleteConsumption
      empty: OperationResult
      init: OperationResult }
