
using TL;
namespace Worker.Functions;

public class FetchCSVAsync()
{

    private readonly string _file_username;
    private readonly string _file_root_path;
    private readonly string _file_name;
    private readonly string _file_full_csv_path;


    public async Task DumpCSV(WTelegram.Client client, StreamWriter _writer)
    {

        // IF ON LINUX, PATCH THIS OR ADD A STATE TO CHECK OS TYPE
        var chats = await client.Messages_GetAllChats().ConfigureAwait(false);
        string _file_username = Environment.UserName;
        string _file_root_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string _file_name = "grouplist.csv";
        string _file_full_csv_path = Path.Combine(_file_root_path, "code-projects", "ShillBot", "worker", "data", _file_name);

        try
        {

            if (!File.Exists(_file_full_csv_path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_file_full_csv_path));
                using (File.Create(_file_full_csv_path)) { }
                Console.WriteLine("CSV file created!");
            }

            Console.WriteLine("Config in existance!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("DEBUG: " + ex.Message);
        }

        string[] columns = new string[] { "channel_type", "channel_id", "access_hash", "channel_participants", "channel_title", };

        // Check if the CSV file is already in use
        while (IsFileInUse(_file_full_csv_path))
        {
            Console.WriteLine("CSV file is in use, retry in 5 seconds...");
            await Task.Delay(5000);
        }

        // Create a list to store the chat data
        var group_csv_data = new List<string[]>();

        // Iterate over the chats and extract the relevant data
        foreach (var (id, chat) in chats.chats)
        {
            string channelType = string.Empty;
            string channelId = id.ToString();
            string accessHash = string.Empty;
            string channelParticipants = string.Empty;
            string channelTitle = string.Empty;

            switch (chat)
            {
                case Chat smallgroup when smallgroup.IsActive:

                    channelType = "small_group";
                    channelId = id.ToString();
                    // there is no access hash for chats
                    accessHash = "";
                    channelParticipants = smallgroup.participants_count.ToString();
                    channelTitle = smallgroup.title;

                    break;

                case Channel channel when channel.IsChannel:

                    channelType = "channel";
                    channelId = id.ToString();
                    accessHash = channel.access_hash.ToString("X");
                    channelParticipants = channel.participants_count.ToString();
                    channelTitle = channel.title;

                    break;

                case Channel group: // no broadcast flag => it's a big group, also called supergroup or megagroup
                    channelType = "group";
                    channelId = id.ToString();
                    accessHash = group.access_hash.ToString("X");
                    channelParticipants = group.participants_count.ToString();
                    channelTitle = group.title;

                    break;
            }

            // Add the chat data to the list
            group_csv_data.Add(new string[] { channelType, channelId, accessHash, channelParticipants, channelTitle });
        }

        // Write the chat data to the CSV file
        using (StreamWriter writer = new StreamWriter(_file_full_csv_path))
        {
            writer.WriteLine(string.Join(",", columns));
            foreach (var data in group_csv_data)
            {
                writer.WriteLine(string.Join(",", data));
                Console.WriteLine($"Written: {string.Join(",", data)} to file {_file_full_csv_path}");
            }
        }

        Console.WriteLine("Data written to file. Check files if needed! Waiting 3 seconds... " + _file_full_csv_path);
        await Task.Delay(3000);
        _writer.AutoFlush = true;

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