// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence.Testing.FunctionalTests
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using EfCosmosClientSample.DataPersistence.Extensions;
  using EfCosmosClientSample.DataPersistence.Repositories;

  [TestClass]
  public class ProductRepositoryTests
  {
    private IDisposable _disposable;
    private IProductRepository _productRepository;

    [TestInitialize]
    public void Initialize()
    {
      var services = new ServiceCollection();

      services.AddDataPersistance(options =>
      {
        options.AccountEndpoint = "https://localhost:8081";
        options.AccountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        options.DatabaseName = "devtest";
      });

      var serviceProvider = services.BuildServiceProvider();

      _disposable = serviceProvider;
      _productRepository = serviceProvider.GetRequiredService<IProductRepository>();
    }

    [TestCleanup]
    public void Cleanup() => _disposable?.Dispose();

    [TestMethod]
    public async Task TestMethod1()
    {
      var products = await _productRepository.SearchProductsAsync(
        "test", null, null, null, CancellationToken.None);
    }
  }
}
