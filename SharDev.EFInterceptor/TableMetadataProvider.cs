using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SharDev.EFInterceptor
{
    public sealed class TableMetadataProvider
    {
        public string GetTableNameFromBaseType(Type baseType)
        {
            var tempTableName = (baseType.GetCustomAttributes(typeof(TableAttribute), true).Single() as TableAttribute).Name;

            return tempTableName;
        }

        public IReadOnlyDictionary<string, string> GetFieldWithPositionsFromBaseType(Type baseType)
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
    }
}
