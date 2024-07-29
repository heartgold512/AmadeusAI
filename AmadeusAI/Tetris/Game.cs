using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmadeusAI.Tetris;

namespace AmadeusAI.Tetris
{
    using AmadeusAI.Tetris;
    using Octokit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

    namespace AmadeusAI.Tetris
    {
        public class Game
        {
            private readonly Dictionary<string, string> blockTypes;
            private readonly string imageFolderPath;
            //
            private bool Game_State; //when start is pressed in the xaml cs file seperated this will be set to true and carried as a constructor 
            private int points; //points per line
            private int score;
            private int leaderboard;
            private int Highscore;
            private int points_to_clear;
            private bool gameRunning;
            private int pointsPerLine = 100;
            private int level;
            private int linesCleared;
            private int[,] gameBoard;
            private Grid gameGrid;
            public event EventHandler GameOver;
         
            private const int blockSize = 20;
            //private Tetrimino currentTetrimino;
            private DispatcherTimer gameTimer;


            public Game(Grid grid, Dictionary<string, string> blockTypes, string imageFolderPath)
            {
                this.blockTypes = blockTypes;
                this.imageFolderPath = imageFolderPath;
                InitializeGame();
                InitializeTimer();

                gameGrid = grid;
                firstlaunch();
            }
            private void DrawBlocksOnGrid()
            {
                gameGrid.Children.Clear();

                for (int row = 0; row < gameBoard.GetLength(0); row++)
                {
                    for (int col = 0; col < gameBoard.GetLength(1); col++)
                    {
                        int blockType = gameBoard[row, col];

                        if (blockType != 0) // Assuming 0 means an empty cell
                        {
                            string imagePath = GetImagePathForBlockType(blockType);
                            BitmapImage image = new BitmapImage(new Uri(imagePath));

                            Image img = new Image
                            {
                                Source = image,
                                Width = blockSize,
                                Height = blockSize,
                            };

                            Grid.SetColumn(img, col);
                            Grid.SetRow(img, row);
                            gameGrid.Children.Add(img);
                        }
                    }
                }
            }

            private string GetImagePathForBlockType(int blockType)
            {
                // You need to map the blockType to the corresponding image path.
                // This could be based on the dictionary or other logic depending on your implementation.
                // For simplicity, I assume blockType directly corresponds to the index in blockTypes.
                string blockTypeKey = blockTypes.Keys.ElementAt(blockType - 1); // Subtract 1 to convert to 0-based index
                return imageFolderPath + blockTypes[blockTypeKey];
            }
            private void firstlaunch()
            {
              

                if (Tetrissettings.Default.FirstLaunchTetris)
                {
                    return;
                }
                System.Windows.MessageBox.Show("you really dont know how to play Tetris huh?");
            }

            private void InitializeTimer()
            {
                gameTimer = new DispatcherTimer();
                gameTimer.Tick += GameTick;
                gameTimer.Interval = TimeSpan.FromMilliseconds(500); // Adjust the interval as needed
            }
            private void GameTick(object sender, EventArgs e)
            {
                if (gameRunning)
                {
                 
                    CheckCollisions(); // Check for collisions after moving the tetrimino
                    ClearLines();      // Check and clear completed lines
                    UpdateScore();     // Update the score based on game state
                    CheckForGameOver(); // Check if the game is over
                    DrawBlocksOnGrid();
                }
                // Implement logic for each tick of the game (e.g., move the tetrimino down, check for collisions, etc.)
            }

    

            private void InitializeGame()
            {
                gameRunning = false;
                score = 0;
                level = 1;
                linesCleared = 0;
                gameBoard = new int[200, 404]; // Assuming this is the xaml size png
                                             //currentTetrimino = new Tetrimino();
                gameTimer = new DispatcherTimer();
                gameTimer.Tick += GameTick;
                gameTimer.Interval = TimeSpan.FromMilliseconds(500); // Adjust the speed as needed

            }

            public void GameStart()
            {
                if (Game_State == true)
                {
                    gameRunning = true;
                    gameTimer.Start();
                }

            }
            public void GamePause(object sender, EventArgs e)
            {
                // Check for game over condition
                if (Game_State == false)
                {
                    gameTimer.Stop();
                    // Implement game over logic (e.g., show a message, reset the game, etc.)
                }
            }

            private void CheckCollisions()
            {
                // Implement logic to check for collisions with the game board
            }

            private void ClearLines()
            {
                // Implement logic to clear completed lines
            }

            private void UpdateScore()
            {
                // Implement logic to update the score based on lines cleared, level, etc.
            }

            private bool IsGameOver()
            {
                // Implement logic to check if the game is over (e.g., tetrimino can't move down further)
                return false;
                Reset();
            }
            public void Reset()
            {
               // Level = 1;
              //  Score = 0;
              //  Lines = 0;
                
            }
            private void CheckForGameOver()
            {
                if (IsGameOver())
                {
                    OnGameOver();
                }
            }
            private void OnGameOver()
            {
                GameOver?.Invoke(this, EventArgs.Empty); //pass
            }

            public void HandleKeyDown(object sender, KeyEventArgs e)
            {
                if (!gameRunning)
                {
                    switch (e.Key)
                    {
                        case Key.Z: //RotationTetrimino(RotationDirection.Left);
                                    break;
                        case Key.X: //RotationTetrimino(RotationDirection.Right);
                                    break;
                        case Key.Left:
                            //TetriminoMoveLeft etc
                            break;
                        case Key.Right:
                               break;
                        case Key.Down:
                             break;
                        case Key.Up:
                            break;
                        case Key.P:
                            GamePause(sender, e);
                            break;
                        default:
                            break;
                    }
                }

            }
        }
    }
}
        // if  bt_start_stop.Content = "Pause" -> set Game_State = true;

// highscore xml







