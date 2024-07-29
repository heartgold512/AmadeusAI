using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using AmadeusAI;
using Octokit;
namespace AmadeusAI
{
    public partial class ChatBox
    {
       private ChatLog log;
        public ChatBox()
        {

            InitializeComponent();
            this.Closed += ChatBox_Closed;
            log = new ChatLog();
        }
        private void ChatBox_Closed(object sender, EventArgs e)
        {
            // Shut down the application needs sorting better imo maybe an option for certain terms
            this.Close();

            //this will create a possibly annoying start loop 
        }
        private void Send_Click(object sender, EventArgs e)
        {   //declare string to textbox etc
            bool success = true;
            try
            {
                string text = UserInputTextBox.Text;
                ListBoxItem newItem = new ListBoxItem();
                newItem.Content = $"Initializing Message. . . {Environment.NewLine}Parsing: {text}";
                //  if (success)
                //     {
                log.AddChatText(text);

                // Update the Text property of ChatTextBlock in the log window
                log.ChatTextBlock.Text += $"{text}{Environment.NewLine}";
                {
                    newItem.Content += $"{Environment.NewLine}Adding To Logs: {text}";

                    //   }

                    // Set the text in ChatRectangleText of ChatLog
                    MessageListBox.Items.Add(newItem);
                    UserInputTextBox.Clear();
                    /// log.SetChatText(text);
                }
            }
            catch (Exception ex)
            {
                Messageout.Text = "Error Initialising Message to Logs Check the Error Logs for more";
                //Error logs
                //error.AddError(ex);
            }
           
        }

        private void UserInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
