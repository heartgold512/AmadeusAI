//Created 1st Jan 2024 (i think?) ps this is the one document i dont really know about when i created it though i did create it Source: Trust me
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MenuItem = System.Windows.Forms.MenuItem;
using System.Windows.Forms;
using AmadeusAI.Tetris;
using AmadeusAI.Scanner;
using AmadeusAI;
using System.Windows;
using System.Windows.Controls;


namespace AmadeusAI
{

    internal class Trayicon
    { //this is the class which defines the tray icon logic
      //the tray icon is opened and different info can be selected to change ip and location
      //this  changes the weather paramater
       
        private static bool isIconCreated = false; //failed for some reason need to find a way to term
        public NotifyIcon NotifyIcon;
        private Gamemenu gamemenu;
        private MusicHub musicHub;
        private TetrisAma tetris;
        private PathFinder pathfinder;
        private FontSelector fontselector;
        private ChatLog chatLog;
        private ChatBox chatbox;
        private ErrorLogger Error;
        private string message; // Declare message at the class level
        private string newPathToSearch; // Declare newPathToSearch at the class level
        private Dir rewrite;
        private Expression[] toSay;
        string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;

        internal class Notifications
        {public NotifyIcon NotifyIcon;
         public Popupcall Popup;

            // Constructor to initialize the NotifyIcon
            public Notifications(NotifyIcon notifyIcon)
            {
                NotifyIcon = notifyIcon;
            }

            public void ShowBalloonNotification(string title, string text)
            {
                NotifyIcon.BalloonTipTitle = title;
                NotifyIcon.BalloonTipText = text;
                NotifyIcon.ShowBalloonTip(3000); // Display the balloon for 3 seconds
            }

            public void Call_Click(object sender, EventArgs e)
            {
                // Handle the click event. For example, you can show a window.
                // Make some weird algorithm for datetime etc and random response.
#if DEBUG
                // Calling sound
                ShowBalloonNotification("Incoming Call", "You have an incoming call!");
            //    Popup = new Popupcall();
                // Subscribe to the error event
             //   Popup.Errorcatch += OnPopupcallError;
          //      Popup.Show(); //
//
                // Create Message Instance and add a button to answer then make main be overridden with the call part of the program.
#endif
                ShowBalloonNotification("Incoming Call", "Nah... Maybe Later. . .");
            }
         //   private void OnPopupcallError(object sender, string errorMessage)
     //       {
   //             ShowBalloonNotification("Error", "Error: " + errorMessage);
       //     }
        }


        //private Queue<IEnumerable<Expression>> saying = new Queue<IEnumerable<Expression>>();
        public Trayicon()
        {
            //if main is shut terminate the icon
            //if(MainWindow.xaml.cs is shut close prevent the creation of another instead of the other if statment here 

            if (!isIconCreated) //icon spams if not included
                try
                {
                    NotifyIcon = new NotifyIcon
                    {
                        Icon = new Icon(projectDirectory + "gui\\Notif\\logo39.ico"), // customisable
                        Visible = true
                    };
                    var notifications = new Notifications(NotifyIcon);
                    NotifyIcon.Click += NotifyIcon_Click;
                    //i want to customise this but here are a few basic ideas
                    //later down the line i learned of ContextStrips that add more customisation to tray icons such as colour im settling on Darkish Grey
                    var ContextMenuStrip = new ContextMenuStrip();
                    ContextMenuStrip.BackColor = Color.DarkGray; // Set the background color
                    ContextMenuStrip.ForeColor = Color.OrangeRed;
                    ContextMenuStrip.Font = new Font("Impact", 10);
                    //needs redesigning getip cos build error
                    //ContextMenuStrip.MenuItems.Add(new MenuItem("Current Ip and Country", MenuItem_Click)); //quite obvious but it displays your Ip and country, purpose is to provide you with weather information, natural disasters incoming and storms
                    ContextMenuStrip.Items.Add("Game Menu", null, Gamemenu_Click);
                    ContextMenuStrip.Items.Add("Ip Settings and Ports", null, MenuItem_Click);
                    ContextMenuStrip.Items.Add("Chat Logs", null, Logs_Click);
                    ContextMenuStrip.Items.Add("Call", null, notifications.Call_Click);
                    ContextMenuStrip.Items.Add("Type to Amadeus", null, ChatBox_Click);
                    ContextMenuStrip.Items.Add("Error Logs", null, ErrorLogger_Click);
                    ContextMenuStrip.Items.Add("Font Selector", null, SaveFont_Click);
                    ContextMenuStrip.Items.Add("MusicHub", null, MusicHub_Click);
                    ContextMenuStrip.Items.Add("Tetris", null, Tetris_Click);
                    ContextMenuStrip.Items.Add("PathFinder", null, PathFinder_Click);
                    ContextMenuStrip.Items.Add("Rewrite App Directory", null, Rewrite_Click);
                    NotifyIcon.ContextMenuStrip = ContextMenuStrip;
                }

                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("path is incorrect please check " + ex + "current path is");

                }
            else
            {
                return;
            }
        }

