using System;
using Azure.Data.Tables;

namespace az204_test;

public static class TableClientFactory
{
    public static TableClient GetClient()
    {
        string storageConnectionString = "<<SomeConnectionString>>";
        var serviceClient = new TableServiceClient(storageConnectionString);
        return serviceClient.GetTableClient("people");
    }
}
