module MaterialFunctions

open Domain

let addMaterial
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    material
    =
    Warehouse
        { Materials = material :: materialsInWarehouse
          Consumers = consumersInWarehouse
          Consumptions = consumptionsInWarehouse }

let deleteMaterial
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    name
    =
    Warehouse
        { Materials =
              materialsInWarehouse
              |> List.filter (fun material -> not <| material.Name.Equals(name))
          Consumers = consumersInWarehouse
          Consumptions = consumptionsInWarehouse }

let findMaterialForName (materials: List<Material>) name =
    materials
    |> List.tryFind (fun m -> m.Name.Equals(name))

let private updateMaterialCommon
    { Materials = materialsInWarehouse
      Consumers = consumersInWarehouse
      Consumptions = consumptionsInWarehouse }
    materialName
    updateFunction
    =
    let mat =
        findMaterialForName materialsInWarehouse materialName

    match mat with
    | None -> ConsumptionFailures(MaterialNotFoundFailure("Material " + materialName + " not found."))
    | Some material ->
        let updatedMaterial = updateFunction material

        Warehouse
            { Materials =
                  updatedMaterial :: materialsInWarehouse
                  |> List.filter (fun mat -> not <| material.Equals(mat))
              Consumers = consumersInWarehouse
              Consumptions = consumptionsInWarehouse }

let updatePrice (model: Warehouse) (request: UpdatePriceRequest) =
    updateMaterialCommon model request.MaterialName (fun material -> { material with Price = request.Price })

let updateStock (model: Warehouse) request =
    updateMaterialCommon
        model
        request.MaterialName
        (fun material ->
            { material with
                  Stock = request.Stock + material.Stock })
