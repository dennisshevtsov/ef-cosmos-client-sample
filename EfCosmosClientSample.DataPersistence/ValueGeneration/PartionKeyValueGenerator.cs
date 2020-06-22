// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence.ValueGeneration
{
  using Microsoft.EntityFrameworkCore.ChangeTracking;
  using Microsoft.EntityFrameworkCore.ValueGeneration;

  public sealed class PartionKeyValueGenerator : ValueGenerator
  {
    public override bool GeneratesTemporaryValues => false;

    protected override object NextValue(EntityEntry entry) => entry.Entity.GetType().Name;
  }
}
