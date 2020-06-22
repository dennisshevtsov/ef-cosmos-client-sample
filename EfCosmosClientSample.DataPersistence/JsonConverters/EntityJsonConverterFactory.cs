// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence.JsonConverters
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Text.Json;
  using System.Text.Json.Serialization;

  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata;

  public sealed class EntityJsonConverterFactory : JsonConverterFactory
  {
    private readonly ConcurrentDictionary<Type, JsonConverter> _converterDictionary;
    private readonly IModel _model;

    public EntityJsonConverterFactory(IModel model)
    {
      _converterDictionary = new ConcurrentDictionary<Type, JsonConverter>();
      _model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public override bool CanConvert(Type typeToConvert)
      => _converterDictionary.ContainsKey(typeToConvert);

    public override JsonConverter CreateConverter(
      Type typeToConvert, JsonSerializerOptions options)
    {
      JsonConverter converter = null;

      if (!_converterDictionary.TryGetValue(typeToConvert, out converter))
      {
        var entityType = _model.FindEntityType(typeToConvert);

        if (entityType != null)
        {
          var converterType = typeof(EntityJsonConverter<>).MakeGenericType(entityType.ClrType);
          var propertyDictionary = new Dictionary<string, PropertyInfo>();
          var properties = entityType.GetProperties()
                                     .Where(property => property.PropertyInfo != null);

          foreach (var property in properties)
          {
            propertyDictionary.Add(property.GetPropertyName(), property.PropertyInfo);
          }

          var navigations = entityType.GetNavigations();

          foreach (var navigation in navigations)
          {
            var targetType = navigation.GetTargetType();

            propertyDictionary.Add(targetType.GetContainingPropertyName(),
                                   navigation.PropertyInfo);
          }

          converter = Activator.CreateInstance(converterType, propertyDictionary) as JsonConverter;
          _converterDictionary.TryAdd(typeToConvert, converter);
        }
      }

      return converter;
    }
  }
}
