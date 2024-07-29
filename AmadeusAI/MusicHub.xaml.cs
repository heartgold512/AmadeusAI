//Created Jan 2nd 2024
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Media;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Shapes;
using AmadeusAI.Parsers.Models;
using System.Windows.Navigation;
using Application = System.Windows.Application;
using Path = System.IO.Path;
using System.Drawing;
using Octokit;
using NAudio.Wave;//i found this nuget package which works
using AmadeusAI.Behaviours;
using System.ComponentModel;
using System.Windows.Media.Imaging; //reset the bitmap
namespace AmadeusAI
{ //we are eventually going to add a tetris game somewhere hey maybe this can be the Easter egg, id always loved secrets as a kid and still do
  //TODO: set up a Get length method to set up the duration of the song length whilst its playing the style will be bold white text and a pause button in bold white and a play in orange
    
    public partial class MusicHub
    {
        private IWavePlayer WaveOutDevice;
        private AudioFileReader audioFileReader;
        private Playlist playlist;
        private NotifyIcon notifyIcon;
        private bool notificationsEnabled = true;
        public string newpath;
        private DispatcherTimer timer;
        public MusicHub(string message)
        {

            InitializeComponent();
            MusicName.Text = "Song Name: " + message; //The display of your song and then the pre Song Name as seen in the xaml
            //here we retrieve the file name and then cut off the file types or we can make a setting to turn on and off file types in the xaml
            this.Closed += MusicHub_Closed;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Update every second
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Update the displayed song time
                if (WaveOutDevice != null && audioFileReader != null)
                {
                    TimeSpan currentTime = audioFileReader.CurrentTime;
                    MusicTime.Text = currentTime.ToString(@"mm\:ss");
                }
            }
            catch
            {
                return;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var login = new Login(messageboximg);

                // Start the animation
                login.StartAnimation();

             //how the bloody hell do i get the image back   without reopening the tray
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Problems Cycling image" + ex);
            }
        }
        private void OpenPlaylist_Click(object sender, RoutedEventArgs e)
        { //starts the xaml playlist instance we havnt yet really made
           playlist = new Playlist();
           playlist.Show();

        }
    
        private void ShowNotification(string title, string text, ToolTipIcon icon)
        {
            //this is a default template for use of notification customisation, i went on Stack
            if (!notificationsEnabled) return;
       
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
        private void DisableNotification_Click(object sender, RoutedEventArgs e)
        {
            notificationsEnabled = !notificationsEnabled;
            if (!notificationsEnabled)
            {
                Status_Lbl.Content = "Off";
            }
            else
            {
                Status_Lbl.Content = "On";
            }
        }
       

        private void MusicHub_Closed(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (WaveOutDevice != null)
            {
                if (WaveOutDevice.PlaybackState == PlaybackState.Playing)
                {
                    WaveOutDevice.Pause();
                }
                else if (WaveOutDevice.PlaybackState == PlaybackState.Paused)
                {
                    WaveOutDevice.Play();
                }
            }
            //pause the music currently playing 
            //if no music is playing do nothing (simple)
            //pause and restart the song without save state
            //harder task: implement a save state pause
        }

            private void Play_Click(object sender, RoutedEventArgs e)
        {

            if (WaveOutDevice != null)
            {
                if (WaveOutDevice.PlaybackState == PlaybackState.Playing)
                {
                    return;
                }
                else if (WaveOutDevice.PlaybackState == PlaybackState.Paused)
                {
                    WaveOutDevice.Play();
                    return;
                }
            }
            if (string.IsNullOrEmpty(newpath))
            {
                // If no music is selected, display a message and show a notification
                System.Windows.MessageBox.Show("Please select a music file first."); //improvments to trigger both of these qat the same time
                ShowNotification("Music Hub", "No music selected.", ToolTipIcon.Warning);
                return;
            }
            //DisposeWave();
            PlayMusic(newpath);


            MusicName.Text = "Now playing: " + Path.GetFileNameWithoutExtension(newpath); //test this
           
            //TODO: if no music is selected do nothing 
            //if there is a valid filepath play
            //later we will add a small white text timestamp saying how long of the video is being played as a progress bar
            //if no music is selected or playlist is empty a message will be displayed as well as a ballontip and notification sound
        }
      private void Get_length()
        {
            //get the time of the song and display it with similar syntax to this to update the timer event counter   this.Timed += Timed_Tick; 
        }

        private void MusicSelector_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Audio Files (*.wav;*.mp3;*.ogg)|*.wav;*.mp3;*.ogg|All Files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Get the selected file path and add it to the Music Directory After the music is done itll delete from the file unless it is looped to play again which we have yet to impelment
                    //reset the song playing if one to stop and set with a new one such as making it null? 



                    string filePath = openFileDialog.FileName;
                    using (var reader = new AudioFileReader(filePath))
                    {
                        TimeSpan duration = reader.TotalTime;
                        EndTime.Text = duration.ToString(@"mm\:ss") + "/";
                        
                        // Get the file name without extension
                        //string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

                        // Display the file name without extension in a message box
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                {

                    MusicName.Text = ("Selected File: " + filePath);
                });
                    // If you want to store the path for later use, you can assign it to the 'newpath' variable
                    newpath = filePath;

                }
            }
            catch
            {
                //if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                //{ 
                    System.Windows.MessageBox.Show("Cannot Load contents of the file due to Format issues");

                    return;
                //}
            }
        }

        private void PlayMusic(string filePath)
        {
            //might want to use a media player library or control interface for anyone who want to do that and share :)
            //you might just want a basic soundplayer using the system media
            try
            {
               DisposeWave(); // If a song is already playing, stop and dispose it 


                audioFileReader = new AudioFileReader(filePath);
                WaveOutDevice = new WaveOutEvent();
                WaveOutDevice.Init(audioFileReader);
                WaveOutDevice.Play();

                timer.Stop();
                timer.Start();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Added to Error Log to find available solutions please check Error Logs: " + audioFileReader + filePath + ex ); //Error Logs will have every exception contained when the program is first ran added to a text document, an Ai model will summarise the given text 
                                                                                                                                 //asks if would like to save in the music dir in the project
                                                                                                                                 //i hate pathing
            }
        }
        
        private void timey()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
            }

        }
        private void DisposeWave()
        {
            
            if (WaveOutDevice != null)
            {
                if (WaveOutDevice.PlaybackState == PlaybackState.Playing)
                {
                    WaveOutDevice.Stop();
                }
                WaveOutDevice.Dispose();
                
            }
            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                
            }
       
        }
         
        
    }
        

}
//we will later add this to the ad dplaylist method "add to playlist as a method" and pass this into an Array to play the Music in a specific order
//add button play from playlist
//the playlist file is gonna be a pain in the ass 