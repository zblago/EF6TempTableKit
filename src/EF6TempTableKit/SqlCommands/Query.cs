using System.Data.Entity.Core.Objects;
using System.Text;

namespace EF6TempTableKit.SqlCommands
{
    internal sealed class Query
    {
        /// <summary>The Sql Query</summary>
        public string SqlQuery { get; set; }

        /// <summary>Only attaching new Data. No DDL</summary>
        public bool IsDataAppend { get; set; }

        /// <summary>The parameters of the query</summary>
        public ObjectParameter[] Parameters { get; set; }
    }
}
