// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence.Entities
{
  using System;
  using System.Collections.Generic;

  public sealed class ProductEntity
  {
    public Guid ProductId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public float Price { get; set; }

    public bool Enabled { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; }

    public IEnumerable<ProductTagEntity> Tags { get; set; }
  }
}
