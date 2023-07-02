# FairMinesweeper
Implementation of the classic Minesweeper game but with an added feature - if there is an unopened cell whose content is obvious (we know for sure that there is or isn't a mine), then every unopened cell that doesn't have an obvious value will automatically contain a mine. But, if there is no cell whose content can be determined from the current state of the game, every unopened cell is guaranteed to not contain a mine. 

With this added functionality, players cannot "cheat" by clicking randomly on the game field, because they will lose automatically that way. But, when the next move can't be calculated, they can be sure that they will not lose.

