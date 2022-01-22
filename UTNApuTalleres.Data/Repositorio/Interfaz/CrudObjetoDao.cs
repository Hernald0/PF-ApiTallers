using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public abstract class CrudObjetoDao : ICrudObjeto<object>
    {

        private PostgresqlConfiguration _connectionString;


        public  CrudObjetoDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public void create(object obj)
        {
            throw new NotImplementedException();
        }

        public int delete(int id)
        {
            throw new NotImplementedException();
        }

        public bool find(object obj)
        {
            throw new NotImplementedException();
        }

        public object find(int id)
        {
            throw new NotImplementedException();
        }

        public List<object> findAll()
        {
            throw new NotImplementedException();
        }

        public void update(object obj)
        {
            throw new NotImplementedException();
        }
        

    }
}
