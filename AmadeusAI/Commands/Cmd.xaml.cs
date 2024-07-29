using AmadeusAI.Tetris;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AmadeusAI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Cmd : Window
    {
        Visibility makeVisible = Visibility.Visible;
        private Cmd command;
        public string cmdtxt;
        public Cmd()
        {
            InitializeComponent();
        }

        private void cmd_Click(object sender, RoutedEventArgs e)
        {

            CommandTextBox.Visibility = makeVisible;
            if (CommandTextBox.Visibility == makeVisible)
            {
                CommandTextBox.Foreground = Brushes.White;
            }
            Confirmcmd.Visibility = makeVisible;
            //add a way to run programs for example resource monitor can be ran by "Run Resmon" etc
        }

        private void Command1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Command2_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CommandTextBox_KeyDown(object sender, RoutedEventArgs e)
        {
          
           cmdtxt = CommandTextBox.Text;
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
