// File: CSVParser.cs
// Created: 23.12.2023
// 
// See <summary> tags for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Media;
using System.Windows.Forms;
using AmadeusAI.Parsers.Models;
using AmadeusAI.Behaviours;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting.Messaging;
using static System.Net.Mime.MediaTypeNames;


namespace AmadeusAI.Parsers
{
    internal class CSVParse : IntelligenceParser
    { //here Newpath_tosearch needs to be implemented from custommsgxml.cs and the 
        public string path; //the plan is to make defpath overridable and allow saving in any locations to then change here
        public string Newpath_tosearch;
        private SettingsWindow settingsWindow;
        private Dir custommessagebox;
        /// <summary>
        ///     Retrieves the csv data containing Amadeus's responses and triggers.
        /// </summary>
        /// <param name="csvFileName">Name of the csv file to be parsed.</param>
        /// <returns>A string containing the path to the csv data or null if there is no csv file to load.</returns>

        public void SetNewPathToSearch(string newPath)
        {
            Newpath_tosearch = newPath;
        }

        public string GetData(string csvFileName)

        { //if newpathtosearch is null attempt this if not null attempt the same thing but with the other path{

            Newpath_tosearch = AmadeusAISettings.Default.CustomPath;
            //System.Windows.MessageBox.Show(Newpath_tosearch);
            if (!string.IsNullOrEmpty(Newpath_tosearch))
            {
              

                path = Path.Combine(Newpath_tosearch, "AmadeusAI\\docs\\AmadeusAI " + AmadeusAISettings.Default.Language +
                        (AmadeusAISettings.Default.Language == "English" ? " (Original)" : "") + " - " + csvFileName + ".csv");
                 System.Windows.MessageBox.Show("your Custom Path: " + path);
                bool isDefaultPathConfirmed = false;
                if (!File.Exists(path)&& !isDefaultPathConfirmed)
                {
                    // File exists, return the custom path
                    var customMessageBox = new Dir("you have chosen to install in a different path: " + Newpath_tosearch, Newpath_tosearch);
                    bool? result = customMessageBox.ShowDialog();
                   
                    if (result.HasValue && result.Value)
                    {
                        // User confirmed default path, set the flag
                        isDefaultPathConfirmed = true;
                    }
                }
            
                //customMessageBox.SetNewPath(Newpath_tosearch);
                //bool? result = customMessageBox.ShowDialog();

                /*if (result.HasValue && result.Value)
                {
                    // User confirmed default path, set the flag
                    isDefaultPathConfirmed = true;
                }
                */
                return path;
            }

                else
                {
                    //string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
                     path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                      "AmadeusAI\\docs\\AmadeusAI " + AmadeusAISettings.Default.Language + (AmadeusAISettings.Default.Language == "English" ? " (Original)" : "") + " - " + csvFileName + ".csv");
                     
                    bool isDefaultPathConfirmed = false;

                    if (!File.Exists(path) && !isDefaultPathConfirmed)
                    {
                        // if ( is Firstlaunch) {
                   
                        var customMessageBox = new Dir("Default path to Save is : C:\\**\\**\\AppData\\Roaming\\" + "the intended path is: " + path +
                            " This is a Manuel installation with " + "right button || button on the right" + " to install here?", Newpath_tosearch);
                        //}
                        //else{ //this is accessed through wanting to change install locations of csv yet to be added
                        //}
                        bool? result = customMessageBox.ShowDialog();

                        // Check the user's response
                        if (result.HasValue && result.Value)
                        {
                            // User confirmed default path, set the flag
                            isDefaultPathConfirmed = true;
                        }

                    }
                    }
                    return path;
      
                }



        // Rest of the class.../// <summary>
        ///     Traverses a csv file delimited by semi-colons and populates a list of responses and their triggers.
        /// </summary>
        /// <param name="csvPath">The full path to the csv file.</param>
        /// <returns>A list of amareResponses which contain a character's expressions and the triggers to those expressions</returns>
        public List<amareresponse> ParseData(string csvPath)
        {
            if (string.IsNullOrWhiteSpace(csvPath))
            {
                return new List<amareresponse>();
            }
          
                using (var reader = new StreamReader(csvPath))
                {
                    var characterResponses = new List<amareresponse>();
                    //pass characterResponses as a constructor to the chatlogs
                    // Parse process responses
                    while (!reader.EndOfStream)
                    {

                        var res = new amareresponse();

                        var row = reader.ReadLine();

                        if (row.StartsWith("#") || row.StartsWith("\"#") || string.IsNullOrWhiteSpace(row) ||
                            row.All(x => x == ',' || x == '\"'))
                        {
                            continue;
                        }

                        if (csvPath.EndsWith("startup.csv"))
                        {
                            row = "," + row;
                        }

                        var columns = new List<StringBuilder>();

                        // Read columns seperated by ",", but also consider verbose entries in quotation marks
                        var currentIndex = 0;
                        var quotationCount = 0;
                        columns.Add(new StringBuilder());
                        foreach (var c in row)
                        {
                            if (quotationCount % 2 == 0 && c == ',')
                            {
                                quotationCount = 0;
                                currentIndex++;
                                columns.Add(new StringBuilder());
                                continue;
                            }

                            if (c == '"')
                            {
                                quotationCount++;

                                if (quotationCount % 2 == 1 && quotationCount > 1)
                                {
                                    columns[currentIndex].Append(c);
                                }

                                continue;
                            }

                            columns[currentIndex].Append(c);
                        }

                        // Separate response triggers by comma in case there are multiple triggers to the current response
                        var responseTriggers = columns[1].ToString().Split(',');
                        foreach (var trigger in responseTriggers)
                        {
                            if (!string.IsNullOrWhiteSpace(trigger))
                            {
                                res.ResponseTriggers.Add(trigger.Trim());
                            }
                        }

                        // Get text/face pairs
                        for (var textCell = 2; textCell < columns.Count - 1; textCell += 2)
                        {
                            if (!string.IsNullOrWhiteSpace(columns[textCell].ToString()))
                            {
                                var resText = columns[textCell].ToString();
                                var responseLength = resText.Length;
                                var face = string.IsNullOrWhiteSpace(columns[textCell + 1].ToString())
                                    ? "a"
                                    : columns[textCell + 1].ToString(); // "a" face is default

                                // If text can fit in a single box then add it to the response chain, otherwise break it up.
                                while (responseLength > 0)
                                {
                                    // The dialogue box capacity differs based on character width.
                                    // From some testing it seems like a max of 90 works fine in a lot of cases but this isn't a perfect solution.
                                    if (responseLength <= 90)
                                    {
                                        res.ResponseChain.Add(new Expression(resText, face));
                                        break;
                                    }

                                    // Get next response segment and update remaining response
                                    var responseSegment = resText.Substring(0, 90);
                                    resText = resText.Substring(90).Trim();
                                    responseLength = resText.Length;

                                    res.ResponseChain.Add(new Expression(responseSegment.Trim(), face));
                                }
                            }
                        }

                    // Add response to list
                        characterResponses.Add(res);
                   
                    //carry as a constructor to a generative xaml ChatLogs
                }

                    return characterResponses;
                }

            }




        }
       
  }
