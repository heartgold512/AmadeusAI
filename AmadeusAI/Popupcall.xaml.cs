using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;  //this is an optional telegram connection ie for a custom bot i'm not too sure how to do this so yeah, but i love learning
using Telegram.Bot.Args;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace AmadeusAI
{
    /// <summary>
    /// Interaction logic for Popupcall.xaml
    /// </summary>

    public partial class Popupcall : Window
    {
        //    public event EventHandler<string> Errorcatch;






        public Popupcall()
        {
            InitializeComponent();

        }
        public async void YesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string botToken = Telegramsettings.Default.TelegramBotToken;
                string chatId = Telegramsettings.Default.ChatId;

                // Ensure both the token and chat ID are set
                Dictionary<string, string> messages = new Dictionary<string, string>
{
    { "NoChatId", "Please configure the Telegram Chat ID in the settings." },
    { "NoBotToken", "Please configure the bot token in the settings." },
    { "NotBoth", "Please configure the Telegram bot token and ChatID in the settings." }
};

                // Check scenarios and display corresponding message
                if (string.IsNullOrEmpty(chatId) && string.IsNullOrEmpty(botToken))
                {
                    Info.Text = messages["NotBoth"];
                }
                else if (string.IsNullOrEmpty(chatId))
                {
                    Info.Text = messages["NoChatId"];
                }
                else if (string.IsNullOrEmpty(botToken))
                {
                    Info.Text = messages["NoBotToken"];
                }
                var botClient = new TelegramBotClient(botToken);

                // Send a message
                await botClient.SendTextMessageAsync(chatId, "Hello from AmadeusAI, this is a test");

                Info.Text = "Message sent successfully!";

            }
            catch (Exception ex)
            {
                Info.Text = "Error: " + ex;
                Console.WriteLine("Error information: " + ex, " ...huh funny couldn't connect?");
                //    Errorcatch?.Invoke(this, ex.Message);


            }
        }
        public void NoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch
            {
            }
        }
    }

}
