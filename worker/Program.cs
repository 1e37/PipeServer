using System.Diagnostics;
using System.IO.Pipes;

// dotnet add package WTelegramClient --version 4.1.8

class ShillBot_worker
{
    // listen for Commands
    static async Task Main(string[] args)
    {


        if (args.Length < 2)

        {

            Console.WriteLine("Whoops! MINIMUM INPUT EXPECTED : [0] pipeName [1] {UserConfigReceived}");

            return;

        }
        // our pipe we will communicate with
        var pipeName = args[0];

        // Split down the JOSN
        var UserConfigReceived = args[1].Split(',');

        // split the config reveiced input 
        var ApiId = int.Parse(UserConfigReceived[0]);
        var ApiHash = UserConfigReceived[1];
        var PhoneNumber = UserConfigReceived[2];
        var FirstName = UserConfigReceived[3];
        var LastName = UserConfigReceived[4];


        string worker_name = "[Worker-" + UserConfigReceived[0] + "] : ";
        string pipe_name = args[0];
        // New Pipe Instance
        using var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);
        // Start IO Pipe
        using (var reader = new StreamReader(pipe))
        using (var writer = new StreamWriter(pipe))
        {
            // Connect to the Pipe
            try
            {
                pipe.Connect(100); // Try to connect with a timeout of 100ms
                writer.WriteLine(worker_name + "!! HELLO SERVER  :  connection on pipe: " + args[0] + " READY !!");
                //  pipe.WriteLine("Hello, Server!");
            }
            catch (TimeoutException)
            {
                Console.WriteLine(worker_name + ":!!!! " + args[0] + "!!! PIPE UNREACHABLE!!!");
            }
            writer.AutoFlush = true;

            var serviceHandler = new Worker.ServiceHandler.Connect(pipeName, UserConfigReceived, ApiId, ApiHash, PhoneNumber, FirstName, LastName, pipe, writer, reader);

            string command;
            while ((command = reader.ReadLine()) != null)
            {
                writer.WriteLine(worker_name + " : Received command from server: " + command);
                // Handle multiple commands
                switch (command)
                {
                    case "Job":
                        // Handle Command
                        writer.WriteLine(worker_name + " :  [Job] Starting Shilling mode...");
                        writer.AutoFlush = true;
                        serviceHandler.StartJob();
                        
                        break;
                    case "ConnectAndDump":
                        serviceHandler.ConnectAndDump();
                        break;
                    case "kill":
                    var process = Process.GetCurrentProcess();
                     process.Kill();
                        break;
                    default:
                        Console.WriteLine(worker_name + "Command not found! : " + command);
                        break;
                }
            }
        }
    }
}