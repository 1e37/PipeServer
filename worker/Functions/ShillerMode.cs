using TL;


namespace Worker.Functions;

public class ShillMode()
{
    public async Task StartShill(WTelegram.Client client, StreamWriter _writer, int workername)
    {
        // !!! DUPLICATED CODE 
        string _file_username = Environment.UserName;
        string _file_root_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string _file_name = "grouplist.csv";
        string _file_name_worker = workername + "-grouplist.csv";
        string _file_full_csv_path_original = Path.Combine(_file_root_path, "code-projects", "ShillBot", "worker", "data", _file_name);
        string _file_full_csv_path_worker = Path.Combine(_file_root_path, "code-projects", "ShillBot", "worker", "data", "userdata", _file_name_worker);

        try
        {

            if (!File.Exists(_file_full_csv_path_original))
            {
                Console.WriteLine("No Original CSV in existance!");
                Console.WriteLine("RUN DUMP FIRST!");
                Console.WriteLine("Exiting worker..");
                   var process = System.Diagnostics.Process.GetCurrentProcess();
                   process.Kill();
            }

            if (File.Exists(_file_full_csv_path_original))
            {
                // Lets Duplicate the file, so we dont have to work with the root file for Integrity reasons!
                Console.WriteLine("!!!ShillMode!!! Creating/cleaning duplication csv job for worker: " + workername);

                // Clean UP!
                if (File.Exists(_file_full_csv_path_worker))
                {
                    File.Delete(_file_full_csv_path_worker);
                    Console.Write("Deleted old Worker file :" + _file_full_csv_path_worker);
                }

                File.Create(_file_full_csv_path_worker);
                Console.Write("Created File :" + _file_full_csv_path_worker);

                _writer.AutoFlush = true;

                // TODO ; 
                // READ from CSV and foreach entry do a shill in an interval of % hours.
                // Console.Write("Sending Message to a Chat ID: ");
                long chatId = long.Parse("1111111111");
                var peer = new InputPeerChat(chatId);
                Console.WriteLine($"Sending a message to chat {chatId}");
                await client.SendMessageAsync(peer, "Hello!");

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("DEBUG: " + ex.Message);
        }

    }

    // Helper method to check if a file is in use or no
    bool IsFileInUse(string filePath)
    {
        try
        {
            using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Write))
            {
                fileStream.Close();
            }
        }
        catch (IOException)
        {
            return true;
        }
        return false;
    }
}