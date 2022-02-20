using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio
{
    public class TallerDao : ITallerDao
    {
        private PostgresqlConfiguration _connectionString;


        public TallerDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }
        public async Task<bool> create(Taller taller)
        {
            var db = dbConnection();

            var sql_insert = @"
                            INSERT INTO public.""Talleres""(""NombreTaller"", ""IdPertonatit"")
                            VALUES ( @NombreTaller, @IdPersona)
                            ";

            var result = await db.ExecuteAsync(sql_insert, new
            {
                NombreTaller = taller.Nombretaller,
                IdPersona = taller.PersonaTitular.Id
            });

            return result > 0;
        }

        public Task<bool> delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Taller> find(int id)
        {
            var db = dbConnection();

            var sql_query = @"
                            SELECT *
                            FROM ""Personas"" as p
                            inner join ""Talleres"" as t
                                on(p.""Id"" = t.""ReferenteId"");
                            WHERE  t.""Id""= @Id ";

            using (var connection = dbConnection())
            {

                var oTaller = await connection.QueryAsync<Taller, Persona, Taller>(
                             sql_query,
                             (taller,persona) =>
                             {
                                 taller.PersonaTitular = persona;
                                
                                 return taller;
                             },
                             new { Id = id }

                         )
                   ;
                
                return oTaller.FirstOrDefault();
            }


        }

        public async Task<IEnumerable<Taller>> findAll()
        {
            var db = dbConnection();

            var sql_query = @"
                            SELECT *
                            FROM ""Personas"" as p
                            inner join ""Talleres"" as t
                                on(p.""Id"" = t.""ReferenteId"");
                             ";

            using (var connection = dbConnection())
            {

                var oTaller = await connection.QueryAsync<Taller, Persona, Taller>(
                             sql_query,
                             (taller, persona) =>
                             {
                                 taller.PersonaTitular = persona;

                                 return taller;
                             } 
                         )
                   ;

                return oTaller;
            }
        }

        public Task<bool> update(Taller Taller)
        {
            throw new NotImplementedException();
        }
    }
}
