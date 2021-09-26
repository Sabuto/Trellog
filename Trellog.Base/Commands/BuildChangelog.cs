using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MarkdownOut;
using TrelloNet;

namespace Trellog.Base.Commands
{
    public class BuildChangelog
    {
        public BuildChangelog(string location)
        {
            _changelogLocation = location;
        }

        private string _changelogLocation { get; }

        public List<Card> Changed { get; set; } = new();
        public List<Card> Added { get; set; } = new();
        public List<Card> Fixed { get; set; } = new();

        public string OutputChangelog(ITrello trello)
        {
            if (!ChangelogFileExists())
                File.Create(_changelogLocation).Close();

            StringBuilder? changelogText = new();

            if (Added.Any())
            {
                changelogText.AppendLine(MdText.Format("Added", MdFormat.Heading3));
                foreach (Card card in Added)
                {
                    string? checklistItems = CheckForChecklistsAndProcess(card, trello);
                    if (checklistItems == null)
                        changelogText.AppendLine(MdText.Format(card.Name, MdFormat.UnorderedListItem));
                    else
                    {
                        changelogText.AppendLine(MdText.Format($"{card.Name}:", MdFormat.UnorderedListItem));
                        changelogText.AppendLine(checklistItems);
                    }
                }

                changelogText.Append(MdText.LineBreak);
            }

            if (Changed.Any())
            {
                changelogText.AppendLine(MdText.Format("Changed", MdFormat.Heading3));
                foreach (Card card in Changed)
                {
                    string? checklistItems = CheckForChecklistsAndProcess(card, trello);
                    if (checklistItems == null)
                        changelogText.AppendLine(MdText.Format(card.Name, MdFormat.UnorderedListItem));
                    else
                    {
                        changelogText.AppendLine(MdText.Format($"{card.Name}:", MdFormat.UnorderedListItem));
                        changelogText.AppendLine(checklistItems);
                    }
                }

                changelogText.Append(MdText.LineBreak);
            }

            if (Fixed.Any())
            {
                changelogText.AppendLine(MdText.Format("Fixed", MdFormat.Heading3));
                foreach (Card card in Fixed)
                {
                    string? checklistItems = CheckForChecklistsAndProcess(card, trello);
                    if (checklistItems == null)
                        changelogText.AppendLine(MdText.Format(card.Name, MdFormat.UnorderedListItem));
                    else
                    {
                        changelogText.AppendLine(MdText.Format($"{card.Name}:", MdFormat.UnorderedListItem));
                        changelogText.AppendLine(checklistItems);
                    }
                }

                changelogText.Append(MdText.LineBreak);
            }

            MdWriter writer = new(_changelogLocation);

            writer.Write(changelogText);
            writer.Close();

            return changelogText.ToString();
        }

        private string CheckForChecklistsAndProcess(Card card, ITrello trello)
        {
            StringBuilder items = new();
            Dictionary<string, List<CheckItem>> persisted_items = new();
            foreach (var checklist in card.Checklists)
            {
                foreach (var item in checklist.CheckItems.Where(x => x.Checked))
                {
                    items.Append(MdText.ListItemIndent);
                    items.AppendLine(MdText.Format(item.Name, MdFormat.UnorderedListItem));
                }

                // List<CheckItem> itemsToAdd = new();
                // foreach (var item in checklist.CheckItems.Where(x => !x.Checked))
                // {
                //     itemsToAdd.Add(item);
                // }
                //
                // persisted_items[checklist.Name] = itemsToAdd;
            }
            //
            // var lists = trello.Lists.ForBoard(new BoardId(card.IdBoard));
            // var toDo = lists.FirstOrDefault(x => x.Name == "To Do");
            // NewCard newCard = new NewCard(card.Name, new ListId(toDo?.Id));
            // if (persisted_items.Any())
            // {
            //     Card? addedCard = trello.Cards.Add(newCard);
            //
            //     foreach (var persistedItem in persisted_items)
            //     {
            //         var cl = new Card.Checklist {Name = persistedItem.Key};
            //         cl.CheckItems = new List<Card.CheckItem>();
            //         foreach (var checkItem in persistedItem.Value)
            //         {
            //             Card.CheckItem ci = new() {Name = checkItem.Name, Checked = false};
            //             cl.CheckItems.Add(ci);
            //         }
            //
            //         var test = trello.Cards.AddChecklist(addedCard, cl);
            //     }
            // }

            return items.ToString();
        }

        public bool ChangelogFileExists()
        {
            return File.Exists(_changelogLocation);
        }
    }
}