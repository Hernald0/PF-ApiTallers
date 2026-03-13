using System;
using System.Collections.Generic;
using System.Text;

namespace UTNApiTalleres.Data
{
    
    public class PostgresqlConfiguration
    {
        public string ConnectionString { set; get; }

        //public PostgresqlConfiguration(string connectionString) => ConnectionString = connectionString;
        public PostgresqlConfiguration(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString),
                    "La connection string de PostgreSQL es null o vacía");

            ConnectionString = connectionString;
        }

    }
}
