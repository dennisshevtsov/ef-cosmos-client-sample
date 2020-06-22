// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence.Configurations
{
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;
  using Microsoft.EntityFrameworkCore.ValueGeneration;

  using EfCosmosClientSample.DataPersistence.Entities;
  using EfCosmosClientSample.DataPersistence.ValueGeneration;

  internal sealed class ProductEntityTypeConfiguration
    : IEntityTypeConfiguration<ProductEntity>
  {
    private const string PartionKeyName = "modelName";

    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
      builder.ToContainer("cdbcdb");

      builder.HasKey(entity => entity.ProductId);
      builder.HasPartitionKey(PartionKeyName);
      builder.HasDiscriminator(PartionKeyName, typeof(string));

      builder.Property(entity => entity.ProductId).ToJsonProperty("productId").HasValueGenerator<GuidValueGenerator>();
      builder.Property(typeof(string), PartionKeyName).ToJsonProperty(PartionKeyName).HasValueGenerator<PartionKeyValueGenerator>();
      builder.Property(entity => entity.Name).ToJsonProperty("name");
      builder.Property(entity => entity.Description).ToJsonProperty("description");
      builder.Property(entity => entity.Price).ToJsonProperty("price");
      builder.Property(entity => entity.Enabled).ToJsonProperty("enabled");
      builder.Property(entity => entity.CreatedOn).ToJsonProperty("createdOn");
      builder.Property(entity => entity.CreatedBy).ToJsonProperty("createdBy");

      Configure(builder.OwnsMany(entity => entity.Tags));
    }

    private static void Configure(
      OwnedNavigationBuilder<ProductEntity, ProductTagEntity> builder)
    {
      builder.ToJsonProperty("tags");

      builder.Property(entity => entity.Name).ToJsonProperty("name");
    }
  }
}
