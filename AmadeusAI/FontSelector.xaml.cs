using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
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
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace AmadeusAI
{ //here we are simply showing the default fonts, we will allow the user to customise the font also which in turn will change the output font of 
  //the csv files from default boring 
  //gonna have to make an instance
  //this is a reselector and accessed through a tray ico n but a simple button can select the font and size on the first launch menu
 
    public partial class FontSelector
    {
        List<string> customFonts = new List<string>();
        public FontSelector()
        {

            InitializeComponent();
            //here we retrieve the file name and then cut off the file types or we can make a setting to turn on and off file types in the xaml
            this.Closed += FontSelector_Closed; 
        }
        private void FontSelector_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveFont_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected font from the xaml markup
            try
            {
           
                var SelectedFont = fontSelector.SelectedItem as System.Windows.Media.FontFamily;
                var SelectedTypeface = typefaceSelector.SelectedItem as FamilyTypeface;
                double FontSize = AmadeusAISettings.Default.FontSize;
                string FontName = SelectedFont.Source;
                AmadeusAISettings.Default.FontFamily = FontName;
                AmadeusAISettings.Default.FontSize = fontSizeSlider.Value;
                // Save the settings

                AmadeusAISettings.Default.Save();

                System.Windows.Forms.MessageBox.Show("New Font Style Saved Sucessfuly Amadeus will now use: " + FontName + " of:" + FontSize);
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Problem Saving Font Style see Error information for more Or Please visit My Github To Post an Issue. Error: " +ex);
                Console.WriteLine("Error: " +ex);
            }
        }

        private void FontUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Font Files (*.otf;*.ttf;*.woff;*.woff2;*.vfb;*.pfa;*.fnt)|*.otf;*.ttf;*.woff;*.woff2;*.vfb;*.pfa;*.fnt|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Get the selected file path and add it to the Music Directory After the music is done itll delete from the file unless it is looped to play again which we have yet to impelment
                    string filePath = openFileDialog.FileName;
                    var newFontFamily = new System.Windows.Media.FontFamily("file://" + filePath);
                    Fonts.SystemFontFamilies.Add(newFontFamily);
              
                    AmadeusAISettings.Default.Save();
                    //filePath add to ItemsSource="{x:Static Fonts.SystemFontFamilies}"
                    //get font name and styles  filepath add ItemsSource="{Binding SelectedItem.FamilyTypefaces, ElementName=fontSelector}" 
                    //message box would you like to perma save y/n
                    //if yes
                    //save file to font list ItemsSource="{x:Static Fonts.SystemFontFamilies}" Background="orange"/>
                    //if no do nothing for now
                    //TODO: carry Font across to the list in the xaml and save the newly chosen font permanently even when the program is closed
                }
            }
            catch (Exception ex)
            {//needs improving
                System.Windows.Forms.MessageBox.Show("Please Select A font Or another" + ex);
            }



        }

        private void Load_Fonts()
        {
            //font LHS refresh
            fontSelector.ItemsSource = Fonts.SystemFontFamilies;
        }
    }
}
