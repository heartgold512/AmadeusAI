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
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Controls;

namespace AmadeusAI
{
    public partial class PathFinder : Window
    {
        private System.Windows.Shapes.Rectangle player;
        private bool canDash = true;
        private DispatcherTimer timer;

        public PathFinder()
        {
            InitializeComponent();
            InitializeGame();
            SetupKeyboardEvents();
            SetupMouseEvents();

            // Start a timer to update the position of CERN members periodically
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5); // Adjust the interval as needed
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void InitializeGame()
        {
            // Initialize your game here
            player = Player;
        }

        private void SetupKeyboardEvents()
        {
            // Keyboard event handlers
            this.PreviewKeyDown += PathFinder_PreviewKeyDown;
        }

        private void SetupMouseEvents()
        {
            // Mouse event handlers
            this.MouseLeftButtonDown += PathFinder_MouseLeftButtonDown;
        }

        // Game loop tick event handler
        private void GameLoopTimer_Tick(object sender, EventArgs e)
        {
            // Update player position based on keyboard/mouse input
            // (Movement code is handled in the keyboard/mouse event handlers)
        }

        private void PathFinder_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Handle keyboard input for player movement
            switch (e.Key)
            {
                case Key.Up:
                    MovePlayer(0, -2);
                    break;
                case Key.Down:
                    MovePlayer(0, 2);
                    break;
                case Key.Left:
                    MovePlayer(-2, 0);
                    break;
                case Key.Right:
                    MovePlayer(2, 0);
                    break;
                case Key.Space:
                    if (canDash)
                    {
                        DashPlayer();
                        canDash = false;
                        // Allow dashing again after 3 seconds
                        DispatcherTimer cooldownTimer = new DispatcherTimer();
                        cooldownTimer.Interval = TimeSpan.FromSeconds(3);
                        cooldownTimer.Tick += (s, args) =>
                        {
                            canDash = true;
                            cooldownTimer.Stop();
                        };
                        cooldownTimer.Start();
                    }
                    break;
            }
        }

        private void PathFinder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Handle mouse input for player movement
            System.Windows.Point mousePosition = e.GetPosition(this);
            MovePlayerTo(mousePosition);
        }

        private void MovePlayer(double deltaX, double deltaY)
        {
            player.Margin = new Thickness(
                player.Margin.Left + deltaX,
                player.Margin.Top + deltaY,
                player.Margin.Right,
                player.Margin.Bottom
            );
        }

        private void MovePlayerTo(System.Windows.Point position)
        {
            player.Margin = new Thickness(
                position.X - player.Width / 2,
                position.Y - player.Height / 2,
                0,
                0
            );
        }

        private void DashPlayer()
        {
            MovePlayer(10, 10); // Move the player 10 pixels 
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateCERNMemberPosition(CERN_member1);
            UpdateCERNMemberPosition(CERN_member2);
            UpdateCERNMemberPosition(CERN_member3);
            // Add more CERN members if needed
        }

        // Define a function to update the position of a CERN member towards the player
        private void UpdateCERNMemberPosition(System.Windows.Shapes.Rectangle CERNMember)
        {
            // Calculate the direction towards the player
            double playerX = player.Margin.Left + player.Width / 2; // Get x-coordinate of the center of the player
            double playerY = player.Margin.Top + player.Height / 2; // Get  y-coordinate of the center of the player

            double cernX = CERNMember.Margin.Left + CERNMember.Width / 2;
            double cernY = CERNMember.Margin.Top + CERNMember.Height / 2;

            double deltaX = playerX - cernX;
            double deltaY = playerY - cernY;

            // Normalize the direction
            double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            deltaX /= length;
            deltaY /= length;

            // Define the speed of the CERN members
            double speed = 2.0;

            // Update the position of the CERN member
            double newLeft = CERNMember.Margin.Left + deltaX * speed;
            double newTop = CERNMember.Margin.Top + deltaY * speed;


            newLeft += (CERNMember.Width / 2);
            newTop += (CERNMember.Height / 2);

            CERNMember.Margin = new Thickness(newLeft, newTop, 0, 0);
        }
    }
}