        public void ExitApplication()
        {
            // Dispose the NotifyIcon object
            NotifyIcon.Dispose();

            // Exit the application
            Environment.Exit(0);
        }
    
        private void Gamemenu_Click(object sender, EventArgs e)
        {
            try
            {
                gamemenu = new Gamemenu();
                gamemenu.Show();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Tetris is unavailable: " + ex);
            }

        }
     

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            // Handle the click event. For example, you can show a window.
        }
        private void Tetris_Click(object sender, EventArgs e)
        {
            try
            {
                tetris = new TetrisAma("Minigame 001");
                tetris.Show();
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show("Tetris is unavailable: " + ex);
            }

        }
        private void PathFinder_Click(object sender, EventArgs e)
        {
            try
            {
                // pathfinder = new PathFinder();
                // pathfinder.Show();
              pathfinder = new PathFinder();
                pathfinder.Show();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Tetris is unavailable: " + ex);
            }

        }
        private void ErrorLogger_Click(object sender, EventArgs e)
        {
            Logger manualInstance = Logger.CreateNewInstance();
            Error = new ErrorLogger();
            Error.Show();

        }
        private void ChatBox_Click(object sender, EventArgs e)
        {
            chatbox = new ChatBox();
            chatbox.Show();

        }
        private void Rewrite_Click(object sender, EventArgs e)
        {
            if (message == null)
            {
#if DEBUG
                System.Windows.MessageBox.Show("Message is null: ");
                //create here the custom messag ebox messag eto return in the desgin we ar eoding later
#endif
           
            }
            if (newPathToSearch == null)
            {
#if DEBUG
                System.Windows.MessageBox.Show("newpath is null: ");
#endif
             
            }
            rewrite = new Dir(message, newPathToSearch);
            rewrite.Show();
        }


    
        private void MenuItem_Click(object sender, EventArgs e)
        {
            //This is the default menu click event
            var menuItem = (MenuItem)sender;
            if (menuItem.Text == "Current Ip and Country")
            {
                // Get the current IP address
                //    string ipAddress = IpScanner.GetLocalIPAddressAsync();

                // Show the IP address in a MessageBox
                //  MessageBox.Show($"Current IP Address: {ipAddress}");
                return;
            }
            else
            {
            
                System.Windows.MessageBox.Show($"You Selected {menuItem.Text}");
            }
            return;
        }
        private void MusicHub_Click(object sender, EventArgs e)
        {
            musicHub = new MusicHub("Music Player");
            musicHub.Show();
        }
        private void SaveFont_Click(object sender, EventArgs e)
        {
            fontselector = new FontSelector();
            fontselector.Show();
        }
        private void Logs_Click(object sender, EventArgs e)
        {
            try
            {
                chatLog = new ChatLog();
                chatLog.Show();
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("" + ex);
            }
            //opens the chatlog
        }
    }
}
