using System.Diagnostics;

using SharpShiller.ConfigHandler;
using SharpShiller.PipeHandler;
using static SharpShiller.ConfigHandler.ConfigHandler;

class Program
{
    static ConfigHandler _configHandler = new ConfigHandler();
    static PipeHandler _pipeHandler = new PipeHandler();
    static Dictionary<int, Process> _processes = new Dictionary<int, Process>();



    static void Main(string[] args)
    {
        Console.WriteLine("~~~PipeServer~~~");

        var pipeHandler = new PipeHandler();

        while (true)
        {
            // Reserve/Connect PIPE0 for the Frontend and liste for received commands from the service worker.
            (PipeHandler.Pipe pipe, string pipeName) = pipeHandler.GetAvailablePipe();
            if (pipe != null)
            {
                try
                {
                    Console.WriteLine($"[SERVER]: Reserved Frontend pipe with type : {pipe} and name: {pipeName}");
                    Console.WriteLine($"[SERVER]: Waiting for clients to connect on {pipeName} ...");
                    pipe.Stream.WaitForConnection();
                    Console.WriteLine($"[SERVER] : Client connected on {pipeName}");
                    

                    using (var stream = new BufferedStream(pipe.Stream, 1024)) // Use a buffered stream for better performance
                    using (var reader = new StreamReader(stream))
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.AutoFlush = true;
                        bool connectSent = false;

                        writer.WriteLine("[Server ]Hello Frontend!");

                        while (true)
                        {
                            string line = reader.ReadLine();
                            Console.WriteLine($"Received from Frontend : {line}");

                            if (!connectSent)
                            {
                                Console.WriteLine("Sending 'Hello' back to Frontend..");
                                writer.WriteLine("[Server ]Hello Frontend!");
                                connectSent = true;
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("[SERVER] : Client Cannot communicate! :" + ex.Message);

                }
            }

            Console.Write("Enter '1' to start a new worker, 'k' to kill a worker, or 'q' to quit: ");

            string input = Console.ReadLine();
            if (input == "1")
            {
                Console.WriteLine("Program: Starting new process...");
                Config config = _configHandler.GetConfig();

                if (config != null)
                {
                    // Spawn a Pipe only if a config is avaliable

                    if (pipe != null)
                    {
                        Console.WriteLine($"Program: Got available pipe: " + pipe + " with name: " + pipeName + " and config : " + config.ToString());
                        Process process = StartProcess(pipeName, config);
                        Console.WriteLine("Program: Waiting for child process to connect to pipe...");
                        // Wait for the child process to connect to the pipe
                        try
                        {
                            pipe.Stream.WaitForConnection();
                            Console.WriteLine("Program : Client  on pipe " + pipeName + " Connected Sucessfully!");
                            using (var reader = new StreamReader(pipe.Stream))
                            using (var writer = new StreamWriter(pipe.Stream))
                            {
                                writer.AutoFlush = true;

                                // while connection is established..
                                bool connectSent = false;
                                while (true)
                                {
                                    // Listen for Client communication
                                    string line = reader.ReadLine();
                                    Console.WriteLine($"Received from Frontend : {line}");
                                    // standardisized input string needed.
                                    // Check if a command has been already send or not
                                    if (!connectSent)
                                    {
                                        Console.WriteLine($"Sending 'Job' command to worker..");
                                        writer.WriteLine("Job");
                                        connectSent = true;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("[Server] : Client communication failed! :" + ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Program: No available pipes!.");
                    }
                }

                else

                {

                    Console.WriteLine("Program: No more configs available!!!");

                }

            }

            Console.WriteLine("Program exiting...");

        }

    }

    static Process StartProcess(string pipeName, Config config)
    {
        // Start a child process
        var child = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"worker.dll {pipeName} {config.ToCsvString()}",
                WorkingDirectory = @"C:\Users\$env:UserName\PipeServer\worker\bin\Debug\net8.0",
            }

        };

        child.Start();
        return child;
    }
}