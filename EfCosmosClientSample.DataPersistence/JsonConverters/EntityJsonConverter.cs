// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.

namespace EfCosmosClientSample.DataPersistence.JsonConverters
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Text.Json;
  using System.Text.Json.Serialization;

  internal sealed class EntityJsonConverter<TEntity>
    : JsonConverter<TEntity>
    where TEntity : class
  {
    private readonly IDictionary<string, PropertyInfo> _propertyDictionary;

    public EntityJsonConverter(IDictionary<string, PropertyInfo> propertyDictionary)
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
              if (_propertyDictionary.TryGetValue(propertyName, out var property))
              {
                object propertyValue = null;

                Type propertyTypeToConvert;

                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                  propertyTypeToConvert = property.PropertyType.GetGenericArguments()[0];
                }
                else
                {
                  propertyTypeToConvert = property.PropertyType;
                }

                var converter = options.GetConverter(propertyTypeToConvert);

                if (converter != null)
                {
                  propertyValue = JsonSerializer.Deserialize(ref reader, property.PropertyType, options);
                }
                else if (reader.Read())
                {
                  if ((property.PropertyType == typeof(int) ||
                       property.PropertyType == typeof(int?)) &&
                      reader.TryGetInt32(out var intPropertyValue))
                  {
                    propertyValue = intPropertyValue;
                  }
                  else if ((property.PropertyType == typeof(float) ||
                            property.PropertyType == typeof(float?)) &&
                           reader.TryGetSingle(out var floatPropertyValue))
                  {
                    propertyValue = floatPropertyValue;
                  }
                  else if ((property.PropertyType == typeof(DateTime) ||
                            property.PropertyType == typeof(DateTime?)) &&
                           reader.TryGetDateTime(out var dateTimePropertyValue))
                  {
                    propertyValue = dateTimePropertyValue;
                  }
                  else if ((property.PropertyType == typeof(Guid) ||
                            property.PropertyType == typeof(Guid?)) &&
                           reader.TryGetGuid(out var guidPropertyValue))
                  {
                    propertyValue = guidPropertyValue;
                  }
                }

                if (propertyValue != null)
                {
                  property.SetValue(entity, propertyValue);
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
