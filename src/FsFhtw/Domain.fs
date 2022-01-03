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

type AddMaterial = Warehouse -> Material -> Warehouse
type AddConsumer = Warehouse -> Consumer -> Warehouse
type DeleteMaterial = Warehouse -> string -> Warehouse
type DeleteConsumer = Warehouse -> Consumer -> Warehouse
type AddConsumption = Warehouse -> string * string * int -> Warehouse
type DeleteConsumption = Warehouse -> Guid -> Warehouse

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
      empty: Warehouse
      init: Warehouse }
