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
using AmadeusAI.Behaviours;
using System.Windows.Input; //forimage control
using Application = System.Windows.Application;
using Path = System.IO.Path;
//this is a part first time wscript to save the prepacked default CsV files if the File' detected are not already there.
namespace AmadeusAI
{
    public partial class Dir
    {
       
        private string Newpath_tosearch; //need to pass this is not null and then dump the other path finding to save resources in the other file
        public string CustomFolderPath;
        public string message { get; set; } //
        public string newPathToSearch {  get; set; }//
        public Dir(string message,string newPathToSearch)
        {

            InitializeComponent();
            MessageTextBlock.Text = message;
            this.Newpath_tosearch = newPathToSearch; //passing to csv parsers
            this.Closed += Dir_Closed;
            
        }
        private void Dir_Closed(object sender, EventArgs e)
        {
            // Shut down the application needs sorting better imo maybe an option for certain terms
            this.Close();
           
            //this will create a possibly annoying start loop 
        }
        public void SetNewPath(string newPath)
        {
            Newpath_tosearch = newPath;
        }
       


            private void buttonyes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Newpath_tosearch))
                {
                    // Construct the path to the directory to delete
                    string directoryToDelete = Path.Combine(Newpath_tosearch, "AmadeusAI", "docs");

                    // Check if the directory exists
                    if (Directory.Exists(directoryToDelete))
                    {
                        // Ask the user if they want to delete the directory
                        MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to delete the folder " + directoryToDelete + "?",
                            "Confirmation?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            // The user clicked Yes, delete the directory
                            Directory.Delete(directoryToDelete, true); // 'true' means all files and subdirectories will be deleted as well

                            // Clear Newpath_tosearch
                            Newpath_tosearch = null;
                        }
                    }
                }
                //Get the path to the Roaming folder in APPdata a idden folder in User's a simple tutorial on yt explains this (youtube)
                string roamingFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                // Create a subdirectory for your application (change "AmadeusAI" to your actual application name ie if you distributed and changed this project you will have to manuelly change where the doc file are taken from)
                string appSubdirectory = System.IO.Path.Combine(roamingFolderPath, "AmadeusAI");

                // Check if the subdirectory exists, create it if not
                if (!Directory.Exists(appSubdirectory))
                {
                    Directory.CreateDirectory(appSubdirectory);
                }
                else
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("Folder " + appSubdirectory + " may exist. Overwrite?" +
                        "\nThis may delete data. Are you sure? If your custom CSV's are also saved in (if null there will be nothing here): " + Newpath_tosearch +" -> they will be deleted too."," " +
                        "Confirmation?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        // The user clicked No or closed the message box, do not overwrite
                        return;
                    }
                }

                // Full path to the source "docs" folder
                //string sourceFolderPath = @"C:\Users\Bradley\Desktop\Beta's\Amadeus-newer2\AmadeusAI\docs"; //my hardcoded path
                string sourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "docs"); //
                // Full path to the destination "docs" folder
                string destinationFolderPath = Path.Combine(appSubdirectory, "docs");
#if DEBUG 
              System.Windows.MessageBox.Show(sourceFolderPath);
