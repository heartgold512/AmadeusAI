using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AmadeusAI.Behaviours
{ //this is the Amadeus Logo file
    public class Login
    {
        private readonly string ProjectDirectory;
        private const string AnimationFolderPath = "gui/animation"; //jumping dir's
        private const string BackgroundImageFileName = "gui/bg/bg1.png";
        public Image imageControl; // Use WPF's Image control
        private string[] imageFiles;
        private int currentImageIndex;

        public Login(Image imageControl) //WPF
        {
            this.imageControl = imageControl;
            ProjectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            try
            {
                // Construct paths with defensive checks
                string animationFolderPath = Path.Combine(ProjectDirectory, AnimationFolderPath);
                string backgroundImagePath = Path.Combine(ProjectDirectory, BackgroundImageFileName);

                // Load background image
                if (File.Exists(backgroundImagePath))
                {
                    imageControl.Source = new BitmapImage(new Uri(backgroundImagePath));
                }

                // Load image files
                imageFiles = Directory.GetFiles(animationFolderPath, "logo*.png");
            }
            catch (Exception ex)
            {
              MessageBox.Show("Error with: " + ex);
            }
        }

        public void StartAnimation()
        {
            // Start the animation loop
            Task.Run(() => AnimateImages());

            //this should probably be improved
        }

        private async Task AnimateImages()
        {
            //while (true) this keeps  cycling 
                for (int i = 0; i < imageFiles.Length; i++)
                {
                // Load and display the next image
                string imagePath = imageFiles[i /*currentImageIndex*/];
                if (File.Exists(imagePath))
                {
                    imageControl.Dispatcher.Invoke(() =>
                    {
                        imageControl.Source = new BitmapImage(new Uri(imagePath));
                     //this could be improved too
                    });
                }

                // Increment the image index
                currentImageIndex = (currentImageIndex + 1) % imageFiles.Length;

                // Adjust the delay to control the speed of the animation
                await Task.Delay(12); // Adjust the delay time as needed for animation to cycle through 
            }
            imageControl.Dispatcher.Invoke(() =>
            {
                imageControl.Source = null;
                imageControl = null;
                imageFiles = null;
            });
            
            
            //to free up space we essentially have to detonate the class afte rone full cycle
            //we also have a phone call class later if we want to make an android version we can basically copy this class and keep making instances with different logics though
        }

    }
  
    
}
