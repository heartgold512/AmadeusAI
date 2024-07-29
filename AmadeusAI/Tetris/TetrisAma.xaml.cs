//Created Jan 6th 2024
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Media;
using System.Windows.Forms;
using System.Windows.Shapes;
using AmadeusAI.Parsers.Models;
using System.Windows.Navigation;
using Application = System.Windows.Application;
using Path = System.IO.Path;
using System.Drawing;
using Octokit;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using NAudio.Wave;//i found this nuget package which works
using AmadeusAI.Tetris;
using System.Windows.Input;
using AmadeusAI.Tetris.AmadeusAI.Tetris;
using System.Web.ModelBinding;
using System.Threading.Tasks; //new folder containing game logic alone
namespace AmadeusAI
{
    public partial class TetrisAma
    {
        /// <summary>
        /// From classes Game & Tetriimino
        /// </summary>
        //private Tetrimino currentTetrimino;
        //private Game game;
        public event EventHandler TetrisClosed; //declared lsiten handler
        private DispatcherTimer timer;
        private DispatcherTimer dropTimer;
        private readonly Queue<IEnumerable<Expression>> saying = new Queue<IEnumerable<Expression>>();
        private TimeSpan elapsedTime;
        private Game tetrisGame;
        private bool isMovable = true;
        const double MaxDown = 250; // Maximum down value
        const double MaxLeft = 180;
        const double MaxRight = 180;
        private TetQueue tetQueue;

        public Dictionary<string, string> defaultBlockTypes { get; }

        public TetrisAma(string message)
        {
            try
            {
                InitializeComponent();
                defaultBlockTypes = new Dictionary<string, string>
        {
            {"IBlock", "Block-I.png"},
            {"JBlock", "Block-J.png"},
            {"TBlock", "Block-T.png"},
            {"LBlock", "Block-L.png"},
            {"SBlock", "Block-S.png"},
            {"ZBlock", "Block-Z.png"},
            {"OBlock", "Block-O.png"}
        };
                //here we retrieve the file name and then cut off the file types or we can make a setting to turn on and off file types in the xaml
                this.Closed += Tetris_Closed;
                this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
                InitializeGame();
                 //game = new Game();
                //game.GameOver += GameOverHandler; //handle a game over function
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
                tetQueue = new TetQueue(defaultBlockTypes); // Pass your block types dictionary
                LoadRandomBlock();

            }
             catch(Exception ex)
            {
                System.Windows.MessageBox.Show("An error has occured initialising Tetris: " + ex);
            }

        }
        private void LoadRandomBlock()
        {
            try
            {
                Block randomBlock = tetQueue.LoadRandomBlock();

                if (randomBlock != null)
                {
                    tetriminoblock.Source = new BitmapImage(new Uri(randomBlock.ImagePath));
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An error occurred while loading the random block: {ex.Message}");
            }
        }
        private void CreateNewBlock()
        {
            try
            {
                Block randomBlock = tetQueue.LoadRandomBlock();

                if (randomBlock != null)
                {
                //    Image newBlock = new Image();
              //      newBlock.Source = new BitmapImage(new Uri(randomBlock.ImagePath));
                

                    // Add the new block to the UI
                 

                    // Link the new random block to the new Image element's source
                //    newBlock.Tag = randomBlock;

                    // Reset the position of the new block to the top
           //         newBlock.Margin = new Thickness(tetriminoblock.Margin.Left, 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An error occurred while loading the random block: {ex.Message}");
            }

        }

        private void ResetBlockPosition()
        {
            tetriminoblock.Margin = new Thickness(tetriminoblock.Margin.Left, 0, 0, 0);
            isMovable = true; // Allow the new block to move
        }
        private void deleteblocks()
        {

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            const double MaxDown = 385;
       
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            timerLabel.Content = elapsedTime.ToString(@"hh\:mm\:ss");
            tetriminoblock.Margin = new Thickness(tetriminoblock.Margin.Left, tetriminoblock.Margin.Top + 40, 0, 0);
            double newTop = tetriminoblock.Margin.Top + 40;

            // Stop the timer when the block reaches a certain point (adjust as needed)
            //make an if statment to auto complete to the location
            //if(Keysdown is a){res();} skips the timer
            if (newTop > MaxDown) 
            {
                newTop = MaxDown;
                // Load a new random block
                isMovable = false;
                CreateNewBlock();
                // Reset the position of the new block to the top
               
            }
           

            // Move the block down
            tetriminoblock.Margin = new Thickness(tetriminoblock.Margin.Left, newTop, 0, 0);

            if (tetriminoblock.Margin.Top >= MaxDown)
            {
                //restmethod rest can be blown up and are state blocks
                rest();
                //eventually this will be a class in the folder. im just lazy atm
            }
        }
        private void rest()
        {
            return;


        }
        private void InitializeGame()
        {
            //currentTetrimino = new Tetrimino(TetriminoType.I, @"..\..\gui\square.png");
            //currentTetrimino = new Tetrimino(TetriminoType.I, "C:\\Path\\To\\Your\\Image.png");
            //method from the tetris folder
            //LoadTetriminoImage(currentTetrimino);
        }
      
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            switch (e.Key)
            {
                case System.Windows.Input.Key.Left:
                    // Move the square left only if it is movable
                   
                        double newLeft = tetriminoblock.Margin.Left - 30;

                        // If new right margin exceeds maximum, set it to maximum
                        if (newLeft > MaxLeft)
                        {
                            newLeft = MaxLeft;
                        }
                    if (isMovable)
                    {
                        // Move the square right
                        tetriminoblock.Margin = new Thickness(newLeft, tetriminoblock.Margin.Top, 0, 0);
                    }
                    break;
            
                case System.Windows.Input.Key.Right:
                    // Calculate new right margin
                  
                        double newRight = tetriminoblock.Margin.Left + 30;

                        // If new right margin exceeds maximum, set it to maximum
                        if (newRight > MaxRight)
                        {
                            newRight = MaxRight;
                        }

                    if (isMovable)
                    {
                        // Move the square right
                        tetriminoblock.Margin = new Thickness(newRight, tetriminoblock.Margin.Top, 0, 0);
                    }
                    break;
                    
                    

                case System.Windows.Input.Key.Down:
                    if (isMovable != false) //jobs a good en
                    {
                        // Calculate new top margin
                        double newTop = tetriminoblock.Margin.Top + 5;




                        // If new top margin exceeds maximum, set it to maximum
                        //this if statment hell needs redesigning
                        if (newTop < MaxDown)
                        {
                            newTop = MaxDown;
                            isMovable = false; // Block has reached the bottom and is no longer movable
                        }


                        // Move the square down
                        tetriminoblock.Margin = new Thickness(tetriminoblock.Margin.Left, newTop, 0, 0);
                    }
                        break;
                    
            }
        }




        /*
        private void LoadTetriminoImage(Tetrimino tetrimino)
        {
            // Assuming have a Grid named "Tetrisback" in the XAML
            BitmapImage tetriminoImage = tetrimino.LoadTetriminoImage();
            if (tetriminoImage != null)
            {
                System.Windows.Controls.Image imageControl = new System.Windows.Controls.Image();
                imageControl.Source = tetriminoImage;
                
            }

            else
            {
                // Log an error message or throw an exception
               System.Windows.MessageBox.Show("Error: The image could not be loaded.");
            }

        }
        */
        public async void Tetris_Closed(object sender, EventArgs e)
        {
            try
            {
                this.Say(new[]
            {
            new Expression("Leaving so soon?", "q"),
            new Expression("did you like the game?", "n"),
            new Expression("Yeah its a bit janky", "a")
        });
            }

            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Error has occured with speech?: " +ex);
            }

            await Task.Delay(5000);

            this.Close();
        }
        public async void Say(IEnumerable<Expression> text)
        {
            this.saying.Enqueue(text);
        }
        private void TetrisBlockY_Click(object sender, MouseButtonEventArgs e)
        {
          //animation maybe depending on the block clicked
        }



