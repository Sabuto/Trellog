# Trellog

This is a simple app that I needed, i am releasing it as opensource in the hopes it's useful for someone else too.

## What it does

Tellog allows you to take a list from trello and create a changelog in markdown from that list, it will also transfer
the cards to a new list in one card with activities added to that card from the list.

If you have a checklist on the card it will add an unordered list of the checked items below the card name in the release for example:

Card on Trello named Tasks with the label Added:
- [x] Task one
- [ ] Task two
- [x] Task three

Output to changelog file:
### Added
- Tasks
  - Task one
  - Task three
  
## Todo
- If there are un checked items on the checklist it will create a card on the To Do list with the name of the card and the checklist
- Delete Cards from the next release List

