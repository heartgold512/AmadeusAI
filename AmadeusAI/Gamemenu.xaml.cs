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
    public partial class Gamemenu : Window
    {
        private TetrisAma tetris;
        private Dictionary<string, Func<bool>> Minigames;
        private List<string> imagePaths;
        private int currentIndex = 0;
        private DispatcherTimer timer;
        private void CommonButtonClick(object sender, RoutedEventArgs e, string message)
        {
            // Your common code here

            // Create an instance of the Message window
            Message messageWindow = new Message();

            // Set the message text
            messageWindow.Messager.Text = message;

            // Show the Message window
            messageWindow.ShowDialog();
        }

        private void Bt1_Click(object sender, RoutedEventArgs e)
        {
            // Your code here
            tetris = new TetrisAma("minigame 001");
            tetris.Show();
        }

        private void Bt2_Click(object sender, RoutedEventArgs e)
        {
            // Your code here
            CommonButtonClick(sender, e, "Coming Soon...");
        }

        private void Bt3_Click(object sender, RoutedEventArgs e)
        {
            // Your code here
            CommonButtonClick(sender, e, "Coming Soon");
        }

        private void Bt4_Click(object sender, RoutedEventArgs e)
        {
            // Your code here
            CommonButtonClick(sender, e, "Coming Soon");
        }

        private void Bt5_Click(object sender, RoutedEventArgs e)
        {
            // Your code here
            CommonButtonClick(sender, e, "Coming Soon");
        }

        private void Bt6_Click(object sender, RoutedEventArgs e)
        {
            // Your code here
            CommonButtonClick(sender, e, "Coming Soon");
        }

        private void Bt7_Click(object sender, RoutedEventArgs e)
        {
            // Your code here
            CommonButtonClick(sender, e, "Coming Soon");
        }

        private void Bt8_Click(object sender, RoutedEventArgs e)
        {
            // Your code here
            CommonButtonClick(sender, e, "Coming Soon");
        }

        private void Bt9_Click(object sender, RoutedEventArgs e)
        {
            // Your code here
            CommonButtonClick(sender, e, "Coming Soon");
        }


        private void Bt_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        // initialiseComponent();
        //   this.Closed += Tetris_Closed;
        public Gamemenu()
        {
            InitializeComponent();
            imagePaths = new List<string>
            {
                "/gui/CRT/nn0.png",
                "/gui/CRT/nn1.png",
                "/gui/CRT/nn2.png",
                "/gui/CRT/nn3.png",
                "/gui/CRT/nn4.png",
                "/gui/CRT/nn5.png",
                "/gui/CRT/nn6.png",
                "/gui/CRT/nn7.png",
                "/gui/CRT/nn8.png",
                "/gui/CRT/nn9.png"
            };
            

            // Setup the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Change this interval as needed
            timer.Tick += Timer_Tick;
            timer.Start();
            UpdateTimerImages();
            Minigames = new Dictionary<string, Func<bool>>
            {

            };



        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the timer images every tick
            UpdateTimerImages();
        }

        private void UpdateTimerImages()
        {
            // Get the current time
            DateTime currentTime = DateTime.Now;

            // Extract hours and minutes
            int hours = currentTime.Hour;
            int minutes = currentTime.Minute;

            // Update the image sources based on the time
            Hour1.Source = new BitmapImage(new Uri($"/gui/CRT/nn{hours / 10}.png", UriKind.Relative));
            Hour2.Source = new BitmapImage(new Uri($"/gui/CRT/nn{hours % 10}.png", UriKind.Relative));
            Minute2.Source = new BitmapImage(new Uri($"/gui/CRT/nn{minutes / 10}.png", UriKind.Relative));
            Minute1.Source = new BitmapImage(new Uri($"/gui/CRT/nn{minutes % 10}.png", UriKind.Relative));
        }


    }
}