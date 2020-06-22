// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence.JsonConverters
{
  using System;
  using System.Collections.Generic;
  using System.Text.Json;
  using System.Text.Json.Serialization;

  using Microsoft.EntityFrameworkCore.Metadata;

  internal sealed class EntityJsonConverter<TEntity>
    : JsonConverter<TEntity>
    where TEntity : class
  {
    private readonly IDictionary<string, IProperty> _propertyDictionary;

    public EntityJsonConverter(IDictionary<string, IProperty> propertyDictionary)
    {
      _propertyDictionary = propertyDictionary ?? throw new ArgumentNullException(nameof(propertyDictionary));
    }

    public override TEntity Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options)
    {
      TEntity entity = null;

      if (reader.TokenType == JsonTokenType.StartObject)
      {
        var braces = 1;

        entity = Activator.CreateInstance<TEntity>();

        while (braces != 0 && reader.Read())
        {
          if (reader.TokenType == JsonTokenType.PropertyName)
          {
            var propertyName = reader.GetString();

            if (!string.IsNullOrWhiteSpace(propertyName))
            {
              if (_propertyDictionary.TryGetValue(propertyName, out var property) &&
                  reader.Read())
              {
                object propertyValue = null;

                if (property.ClrType == typeof(string))
                {
                  propertyValue = reader.GetString();
                }
                else if ((property.ClrType == typeof(int) ||
                          property.ClrType == typeof(int?)) &&
                         reader.TryGetInt32(out var intPropertyValue))
                {
                  propertyValue = intPropertyValue;
                }
                else if ((property.ClrType == typeof(float) ||
                          property.ClrType == typeof(float?)) &&
                         reader.TryGetSingle(out var floatPropertyValue))
                {
                  propertyValue = floatPropertyValue;
                }
                else if ((property.ClrType == typeof(DateTime) ||
                          property.ClrType == typeof(DateTime?)) &&
                         reader.TryGetDateTime(out var dateTimePropertyValue))
                {
                  propertyValue = dateTimePropertyValue;
                }
                else if ((property.ClrType == typeof(Guid) ||
                          property.ClrType == typeof(Guid?)) &&
                         reader.TryGetGuid(out var guidPropertyValue))
                {
                  propertyValue = guidPropertyValue;
                }

                if (propertyValue != null)
                {
                  property.PropertyInfo.SetValue(entity, propertyValue);
                }
              }
            }
          }
          else if (reader.TokenType == JsonTokenType.StartObject)
          {
            ++braces;
          }
          else if (reader.TokenType == JsonTokenType.EndObject)
          {
            --braces;
          }
        }
      }

      return entity;
    }

    public override void Write(Utf8JsonWriter writer, TEntity value, JsonSerializerOptions options)
      => throw new NotImplementedException();
  }
}
