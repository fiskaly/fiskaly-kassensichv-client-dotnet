Imports System
Imports Fiskaly.Client
Imports System.Net.Http
Imports Serilog

Module Program
    Sub Main(args As String())
        Log.Logger = New LoggerConfiguration().WriteTo.Console().CreateLogger()
        Dim apiKey as String = Environment.GetEnvironmentVariable("API_KEY")
        Dim apiSecret as String  = Environment.GetEnvironmentVariable("API_SECRET")
        Dim client As HttpClient = ClientFactory.Create(apiKey, apiSecret).AsTask().Result
    End Sub
End Module
