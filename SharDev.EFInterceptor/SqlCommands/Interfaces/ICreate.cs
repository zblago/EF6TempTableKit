using System.Collections.Generic;

namespace SharDev.EFInterceptor.SqlCommands.Interfaces
{
    public interface ICreate
    {
        IInsertQuery Create(IReadOnlyDictionary<string, string> fieldsWithTypes);
    }
}
