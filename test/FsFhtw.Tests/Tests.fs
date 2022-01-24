module FsFhtw.Tests

open Xunit
open FsCheck

[<Fact>]
let ``That the laws of reality still apply`` () =
    Assert.True(1 = 1)

[<Fact>]
let ``That updating stock of material works`` () =
    let initialState = WarehouseFunctions.initialWarehouse
    let warehouse = match initialState with
                    | Domain.Warehouse w ->  w

    let actual =
        WarehouseImplementation.update (Domain.UpdateStock {MaterialName = "Test"; Stock = 10}) warehouse

    let expected = 20

    let warehouseResult = match actual with
                          | Domain.Warehouse w ->  w
    let material = warehouseResult.Materials.Head

    Assert.Equal(expected, material.Stock)
