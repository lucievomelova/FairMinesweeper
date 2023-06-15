# FairMinesweeper
Implementation of the classic Minesweeper game but with an added feature - if there is an unopened cell whose content is clear, then every cell that doesn't have a certain value will automatically contain a mine. But, if there is no cell whose content can be determined from the current state of the game, every unopened cell is guaranteed to not contain a mine. 

With this added functionality, players cannot "cheat" by clicking randomly on the game field, because they will lose automatically that way. But, when the next move cannotbe calculated, they can be sure that they will not lose.

