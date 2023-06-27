using System.Reflection;
using Common.Utils;
using Config.Net;
using MongoDB.Driver;

namespace Common
{
    public static class Global
    {
        public static readonly IConfig config = new ConfigurationBuilder<IConfig>().UseJsonFile("config.json").Build();
        public static readonly Logger c = new("Global");

        public static readonly MongoClient MongoClient = new MongoClient(
            new MongoClientSettings
            {
                Server = new MongoServerAddress(config.Database.Host, config.Database.Port),
                Credential = MongoCredential.CreateCredential("admin", config.Database.Username, config.Database.Password)
            });
        public static readonly IMongoDatabase db = MongoClient.GetDatabase(config.Database.Name);
        public static long GetUnixInSeconds() => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        public static uint GetRandomSeed() => (uint)(GetUnixInSeconds() * new Random().Next(1, 10) / 10);
    }

    public interface IConfig
    {
        [Option(DefaultValue = VerboseLevel.Normal)]
        VerboseLevel VerboseLevel { get; set; }

        [Option(DefaultValue = false)]
        bool UseLocalCache { get; set; }

        [Option(DefaultValue = true)]
        bool CreateAccountOnLoginAttempt { get; set; }

        IDatabase Database { get; set; }

        public interface IDatabase
        {
            [Option(DefaultValue = "127.0.0.1")]
            string Host { get; set; }

            [Option(DefaultValue = 27017)]
            int Port { get; set; }
            
            [Option(DefaultValue = "PemukulPaku")]
            string Name { get; set; }


            string Username { get; set; }
            string Password { get; set; }
        }

        [Option]
        IGameserver Gameserver { get; set; }

        [Option]
        IHttp Http { get; set; }

        public interface IGameserver
        {
            [Option(DefaultValue = "127.0.0.1")]
            public string Host { get; set; }

            [Option(DefaultValue = (uint)(16100))]
            public uint Port { get; set; }

            [Option(DefaultValue = "overseas01")]
            public string RegionName { get; set; }
        }

        public interface IHttp
        {

            [Option(DefaultValue = (uint)(80))]
            public uint HttpPort { get; set; }

            [Option(DefaultValue = (uint)(443))]
            public uint HttpsPort { get; set; }
        }
    }

    public enum VerboseLevel
    {
        Silent = 0,
        Normal = 1,
        Debug = 2
    }
}