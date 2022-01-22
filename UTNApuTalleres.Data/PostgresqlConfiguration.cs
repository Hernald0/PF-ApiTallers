using System;
using System.Collections.Generic;
using System.Text;

namespace UTNApiTalleres.Data
{
    
    public class PostgresqlConfiguration
    {
        public string ConnectionString { set; get; }

        public PostgresqlConfiguration(string connectionString) => ConnectionString = connectionString;


    }
}
