using System.IO;

namespace Trellog.Base.Commands
{
    public class Config
    {
        public Config()
        {
            ConfigService = new Services.Config();
            LoadData();
            ConfigData ??= new Models.Config();
        }

        public void CreateBaseData(string apikey, string accessToken)
        {
            ConfigData.ApiKey = apikey;
            ConfigData.AppKey = accessToken;

            SaveData();
        }

        public void CreateBoardsAndListsData(string boardId, string listId)
        {
            ConfigData.BoardId = boardId;
            ConfigData.ListId = listId;
            
            SaveData();
        }

        public void LoadData()
        {
            ConfigData = ConfigService.LoadConfig(ConfigFile);
        }

        public void SaveData()
        {
            ConfigService.SaveConfig(ConfigData!, ConfigFile);
        }

        public string ConfigFile => "trello.json";

        public Models.Config? ConfigData { get; set; }

        public Services.Config ConfigService { get; }
    }
}