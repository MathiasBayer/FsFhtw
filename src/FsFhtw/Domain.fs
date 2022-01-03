module Domain

type Material =
    { Name: string
      Price: double
      Weight: double
      Stock: int
      ReportingStock: int }

type Warehouse = { Materials : list<Material> }

type AddMaterial = Warehouse -> Material -> Warehouse

type Message =
    | EmptyWarehouse

type WarehouseApi  =
    {
        add: AddMaterial
        empty: Warehouse
    }

