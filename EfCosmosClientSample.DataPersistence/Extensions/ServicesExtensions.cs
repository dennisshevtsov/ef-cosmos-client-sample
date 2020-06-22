// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence.Extensions
{
  using System;
  using EfCosmosClientSample.DataPersistence.Repositories;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Options;

  public static class ServicesExtensions
  {
    public static IServiceCollection AddDataPersistance(
      this IServiceCollection services,
      Action<CosmosDbOptions> configureOptions)
    {
      if (services == null)
      {
        throw new ArgumentNullException(nameof(services));
      }

      if (configureOptions == null)
      {
        throw new ArgumentNullException(nameof(configureOptions));
      }

      services.Configure(configureOptions);
      services.AddDbContext<DbContext, DataPersistenceDbContext>(
        (provider, options) =>
        {
          var cosmosDbOptions = provider.GetRequiredService<IOptions<CosmosDbOptions>>().Value;

          options.UseCosmos(cosmosDbOptions.AccountEndpoint,
                            cosmosDbOptions.AccountKey,
                            cosmosDbOptions.DatabaseName);
        },
        ServiceLifetime.Transient);
      services.AddScoped<Func<DbContext>>(provider => () => provider.GetRequiredService<DbContext>());

      services.AddScoped<IProductRepository, ProductRepository>();

      return services;
    }
  }
}
