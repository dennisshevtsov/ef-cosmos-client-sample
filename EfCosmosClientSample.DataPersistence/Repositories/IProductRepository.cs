// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence.Repositories
{
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  using EfCosmosClientSample.DataPersistence.Entities;

  public interface IProductRepository
  {
    public Task<IEnumerable<ProductEntity>> SearchProductsAsync(
      string term,
      float? min,
      float? max,
      IEnumerable<string> tags,
      CancellationToken cancellationToken);
  }
}
