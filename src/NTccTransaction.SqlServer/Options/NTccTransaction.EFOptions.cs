using System;

namespace NTccTransaction.SqlServer
{
    public class EFOptions
    {
        public const string DefaultSchema = "";

        /// <summary>
        /// Gets or sets the schema to use when creating database objects.
        /// Default is <see cref="DefaultSchema" />.
        /// </summary>
        public string Schema { get; set; } = DefaultSchema;


        /// <summary>
        /// Data version
        /// </summary>
        public string Version { get; set; }
    }
}