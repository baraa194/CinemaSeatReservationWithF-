namespace CinemaSeatReservationWithFSharp

open System
open System.IO
open Microsoft.Extensions.Configuration
open System.Data
open Microsoft.Data.SqlClient

module Db =

    let private configuration =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional = true, reloadOnChange = true)
            .AddEnvironmentVariables()
            .Build()

    let getConnectionString() =
       
        configuration.GetConnectionString("Default")

    let getConnection() : IDbConnection =
        let cs = getConnectionString()
        new SqlConnection(cs) :> IDbConnection


