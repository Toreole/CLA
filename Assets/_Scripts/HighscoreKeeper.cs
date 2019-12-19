using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LATwo
{
    public class HighscoreKeeper
    {
        private static readonly string saveFile = Application.persistentDataPath + "/highscores.sf";
        private static List<Score> scores = new List<Score>();

        static HighscoreKeeper()
        {
            //upon starting the game, the old savefile should already be loaded.
            if(File.Exists(saveFile))
            {
                var stream = File.OpenRead(saveFile);
                BinaryFormatter formatter = new BinaryFormatter();
                scores = (List<Score>)formatter.Deserialize(stream);
                stream.Flush();
                stream.Close();
            }
            Message<VerifyScore>.Add(EventAddScore);
        }

        static void Save()
        {
            var stream = File.Open(saveFile, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, scores);
            stream.Flush();
            stream.Close();
        }

        static void EventAddScore(VerifyScore v)
            => AddScore(v.score, v.name); 

        public static void AddScore(int score, string name)
        {
            var s = new Score()
            {
                score = score,
                name = name,
                date = DateTime.Now
            };
            scores.Add(s);
            scores.Sort(new ScoreComparer());
            //if(scores.Count == 0)
            //{
            //    scores.Add(s);
            //    return;
            //}
            //for (int i = 0; i < scores.Count; i++)
            //{
            //    if (s.score > scores[i].score)
            //    {
            //        scores.Insert(i, s);
            //        break;
            //    }
            //}
            Save();
        }

        public static int GetScoreTexts(ref string[] texts)
        {
            texts = new string[10];
            int i;
            for(i = 0; i < scores.Count && i < 10; i++)
            {
                texts[i] = scores[i].ToString();
            }
            return i;
        }

        [Serializable]
        internal class Score
        {
            public int score;
            public DateTime date;
            public string name; //max length of 3 characters.

            public override string ToString()
            {
                return $"{name}: {score.ToScoreString(6)} : {date.Day}.{date.Month}.{date.Year}";
            }
        }

        internal class ScoreComparer : IComparer<Score>
        {
            public int Compare(Score x, Score y)
            {
                return (x.score > y.score ? -1 : (x.score < y.score ? 1 : (x.date > y.date ? -1 : 1)));
            }
        }
    }
}