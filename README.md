# F# implicit call bug

Reproduce the bug: 

In `Program.fs` there is this function
```fs
let createResources() =
    let account = StorageAccount("storageAccount",
        StorageAccountArgs(
            ResourceGroupName = "resourceGroup",
            Sku = input (SkuArgs(Name = SkuName.Standard_LRS)),
            Kind = Kind.StorageV2
        )
    )

    ignore account
```
Notice the line `Sku = input (SkuArgs(Name = SkuName.Standard_LRS))` requires an explicit `input` function but since F# 6.0, we can just write it without `input` like this
```fs
let createResources() =
    let account = StorageAccount("storageAccount",
        StorageAccountArgs(
            ResourceGroupName = "resourceGroup",
            Sku = SkuArgs(Name = SkuName.Standard_LRS),
            Kind = Kind.StorageV2
        )
    )

    ignore account
```
However, once you run this program, it fails with the following error
```
System.ArgumentNullException: [Input] Pulumi.AzureNative.Storage.Inputs.SkuArgs.Name is required but was not given a value (Parameter 'Name')
   at async Task<ImmutableDictionary<string, object>> Pulumi.InputArgs.ToDictionaryAsync()
   at async Task<ImmutableDictionary<string, object>> Pulumi.Serialization.Serializer.SerializeInputArgsAsync(string ctx, InputArgs args, bool keepResources, bool keepOutputValues)
   at async Task<object> Pulumi.Serialization.Serializer.SerializeAsync(string ctx, object prop, bool keepResources, bool keepOutputValues) x 5
   at async Task<RawSerializationResult> Pulumi.Deployment.SerializeFilteredPropertiesRawAsync(string label, IDictionary<string, object> args, Predicate<string> acceptKey, bool keepResources, bool keepOutputValues)
   at async Task<SerializationResult> Pulumi.Deployment.SerializeFilteredPropertiesAsync(string label, IDictionary<string, object> args, Predicate<string> acceptKey, bool keepResources, bool keepOutputValues)
   at async Task<PrepareResult> Pulumi.Deployment.PrepareResourceAsync(string label, Resource res, bool custom, bool remote, ResourceArgs args, ResourceOptions options)
   at async Task<(string urn, string id, Struct data, ImmutableDictionary<string, ImmutableHashSet<Resource>> dependencies)> Pulumi.Deployment.RegisterResourceAsync(Resource resource, bool remote, Func<string, Resource> newDependency, ResourceArgs args, ResourceOptions options)
   at async Task<(string urn, string id, Struct data, ImmutableDictionary<string, ImmutableHashSet<Resource>> dependencies)> Pulumi.Deployment.ReadOrRegisterResourceAsync(Resource resource, bool remote, Func<string, Resource> newDependency, ResourceArgs args, ResourceOptions options)
   at async Task Pulumi.Deployment.CompleteResourceAsync(Resource resource, bool remote, Func<string, Resource> newDependency, ResourceArgs args, ResourceOptions options, ImmutableDictionary<string, IOutputCompletionSource> completionSources)
   at async Task<T> Pulumi.Output<T>.GetValueAsync(T whenUnknown)
   at async Task<string> Pulumi.Deployment+EngineLogger.TryGetResourceUrnAsync(Resource resource)
   at Pulumi.Deployment.TestAsync(IMocks mocks, Func`2 runAsync, TestOptions options)
```
Saying that parameter 'Name' wasn't provided but in fact we did provide it in `SkuArgs(Name = SkuName.Standard_LRS)`

If you bring back `input (SkuArgs(Name = SkuName.Standard_LRS))` then the program works just fine and prints out
```
Created 2 resources
```