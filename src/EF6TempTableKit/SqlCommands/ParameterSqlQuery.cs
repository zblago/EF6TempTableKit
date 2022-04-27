using System;
using System.Data.Entity.Core.Objects;
using System.Text;

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

        /// <summary>
        /// Internal Ctor
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        private ParameterSqlQuery(string sql, ObjectParameter[] parameters)
        {
            this.Sql = sql;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Creates new Query with adjusted Parameter names
        /// </summary>
        /// <param name="parameterIndex">Index for Parameters</param>
        /// <returns></returns>
        public ParameterSqlQuery ToAdjustedParameters(int parameterIndex)
        {
            return ToAdjustedParameters(this.Sql, this.Parameters, parameterIndex);
        }

        /// <summary>
        /// Creates new Query with adjusted Parameter names
        /// </summary>
        /// <param name="sql">Sql Statement to adjust</param>
        /// <param name="parameters">Parameters of the sql statement</param>
        /// <param name="parameterIndex">Index for Parameters</param>
        /// <returns></returns>
        public static ParameterSqlQuery ToAdjustedParameters(string sql, ObjectParameter[] parameters, int parameterIndex)
        {
            // No Parameters, no need to adjust
            if (parameters.Length == 0)
            {
                return new ParameterSqlQuery(sql, parameters);
            }

            var sb = new StringBuilder(sql);

            var adjustedParameters = new ObjectParameter[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                // Create new Parameter with adjusted name
                var adjustedName = $"ttk{parameterIndex}{parameter.Name}";
                var adjusted = new ObjectParameter(adjustedName, parameter.Value);
                adjustedParameters[i] = adjusted;

                // Replace in the string
                sb.Replace("@" + parameter.Name, "@" + adjustedName);
            }

            return new ParameterSqlQuery(sb.ToString(), adjustedParameters);
        }
    }
}