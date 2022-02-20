using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;
using System.Linq;
using System.Data;

namespace UTNApiTalleres.Data.Repositorio
{
    public class ClienteDao : IClienteDao
    {
        private PostgresqlConfiguration _connectionString;


        public ClienteDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<bool> create(Cliente Cliente)
        {
            var db = dbConnection();

            var query = @"INSERT INTO  public.""Clientes""( ""PersonaId"", ""TallerId"")
                        values( @PersonaId, @TallerId )";

            var parameters = new DynamicParameters();

            parameters.Add("PersonaId", Cliente.Persona.Id, DbType.Int32);
            parameters.Add("TallerId",  Cliente.Taller.Id,  DbType.Int32);

            var result = await db.ExecuteAsync(query, parameters);

            return result > 0;

        }


        public async Task<bool> update(Cliente Cliente)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> delete(int id)
        {

            var db = dbConnection();

            var sql_script = @"
                                DELETE 
                                FROM  public.""Clientes""
                                WHERE ""Id"" = @Id  
                            ";

            var result = await db.ExecuteAsync(sql_script, new { Id = id });

            return result > 0;
        }

        public async Task<Cliente> find(int id)
        {          
            var sql_query = @" select  * 
                                from public.""Clientes"" as a 
                                inner join
                                     public.""Personas"" as b
                                      on(a.""Id"" = b.""Id"")
                                inner join 
                                     public.""Talleres"" as c
                                      on(a.""TallerId"" = c.""Id"")
                                where b.""Id"" = @Id";      

            using (var connection = dbConnection())
            {      

                var oCliente =  await connection.QueryAsync<Cliente, Persona, Taller, Cliente>(                             
                             sql_query,                             
                             (cliente, persona, taller) =>
                             {                                 
                                 cliente.Persona = persona;
                                 cliente.Taller = taller;
                                 return cliente;
                             },
                             new { Id = id }
                  
                         )
                   ;
                ;        

                return oCliente.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<Cliente>> findAll()
        {
            var sql_query = @" select  * 
                                from public.""Clientes"" as a 
                                inner join
                                     public.""Personas"" as b
                                      on(a.""Id"" = b.""Id"")
                                inner join 
                                     public.""Talleres"" as c
                                      on(a.""TallerId"" = c.""Id"")
                              ";
            

            using (var connection = dbConnection())
            {                

                var oCliente = await connection.QueryAsync<Cliente, Persona, Taller, Cliente>(

                             sql_query,
                             (cliente, persona, taller) =>
                             {
                                 cliente.Taller = taller;
                                 cliente.Persona = persona;
                                 
                                 return cliente;
                             }                         
                         )
                   ;
                ;

                return oCliente;
            }
        }
    }
}

