using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiTalleres.Models;
using Dapper;
using UTNApiTalleres.Data.Repositorio.Interfaz;

namespace UTNApiTalleres.Data.Repositorio
{
    public class AseguradoraDao : IAseguradoraDao
    {

        private PostgresqlConfiguration _connectionString;


        public AseguradoraDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public Task<bool> create(Aseguradora aseguradora)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> update(Aseguradora aseguradora)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Aseguradora> find(int id)
        {
            var db = dbConnection();

            var command = @" select id, nombre
                             from aseguradora
                             where id = @Id ";

            return await db.QueryFirstOrDefaultAsync<Aseguradora>(command, new {Id = id });
        }

        public async Task<IEnumerable<Aseguradora>> findAll()
        {
            var db = dbConnection();

            var command = @" select ""Id"", ""Nombre""
                             from public.""Aseguradoras""
                            ";

            return await db.QueryAsync<Aseguradora>(command, new { });
        }

      
    }
}
