// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence
{
  using Microsoft.EntityFrameworkCore;

  using EfCosmosClientSample.DataPersistence.Configurations;

  public sealed class DataPersistenceDbContext : DbContext
  {
    public DataPersistenceDbContext(DbContextOptions<DataPersistenceDbContext> options)
      : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
      => builder.ApplyConfiguration(new ProductEntityTypeConfiguration());
  }
}
