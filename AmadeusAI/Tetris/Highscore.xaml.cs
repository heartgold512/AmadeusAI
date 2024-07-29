using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusAI.Tetris
{

    

    public partial class Highscore
        {

            private bool Game_State;
            private TetrisAma tetris;
            public Highscore()
            {



            }




            private void Bt_start_stop_Click(object sender, System.Windows.RoutedEventArgs e)
            {

            }

            private void Bt_Retry_Click(object sender, System.Windows.RoutedEventArgs e)
            {

                if (bt_continue.Content.ToString() == "Retry")
                {
                    // Logic to handle the "Start" state
                    bt_continue.Content = "Restarting . . .";
                    tetris.Show();
                    this.Close();

                    // Add any additional logic you need when transitioning to the "Pause" state
                }
            }
      
        private void Retry_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
    }
  

