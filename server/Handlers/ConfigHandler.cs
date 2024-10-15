using System.Collections.Generic;
using System;

namespace SharpShiller.ConfigHandler
{
    public class ConfigHandler
    {
        private readonly List<Config> _configs = new List<Config>();
        private int _currentIndex = 0;

        public ConfigHandler()
        {
            // Initialize our configs as list
             _configs.Add(new Config
             {
                // = ApiID = worker id 
                // First config
                 ApiId = "1",
                 ApiHash = "test",
                 PhoneNumber = "test",
                 FirstName = "test",
                 LastName = "test"
             });
            // Second config
            _configs.Add(new Config
            {
                ApiId = "2",
                ApiHash = "test",
                PhoneNumber = "test",
                FirstName = "test",
                LastName = "test"
            });

            // ...
        }

        public class Config
{
    public string ApiId { get; set; }
    public string ApiHash { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

        public override string ToString()
        {
            return $"ApiId = {ApiId}, ApiHash = {ApiHash}, PhoneNumber = {PhoneNumber}, FirstName = {FirstName}, LastName = {LastName}";
        }

        public string ToCsvString()
        {
            return $"{ApiId},{ApiHash},{PhoneNumber},{FirstName},{LastName}";
        }
}

        public Config GetConfig()
        {
            if (_currentIndex < _configs.Count)
            {
                Config config = _configs[_currentIndex];
                _currentIndex++;
                return config;
            }
            else
            {
                return null;
            }
        }
    }
}