using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingGame_mode_view.Model
{
    class Word
    {
        private readonly string content;
        private int numberOfTyped;

        public string Content
        { get { return content; } }
        public int NumberOfTyped
        { get { return numberOfTyped; } }

        public Word(string content)
        {
            this.content = content;
            numberOfTyped = 0;
        }

        public int Typing(char type_letter)
        {
            if (type_letter == content[numberOfTyped])
            {
                if (numberOfTyped < content.Length)
                    numberOfTyped++;
            }

            if (numberOfTyped >= content.Length)
                return -1;  // meaning finished
            else
                return numberOfTyped;
        }
    }
}
