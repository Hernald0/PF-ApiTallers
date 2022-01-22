using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

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

        public Task<bool> create(Cliente Cliente)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> update(Cliente Cliente)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Cliente> find(int id)
        {
            var db = dbConnection();

            var command = @" select id, nombre
                             from Cliente
                             where id = @Id ";
             
            return await db.QueryFirstOrDefaultAsync<Cliente>(command, new { Id = id }); 

            //Cliente oCliente = await db.QueryFirstOrDefaultAsync<Cliente>(command, new { Id = id });

            //return Helper.buscarDatos(oCliente);
        }

        public async Task<IEnumerable<Cliente>> findAll()
        {
            var db = dbConnection();

            var command = @" select ""Id"", ""Nombre""
                             from public.""Clientes""
                            ";

            return await db.QueryAsync<Cliente>(command, new { });
        }
    }
}

/*
public class Helper
{

    private PostgresqlConfiguration _connectionString;


    public HelperDao(PostgresqlConfiguration connectionString)
    {
        this._connectionString = connectionString;
    }

    protected NpgsqlConnection dbConnection()
    {
        return new NpgsqlConnection(this._connectionString.ConnectionString);
    }

    public static Object buscarDatos(Object oModelo)
    {

        var db = dbConnection();

        switch (oModelo.ToString())
        {
            case "Personas":
                

                var command = @" select ""Id"", ""Nombre""
                             from public.""Personas""
                             where Id = ";

                return await db.QueryAsync<Persona>(command, new { });
                break;

        }
    }
}*/
