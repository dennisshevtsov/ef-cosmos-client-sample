// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence
{
  public sealed class CosmosDbOptions
  {
    public string DatabaseName { get; set; }

    public string AccountEndpoint { get; set; }

    public string AccountKey { get; set; }
  }
}
