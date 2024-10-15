using System.Formats.Asn1;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using WTelegram;


namespace Worker.ServiceHandler;
public class Connect
{
    // Constructor
    private readonly string _pipeName;
    private readonly string[] _UserConfigReceived;
    private readonly int _apiId;
    private readonly string _apiHash;
    private readonly string _phoneNumber;
    private readonly string _firstName;
    private readonly string _lastName;
    private readonly NamedPipeClientStream _pipe;

    private readonly StreamWriter _writer;
    private readonly StreamReader _reader;


    // Instances
    private WTelegram.Client _client;
    private Worker.Functions.FetchCSVAsync FetchCSVAsync;
    private Worker.Functions.ShillMode ShillMode;

    // INIT Functiom
    public Connect(string pipeName, string[] UserConfigReceived, int apiId, string apiHash, string phoneNumber, string firstName, string lastName, NamedPipeClientStream pipe, StreamWriter writer, StreamReader reader)
    {
        _pipeName = pipeName;
        _UserConfigReceived = UserConfigReceived;
        _apiId = apiId;
        _apiHash = apiHash;
        _phoneNumber = phoneNumber;
        _firstName = firstName;
        _lastName = lastName;
        _pipe = pipe;
        _reader = reader;
        _writer = writer;
    }

    
    public async Task Authenticate()
    {
        
        static string Config(string[] _configReceived, string what)
        {
            switch (what)
            {
                case "api_id": return _configReceived[0];
                case "api_hash":
                    if (_configReceived[1].Length % 2 != 0)
                    {
                        throw new ArgumentException("api_hash must be a hex-encoded string with an even number of characters");
                    }
                    return _configReceived[1];
                case "phone_number": return _configReceived[2];
                case "first_name": return _configReceived[3];
                case "last_name": return _configReceived[4];
                case "verification_code": Console.Write("Code: "); return Console.ReadLine();
                default: return null; 
            }
        }

        if (!_pipe.IsConnected)
        {
            Console.WriteLine("Pipe connection !!! DOWN !!!");
            return;
        }

        try
        {
            _client = new WTelegram.Client(what => Config(_UserConfigReceived, what));
            var user = await _client.LoginUserIfNeeded().ConfigureAwait(false);
           
        }
        catch (Exception ex)
        {
            Console.WriteLine("Worker-Console : Login failed!: " + ex.Message);
        }
    }

 public async void ConnectAndDump()
    {
        if (_client == null)
        {
            await Authenticate();
            Console.WriteLine("Authentication Complete!");
        }

        // Run the Functions/FetchCSVAsync.cs Method
        FetchCSVAsync = new Worker.Functions.FetchCSVAsync();

        // DUMP IT !
        FetchCSVAsync.DumpCSV(_client, _writer);

    }

     public async void StartJob()
    {
        if (_client == null)
        {
            await Authenticate();
            Console.WriteLine("[JOB] - Authentication request Complete!");
        }

        Console.WriteLine("[JOB] - Running task now!");

        ShillMode = new Worker.Functions.ShillMode();

        ShillMode.StartShill(_client, _writer, _apiId);

    }
}
