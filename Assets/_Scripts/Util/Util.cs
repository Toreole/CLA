using System;
using System.Collections.Generic;
using UnityEngine;

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
