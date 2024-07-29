using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AmadeusAI
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        private ChatBox chatBox;
        private Timer batteryTimer;
        private NotifyIcon notifyIcon;

        public Message()
        {
            InitializeComponent();
           batteryTimer = new System.Windows.Forms.Timer();
         batteryTimer.Interval = 60000; // Update battery level every minute (adjust as needed)
           batteryTimer.Tick += BatteryTimer_Tick;
            batteryTimer.Start();

        }
        public void SetMessageText(string message)
        {
            Messager.Text = message;
        }

        private void BatteryTimer_Tick(object sender, EventArgs e)
        {
            UpdateBatteryImage();
        }
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void UpdateBatteryImage()
        {
            try {
                // Access battery information and update the image accordingly
                System.Windows.Forms.PowerStatus status = System.Windows.Forms.SystemInformation.PowerStatus;

                if (status.BatteryChargeStatus == BatteryChargeStatus.NoSystemBattery)
                {
                    // Device does not have a battery, handle this case (e.g., show a different image)
#if DEBUG
                    System.Windows.MessageBox.Show("no Battery detected");
#endif
                    return;
                }
                // Example: Assuming you have different images for different battery levels
                if (status.BatteryLifePercent > 0.8)
                {
                    // Set image for high battery level
                    batteryPlaceholder.Source = new BitmapImage(new Uri("/gui/battery80.png", UriKind.Relative));
                }
                else if (status.BatteryLifePercent > 0.6)
                {
                    // Set image for medium battery level
                    batteryPlaceholder.Source = new BitmapImage(new Uri("/gui/battery60.png", UriKind.Relative));
                }
                else
                {
                    // Set image for low battery level
                    return;
                }

            }
            catch(Exception ex){
#if DEBUG
                Console.WriteLine("Exception with battery image source?" + ex);
                System.Windows.MessageBox.Show("An exception has occured at: " /*+placewholder for line*/ + ex);
                SaveFileDialog saveFileDialog = new SaveFileDialog(); //option to write to document

                return;
#endif

            }
            }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //animate to unselect
            //  <Image Source="/gui/small btns/connect_select.png"/>

            Close();
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            shortcut.Text = "Shortcut: Ctrl+Shift+F10";

            // settings = new SettingsWindow(mainWindow);
            //   settings.Show();

        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
     
        private void Ring_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Instantiate Trayicon to access Notifications
                Trayicon trayicon = new Trayicon();

                // Create an instance of Notifications using the NotifyIcon from Trayicon
                Trayicon.Notifications notifications = new Trayicon.Notifications(trayicon.NotifyIcon);

                // Call the method to show the notification
                notifications.ShowBalloonNotification("Incoming Call", "You have an incoming call!");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
#if DEBUG
                Message messageWindow = new Message();

                // Set the message text
                messageWindow.Messager.Text = "An error occurred: " + ex.Message;

                // Show the Message window
                messageWindow.ShowDialog();
#endif
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    

        private void ChatBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                chatBox = new ChatBox();
                chatBox.Show();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
