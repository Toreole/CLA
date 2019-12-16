using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LATwo
{
    public static class Util
    {
        public static string ToScoreString(this int score, int places)
        {
            string text = score.ToString();
            for (int l = text.Length; l < places; l++)
                text = "0" + text;
            return text;
        }
    }
}
