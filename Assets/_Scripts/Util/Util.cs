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

        public static Vector4[] GetPoints(this Pulse[] pulses)
        {
            Vector4[] points = new Vector4[pulses.Length];
            for (int i = 0; i < pulses.Length; i++)
                points[i] = pulses[i].screenPoint;
            return points;
        }
        public static float[] GetTimes(this Pulse[] pulses)
        {
            float[] times = new float[pulses.Length];
            for (int i = 0; i < pulses.Length; i++)
                times[i] = pulses[i].time;
            return times;
        }
    }
}