#endif
                // Check if the destination directory exists, create it if not
                if (!Directory.Exists(destinationFolderPath))
                {
                    Directory.CreateDirectory(destinationFolderPath);
                }

                // Copy all files and subdirectories from the source to the destination
                foreach (string filePath in Directory.GetFiles(sourceFolderPath))
                {
                    string destinationFilePath = Path.Combine(destinationFolderPath, Path.GetFileName(filePath));
                    File.Copy(filePath, destinationFilePath, true); // Change 'true' to 'false' if you don't want to overwrite existing files
                }

                foreach (string subdirectoryPath in Directory.GetDirectories(sourceFolderPath, "*", SearchOption.AllDirectories))
                {
                    string destinationSubdirectoryPath = Path.Combine(destinationFolderPath, subdirectoryPath.Substring(sourceFolderPath.Length + 1));
                    Directory.CreateDirectory(destinationSubdirectoryPath);
                }

                // Display a success message or perform any additional actions
                System.Windows.MessageBox.Show("Folder and its contents saved successfully in :"+ appSubdirectory +" . . .now restarting AmadeusAI");
                
            }
            catch (Exception ex)
            {
                // Handle exceptions, show an error message, etc.
                System.Windows.MessageBox.Show("Failed to save folder and its contents. Error: " + ex.Message);
             //retry, yes,no, if fails recommend that the user selects an alternative path to save in rather than the defaULT THROUGH A MESSAAge box
             //if yes rerun the logic of saving the folder as default 
            }
        }
       
        private void CopyFolder(string sourceFolderPath, string destinationFolderPath)
        {
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            foreach (string filePath in Directory.GetFiles(sourceFolderPath))
            {
                string destinationFilePath = Path.Combine(destinationFolderPath, Path.GetFileName(filePath));
                File.Copy(filePath, destinationFilePath, true);
            }

            foreach (string subdirectoryPath in Directory.GetDirectories(sourceFolderPath, "*", SearchOption.AllDirectories))
            {
                string destinationSubdirectoryPath = Path.Combine(destinationFolderPath, subdirectoryPath.Substring(sourceFolderPath.Length + 1));
                Directory.CreateDirectory(destinationSubdirectoryPath);
            }
        }
        private void DeleteFolder(string folderPath) // was going to declare outside 
        {
            if (Directory.Exists(folderPath))
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to delete the folder at:  " + folderPath + " ?",
                    " Confirmation?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Directory.Delete(folderPath, true);
                }
            }
        }

        private void buttonselector_Click(object sender, RoutedEventArgs e)
        {
            // Prompt the user to choose a folder locaion to save to
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select a folder to save the files this will be saved as Newpath_tosearch";

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Update the path with the chosen folder path but only allow once as there are two folders not one
                Newpath_tosearch = folderBrowserDialog.SelectedPath;
                AmadeusAISettings.Default.CustomPath = Newpath_tosearch;
                //save the Custompath to load upon application reload
                CustomFolderPath = Path.Combine(Newpath_tosearch, "AmadeusAI", "docs");// Full path to the destination "docs" folder
                string destinationFolderPath = Path.Combine(CustomFolderPath);
                string sourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "docs");
                /*if (!Directory.Exists(CustomFolderPath))
                {
                    Directory.CreateDirectory(CustomFolderPath);

                } */
                //DeleteFolder(CustomFolderPath);

                CopyFolder(sourceFolderPath, destinationFolderPath);

                System.Windows.MessageBox.Show("Folder and its contents saved successfully in :" + CustomFolderPath + " . . .now restarting AmadeusAI");
                AmadeusAISettings.Default.Save();
                Environment.Exit(1);
                
                // Copy all files and subdirectories from the source to the destination

                /*foreach (string filePath in Directory.GetFiles(CustomFolderPath))
                {
                    string destinationFilePath = Path.Combine(CustomFolderPath, Path.GetFileName(filePath));
                    File.Copy(filePath, destinationFilePath, true); // Change 'true' to 'false' if you don't want to overwrite existing files
                }

                foreach (string subdirectoryPath in Directory.GetDirectories(CustomFolderPath, "*", SearchOption.AllDirectories))
                {
                    string destinationSubdirectoryPath = Path.Combine(destinationFolderPath, subdirectoryPath.Substring(CustomFolderPath.Length + 1));
                    Directory.CreateDirectory(destinationSubdirectoryPath);
                }
                //needs improving the files in docs are not saved yet
                // Display a success message or perform any additional actions
                System.Windows.MessageBox.Show("Folder and its contents saved successfully in :" + CustomFolderPath + " . . .now restarting AmadeusAI");
            }

            //set the appsubdirectory to null so both places are not searched freeing up space?
            //new Path 
            //System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer()
            //replace but with folder
            //newpath is set here 
            //ding
            // Specify the .wav file you can change this if you want
            //soundPlayer.SoundLocation = "";

                // private voidd method_sound(){
            // Load .wav file
            //soundPlayer.Load();

            // Play .wav file
            //soundPlayer.Play();

            //SoundPlayer.Stop();
                }
                */
            }

            try

                {
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Update the path with the chosen file path
                  
                    Newpath_tosearch = folderBrowserDialog.SelectedPath;
               
                }
            } 
            catch (Exception ex)
            {
                //code to return error mssg so i can diagnose why the hell i need to enter the file 4 times for it to finally accept on the fifth attempt
                // The user canceled, handle it accordingly (e.g., show an error message or exit)
                System.Windows.MessageBox.Show("Operation canceled. The application may not work properly without the necessary data and therefore be unstable. Specifc Error msg: " + ex);
                //this needs handling better 
                // You might want to return null or some default value depending on your requirements
                //return null
                //get rid of null and make a recalibrate setting
            }

        }
        public void msg()
        {
#if DEBUG
            System.Windows.MessageBox.Show("Remember if you would like to update the Csv files or use your own " +
                    " rename the CSV file's in the Source code so that they are personalised and updated for you so they are not overwritten by the default Csv" +
                    "For More Information Please check out our website at {get_website_name)");
#endif
        }
        private void customcsv()
        {//setting for "i dont want Amadeus" i want another character?
            //option to change the image in the xaml format 
            //select your own csv to run with chat bot with idle's , website reactions following similar struct to the default layout
            //files change name to search x5 and save as deafult instead of finding something "AmadeusAI" this will help in distribution as this is a community encouraged project i dont care how the code is used as long as it helps the collective and they share their idea's and code
            msg();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"; //this may be changed for example ive been thinking of adding a chat box and have words in a json that are fetched an ai answers based on connections and word phrases
            openFileDialog.Title = "Select a startup CSV File an example is saved in the repo";

        }
    }
}