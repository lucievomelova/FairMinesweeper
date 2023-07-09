namespace Minesweeper
{
    /// <summary> Class for encoding game cells to characters (that will be saved to a file)
    /// and decoding characters to get original cell value form stored file. </summary>
    public class Encoder
    {
        public static char? CellToChar(Cell cell)
        {
            if(cell.value >= 0 && cell.isOpened) // opened number
                return (char)cell.value;
            if (cell.value >= 0 && cell.isFlag) // number marked with a flag
                return (char)(Values.NUMBER_MARKED_WITH_A_FLAG + cell.value);
            if (cell.value >= 0 && cell.isQuestionMark) // number marked with a question mark
                return (char)(Values.NUMBER_MARKED_WITH_A_QUESTION_MARK + cell.value);
            if(cell.value >= 0 && !cell.isOpened) // unopened number
                return (char)(Values.UNOPENED_NUMBER + cell.value);
            if (cell.value == Values.MINE && cell.isFlag) // correctly marked mine
                return (char)(Values.MINE_MARKED_WITH_A_FLAG);
            if (cell.value == Values.MINE && cell.isQuestionMark) // mine marked with a question mark
                return (char)(Values.MINE_MARKED_WITH_A_QUESTION_MARK);
            if (cell.value == Values.MINE && !cell.isFlag) // unmarked mine
                return (char)(Values.UNMARKED_MINE);
            return null;
        }
        
        public static Cell CharToCell(char c, int row, int column)
        {
            Cell cell = new Cell(row, column);  
            if (c <= 8) // opened number
            {
                cell.value = c;
                cell.isOpened = true;
                cell.isKnown = true;
            }
            if (c >= Values.NUMBER_MARKED_WITH_A_FLAG && c < Values.NUMBER_MARKED_WITH_A_QUESTION_MARK) // number marked with a flag
            {
                cell.value = c - Values.NUMBER_MARKED_WITH_A_FLAG;
                cell.isFlag = true;
            }
            if (c >= Values.NUMBER_MARKED_WITH_A_QUESTION_MARK && c < Values.UNOPENED_NUMBER) // number marked with a question mark
            {
                cell.value = c - Values.NUMBER_MARKED_WITH_A_QUESTION_MARK;
                cell.isQuestionMark = true;
            }
            if(c >= Values.UNOPENED_NUMBER && c < Values.MINE_MARKED_WITH_A_FLAG) // unopened number
            {
                cell.value = c - Values.UNOPENED_NUMBER;
            }
            if (c == Values.MINE_MARKED_WITH_A_FLAG) // correctly marked mine
            {
                cell.value = Values.MINE;
                cell.isFlag = true;
            }
            if (c == Values.MINE_MARKED_WITH_A_QUESTION_MARK) // mine marked with a question mark
            {
                cell.value = Values.MINE;
                cell.isQuestionMark = true;
            }
            if (c == Values.UNMARKED_MINE) // unmarked mine
            {
                cell.value = Values.MINE;
            }
            return cell;
        }
    }
}