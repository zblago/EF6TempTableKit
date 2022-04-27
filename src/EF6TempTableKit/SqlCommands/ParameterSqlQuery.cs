using System.Data.Entity.Core.Objects;

namespace EF6TempTableKit.SqlCommands
{
    /// <summary>
    /// Sql Query with its parameters
    /// </summary>
    internal class ParameterSqlQuery
    {
        /// <summary>The SQL String</summary>
        public string Sql { get; }

        /// <summary>The Parameters for this query</summary>
        public ObjectParameter[] Parameters { get; }

        public ParameterSqlQuery(string sql, ObjectParameterCollection parameters)
        {
            this.Sql = sql;
            // Copy the parameters as original collection might change after we created this 
            var copiedParameter = new ObjectParameter[parameters.Count];
            parameters.CopyTo(copiedParameter, 0);
            this.Parameters = copiedParameter;
        }
    }
}