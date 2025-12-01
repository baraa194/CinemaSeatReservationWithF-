open System
open System.IO
open System.Text.RegularExpressions
open Microsoft.Data.SqlClient
open Microsoft.Extensions.Configuration

let loadConnectionString () =
    try
        let config =
            ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional = false, reloadOnChange = false)
                .Build()

        let cs = config.GetConnectionString("Default")
        printfn "Loaded connection string: %s" cs
        cs
    with ex ->
        printfn "ERROR loading connection string: %s" ex.Message
        raise ex

let execSqlScript (connStr:string) (scriptPath:string) =
    if not (File.Exists scriptPath) then
        printfn "ERROR: Script not found: %s" scriptPath
    else
        try
            printfn "Reading SQL script..."
            let script = File.ReadAllText(scriptPath)

            let batches =
                Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline ||| RegexOptions.IgnoreCase)
                |> Array.map (fun x -> x.Trim())
                |> Array.filter (fun x -> x <> "")

            printfn "Connecting to SQL Server..."
            use conn = new SqlConnection(connStr)
            conn.Open()
            printfn "Connected!"

            printfn "Executing batches (%d total)..." batches.Length
            for i = 0 to batches.Length - 1 do
                printfn "Executing batch %d..." (i+1)
                use cmd = new SqlCommand(batches.[i], conn)
                cmd.ExecuteNonQuery() |> ignore

            printfn "All batches executed successfully."
        with ex ->
            printfn "ERROR executing SQL script: %s" ex.Message

[<EntryPoint>]
let main argv =
    printfn "Starting database initialization..."

    let connStr = loadConnectionString()

    let scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "init.sql")
    execSqlScript connStr scriptPath

    printfn "Done. Press ENTER to exit..."
    Console.ReadLine() |> ignore
    0
