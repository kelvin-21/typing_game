using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TypingGame_mode_view.Model;

namespace TypingGame_mode_view.View
{
    class SuperButton
    {
        public Word word;
        public Button button;

        public string GetSuperWordContent
        { get { return word.Content; } }

        public SuperButton(Word word, Button button)
        {
            this.word = word;
            this.button = button;
        }

        public void Fall()
        {
            List<string> position = button.Margin.ToString().Split(',').ToList();  // make Margin (string) to List<string>
            List<int> int_position = position.Select(s => int.Parse(s)).ToList();  // make List<string> to List<int>
            int_position[1] += 2;  // make it fall
            button.Margin = new Thickness(int_position[0], int_position[1], int_position[2], int_position[3]);
        }

        public void GoBack()
        {
            button.Content = "";

            List<string> position = button.Margin.ToString().Split(',').ToList();  // make Margin (string) to List<string>
            List<int> int_position = position.Select(s => int.Parse(s)).ToList();  // make List<string> to List<int>
            int_position[1] = 10;
            button.Margin = new Thickness(int_position[0], int_position[1], int_position[2], int_position[3]);
        }

        public int Isbottom()
        {
            List<string> position = button.Margin.ToString().Split(',').ToList();  // make Margin (string) to List<string>
            List<int> int_position = position.Select(s => int.Parse(s)).ToList();  // make List<string> to List<int>
            if (int_position[1] > 300)
                return 1;
            else
                return 0;
        }
    }
}
