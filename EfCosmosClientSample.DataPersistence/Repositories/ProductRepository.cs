// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence.Repositories
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json;
  using System.Threading;
  using System.Threading.Tasks;

  using Microsoft.Azure.Cosmos;
  using Microsoft.EntityFrameworkCore;

  using EfCosmosClientSample.DataPersistence.Entities;
  using EfCosmosClientSample.DataPersistence.JsonConverters;
  using Microsoft.Extensions.Options;
  using System.Text.Json.Serialization;

  public sealed class ProductRepository : IProductRepository
  {
    private readonly Func<DbContext> _dbContextProvider;
    private readonly CosmosDbOptions _dbOptions;

    public ProductRepository(
      Func<DbContext> dbContextProvider,
      IOptions<CosmosDbOptions> dbOptions)
    {
      _dbContextProvider = dbContextProvider
        ?? throw new ArgumentNullException(nameof(dbContextProvider));
      _dbOptions = dbOptions?.Value
        ?? throw new ArgumentNullException(nameof(dbOptions));
    }

    public async Task<IEnumerable<ProductEntity>> SearchProductsAsync(
      string term,
      float? min,
      float? max,
      IEnumerable<string> tags,
      CancellationToken cancellationToken)
    {
      var context = _dbContextProvider.Invoke();
      var client = context.Database.GetCosmosClient();
      var database = client.GetDatabase(_dbOptions.DatabaseName);

      var productEntities = new List<ProductEntity>();

      if (database != null)
      {
        var entityType = context.Model.FindEntityType(typeof(ProductEntity));

        if (entityType != null)
        {
          var containerId = entityType.GetContainer();
          var container = database.GetContainer(containerId);

          if (container != null)
          {
            var queryDefinition = new QueryDefinition(
              "SELECT * FROM c WHERE CONTAINS(c.name, @term)")
              .WithParameter("@term", term);
            var feedIterator = container.GetItemQueryStreamIterator(
              queryDefinition,
              null,
              new QueryRequestOptions
              {
                PartitionKey = new PartitionKey(nameof(ProductEntity)),
              });

            while (feedIterator.HasMoreResults)
            {
               var propertyDictionary =
                 entityType.GetProperties()
                           .Where(property => property.PropertyInfo != null)
                           .ToDictionary(property => property.GetPropertyName(),
                                         property => property);
              var jsonSerializerOptions = new JsonSerializerOptions
              {
                Converters =
                {
                  new EntityJsonConverter<ProductEntity>(propertyDictionary),
                },
              };

              using (var responseMessage = await feedIterator.ReadNextAsync(cancellationToken))
              {
                var productEntityBatch =
                  await JsonSerializer.DeserializeAsync<CosmosResponse<ProductEntity>>(
                    responseMessage.Content,
                    jsonSerializerOptions,
                    cancellationToken);

                productEntities.AddRange(productEntityBatch.Documents);
              }
            }
          }
        }
      }

      return productEntities;
    }
  }

  public sealed class CosmosResponse<T>
  {
    [JsonPropertyName("Documents")]
    public IEnumerable<T> Documents { get; set; }
  }
}
