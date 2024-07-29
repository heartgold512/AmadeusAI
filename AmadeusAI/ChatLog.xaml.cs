using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmadeusAI.Behaviours;
using AmadeusAI;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using AmadeusAI.Parsers;
//using AmadeusAI.docs; ps we will have to pass all doc files into compilation and be recognised here for the words to show up here

namespace AmadeusAI
{
    public partial class ChatLog
    {
        private StringBuilder chatText = new StringBuilder();
        private readonly CSVParse csvParser = new CSVParse();
        public DateTime CurrentDateTime => DateTime.Now;

        // private StringBuilder chatText = new StringBuilder();
        //alright so first we wil design our message boxes which will "spawn in" at set parameters and fade in or write to an xaml after the message is displayed to the Mainwindow.xaml from the Csv files in docs

        
        public void AddChatText(string text)
        {
            // Append the new text to the existing chatText
            chatText.AppendLine(text);
            MessageBoxo.Text = text;
            // Update your TextBlock or wherever you want to display the chat log
            // Assuming you have a TextBlock named ChatLogTextBlock
            //  ChatTextBlock.Text = chatText.ToString();
        }
        public void LoadCSVData()
        {
            // Replace "your_csv_file.csv" with the actual CSV file path
           // List<string> parsedContent = csvParser.GetParsedContent("your_csv_file.csv");

         //   foreach (var line in parsedContent)
        //    {
        //        AddChatText(line);
        //    }
        }
        public ChatLog()
        {

            InitializeComponent();
            LoadCSVData();
            //MessageBoxo.Text = AmadeusAI.Properties.Settings.Default.UserName;

            //The display of your song and then the pre Song Name as seen in the xaml

            //here we retrieve the file name and then cut off the file types or we can make a setting to turn on and off file types in the xaml
            MyScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
          
            this.Closed += chatLog_Closed;
          

        }
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Check if the vertical offset is near the bottom
            if (e.VerticalOffset >= MyScrollViewer.ScrollableHeight - 50)
            {
                // Load more items or add new items to your ItemsControl
                 LoadMoreItems();
            }
        }
        private void LoadMoreItems()
        {

        }
        private void chatLog_Closed(object sender, EventArgs e)
        {
            //AmadeusAI.Properties.Settings.Default.Save();
            this.Close();
        }
       
    }
}
   

    

