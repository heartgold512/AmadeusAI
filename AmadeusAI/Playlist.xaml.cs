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
{ //we are eventually going to add a tetris game just pray pls
    public partial class Playlist
    {
        private IWavePlayer WaveOutDevice;
        private int currentSongIndex = 0;
        private AudioFileReader audioFileReader;
        public Playlist()
        {
            InitializeComponent();




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
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error updating timer: " + ex.Message);
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
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Problems Cycling image" + ex);
            }
        }
        private void RemoveFromPlaylist_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private void OpenPlaylist_Click(object sender, System.Windows.RoutedEventArgs e)
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

                    playlistListBox.Items.Add(filePath);

                    using (var reader = new AudioFileReader(filePath))
                    {
                        audioFileReader = new AudioFileReader(filePath);

                        // Update MusicTime with the song's length
                        TimeSpan duration = audioFileReader.TotalTime;
                        MusicTime.Text = duration.ToString(@"mm\:ss");

                        // Get the file name without extension
                        //string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

                        // Display the file name without extension in a message box
                    }
                    //   using (var reader = new AudioFileReader(filePath))
                    //     {
                    //         TimeSpan duration = reader.TotalTime;
                    //         EndTime.Text = duration.ToString(@"mm\:ss") + "/";

                    // Get the file name without extension
                    //string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

                    // Display the file name without extension in a message box
                    //     }


                    // You can store the actual file path as a Tag or in a custom property of the ListBoxItem

                    // Add the new item to the playlist

                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        //   MusicName.Text = ("Selected File: " + filePath);
                    });
                    // If you want to store the path for later use, you can assign it to the 'newpath' variable
                    //  newpath = filePath;
                    //
                }
            }
            catch (Exception ex) { }
        }
        private void PausePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (WaveOutDevice != null)
            {
                if (WaveOutDevice.PlaybackState == PlaybackState.Playing)
                {
                    WaveOutDevice.Pause();

                    Pause_Playlist.Content = "Play";

                }
                else if (WaveOutDevice.PlaybackState == PlaybackState.Paused)
                {
                    WaveOutDevice.Play();

                    Pause_Playlist.Content = "Pause";
                }
            }
            //pause the music currently playing 
            //if no music is playing do nothing (simple)
            //pause and restart the song without save state
            //harder task: implement a save state pause
        }

        // Create a new AudioFileReader for the selected file


        // Assign the audioFileReader to the WaveOutDevice

        private void PlayPlaylist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (WaveOutDevice == null)
                {
                    WaveOutDevice = new WaveOut(); // Initialize WaveOutDevice if it's null
                }

                if (playlistListBox.Items == null || playlistListBox.Items.Count == 0)
                {
                    // If no music is selected, display a message and show a notification
                    System.Windows.MessageBox.Show("Playlist is empty.");
                    return;
                }


                string filePath = playlistListBox.Items[currentSongIndex].ToString();
                if (!File.Exists(filePath))
                {
                    System.Windows.MessageBox.Show("File not found: " + filePath);
                    return;
                }

                if (WaveOutDevice.PlaybackState == PlaybackState.Playing)
                {
                    return;
                }
                else if (WaveOutDevice.PlaybackState == PlaybackState.Paused)
                {
                    WaveOutDevice.Play();
                    WaveOutDevice.PlaybackStopped += WaveOutDevice_PlaybackStopped; //event to which the music finishes
                    currentSongIndex++;
                    if (currentSongIndex >= playlistListBox.Items.Count)
                    {
                        currentSongIndex = 0; // Loop back to the beginning if all songs have been played
                    }
                }

                // Create a new AudioFileReader for the selected file
                AudioFileReader audioFileReader = new AudioFileReader(filePath);

                // Assign the audioFileReader to the WaveOutDevice
                WaveOutDevice.Init(audioFileReader);

                // Start playing the audio
                WaveOutDevice.Play();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void WaveOutDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            // Remove the finished song from the playlist
            playlistListBox.Items.RemoveAt(0);

            // Play the next song in the playlist
            PlayPlaylist_Click(null, null);
        }

        private void RemovefromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if there's an item selected in the playlist
                if (playlistListBox.SelectedItem != null)
                {
                    // Remove the selected item from the playlist
                    playlistListBox.Items.Remove(playlistListBox.SelectedItem);
                }
                else
                {
                    // If no item is selected, prompt the user to select one
                    System.Windows.MessageBox.Show("Please select a song to remove from the playlist.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }




    }
}
