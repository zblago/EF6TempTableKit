using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EF6TempTableKit
{
    public sealed class TableMetadataProvider
    {
        public string GetTableNameFromBaseType(Type baseType)
        {
            var tempTableName = (baseType.GetCustomAttributes(typeof(TableAttribute), true).Single() as TableAttribute).Name;

            return tempTableName;
        }

        public IReadOnlyDictionary<string, string> GetFieldsWithTypes(Type baseType)
        {
            var fieldsWithTempFieldTypeAttribute = baseType
                .GetProperties()
                .Where(p => p.CustomAttributes.Any(ca => ca.AttributeType == typeof(Attributes.TempFieldTypeAttribute)));

            var fieldsAndTypes = fieldsWithTempFieldTypeAttribute.Select(f => new
            {
                f.Name,
                Type = (f.GetCustomAttributes(typeof(Attributes.TempFieldTypeAttribute), true)
                    .Single() as Attributes.TempFieldTypeAttribute)
                    .FieldType
            }).ToDictionary(ft => ft.Name, ft => ft.Type);

            return fieldsAndTypes;
        }

        public string[] GetClusteredIndexColumns(Type baseType)
        {
            var fieldsWithClusteredIndexAttribute = baseType
                .GetProperties()
                .Where(p => p.CustomAttributes.Any(ca => ca.AttributeType == typeof(Attributes.ClusteredIndexAttribute)));

            var fieldsForClusteredIndex = fieldsWithClusteredIndexAttribute.Select(f => f.Name).ToArray();

            return fieldsForClusteredIndex;
        }

        public IReadOnlyDictionary<string, string[]> GetNonClusteredIndexesWithColumns(Type baseType)
        {
            var propsWithnonClusteredAttributes = baseType
                .GetProperties()
                .Where(p => p.CustomAttributes.Any(ca => ca.AttributeType == typeof(Attributes.NonClusteredIndexAttribute)));

            var listOfIndexes = propsWithnonClusteredAttributes
                .SelectMany(p => p.GetCustomAttributes(typeof(Attributes.NonClusteredIndexAttribute), true)
                        .Select(a => (a as Attributes.NonClusteredIndexAttribute).Name))
                .Distinct();

            IDictionary<string, string[]> indexWithFields = new Dictionary<string, string[]>();

            foreach (var indexName in listOfIndexes)
            {
                var properytNames = baseType.GetProperties()
                    .Where(p =>
                        (p.GetCustomAttributes(typeof(Attributes.NonClusteredIndexAttribute), true) 
                            as IEnumerable<Attributes.NonClusteredIndexAttribute>)
                        .Any(nona => nona.Name == indexName))
                    .Select(p => p.Name)
                    .ToArray();

                indexWithFields.Add(indexName, properytNames);
            }

            return (IReadOnlyDictionary<string, string[]>) indexWithFields;
        }
    }
}
