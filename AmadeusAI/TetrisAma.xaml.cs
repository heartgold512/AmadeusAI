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
using AmadeusAI.Tetris; //new folder containing game logic alone
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
        private TimeSpan elapsedTime;

        public TetrisAma(string message)
        {
            try
            {
                InitializeComponent();

                //here we retrieve the file name and then cut off the file types or we can make a setting to turn on and off file types in the xaml
                this.Closed += Tetris_Closed;
                InitializeGame();
                 //game = new Game();
                //game.GameOver += GameOverHandler; //handle a game over function
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("An error has occured initialising Tetris: " + ex);
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            timerLabel.Content = elapsedTime.ToString(@"hh\:mm\:ss");
        }
        private void InitializeGame()
        {
            //currentTetrimino = new Tetrimino(TetriminoType.I, @"..\..\gui\square.png");
            //currentTetrimino = new Tetrimino(TetriminoType.I, "C:\\Path\\To\\Your\\Image.png");
            //method from the tetris folder
            //LoadTetriminoImage(currentTetrimino);
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
        public void Tetris_Closed(object sender, EventArgs e)
        {
            TetrisClosed?.Invoke(this, EventArgs.Empty);
            this.Close();

            
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
   

        private void Bt_info_Click(object sender, System.Windows.RoutedEventArgs e)
        {//opens tetris settings 
            //modes
            //to lvl 9
            //endless difficulty increase till frame perfect



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
    