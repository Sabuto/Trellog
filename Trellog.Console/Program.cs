using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Trellog.Base.Commands;
using TrelloNet;

namespace Trellog.App
{
    internal class Program
    {
        private const string Seperatortext =
            "[-------------------------------------------------------------------------------]";

        private static Config? ConfigCommand { get; set; }
        private static BuildChangelog? ChangelogCommand { get; set; }

        private static ITrello? Trello { get; set; }

        private static string? ApiKey { get; set; }
        private static string? AccessToken { get; set; }
        private static string? ChangelogLocation { get; set; }

        private static async Task Main(string[] args)
        {
            ConfigCommand = new Config();
            if (!File.Exists(ConfigCommand.ConfigFile))
                await RunSetup();
            ConfigCommand.LoadData();
            ApiKey = ConfigCommand.ConfigData.ApiKey;
            AccessToken = ConfigCommand.ConfigData.AppKey;

            Trello = new Trello(ApiKey);
            Trello.Authorize(AccessToken);

            if (ConfigCommand.ConfigData.BoardId == null || ConfigCommand.ConfigData.ListId == null)
                await GetBoardAndList();

            GetChangelogFile();
            ChangelogCommand = new BuildChangelog(ChangelogLocation!);

            string desc = GetCardsForRelease();

            AddReleaseCard(desc);
        }

        private static void AddReleaseCard(string changelog)
        {
            Console.WriteLine(Seperatortext);
            Console.WriteLine("Creating a new card on the releases list");
            // var boardtoSearch = Trello.Boards.WithId(ConfigCommand.ConfigData.BoardId);
            // var searchFilter = new SearchFilter()
            var lists = Trello.Lists.ForBoard(new BoardId(ConfigCommand.ConfigData.BoardId));
            var list = lists.FirstOrDefault(x => x.Name == "Releases");
            
            Console.WriteLine("Please enter a version number.");
            string? versionNumber = Console.ReadLine();
            NewCard releaseCard = new NewCard(versionNumber, new ListId(list?.Id));
            releaseCard.Desc = changelog;
            Trello.Cards.Add(releaseCard);
        }

        private static string GetCardsForRelease()
        {
            foreach (var card in Trello?.Cards.ForList(new ListId(ConfigCommand.ConfigData.ListId)))
            {
                if (card.Labels.Any(x => x.Name == "Added"))
                    ChangelogCommand?.Added.Add(card);
                if (card.Labels.Any(x => x.Name == "Changed"))
                    ChangelogCommand?.Changed.Add(card);
                if (card.Labels.Any(x => x.Name == "Fixed"))
                    ChangelogCommand?.Fixed.Add(card);
            }

            return ChangelogCommand.OutputChangelog(Trello);
        }

        private static void GetChangelogFile()
        {
            Console.WriteLine(Seperatortext);
            Console.WriteLine("Please specify an output file.");
            ChangelogLocation = Console.ReadLine();
        }

        private static Task GetBoardAndList()
        {
            try
            {
                Member me = Trello.Members.Me();

                Console.WriteLine(Seperatortext);
                Console.WriteLine("Please Select a board to use:");
                Dictionary<int, string> boardsDict = new();
                int i = 1;
                foreach (Board board in Trello.Boards.ForMe())
                {
                    Console.WriteLine($"[{i}] -> {board.Name}");
                    boardsDict[i] = board.Id;
                    i++;
                }

                string? tempSelectedBoard = Console.ReadLine();

                string? selectedBoard = boardsDict[int.Parse(tempSelectedBoard!)];

                Console.WriteLine(Seperatortext);
                Console.WriteLine("Please Select a list to use:");
                Dictionary<int, string> cardsDict = new();
                i = 1;
                foreach (List list in Trello.Lists.ForBoard(new BoardId(selectedBoard)))
                {
                    Console.WriteLine($"[{i}] -> {list.Name}");
                    cardsDict[i] = list.Id;
                    i++;
                }

                tempSelectedBoard = Console.ReadLine();

                string? selectedList = cardsDict[int.Parse(tempSelectedBoard!)];

                ConfigCommand.CreateBoardsAndListsData(selectedBoard, selectedList);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return Task.CompletedTask;
        }

        private static Task RunSetup()
        {
            Console.WriteLine("Welcome to Trellog, we will build a config file and authenticate you with Trello.");
            Console.WriteLine(Seperatortext);
            Console.WriteLine("Please go to the following link to generate an Api key");
            Console.WriteLine("https://trello.com/1/appKey/generate");
            ApiKey = Console.ReadLine();
            Console.WriteLine("Please go to the following link to generate an Access Token");
            Console.WriteLine(
                $"https://trello.com/1/authorize?response_type=token&scope=read,write&expiration=never&name=Trellog&key={ApiKey}");
            AccessToken = Console.ReadLine();
            Console.WriteLine("Saving Data to Config File....");
            ConfigCommand?.CreateBaseData(ApiKey!, AccessToken!);

            return Task.CompletedTask;
        }
    }
}