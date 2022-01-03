module Domain

type Material =
    { Name: string
      Price: float
      Weight: float
      Stock: int
      ReportingStock: int }

type Warehouse = { Materials : list<Material> }

type AddMaterial = Warehouse -> Material -> Warehouse
type DeleteMaterial = Warehouse -> string -> Warehouse

type Message =
    | EmptyWarehouse
    | AddMaterial of Material
    | DeleteMaterial of string

type WarehouseApi  =
    {
        add: AddMaterial
        delete: DeleteMaterial
        empty: Warehouse
    }

