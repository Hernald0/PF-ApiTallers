using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
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
        public Task<bool> create(Taller Taller)
        {
            throw new NotImplementedException();
        }

        public Task<bool> delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Taller> find(int id)
        {
            var db = dbConnection();

            var command = @"
                            SELECT P.*,
	                               T.*
                            FROM ""Personas"" as p
                            inner join ""Talleres"" as t

                                on(p.""Id"" = t.""ReferenteId"");
                            WHERE  t.""Id""= @Id ";

            Taller oTaller = new Taller();
            //Taller oTaller = await db.QueryAsync<Taller, Persona, Taller >(command, );
            /*
            Taller taller = db.QueryAsync<Persona, Taller, Persona>
                            ( command,
                            (Persona, Taller) );
            Taller taller = repo.QueryMultiple<Table1, Table2, Table3>(sql,
                                    new { p1 = 1, splitOn = "Table1ID,Table2ID,Table3ID" }).ToList();
            */
            return oTaller;


        }

        public Task<IEnumerable<Taller>> findAll()
        {
            throw new NotImplementedException();
        }

        public Task<bool> update(Taller Taller)
        {
            throw new NotImplementedException();
        }
    }
}
