using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using AmadeusAI.Tetris;
using System.Windows.Controls;

namespace AmadeusAI.Tetris
{
    public enum TetriminoType
    {
        I, J, L, O, S, T, Z
    }

    public class Tetrimino
    {
        private Image tetriminoImage;
        private const int blockSize = 20;
        public TetriminoType Type { get; set; }
        public int[,] Shape { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        private  void  imageblocks()
        {
            new BitmapImage(new Uri("gui/Block-I.png", UriKind.Relative));
            new BitmapImage(new Uri("gui/Block-J.png", UriKind.Relative));
            new BitmapImage(new Uri("gui/Block-L.png", UriKind.Relative));
            new BitmapImage(new Uri("gui/Block-O.png", UriKind.Relative));
            //new BitmapImage(new Uri("gui/Block-S.png", UriKind.Relative)),
            //new BitmapImage(new Uri("gui/Block-T.png", UriKind.Relative)),
            //new BitmapImage(new Uri("gui/Block-Z.png", UriKind.Relative))
        }
        
        public Tetrimino(TetriminoType type , string ImagePath)
        {
            ImagePath = ImagePath;
            Type = type;
            // Initialize the shape based on the type
 
            // Initial position at the top center of the game board
            X = 202; //width / 2 - Shape.GetLength(1) / 2;
            Y = 0; //for height when needed
            this.tetriminoImage = tetriminoImage;
            LoadTetriminoImage();
        }
        public void UpdateTetriminoUI()
        {
            // Assuming you have a UI element representing the tetrimino
            tetriminoImage.Source = LoadTetriminoImage();

            // Update the position of the UI element
            Canvas.SetTop(tetriminoImage, Y * blockSize);
            Canvas.SetLeft(tetriminoImage, X * blockSize);  // Adjust blockSize as needed
        }
        public void MoveTetriminoDown()
        {
            // Move the tetrimino down by incrementing its Y position
            
            UpdateTetriminoUI();
            // You may want to update the UI to reflect the new position of the tetrimino
            // For example, if you're using WPF, update the UI element representing the tetrimino.
            // You can also redraw the entire game board.

            // Optionally, you can raise an event or perform additional logic when the tetrimino moves down.
        }
        public BitmapImage LoadTetriminoImage()
        {
            string imagePath = $"BlockImages/{Type}.png";
            BitmapImage bitmapImage = new BitmapImage();

            if (File.Exists(imagePath))
            {
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imagePath, UriKind.Relative);
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }


      
        }
    }


