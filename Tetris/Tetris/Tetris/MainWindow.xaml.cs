using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
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
using System.Windows.Threading;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /* Attributes */
        private int[,] gameBoard;   // integer representation of the area where tetrominos fall
        private TetrisBlock currentBlock;
        private TetrisBlock nextBlock;
        private int rowIndex; // represents the top-most row index of the current block
        private int colIndex; // represents the left-most column index of the current block
        private int rotationIndex;  // represents the rotation index of the current block
        private int level;  // current level this game
        private int lines;  // how many lines have been completed this game
        private int points; // total points earned this game
        private int highScore;
        private DispatcherTimer gameTimer;
        private int interval;
        private bool isPaused;
        private Random random;
        private Brush[] rgb = {Brushes.Black, Brushes.Coral, Brushes.LightSeaGreen, Brushes.DeepSkyBlue, Brushes.RoyalBlue, Brushes.Aquamarine, Brushes.Wheat, Brushes.PaleGreen};
        /* Constructors */
        public MainWindow()
        {
            InitializeComponent();

            /*ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Tetris;component/Resources/galaxybg.png"));
            gameBoardView.Background = imageBrush;*/

            gameTimer = new DispatcherTimer();
            gameTimer.Tick += new EventHandler(TetrisTick);

            random = new Random();

            isPaused = false;

            highScore = 0;

            InitializeGame();
            this.Focus();
        }

        /* Methods */
        private void SaveGame()
        {
            if (!isPaused)
                PauseUnpause();

            SaveFileDialog d = new SaveFileDialog();
            d.FileName = "TetrisGame";
            d.DefaultExt = ".tetris";
            d.Filter = "Tetris Files (.tetris)|*.tetris";

            if ((bool)d.ShowDialog())
            {
                string path = d.FileName;

            if (File.Exists(path))
                {
                    File.Delete(path);
                }

                StringBuilder data = new StringBuilder();

                data.Append(this.levelLabel.Content);
                data.Append(",");
                data.Append(this.scoreLabel.Content);
                data.Append(",");
                data.Append(this.lineLabel.Content);
                data.Append("\n");

                for (int i = 0; i < 18; i++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        data.Append(gameBoard[i, k].ToString());
                    }

                    data.Append("\n");
                }

                using (StreamWriter stream = new StreamWriter(path))
                {
                    stream.Write(data);
                }
            }
        }

        private void LoadGame()
        {
            OpenFileDialog d = new OpenFileDialog();
            d.DefaultExt = ".tetris";
            d.Filter = "Tetris Files (.tetris)|*.tetris";

            if ((bool)d.ShowDialog())
            {
                string path = d.FileName;

                if (!File.Exists(path) && System.IO.Path.GetExtension(path).Equals(".tetris"))
                {
                    MessageBox.Show("Invalid TETRIS save file.");
                }

                else
                {
                    InitializeGame();

                    StreamReader stream = new StreamReader(path);
                    string line = stream.ReadLine();
                    string[] data = line.Split(',');

                    level = Int32.Parse(data[0]);
                    levelLabel.Content = string.Format("{0:n0}", level);

                    points = Int32.Parse(data[1]);
                    scoreLabel.Content = string.Format("{0:n0}", points);

                    lines = Int32.Parse(data[2]);
                    lineLabel.Content = string.Format("{0:n0}", lines);

                    line = stream.ReadLine();

                    int i = 0, k = 0;
                    while (line != null)
                    {
                        k = 0;

                        foreach (char c in line)
                        {
                            gameBoard[i, k++] = (int)Char.GetNumericValue(c);
                        }

                        i++;
                        line = stream.ReadLine();
                    }

                    // draw crap
                    DrawFallenBlocks();
                }
            }
        }

        private void InitializeGame()
        {
            interval = 500;
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, interval); // days, hours, minutes, second, milliseconds

            gameBoard = new int[18, 10];
            gameBoardView.Children.Clear();
            nextBlockCanvas.Children.Clear();

            level = 1;
            levelLabel.Content = level.ToString();

            points = 0;
            scoreLabel.Content = points.ToString();

            lines = 0;
            lineLabel.Content = lines.ToString();

            isPaused = false;
            PauseUnpause();

            gameOverLabel.Content = "";

            this.startBtn.IsEnabled = true;
            pauseBtn.IsEnabled = false;
            resumeBtn.IsEnabled = false;
        }

        private void DrawFallenBlocks()
        {
            gameBoardView.Children.Clear();

            for(int y = 17; y >= 0; y--)
            {
                for(int x = 0; x < 10; x++)
                {
                     if(gameBoard[y, x] != 0)
                     {
                         Rectangle r = new Rectangle();
                         r.Width = 35;
                         r.Height = 35;

                         r.Fill = rgb[gameBoard[y, x]];
                         r.StrokeThickness = 2;
                         r.Stroke = Brushes.Black;

                         Canvas.SetTop(r, (y * 35));
                         Canvas.SetLeft(r, (x * 35));

                         gameBoardView.Children.Add(r);
                     }
                }
            }
        }

        private void PauseUnpause()
        {
            this.isPaused = (isPaused) ? false : true;
            this.pauseLabel.Content = (isPaused) ? "Paused" : "";

            switch (this.isPaused)
            {
                case true:
                    gameTimer.Stop();
                    pauseBtn.IsEnabled = false;
                    resumeBtn.IsEnabled = true;
                    break;
                default:
                    gameTimer.Start();
                    pauseBtn.IsEnabled = true;
                    resumeBtn.IsEnabled = false;
                    break;
            }
        }

        private void AdvLevel(int l)
        {
            if (l > level)
            {
                // increase the speed by 25%
                interval = (int)(interval * 0.75);
                gameTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);

                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    SoundPlayer game = new SoundPlayer(@"Resources/level_up.wav");
                    game.PlaySync();
                }).Start();
            }

            level = l;
            levelLabel.Content = string.Format("{0:n0}", level);
        }

        // check if any row is completed, if so ClearRows() and award points
        private void CheckCompletedRows()
        {
            bool flag = true;
            int rowCount = 0;
            int bonusPoints = 0;

            // check all rows
            for(int y = 17; y >= 0; y--)
            {
                flag = true;
                for(int x = 0; x < 10; x++)
                {
                    if (gameBoard[y, x] == 0)
                        flag = false;
                }

                if (flag)
                {
                    ClearRow(y);
                    rowCount++;
                    y++;
                }
            }

            // award points
            if (rowCount > 0)
            {
                if(rowCount > 1)
                    bonusPoints = 50 * (rowCount - 1);

                points += 100 * level * rowCount + bonusPoints;
                scoreLabel.Content = string.Format("{0:n0}", points);
                lines += rowCount;
                this.lineLabel.Content = string.Format("{0:n0}", lines);

                if (points > highScore)
                    highScore = points;

                if (lines >= level * 10)
                    AdvLevel(level + 1);

                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    SoundPlayer game;
                    switch (rowCount)
                    {
                        case 1:
                            game = new SoundPlayer(@"Resources/one_row.wav");
                            break;

                        case 2:
                            game = new SoundPlayer(@"Resources/two_row.wav");
                            break;

                        case 3:
                            game = new SoundPlayer(@"Resources/three_row.wav");
                            break;

                        default:
                            game = new SoundPlayer(@"Resources/four_row.wav");
                            break;
                    }

                    game.PlaySync();
                }).Start();
            }
        }
        
        // clear the given row and shift all rows down
        private void ClearRow(int rowIndex)
        {
            for (int y = rowIndex - 1; y >= 0; y--)
                for (int x = 0; x < 10; x++)
                {
                    if (y == 0)
                    {
                        gameBoard[y, x] = 0;
                    }
                    else
                    {
                        gameBoard[y + 1, x] = gameBoard[y, x];
                    }
                }

            DrawFallenBlocks();
        }

        // pause the game, enable/disble things, display that the game is over. Loser.
        private void GameOver()
        {
            PauseUnpause();
            pauseBtn.IsEnabled = false;
            resumeBtn.IsEnabled = false;
            gameOverLabel.Content = "GAME OVER";

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                SoundPlayer game = new SoundPlayer(@"Resources/game_over.wav");
                game.PlaySync();
            }).Start();
        }

        /* Event Methods */
        private void TetrisTick(object sender, EventArgs e)
        {
            // shift current block down
            if (currentBlock.ShiftBlockDown(rowIndex + 1, colIndex))
                rowIndex++;

            // if current block can't be shifted down
            else
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    SoundPlayer game = new SoundPlayer(@"Resources/tap.wav");
                    game.PlaySync();
                }).Start();

                // add current block to the gameBoard
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        if (rowIndex + y <= 17 && colIndex + x < 10 && colIndex + x >= 0)
                        {
                            int n = currentBlock.Occupied(y, x);
                            if (n != 0)
                                gameBoard[rowIndex + y, colIndex + x] = n;
                        }
                    }
                }

                DrawFallenBlocks();
                CheckCompletedRows();

                currentBlock = nextBlock;
                currentBlock.SetCanvas = gameBoardView;
                rowIndex = 0;
                colIndex = 0;

                // check if the area where the next block will be created is filled, if so the game is over
                int randCol = random.Next(-2, 9);
                List<int> tried = new List<int>();

                while (!currentBlock.ShiftBlockRightLeft(0, randCol) && tried.Count < 11)
                {
                    randCol = random.Next(-2, 9);

                    if (!tried.Contains(randCol))
                        tried.Add(randCol);
                }


                if (tried.Count < 11)
                {
                    colIndex = randCol;

                    // currentBlock.DrawBlock(rowIndex, colIndex);


                    // generate new next block and draw it
                    nextBlockCanvas.Children.Clear();
                    nextBlock = new TetrisBlock(gameBoard, nextBlockCanvas, random.Next(0, 7), random.Next(0, 4));
                    nextBlock.DrawBlock(0, 0);
                }

                else
                    GameOver();
            }
        }

        private void Controls_KeyUp(object sender, KeyEventArgs e)
        {
            if (!isPaused)
            {
                switch (e.Key)
                {
                    case Key.Up:
                        rotationIndex = (rotationIndex + 1) % 4;
                        currentBlock.RotateBlock(rotationIndex, rowIndex, colIndex);
                        break;

                    case Key.Space:
                        while (currentBlock.ShiftBlockDown(rowIndex + 1, colIndex))
                            rowIndex++;
                        break;
                }
            }

            switch(e.Key)
            {
                case Key.N:
                    InitializeGame();
                    break;

                case Key.S:
                    if (startBtn.IsEnabled)
                        StartBtn_Click(null, null);
                    break;

                case Key.P:
                    if(pauseBtn.IsEnabled)
                        PauseUnpause();
                    break;

                case Key.R:
                    if (resumeBtn.IsEnabled)
                        PauseUnpause();
                    break;

                case Key.H:
                    AdvLevel(level + 1);
                    break;
            }
        }

        private void Controls_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isPaused)
            {
                switch (e.Key)
                {
                    case Key.Down:
                        if (currentBlock.ShiftBlockDown(rowIndex + 1, colIndex))
                            rowIndex++;
                        break;

                    case Key.Left:
                        if (currentBlock.ShiftBlockRightLeft(rowIndex, colIndex - 1))
                            colIndex--;
                        break;

                    case Key.Right:
                        if (currentBlock.ShiftBlockRightLeft(rowIndex, colIndex + 1))
                            colIndex++;
                        break;
                }
            }
        }

        private void SaveGameBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveGame();
        }

        private void LoadGameBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadGame();
        }

        private void NewGameBtn_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Tic Tac Toe", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            startBtn.IsEnabled = false;
            PauseUnpause();

            // set up the next and current blocks
            currentBlock = new TetrisBlock(gameBoard, gameBoardView, random.Next(0, 7), random.Next(0, 4));
            int randCol = random.Next(-2, 9);
            int randRotation = random.Next(0, 4);

            while (!currentBlock.ShiftBlockRightLeft(0, randCol))
                randCol = random.Next(-2, 9);

            rowIndex = 0;
            colIndex = randCol;
            rotationIndex = randRotation;

            currentBlock.DrawBlock(rowIndex, colIndex);
            nextBlock = new TetrisBlock(gameBoard, nextBlockCanvas, random.Next(0, 7), random.Next(0, 4));
            nextBlock.DrawBlock(0, 0);
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            PauseUnpause();
        }

        private void ResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            PauseUnpause();
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            Window1 about = new Window1(this.highScore);
            about.Show();
        }

        private void ControlsBtn_Click(object sender, RoutedEventArgs e)
        {
            Controls controls = new Controls();
            controls.Show();
        }
    }
}
