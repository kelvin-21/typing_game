using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingGame_mode_view.Model
{
    class GameModel
    {
        private Queue<Word> words;
        private int health;
        private int score;
        private static GameModel instance;

        public int Words_length
        { get { return words.Count; } }

        public string Health
        { get { return health.ToString(); } }

        public string Score
        { get { return score.ToString(); } }

        public static GameModel Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameModel();
                return instance;
            }
        }

        public GameModel()
        {
            words = new Queue<Word>();
        }

        public void Reset()
        {
            words.Clear();
            InitializeWords();
            health = 100;
            score = 0;
        }

        public void InitializeWords()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\words.txt";
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                Word w = new Word(line.ToString());
                words.Enqueue(w);
            }
            Console.WriteLine("Enqueued " + words.Count.ToString() + " words.");
        }

        public Word WordsDequeue()
        {
            return words.Dequeue();
        }

        public void TypingFinished(Word word)
        {
            score += word.Content.ToString().Length;
            if (score < 0)
                score = 0;
            health += 1;
            if (health >= 100)
                health = 100;
        }

        public void TypingFailed(Word word)
        {
            score -= (word.Content.ToString().Length - word.NumberOfTyped);
            if (score < 0)
                score = 0;
            health -= (word.Content.ToString().Length - word.NumberOfTyped) * 2;
            if (health < 0)
                health = 0;
        }
        
        
    }
}
