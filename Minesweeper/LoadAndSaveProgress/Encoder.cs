namespace Minesweeper
{
    /// <summary> Class for encoding game cells to characters (that will be saved to a file)
    /// and decoding characters to get original cell value form stored file. </summary>
    public class Encoder
    {
        public static char? CellToChar(Cell cell)
        {
            char c = (char)0;
            if(cell.value >= 0 && cell.isOpened) // opened number
                return (char)cell.value;
            
            if (cell.value >= 0 && cell.isFlag) // number marked with a flag
                c = (char)(Values.NUMBER_MARKED_WITH_A_FLAG + cell.value);
            if (cell.value >= 0 && cell.isQuestionMark) // number marked with a question mark
                c = (char)(Values.NUMBER_MARKED_WITH_A_QUESTION_MARK + cell.value);
            if(cell.value >= 0 && !cell.isOpened) // unopened number
                c = (char)(Values.UNOPENED_NUMBER + cell.value);
            if (cell.value == Values.MINE && cell.isFlag) // correctly marked mine
                c = (char)(Values.MINE_MARKED_WITH_A_FLAG);
            if (cell.value == Values.MINE && cell.isQuestionMark) // mine marked with a question mark
                c = (char)(Values.MINE_MARKED_WITH_A_QUESTION_MARK);
            if (cell.value == Values.MINE && !cell.isFlag) // unmarked mine
                c = (char)(Values.UNMARKED_MINE);

            if (cell.isKnown)
                c = (char)(c + Values.KNOWN);
            return c;
        }
        
        public static void CharToCell(char c, Cell cell)
        {
            if (c <= 8) // opened number
            {
                cell.value = c;
                cell.isOpened = true;
                cell.isKnown = true;
            }
            if (c >= Values.NUMBER_MARKED_WITH_A_FLAG && c < Values.NUMBER_MARKED_WITH_A_QUESTION_MARK) // number marked with a flag
            {
                cell.value = c - Values.NUMBER_MARKED_WITH_A_FLAG;
                if (cell.value > Values.KNOWN)
                {
                    cell.isKnown = true;
                    cell.value -= Values.KNOWN;
                }
                cell.isFlag = true;
            }
            if (c >= Values.NUMBER_MARKED_WITH_A_QUESTION_MARK && c < Values.UNOPENED_NUMBER) // number marked with a question mark
            {
                cell.value = c - Values.NUMBER_MARKED_WITH_A_QUESTION_MARK;
                if (cell.value > Values.KNOWN)
                {
                    cell.isKnown = true;
                    cell.value -= Values.KNOWN;
                }
                cell.isQuestionMark = true;
            }
            if(c >= Values.UNOPENED_NUMBER && c < Values.MINE_MARKED_WITH_A_FLAG) // unopened number
            {
                cell.value = c - Values.UNOPENED_NUMBER;
                if (cell.value > Values.KNOWN)
                {
                    cell.isKnown = true;
                    cell.value -= Values.KNOWN;
                }
            }
            if (c == Values.MINE_MARKED_WITH_A_FLAG) // correctly marked mine
            {
                cell.value = Values.MINE;
                cell.isFlag = true;
            }
            if (c == Values.MINE_MARKED_WITH_A_FLAG + Values.KNOWN) // knwon correctly marked mine
            {
                cell.value = Values.MINE;
                cell.isFlag = true;
                cell.isKnown = true;
            }
            if (c == Values.MINE_MARKED_WITH_A_QUESTION_MARK) // mine marked with a question mark
            {
                cell.value = Values.MINE;
                cell.isQuestionMark = true;
            }
            if (c == Values.MINE_MARKED_WITH_A_QUESTION_MARK + Values.KNOWN) // known  mine marked with a question mark
            {
                cell.value = Values.MINE;
                cell.isQuestionMark = true;
                cell.isKnown = true;
            }
            if (c == Values.UNMARKED_MINE) // unmarked mine
            {
                cell.value = Values.MINE;
            }
            if (c == Values.UNMARKED_MINE + Values.KNOWN) // known unmarked mine
            {
                cell.value = Values.MINE;
                cell.isKnown = true;
            }
        }
    }
}