        private void ShowNotification(string title, string text, ToolTipIcon icon)
        {
            //this is a default template for use of notification customisation, i went on Stack
            NotifyIcon notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = SystemIcons.Information,
                BalloonTipTitle = title,
                BalloonTipText = text,
                BalloonTipIcon = icon
            };

            // Display the notification for however long you want
            notifyIcon.ShowBalloonTip(3000);
        }

       

        private void Bt_start_stop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //starts button changes text to pause starts countdown and pieces are revealed and reloads
            if (bt_start_stop.Content.ToString() == "Start")
            {
                // Logic to handle the "Start" state
                timer.Start();
                //game.GameStart(); // Start the game logic
                bt_start_stop.Content = "Pause";
                lb_state.Content = "Pause";
                //game.GameStart(); //need a way not to restart the entire game again we need some form of save state or something
                
                // Add any additional logic you need when transitioning to the "Pause" state
            }
            else
            {
                // Logic to handle the "Pause" state

                timer.Stop();
                //game.GamePause(sender, e);
                bt_start_stop.Content = "Start";  // Add any additional logic you need when transitioning to the "Start" state
                lb_state.Content = "Resume";
                //game.GamePause(sender,  e);
               

            }
        }

        private void NextImage_Click(object sender, MouseButtonEventArgs e)
        {
            // Rotate the image by 90 degrees
            Holdnext.LayoutTransform = new System.Windows.Media.RotateTransform(Holdnext.LayoutTransform is System.Windows.Media.RotateTransform rotateTransform
       ? rotateTransform.Angle + 90
       : 90);
        } 
        private void Bt_info_Click(object sender, System.Windows.RoutedEventArgs e)
        {//opens tetris settings 
            //modes
            //to lvl 9
            //endless difficulty increase till frame perfect
            System.Windows.MessageBox.Show("you really dont know how to play Tetris huh?");


        }
        private void GameOverHandler(object sender, EventArgs e)
        {
            // Handle game over logic here
            ShowNotification("Game Over", "You have been Tetrised By Me!", ToolTipIcon.Error); //you've been Tetrised
            bt_start_stop.Content = "Start";
            lb_state.Content = "Game Over";
        }
    }

} 
    