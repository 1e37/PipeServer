using System.IO.Pipes;

namespace SharpShiller.PipeHandler
{
    public class PipeHandler
    {
        private readonly List<Pipe> _pipes = new List<Pipe>();
        private readonly object _lock = new object();
        private int _pipeCount = 0;
        private readonly List<string> _activePipes = new List<string>();

        public (Pipe, string) GetAvailablePipe()
        {
            Console.WriteLine("PipeHandler: Getting available pipe...");
            lock (_lock)
            {
                string pipeName = $"pipe{_pipeCount}";
                while (_activePipes.Contains(pipeName))
                {
                    _pipeCount++;
                    pipeName = $"pipe{_pipeCount}";
                }

                Console.WriteLine($"PipeHandler: Creating new pipe with name {pipeName}");
                var newPipe = new Pipe { Name = pipeName, Stream = new NamedPipeServerStream(pipeName, PipeDirection.InOut) };

                _pipes.Add(newPipe);
                _activePipes.Add(pipeName);
                newPipe.InUse = true;
                Console.WriteLine($"PipeHandler: Pipe {pipeName} created and added to list");
                return (newPipe, pipeName);
            }
        }

        public void ReleasePipe(Pipe pipe)
        {
            Console.WriteLine("PipeHandler: Releasing pipe...");
            lock (_lock)
            {
                var existingPipe = _pipes.FirstOrDefault(p => p == pipe);
                if (existingPipe != null)
                {
                    existingPipe.InUse = false;
                    _activePipes.Remove(existingPipe.Name);
                    Console.WriteLine($"PipeHandler: Pipe {existingPipe.Name} released");
                }
            }
        }

        public class Pipe
        {
            public string Name { get; set; }
            public NamedPipeServerStream Stream { get; set; }
            public bool InUse { get; set; }

            public override string ToString()
            {
                return Stream.ToString();
            }
        }
    }
}