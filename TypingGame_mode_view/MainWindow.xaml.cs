using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TypingGame_mode_view.Model;
using TypingGame_mode_view.View;


namespace TypingGame_mode_view
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameModel currentGame;
        private Button[] buttons;
        private List<SuperButton> wordsOnBoard;
        private SuperButton cursor;
        private System.Windows.Threading.DispatcherTimer spawnTimer;
        private System.Windows.Threading.DispatcherTimer fallTimer;
        private System.Windows.Threading.DispatcherTimer gameTimer;
        private int game_timer;
        private Random rand;
        private int typingPosition;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (currentGame == null)
            {
                buttons = new Button[5];  // buttons in UI
                buttons[0] = Button_0;
                buttons[1] = Button_1;
                buttons[2] = Button_2;
                buttons[3] = Button_3;
                buttons[4] = Button_4;

                for (int i = 0; i < 5; i++)
                    buttons[i].Click += new RoutedEventHandler(GeneralButtonClick);

                wordsOnBoard = new List<SuperButton>();  // buttons for game logic
                currentGame = GameModel.Instance;
                cursor = null;

                spawnTimer = new System.Windows.Threading.DispatcherTimer();  // Timer for spawning words
                spawnTimer.Tick += WordSpawnEvent;
                spawnTimer.Interval = TimeSpan.FromSeconds(0.5);

                fallTimer = new System.Windows.Threading.DispatcherTimer();  // Timer for falling words
                fallTimer.Tick += WordFallEvent;
                fallTimer.Interval = TimeSpan.FromSeconds(0.1);

                gameTimer = new System.Windows.Threading.DispatcherTimer();  // Timer for the game
                gameTimer.Tick += GameTimer;
                gameTimer.Interval = TimeSpan.FromSeconds(1);

                rand = new Random();
            }

            currentGame.Reset();
            foreach (SuperButton element in wordsOnBoard)
                element.GoBack();
            wordsOnBoard.Clear();
            game_timer = 68;
            Time.Text = "60s";
            Score.Text = currentGame.Score;
            Health.Text = currentGame.Health;

            Announce.Text = "";
            TextBox.Text = "";
            TextBox.Visibility = Visibility.Visible;
            TextBox.Focus();

            spawnTimer.Start();
            fallTimer.Start();
            gameTimer.Start();
        }

        private void GeneralButtonClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (cursor != null)
                if (cursor.button != b)
                    TextBox.Text = "";
            NewCursor(b);
            TextBox.Focus();
        }

        private void WordSpawnEvent(object sender, EventArgs e)
        {
            if (game_timer > 60)
                return;

            List<int> idle = new List<int>();
            int spawnIndex;

            if (currentGame.Words_length != 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (buttons[i].Content.ToString() == "")
                        idle.Add(i);
                }
                if (idle.Count > 0)
                {
                    spawnIndex = idle[rand.Next(0, idle.Count)];
                    SuperButton sb = new SuperButton(currentGame.WordsDequeue(), buttons[spawnIndex]);
                    wordsOnBoard.Add(sb);
                    buttons[spawnIndex].Content = sb.GetSuperWordContent;
                    buttons[spawnIndex].Visibility = Visibility.Visible;

                    if (wordsOnBoard.Count == 1)
                        NewCursor(wordsOnBoard[0].button);

                    Console.WriteLine("Word -{0}- spawned", sb.button.Content);
                }
                else
                    Console.WriteLine("All buttons busy, no space to spawn word");
            }
        }

        private void WordFallEvent(object sender, EventArgs e)
        {
            if (game_timer > 60)
                return;

            for (int i = 0; i < wordsOnBoard.Count; i++)
            {
                SuperButton sb = wordsOnBoard[i];
                sb.Fall();

                if (sb.Isbottom() == 1)
                {
                    wordsOnBoard.Remove(sb);
                    if (wordsOnBoard.Count == 0 && currentGame.Words_length == 0)
                    {
                        sb.GoBack();
                        currentGame.TypingFailed(sb.word);
                        GameEnd(1);
                        return;
                    }

                    if (cursor == sb)
                        NewCursor(wordsOnBoard[0].button);
                    sb.GoBack();

                    currentGame.TypingFailed(sb.word);
                    Score.Text = currentGame.Score;
                    Health.Text = currentGame.Health;
                    TextBox.Text = "";

                    if (currentGame.Health == "0")
                    {
                        GameEnd(-1);
                        return;
                    }
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = TextBox.Text;

            if (wordsOnBoard.Count == 0 && currentGame.Words_length == 0)
            {
                GameEnd(1);
                return;
            }

            if (cursor == null)
            {
                TextBox.Text = "";
                return;
            }

            if (text.Length == 0)
            {
                if (typingPosition == 1)
                {
                    TextBox.Text = cursor.GetSuperWordContent.Substring(0, typingPosition);
                    TextBox.Select(TextBox.Text.Length, 0);
                }
                return;
            }


            // otherwise, the input is normal
            Console.WriteLine("current text : |{0}|, position = {1}", TextBox.Text, typingPosition);
            char newInput = text[text.Length - 1];
            typingPosition = cursor.word.Typing(newInput);
            Console.WriteLine("new input : {0} on position {1}", newInput, typingPosition);

            if (typingPosition >= 0)  // the word has not been finished yet
            {
                TextBox.Text = cursor.GetSuperWordContent.Substring(0, typingPosition);
                TextBox.Select(TextBox.Text.Length, 0);
            }
            else if (typingPosition == -1)  // the word is finished
            {
                Console.WriteLine("else");
                Console.WriteLine("wordsOnBoard length = {0}, currentGame.Words_length = {1}", wordsOnBoard.Count, currentGame.Words_length);

                SuperButton sb = cursor;
                wordsOnBoard.Remove(cursor);
                if (wordsOnBoard.Count != 0)
                    NewCursor(wordsOnBoard[0].button);
                else
                    cursor = null;
                sb.GoBack();

                currentGame.TypingFinished(sb.word);
                Score.Text = currentGame.Score;
                Health.Text = currentGame.Health;
                TextBox.Text = "";

                if (wordsOnBoard.Count == 0 && currentGame.Words_length == 0)
                {
                    GameEnd(1);
                    return;
                }
            }
        }

        public void NewCursor(Button new_cursor)
        {
            for (int i = 0; i < wordsOnBoard.Count; i++)
                if (wordsOnBoard[i].button == new_cursor)
                    cursor = wordsOnBoard[i];
            for (int i = 0; i < 5; i++)
                buttons[i].Background = Brushes.FloralWhite;
            if (cursor != null)
                cursor.button.Background = Brushes.Yellow;
        }

        private void GameTimer(object sender, EventArgs e)
        {
            if (game_timer == 68)
            {
                ProfNg_png.Visibility = Visibility;
                Announce.Visibility = Visibility.Visible;
                Announce.Text = "60 seconds!";
            }
            if (game_timer == 67)
                Announce.Text += "\nWrite down the defintion of Limit of Sequence!";
            if (game_timer == 64)
                Announce.Text = "3";
            if (game_timer == 63)
                Announce.Text = "2";
            if (game_timer == 62)
                Announce.Text = "1";
            if (game_timer == 61)
            {
                Announce.Text = "Go!";
                ProfNgDisappear_png.Visibility = Visibility.Visible;
                ProfNg_png.Visibility = Visibility.Hidden;
            }
            if (game_timer == 60)
            {
                Announce.Text = "";
                Announce.Visibility = Visibility.Hidden;
            }

            game_timer -= 1;
            if (game_timer <= 60)
                Time.Text = game_timer.ToString() + "s";

            if (game_timer == 0)
            {
                GameEnd(-1);
                return;
            }
        }

        public void GameEnd(int flag)  // flag=1 means finished, flag=-1 means failed
        {
            cursor = null;
            spawnTimer.Stop();
            fallTimer.Stop();
            gameTimer.Stop();

            Score.Text = currentGame.Score;
            Health.Text = currentGame.Health;
            TextBox.Text = "";

            foreach (SuperButton sb in wordsOnBoard)
                sb.GoBack();
            wordsOnBoard.Clear();

            for (int i = 0; i < 5; i++)
                buttons[i].Background = Brushes.FloralWhite;
            TextBox.Visibility = Visibility.Hidden;

            ProfNgDisappear_png.Visibility = Visibility.Hidden;
            ProfNg_png.Visibility = Visibility.Visible;

            if (flag == 1)
            {
                Console.WriteLine("Finished!!");
                Announce.Visibility = Visibility.Visible;
                Announce.Text = "You passed!";
            }
            else if (flag == -1)
            {
                Console.WriteLine("Failed!!");
                Announce.Visibility = Visibility.Visible;
                Announce.Text = "You failed!";
            }

            Console.WriteLine("Game end! Thank you!");
        }
    }
}
