module Program

open Pulumi
open Pulumi.FSharp
open Pulumi.AzureNative.Storage
open Pulumi.AzureNative.Storage.Inputs

let createResources() =
    let account = StorageAccount("storageAccount",
        StorageAccountArgs(
            ResourceGroupName = "resourceGroup",
            Sku = input (SkuArgs(Name = SkuName.Standard_LRS)),
            Kind = Kind.StorageV2
        )
    )

    ignore account

type TestStack() = 
    inherit Stack()
    do createResources()

type Mocks() = 
    interface Testing.IMocks with
        member this.CallAsync(args) = task { return args.Args }
        member this.NewResourceAsync(args) =  task { return struct(args.Id, args.Inputs) }

[<EntryPoint>]
let main (args: string[]) = 
    let resources = 
        Deployment
            .TestAsync<TestStack>(Mocks())
            .GetAwaiter()
            .GetResult()

    printfn $"Created {resources.Length} resources"
    0