public class LineCalculation
{
    /*
     Note: Could be compressed to one line, but for the sake of redability, I haven't.
     */
    public static int GetLinesSent(int linesCleared, int combo, bool pc, bool b2b, bool tspin)
    {
        if(linesCleared == 0)
        {
            return 0;
        }
        int lines;

        if (tspin)
        {
            lines = linesCleared * 2;
        }
        else
        {
            lines = (linesCleared == 4 ? 4 : linesCleared - 1);
        }

        if (combo > 0)
        {
            lines += Combo.DEFAULT[combo < Combo.DEFAULT.Length ? combo : Combo.DEFAULT.Length - 1];
        }

        if (pc)
        {
            lines = 8;
        }

        if (b2b && (linesCleared == 4 || (tspin && linesCleared != 0))) lines++;

        return lines;

    }


}

