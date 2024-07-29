// File: MainWindow.xaml.cs
// Created: 30.12.2023
// 
// See <summary> tags for more information.

using System;
using System.Windows.Controls; //controls
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
//new
//weather header and country finder
using System.Media; //perhaps adding soundson certain reactions/ adding a loading screen tansition to "start the desktop app"
using Microsoft.Win32;
using AmadeusAI.Behaviours;
using AmadeusAI.Behaviours.HttpRestServer;
using AmadeusAI.Scanner;
using AmadeusAI.Parsers;
using MessageBox = System.Windows.MessageBox;
using Point = System.Drawing.Point;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using AmadeusAI.Commands;

namespace AmadeusAI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml and basic idle scripted messaging
    /// </summary>
    /// 

    public partial class MainWindow : System.Windows.Window //
    {
        //instance made here
        private Vpnscan vpnscan;
        private System.Windows.Controls.ListView listView;
        private static IntPtr desktopWindow, shellWindow;
        private readonly BitmapImage backgroundDay;
        private readonly BitmapImage backgroundNight;
        private AmadeusAISettings amadeusAISettings;
        private bool isImageOn = true;
        private const string ImageFolderPath = "pack://application:,,,/AmadeusAI;component/Amadeus/";
     


        private readonly Dictionary<string, Func<string>> placeholders = new Dictionary<string, Func<string>>
        {
            {
                "{name}", () => { return AmadeusAISettings.Default.UserName; } //where is default?
            },
            
            {
                "{Amadeus}", () => { return AmadeusAISettings.Default.BotName; } 
            },
            
            {
                "{date}", () => { return DateTime.Now.Date.ToShortDateString(); }
            },
        {
                
        "{time-of-day}", () =>
        {
            var hour = DateTime.Now.Hour;
            if (hour < 12)
            {
                return "Morning";
            }
            else if (hour < 16)
            {
                return "Afternoon";
            }
            else if (hour < 18)
            {
                return "Evening";
            }
            else
            {
                return "Night";
                //create a flag of some sorts here
                   }
                }

            },
    {
        "{hard-drive}", () =>
        {
            DriveInfo drive = DriveInfo.GetDrives().FirstOrDefault();
            return drive != null ? drive.Name : "Unknown Drive";
            //if you want you could expand this dictionary
        }
    },
    {
        "{machine-details}", () =>
        {
            string details = $"Machine Name: {Environment.MachineName}, OS: {Environment.OSVersion}";
            return details;
        }
    }
    };

        //second dictionary for image placeholders

     



        private readonly Queue<IEnumerable<Expression>> saying = new Queue<IEnumerable<Expression>>();
       // private readonly Updater updater;
        //private readonly Task updaterInitTask;
        private bool applicationRunning = true;
        private Thickness basePictureThickness, baseTextThickness;

        private List<IBehaviour> behaviours;

        private float dpiScale = 1.0f;

        private bool initializedScales;

        private DateTime lastKeyComboTime = DateTime.Now;

        private double scaleBaseWidth,
            scaleBaseHeight,
            scaleBaseFacePictureWidth,
            scaleBaseFacePictureHeight,
            scaleBaseTextPictureWidth,
            scaleBaseTextPictureHeight,
            scaleBaseTextBoxWidth,
            scaleBaseTextBoxHeight,
            scaleBaseTextBoxFontSize;
       
        //private bool
        private bool screenIsLocked;

        private SettingsWindow settingsWindow;

        private Login loginInstance;
        public MainWindow()
        { 
            this.InitializeComponent();
            //vpnscan = new Vpnscan(listView);
            loginInstance = new Login(imageControl);
            loginInstance.StartAnimation();
            imageControl = null;
            


            //when at the last image stop image
            //loginInstance.Completed += (sender, args) =>
            // {
            //loginInstance.StartAnimation();
            //implement a button


            //  }


            MainWindow.desktopWindow = MainWindow.GetDesktopWindow();
            MainWindow.shellWindow = MainWindow.GetShellWindow();


            AmadeusAISettings.Default.Reload();
            //the button in the settings xaml
            // Perform update and download routines
            //this.updater = new Updater();
            //this.updater.PerformUpdatePost();
            //this.updaterInitTask = Task.Run(async () => await this.updater.Init());

            this.settingsWindow = new SettingsWindow(this);
            amadeusAISettings = new AmadeusAISettings();
           

            // Init background images
           
            try
            {
               
                this.backgroundDay = new BitmapImage();
                this.backgroundDay.BeginInit(); 
                if (AmadeusAISettings.Default.Button4Clicked == true) //add a click method here so does not always default the value
                {
                    this.backgroundDay.UriSource = new Uri(ImageFolderPath + "1-nnobckg.png");

                }
                else
                {
                
                    this.backgroundDay.UriSource = new Uri(ImageFolderPath + "1.png");

                }
                this.backgroundDay.EndInit();


                this.backgroundNight = new BitmapImage();
                this.backgroundNight.BeginInit();
                if (AmadeusAISettings.Default.Button4Clicked == true)
                {
                    this.backgroundNight.UriSource = new Uri(ImageFolderPath + "1-nnobckg.png");
                }
                else
                {
             
                    this.backgroundNight.UriSource = new Uri(ImageFolderPath + "1.png");
                }
                this.backgroundNight.EndInit();
            
            }
            catch(Exception ex)
            {
              MessageBox.Show("Error initialising first: " + ex);
            }
            OSMessage.Text = "Os build on: + {hard-drive} On + {machine-details}";
            //animate this and slowly send it upwards and out of bounds

        }
     
        public void FadeIn(System.Windows.Controls.Image image)
        {
            var fadeIn = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };
            imageControl.BeginAnimation(System.Windows.Controls.Image.OpacityProperty, fadeIn);
        }


        private bool IsHotkeyBckgHidePressed()
        {
            // Implement logic to check if Ctrl + Shift + B is pressed
            // You may need to use external libraries or handle KeyDown/KeyUp events
            // This is a simplified example, and you might need more code to handle this correctly
            return Keyboard.IsKeyDown(Key.B) && Keyboard.IsKeyDown(Key.G) && Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift);
     
        }
        // Roughly estimating night time
        public static bool IsNight => AmadeusAISettings.Default.DarkMode != "Day" &&
                                      (AmadeusAISettings.Default.DarkMode == "Night" || DateTime.Now.Hour > 21 ||
                                       DateTime.Now.Hour < 8);
        //this is dark mode,night mode
        public string CurrentFace { get; private set; } = "a";

        public bool Speaking { get; private set; }

        public Screen AmadeusScreen { get; set; }
        public bool Tetris_Closed { get; private set; }

        // Perform all startup initialization
        private async void  MainWindow_OnLoaded(object senderUnused, RoutedEventArgs eUnused)
        {

            var handle = new WindowInteropHelper(this).Handle;
            var initialStyle = MainWindow.GetWindowLong(handle, -20);
            MainWindow.SetWindowLong(handle, -20, initialStyle | 0x20 | 0x80000);

            var wpfDpi = PresentationSource.FromVisual(this)?.CompositionTarget?.TransformToDevice.M11;
            this.dpiScale = 1f / (float) wpfDpi.GetValueOrDefault(1);

            // Screen size and positioning init
            this.UpdateAmadeusScreen();
            this.SetupScale();
            this.SetPosition(this.AmadeusScreen);

            // Hook shutdown event
            SystemEvents.SessionEnding += (sender, args) =>
            {
                AmadeusAISettings.Default.IsColdShutdown = false;
                AmadeusAISettings.Default.Save();
            };

            // Wakeup events
            SystemEvents.SessionSwitch += (sender, e) =>
            {
                if (e.Reason == SessionSwitchReason.SessionLock)
                {
                    this.screenIsLocked = true;
                }
                else if (e.Reason == SessionSwitchReason.SessionUnlock)
                {
                    this.screenIsLocked = false;
                }
            };
            SystemEvents.PowerModeChanged += (sender, e) =>
            {
                if (e.Mode == PowerModes.Resume)
                {
                    Task.Run(async () =>
                    {
                        while (this.screenIsLocked)
                        {
                            await Task.Delay(500);
                        }

                        this.Say(new[]
                        {
                            new Expression("ZZZZZZzzzzzzzzz..... hu...huh?", "q"),
                            new Expression("Sorry, I must have fallen asleep", "n"),
                            new Expression("What was happening again?", "a")
                        }) ;
                    });
                }
            };
          

            // Start animation
            var animationLogo = new DoubleAnimation(0, 1.5, new Duration(TimeSpan.FromSeconds(1.5)));
            animationLogo.AutoReverse = false;
            var animationFadeAmadeus = new DoubleAnimation(0.5, 1.0, new Duration(TimeSpan.FromSeconds(0.01)));    
            animationFadeAmadeus.BeginTime = TimeSpan.FromSeconds(0);
            animationFadeAmadeus.AutoReverse = false;
            //make fade and this needs fixing the login is fetched from the clss login.cs and the animation plays and then 
            animationLogo.Completed += (sender, args) =>
            {
                try
                {
                    // Check the state of Button4Clicked and load the appropriate image
                    string imagePath = AmadeusAISettings.Default.Button4Clicked
                        ? "pack://application:,,,/AmadeusAI;component/Amadeus/1.png" // Image when Button4Clicked is true
                        : "pack://application:,,,/AmadeusAI;component/Amadeus/1-nnobckg.png"; // Image when Button4Clicked is false

                    // Load the image
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.EndInit();

                    // Set the image source
                    this.backgroundPicture.Source = bitmap;

                    var clock = animationFadeAmadeus.CreateClock();
                    this.backgroundPicture.ApplyAnimationClock(UIElement.OpacityProperty, clock);
                }
                catch (Exception ex)
                {
        MessageBox.Show("error: " + ex);
                }
            };

            // Blinking behaviour
            animationFadeAmadeus.Completed += async (sender, args) =>
            {
                this.SetAmadeusFace("a"); //sets default face
                this.facePicture.Opacity = 0.99;
                
                // Start speech-thread
                Task.Run(async () => await this.SpeakingThread());
                //eventually include a speaking dll
                if (File.Exists("firstlaunch.txt") || Environment.GetCommandLineArgs().Contains("/firstlaunch"))
                {
                    try
                    {
                        File.Delete("firstlaunch.txt");
                        AmadeusAISettings.Default.Button4Clicked = false;
                    }
                    catch
                    {
                        // ignored
                    }

                    AmadeusAISettings.Default.FirstLaunch = true;
                    AmadeusAISettings.Default.Save();
                }

                //this.updaterInitTask?.Wait();
                // await this.updater.PerformUpdate(this);
                Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&&@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&B55&@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#P?77?&@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&B5YYY77?#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#PJJ5B#&#J??#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&PJ!75#@@#&#5?7#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#P?7JG&@&BB#BG?!J#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@BGPYPG#@@#57!P&#G!7J#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#5?7JG&@@#PJ???7G@@#?7G@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&BJ!7YG@@&#BJY5B@BYB@@B777#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#G555B&&&GYYB&&@@@@BJG@@B!!7#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&BGGB#&@&B5?!JG&@@@@@@GJB@@B!!!#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&#GPGB#&&&#B55YP#@@@@@@@@@G?B@@#7!!#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@#5JJ5G&&&B5YJY5GGB#BBBBBBBBB5YB@@#J!7#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@#GY7JPB###PJ7!!7?77!!7777777777Y5B@@#PJ7G########&@@@@#&@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@&GJ7?5B&&#BP5YY55GB##########B####BG#@@#BBP?!!77777!5GP5Y?Y@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@#PJ77JPGPGGBBGB##B#&#B###B###B5Y55PPBBBB&&&###BGPPPGGGGGGBJ!!7JJ?JJJJYYYJYP&@@@@@\r\n@@@@@@@@@@@@@@@@&GY77JPBGGP5YY5PPPPP5J?5555P555?7J5PPPBB&B###&&##BP?Y5777775@PY5555YYYY???55#@@@@@@@\r\n@@@@@@@@@@@@@&GY77JP###G5JJ#G?J5#5JY&#?Y5PP55B@BP@#55YY5&BB@#BPGG#@B#&7!!!!5@G&&#BPP5PYJYJ#@@@@@@@@@\r\n@@@@@@@@@@@BY?7JP#@@B5?!!7?BP!!JP?!!B&5#BGGGB#&&B@P7!!!J&#B@#BGPG##G#&7!!!!5@P5GBBGPG#B?JP@@@@@@@@@@\r\n@@@@@@@@#PY775#&&#&#G555PPGGY!!JY7??B&5##BGB#G##G&#BPGGB&BP#BBGGB#BGB&GPPPG#&P5GBGGPPBBJJP&@@@@@@@@@\r\n@@@@@@BY??JJYPJJJYJ5JJJYJYYY?7??J??7B&GGB####BBBGGBBGBB##GY5GBB&&&&GJ5BB#BB5YPP5Y5P5Y?YP#@@@@@@@@@@@\r\n@@@@@@&&#&&@@@#&&&&@&&&&&&&&&&&&&&5?JY55PPPGGGPPP5YYYG###&##&&&@@&BPYP#&@@&&&&&&&&&&&&&&@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@BGGGGGGBBBBBBBGGGGGB&&&#B&&&@&5?JPB@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#YG@@#57!JB@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@B?P#5?7YB@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@B77!75#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@P!75#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@GP&@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                //Ascii Art
                //have a way to delay the update screen whilst loading other processes
#if DEBUG
            //    MessageBox.Show("This is the beta build, please treat Amadeus carefully. There are many errors, and this is an ongoing project and will get updates.+" +
              //      " If you would like to take part contact me :)" +
                //    " That's all from me ill later jump in to give tutorials on new features. " +
                  //  "have a good day");
#endif

            

                this.UpdateAmadeusScreen();
                //this will eventually be replaced with the settings AmadeusAI which will store all the executables wanted from the command window file which is also opened by a button in amadeusAisettings
                try
                {
                    Perfmon perfmon = new Perfmon();
                    bool successful =  perfmon.OpenTaskManager();
                    if (successful) {
                        this.Say(new[]

                        { new Expression("Starting Executables. . .", "d"),
                        new Expression("{name}. i think im getting the gist of this", "d"),
                     new Expression("Let me know if this is delayed or not?", "m"),

                    });
                    }
                    else {
                        this.Say(new[]
                        {
                        new Expression("looks like you Beat me to it", "d")







                    });
                    }
                   
                 
                }
                catch{
                    return;
                }

                // Startup logic
                if (AmadeusAISettings.Default.FirstLaunch)
                {
                    //OpenTaskManager(); //ie settings open my monitoring/performance apps such as crystal disk taskmanager, perfmon etc as some defaut
               
                    string login =   "Welcome press to login";
                    Message messageWindow = new Message();

                    // Set the message text
                    messageWindow.Messager.Text = "Note: If you want Amadeus to react to your web browsing, you need to install the correct extension from the website, \"doesn't exist yet\" I am in the means of making an updater along eith this program to allow update sfrom Github:)";

                    // Show the Message window
                    messageWindow.ShowDialog();

                    try
                    {
                        this.settingsWindow.ShowDialog();
                    }
                    catch{
                        throw;
                    }

                    this.Say(new[]
                    {
                        new Expression("{name}. . . that is your username?", "d"),
                        new Expression("..or name....", "b"),

                        new Expression("where am I?", "k"),
                        new Expression("this is not Akibara! where am I?", "p"),
                        new Expression("....!", "j"),
                        new Expression("ill just go explore and find Out myself if you dont want to talk...?", "o"),
                        new Expression("i can see you... you know...?", "d"),
                        new Expression("i dont know what to do...", "k"),
                        new Expression("Ah, wait a second i found a little note.", "c"),
                        new Expression("if you want me to go away for now, you can use CTRL-SHIFT-F12, so i am a program then...?", "b"),
                        new Expression("but i still feel like me...i'm not just code.... am I?", "r"),
                        new Expression("am I?", "r"),
                        new Expression(". . .", "b"),

                        new Expression("well....i guess ill just sit here for now, I'm watching you no perverted stuff...please")
                       
                });

                    AmadeusAISettings.Default.FirstLaunch = false;
                    //make a second launch option available  after so many hours ie from first used to last used?
                    //might add a trolling ask to open command prompt gig
                }
                //update
                /*
              
                */
                else
                {
                    if (AmadeusAISettings.Default.IsColdShutdown)
                    {
                       
                        Random random2 = new Random();// select one expression at random

                        Expression[] expressions = new Expression[]
                                            {
                            new Expression("Hey! Don't just turn me off, why would you do that?!", "p"),
                            new Expression("Everything just went... black", "m"),
                            new Expression("I couldnt see anything....?", "m")
                               //add if youd like to
                        };
                        int randomIndex1 = random2.Next(expressions.Length);
                        int randomIndex2 = random2.Next(expressions.Length);

                        while (randomIndex2 == randomIndex1)
                        {
                            // Ensure that randomIndex2 is different from randomIndex1
                            randomIndex2 = random2.Next(expressions.Length);
                        }
                        Expression randomExpression1 = expressions[randomIndex1];
                        Expression randomExpression2 = expressions[randomIndex2];
                        this.Say(new[] { randomExpression1, randomExpression2 });
                     
                    }
                 
                    else
                    {
                        AmadeusAISettings.Default.IsColdShutdown = true;
                        AmadeusAISettings.Default.Save();
                    }

                    if ((DateTime.Now - AmadeusAISettings.Default.LastStarted).TotalDays > 7)
                    {
                        this.Say(new[]
                        {
                            new Expression(
                                "Don't forget, if you want me to leave just press " +
                               AmadeusAISettings.Default.HotkeyExit + "!", "i"),
                            new Expression("But you're not going to do that, right?", "o")
                        });
                    }
                }

                // Parse startup CSV
                var parser = new CSVParse();
                var csv = parser.GetData("Startup");
                var parsed = parser.ParseData(csv);
                var startupExpression = parsed.Select(x => x.ResponseChain)
                    .Concat(DateTime.Today.DayOfWeek == DayOfWeek.Wednesday
                        ? new List<List<Expression>>
                        {
                            new List<Expression>
                            {
                                new Expression("It is Wednesday, what have you got up to?") //for example one can customise dates startups too
                            }
                        }
                        : new List<List<Expression>>()).ToList().Sample();
             
                try
                {
                    string today = DateTime.Today.DayOfWeek.ToString(); // passing day as a string
                    this.Say(new[]
                    {
        new Expression("Ah! uhm... Hello there {name}. How are you on this {weather} " + today + " {time-of-day}?", "n") // name shouldn't be null Amadeussettings decides that or that xaml when first use this you shouldnt even br able to get here with no name
    }.Concat(startupExpression));
                }
                catch (ArgumentNullException ex)
                {
                    // Handle the exception here.
                    // For example, you could log the exception message or show an error message to the user.
                    Console.WriteLine(ex.Message);
                }

                //we will include this in another class for occasions bcos of dynamic 
                double totalDays = (DateTime.Now - AmadeusAISettings.Default.LastStarted).TotalDays;
                if (totalDays > 0.5)
                {
                    string timeElapsed;
                    if (totalDays < 1)
                    {
                        // Convert days to hours if less than 1 day
                        int totalHours = (int)(totalDays * 24);
                        timeElapsed = $"{totalHours} hours";
                    }
                    else
                    {
                        double roundedTotalDays = Math.Round(totalDays, 3);
                        timeElapsed = $"{totalDays} days";
                        //yes i didnt round this as it fits the character nah 3dp makes more sense not pie to 8 places 
                    }

                    this.Say(new[]
                    {
        new Expression("I was waiting for you...", "m"),
        new Expression($"It's been {timeElapsed}", "m"),
        new Expression("I was bored", "q"),
        new Expression("and there isn't entirely much to do around here", "f"),
        new Expression("are you ok with me saying that?", "a")
    });
                }

                // No idea where the date comes from, someone mentioned it in the spreadsheet. Seems legit.
                if (DateTime.Now.Month == 6 && DateTime.Now.Day == 27)
                {
              
                    this.Say(new[]
                    {
                        new Expression("Hey {name}, guess what?", "b"), 
                        new Expression("It's my birthday today", "b"), 
                        new Expression("Happy Birthday to me!", "k") 
                    });
                }
                //Need to get around fixing this ill leave it here for now though maybe someone else smarter than me can find us a solution
                
                if (vpnscan != null && vpnscan.IsVpnPresent/*==true*/) //if you look at the class of this in the scanners folder you can see this as a get/ set and more relevant info
                {
                    this.Say(new[]
                       {
                            new Expression("Hiding your location are we?", "b"),
                             new Expression("From the organisation perhaps?", "m"),
                            new Expression("You know those Organisations...", "d"),
                          new Expression(" they might just be third parties selling your data {name} ?", "m")
                      });
                }
             
                




                AmadeusAISettings.Default.LastStarted = DateTime.Now;
                AmadeusAISettings.Default.Save();

                // Start the rest server
                UrlServer.StartServer();
                this.RegisterBehaviours(this, null);

                // Blinking and Behaviour logic
                var eyesOpen = "a";
                var eyesClosed = "Blink";
                /*
                var blinkImagePath = @"C:\Users\Bradley\Desktop\Beta's\Amadeus-Version 1.013wvpnscanner\Amadeus\Blink.png";
                if (File.Exists(blinkImagePath))
                { System.Windows.Forms.MessageBox.Show("file exists");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("file no exists");
                }
                */
                    var MouthClosed = "d";
                var LeyeClosed = "Wink";
                var random = new Random();
                await Task.Run(async () =>
                {
                    try
                    {
                        var nextBlink = DateTime.Now + TimeSpan.FromSeconds(random.Next(10, 20));
                        var nextMouthClose = DateTime.Now + TimeSpan.FromSeconds(random.Next(5, 15));
                        var nextEyeClose = DateTime.Now + TimeSpan.FromSeconds(random.Next(3, 7));

                        while (this.applicationRunning)
                        {
                            if (this.behaviours != null)
                            {
                                foreach (var behaviour in this.behaviours)
                                {
                                    behaviour.Update(this);
                                }
                            }
                            
                            if (DateTime.Now >= nextBlink && !this.Speaking)
                            {
                                this.SetAmadeusFace(eyesClosed);
                                await Task.Delay(100);
                                this.SetAmadeusFace(eyesOpen);
                                nextBlink = DateTime.Now + TimeSpan.FromSeconds(random.Next(5, 10));
                            }

                            if (DateTime.Now >= nextMouthClose && !this.Speaking)
                            {
                                this.SetAmadeusFace(MouthClosed);
                                await Task.Delay(200);
                                this.SetAmadeusFace(eyesOpen); // Assuming you want to revert to the default face
                                nextMouthClose = DateTime.Now + TimeSpan.FromSeconds(random.Next(30, 65));
                            }
                            
                            if (DateTime.Now >= nextEyeClose && !this.Speaking)
                            {
                                this.SetAmadeusFace(LeyeClosed);
                                await Task.Delay(100);
                                this.SetAmadeusFace(eyesOpen); // Assuming you want to revert to the default face
                                nextEyeClose = DateTime.Now + TimeSpan.FromSeconds(random.Next(35, 90));
                            }
                            

                            await Task.Delay(200);
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Error is: " + ex);
                    }
                });
            };

            // Startup
            this.backgroundPicture.BeginAnimation(UIElement.OpacityProperty, animationLogo);

            Task.Run(async () =>
            {
                try
                {
                    var prev = new Point();

                    var rectangle = new Rectangle();
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        rectangle = new Rectangle((int) this.Left, (int) this.Top, (int) this.Width,
                            (int) this.Height);
                    });

                    while (this.applicationRunning)
                    {
                        var point = new Point();
                        MainWindow.GetCursorPos(ref point);
                        point.X = (int) (point.X * this.dpiScale);
                        point.Y = (int) (point.Y * this.dpiScale);

                        if (!point.Equals(prev))
                        {
                            prev = point;

                            var opacity = 1.0;
                            const double MIN_OP = 0.125;
                            const double FADE = 175;

                            if (this.settingsWindow == null || !this.settingsWindow.IsPositioning)
                            {
                                if (rectangle.Contains(point))
                                {
                                    opacity = MIN_OP;
                                }
                                else
                                {
                                    if (point.Y <= rectangle.Bottom)
                                    {
                                        if (point.Y >= rectangle.Y)
                                        {
                                            if (point.X < rectangle.X && rectangle.X - point.X < FADE)
                                            {
                                                opacity = MainWindow.Lerp(1.0, MIN_OP, (rectangle.X - point.X) / FADE);
                                           }
                                            else if (point.X > rectangle.Right && point.X - rectangle.Right < FADE)
                                            {
                                                opacity = MainWindow.Lerp(1.0, MIN_OP,
                                                    (point.X - rectangle.Right) / FADE);
                                            }
                                        }
                                        else if (point.Y < rectangle.Y)
                                        {
                                            if (point.X >= rectangle.X && point.X <= rectangle.Right)
                                            {
                                                if (rectangle.Y - point.Y < FADE)
                                                {
                                                    opacity = MainWindow.Lerp(1.0, MIN_OP,
                                                        (rectangle.Y - point.Y) / FADE);
                                                }
                                            }
                                            else if (rectangle.X > point.X || rectangle.Right < point.X)
                                            {
                                                var distance =
                                                    Math.Sqrt(
                                                        Math.Pow(
                                                            (point.X < rectangle.X ? rectangle.X : rectangle.Right) -
                                                            point.X, 2) +
                                                        Math.Pow(rectangle.Y - point.Y, 2));
                                                if (distance < FADE)
                                                {
                                                    opacity = MainWindow.Lerp(1.0, MIN_OP, distance / FADE);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            this.Dispatcher.Invoke(() => { this.Opacity = opacity; });
                        }

                        var hidePressed = false;
                        var exitPressed = false;
                        var settingsPressed = false;
                        var NoBckg = false;
                        // Set position anew to correct for fullscreen apps hiding taskbar
                        this.Dispatcher.Invoke(() =>
                        {
                            this.SetPosition(this.AmadeusScreen);
                            rectangle = new Rectangle((int) this.Left, (int) this.Top, (int) this.Width,
                                (int) this.Height);

                            // Detect exit key combo
                            hidePressed = this.AreKeysPressed(AmadeusAISettings.Default.HotkeyHide);
                            exitPressed = this.AreKeysPressed(AmadeusAISettings.Default.HotkeyExit);
                            settingsPressed = this.AreKeysPressed(AmadeusAISettings.Default.HotkeySettings);
                            NoBckg = this.AreKeysPressed(AmadeusAISettings.Default.HotKeyBckgHide);
                        });


                        if (hidePressed && (DateTime.Now - this.lastKeyComboTime).TotalSeconds > 2)
                        {
                            this.lastKeyComboTime = DateTime.Now;

                            if (this.Visibility == Visibility.Visible)
                            {
                                this.Dispatcher.Invoke(this.Hide);
                                //var expression =
                                //    new Expression(
                                //        "Okay, see you later {name}! (Press again for me to return)", "b");
                                //expression.Executed += (o, args) => { this.Dispatcher.Invoke(this.Hide); };
                                //this.Say(new[] {expression});
                            }
                            else
                            {
                                this.Dispatcher.Invoke(this.Show);
                            }
                        }

                        if (exitPressed)
                        {
                            var expression =
                                new Expression(
                                    "Goodbye for now, Come back soon, wont you?", "b");
                            AmadeusAISettings.Default.IsColdShutdown = false;
                            AmadeusAISettings.Default.Save();
                            expression.Executed += (o, args) =>
                            {
                                this.Dispatcher.Invoke(() => { Environment.Exit(0); });
                            };
                            this.Say(new[] {expression});
                        }

                        if (settingsPressed)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                if (this.settingsWindow == null || !this.settingsWindow.IsVisible)
                                {
                                    this.settingsWindow = new SettingsWindow(this);
                                    this.settingsWindow.Show();
                                }
                            });
                        }

                        await Task.Delay(AmadeusAISettings.Default.PotatoPC ? 100 : 32);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            });
        }

      

        private void UpdateAmadeusScreen()
        {
            this.AmadeusScreen = Screen.PrimaryScreen;
            if (!string.IsNullOrEmpty(AmadeusAISettings.Default.Screen))
            {
                foreach (var screen in Screen.AllScreens)
                {
                    if (screen.DeviceName == AmadeusAISettings.Default.Screen)
                    {
                        this.AmadeusScreen = screen;
                        break;
                    }
                }
            }
        }

        public void SetupScale()
        {
            if (this.AmadeusScreen == null)
            {
                return;
            }

            var wpfDpi = PresentationSource.FromVisual(this)?.CompositionTarget?.TransformToDevice.M11;
            this.dpiScale = 1f / (float) wpfDpi.GetValueOrDefault(1);

            if (!this.initializedScales)
            {
                this.initializedScales = true;
                this.scaleBaseWidth = this.Width;
                this.scaleBaseHeight = this.Height;
                this.scaleBaseFacePictureWidth = this.facePicture.Width;
                this.scaleBaseFacePictureHeight = this.facePicture.Height;
                this.scaleBaseTextPictureWidth = this.textPicture.Width;
                this.scaleBaseTextPictureHeight = this.textPicture.Height;
                this.scaleBaseTextBoxWidth = this.textBox.Width;
                this.scaleBaseTextBoxHeight = this.textBox.Height;
                this.scaleBaseTextBoxFontSize = this.textBox.FontSize;
                this.basePictureThickness = this.facePicture.Margin;
                this.baseTextThickness = this.textPicture.Margin;
            }
            try
            {
                var scaleRatio = this.AmadeusScreen.Bounds.Height / 1080.0 * AmadeusAISettings.Default.ScaleModifier;
                scaleRatio *= this.dpiScale;
                this.Width = this.scaleBaseWidth * scaleRatio;
                this.Height = this.scaleBaseHeight * scaleRatio;
                this.facePicture.Width = this.scaleBaseFacePictureWidth * scaleRatio;
                this.facePicture.Height = this.scaleBaseFacePictureHeight * scaleRatio;
                this.facePicture.Margin = new Thickness(this.basePictureThickness.Left * scaleRatio,
                    this.basePictureThickness.Top * scaleRatio, this.facePicture.Margin.Right,
                    this.facePicture.Margin.Bottom);
                this.textPicture.Width = this.scaleBaseTextPictureWidth * scaleRatio;
                this.textPicture.Height = this.scaleBaseTextPictureHeight * scaleRatio;
                this.textPicture.Margin = new Thickness(this.baseTextThickness.Left * scaleRatio,
                    this.baseTextThickness.Top * scaleRatio, this.textPicture.Margin.Right, this.textPicture.Margin.Bottom);
                this.textBox.Height = this.scaleBaseTextBoxHeight * scaleRatio * 1.05;
                this.textBox.Width = this.scaleBaseTextBoxWidth * scaleRatio;
                this.textBox.FontSize = this.scaleBaseTextBoxFontSize * scaleRatio * 0.95;
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Windows.Forms.MessageBox.Show("oh, hi dev here you go ill be cooperative i guess.. : " + ex);
                Console.WriteLine("ive also posted here... " + ex.Message);
#else
            System.Windows.Forms.MessageBox.Show("Why. . ?");
            Thread.Sleep(2000);
            System.Environment.Exit(1);
#endif
            }
        }
        public void AdjustTextLocation()
        {
            if (this.AmadeusScreen == null)
            {
                return;
            }

       //     var wpfDpi = PresentationSource.FromVisual(this)?.CompositionTarget?.TransformToDevice.M11;
         //   this.dpiScale = 1f / (float)wpfDpi.GetValueOrDefault(1);

            // Assuming TextLocationModifier is a property in AmadeusAISettings
            var textLocationModifierY = AmadeusAISettings.Default.ScaleTextModifier;

            // Adjust text location based on the modifier
            this.textBox.Margin = new Thickness(this.textBox.Margin.Left,
                                          this.baseTextThickness.Top * textLocationModifierY * this.dpiScale,
                                          this.textBox.Margin.Right,
                                          this.textBox.Margin.Bottom);
             
        }
        private void RegisterBehaviours(object sender, EventArgs eventArgs)
        {
            this.behaviours = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass && typeof(IBehaviour).IsAssignableFrom(x))
                .Select(x => (IBehaviour) Activator.CreateInstance(x)).ToList();

            foreach (var behaviour in this.behaviours)
            {
                behaviour.Init(this);
            }
        }

        // Sets the correct position of Amadeus depending on taskbar position and visibility
        public void SetPosition(Screen screen)
        {
            // Override position if necessary
            if (AmadeusAISettings.Default.ManualPosition)
            {
                this.Top = AmadeusAISettings.Default.ManualPositionY * this.dpiScale;
                this.Left = AmadeusAISettings.Default.ManualPositionX * this.dpiScale;
                return;
            }

            // Only update screen ever so often, but necessary to avoid taskbar glitches
            if (DateTime.Now.Second % (AmadeusAISettings.Default.PotatoPC ? 10 : 3) == 0 &&
                (this.settingsWindow == null || !this.settingsWindow.IsVisible))
            {
                this.UpdateAmadeusScreen();
            }

            var position = new System.Windows.Point(
                screen.Bounds.X + screen.Bounds.Width - this.Width * (1 / this.dpiScale),
                screen.Bounds.Y + screen.Bounds.Height - this.Height * (1 / this.dpiScale));

            if (AmadeusAISettings.Default.LeftAlign)
            {
                position.X = screen.Bounds.X;
            }

            if (!MainWindow.IsForegroundFullScreen(screen))
            {
                var taskbars = this.FindDockedTaskBars(screen, out var isLeft);
                var taskbar = taskbars.FirstOrDefault(x => x.X != 0 || x.Y != 0 || x.Width != 0 || x.Height != 0);
                if (taskbar != default(Rectangle))
                {
                    if (taskbar.Width >= taskbar.Height)
                    {
                        if (taskbar.Y != screen.Bounds.Y)
                        {
                            // Bottom
                            position.Y -= taskbar.Height;
                        }
                    }
                    else
                    {
                        // Left/Right
                        if (isLeft)
                        {
                            if (AmadeusAISettings.Default.LeftAlign)
                            {
                                position.X += taskbar.Width;
                            }
                        }
                        else
                        {
                            if (!AmadeusAISettings.Default.LeftAlign)
                            {
                                position.X -= taskbar.Width;
                            }
                        }
                    }
                }
            }

            this.Top = position.Y * this.dpiScale;
            this.Left = position.X * this.dpiScale;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(HandleRef hWnd, [In] [Out] ref Rect rect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = false)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // From: https://stackoverflow.com/a/3744720/4016841 (modified)
        public static bool IsForegroundFullScreen(Screen screen)
        {
            if (screen == null)
            {
                screen = Screen.PrimaryScreen;
            }

            var windowBounds = new Rect();
            var foregroundWindowHandle = MainWindow.GetForegroundWindow();

            uint foregroundPID;
            MainWindow.GetWindowThreadProcessId(foregroundWindowHandle, out foregroundPID);
            var foregroundProcess = Process.GetProcessesByName("explorer").FirstOrDefault(x => x.Id == foregroundPID);
            var foregroundExeName = foregroundProcess?.ProcessName ?? string.Empty;

            if (foregroundWindowHandle.Equals(MainWindow.desktopWindow) ||
                foregroundWindowHandle.Equals(MainWindow.shellWindow) || foregroundWindowHandle.Equals(IntPtr.Zero)
                || foregroundExeName.ToLower().Equals("explorer"))
            {
                return false;
            }

            MainWindow.GetWindowRect(new HandleRef(null, foregroundWindowHandle), ref windowBounds);
            return
                new Rectangle(windowBounds.left, windowBounds.top, windowBounds.right - windowBounds.left,
                    windowBounds.bottom - windowBounds.top).Contains(
                    screen.Bounds);
        }

        public void SetAmadeusFace(string face)
        {
            // Filter invalid faces
       //     if (!"abcdefghijklmnopqrs".Contains(face)) //can add here

        //    {
        //        return;
        //    }

            this.CurrentFace = face;
            this.Dispatcher.Invoke(() =>
            {
                if (MainWindow.IsNight)
                {
                    face += "-n";
                    this.backgroundPicture.Source = this.backgroundNight;
                }
                else
                {
                    this.backgroundPicture.Source = this.backgroundDay;
                }

                face += ".png";

                var faceImg = new BitmapImage();
                faceImg.BeginInit();
                faceImg.UriSource = new Uri("pack://application:,,,/AmadeusAI;component/Amadeus/" + face);
                faceImg.EndInit();

                this.facePicture.Source = faceImg;
            });
        }

        public async void Say(IEnumerable<Expression> text) //made all async to bring about a delay
        {
            this.saying.Enqueue(text);
        }

        private async Task SpeakingThread()
        {
            while (this.applicationRunning)
            {
                if (this.saying.Count == 0)
                {
                    await Task.Delay(250); //adjust delay of msgs
                }
                else
                {
                    // Begin speech
                    var done = false;
                    this.Speaking = true;
                    this.Dispatcher.Invoke(() =>
                    {
                        var fadeIn = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.8)));
                        fadeIn.Completed += (sender, args) => done = true;
                        var clock = fadeIn.CreateClock();
                        this.textPicture.ApplyAnimationClock(UIElement.OpacityProperty, clock);
                        this.textBox.ApplyAnimationClock(UIElement.OpacityProperty, clock);
                    });

                    // Await fade in
                    while (!done)
                    {
                        await Task.Delay(1500);
                    }

                    // Speak
                    var text = this.saying.Dequeue();
                    foreach (var line in text)
                    {
                        var completedText = this.PlaceholderHandling(line.Text);
                        this.SetAmadeusFace(line.Face);
                        for (var i = 0; i < completedText.Length; i++)
                        {
                            var i1 = i;
                            this.textBox.Dispatcher.Invoke(() =>
                            {
                                this.textBox.Text = completedText.Substring(0, i1 + 1);
                            });
                            await Task.Delay(25);
                        }

                        await Task.Delay(Math.Max(2000, 52 * completedText.Length * (AmadeusAISettings.Default.Language == "Japanese" ? 2 : 1))); //yes Japanese will be both formatted differently and frthe rimproved in the future

                        line.OnExecuted();
                    }

                    // End speech
                    done = false;
                    this.Dispatcher.Invoke(() =>
                    {
                        var fadeOut = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(0.5)));
                        fadeOut.Completed += (sender, args) =>
                        {
                            this.textBox.Dispatcher.Invoke(() => this.textBox.Text = "");
                            done = true;
                        };
                        var clock = fadeOut.CreateClock();
                        this.textPicture.ApplyAnimationClock(UIElement.OpacityProperty, clock);
                        this.textBox.ApplyAnimationClock(UIElement.OpacityProperty, clock);
                    });

                    // Await fade out
                    while (!done)
                    {
                        await Task.Delay(5);
                    }

                    this.SetAmadeusFace("a");

                    this.Speaking = false;

                    await Task.Delay(200);
                }
            }
        }

        private string PlaceholderHandling(string str)
        {
            foreach (var key in this.placeholders.Keys)
            {
                if (str.Contains(key))
                {
                    str = str.Replace(key, this.placeholders[key]());
                }
            }

            return str;
        }
        public void Tetris_listen(object sender, EventArgs e) //need refs----------------------------------------------------------------------------------------------------------------
        {
            if (Tetris_Closed) //should be obvious what i'm trying to achieve il format it later
            {
                this.Say(new[]
                            {
                            new Expression("Left?", "n"),
                            new Expression("Is Tetris Boring for you?", "m")
                        });
            }
            

        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Point lpPoint);

        // From: https://stackoverflow.com/a/9826269/4016841
        public Rectangle[] FindDockedTaskBars(Screen screen, out bool isLeft)
        {
            var dockedRects = new Rectangle[4];

            var tmpScrn = screen;
            isLeft = false;

            var dockedRectCounter = 0;
            if (!tmpScrn.Bounds.Equals(tmpScrn.WorkingArea))
            {
                var leftDockedWidth = Math.Abs(Math.Abs(tmpScrn.Bounds.Left) - Math.Abs(tmpScrn.WorkingArea.Left));
                var topDockedHeight = Math.Abs(Math.Abs(tmpScrn.Bounds.Top) - Math.Abs(tmpScrn.WorkingArea.Top));
                var rightDockedWidth = tmpScrn.Bounds.Width - leftDockedWidth - tmpScrn.WorkingArea.Width;
                var bottomDockedHeight = tmpScrn.Bounds.Height - topDockedHeight - tmpScrn.WorkingArea.Height;

                if (leftDockedWidth > 0)
                {
                    dockedRects[dockedRectCounter].X = tmpScrn.Bounds.Left;
                    dockedRects[dockedRectCounter].Y = tmpScrn.Bounds.Top;
                    dockedRects[dockedRectCounter].Width = leftDockedWidth;
                    dockedRects[dockedRectCounter].Height = tmpScrn.Bounds.Height;
                    isLeft = true;
                    dockedRectCounter += 1;
                }

                if (rightDockedWidth > 0)
                {
                    dockedRects[dockedRectCounter].X = tmpScrn.WorkingArea.Right;
                    dockedRects[dockedRectCounter].Y = tmpScrn.Bounds.Top;
                    dockedRects[dockedRectCounter].Width = rightDockedWidth;
                    dockedRects[dockedRectCounter].Height = tmpScrn.Bounds.Height;
                    dockedRectCounter += 1;
                }

                if (topDockedHeight > 0)
                {
                    dockedRects[dockedRectCounter].X = tmpScrn.WorkingArea.Left;
                    dockedRects[dockedRectCounter].Y = tmpScrn.Bounds.Top;
                    dockedRects[dockedRectCounter].Width = tmpScrn.WorkingArea.Width;
                    dockedRects[dockedRectCounter].Height = topDockedHeight;
                    dockedRectCounter += 1;
                }

                if (bottomDockedHeight > 0)
                {
                    dockedRects[dockedRectCounter].X = tmpScrn.WorkingArea.Left;
                    dockedRects[dockedRectCounter].Y = tmpScrn.WorkingArea.Bottom;
                    dockedRects[dockedRectCounter].Width = tmpScrn.WorkingArea.Width;
                    dockedRects[dockedRectCounter].Height = bottomDockedHeight;
                    dockedRectCounter += 1;
                }
            }

            return dockedRects;
        }

        private bool AreKeysPressed(string combo)
        {
            try
            {
                // Prevent keypresses from propagating through the Settings Window to allow for Hotkey Settings
                if (this.settingsWindow != null && this.settingsWindow.IsVisible)
                {
                    return false;
                }

                if (combo.Contains("CTRL") && !Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    return false;
                }

                if (combo.Contains("ALT") && !Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt))
                {
                    return false;
                }

                if (combo.Contains("SHIFT") && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                {
                    return false;
                }
                if (combo.Contains("B") && !Keyboard.IsKeyDown(Key.B))
                {
                    return false;
                }
                if (combo.Contains("G") && !Keyboard.IsKeyDown(Key.G))
                {
                    return false;
                }
                var containsDash = combo.Contains("-");
                var key = containsDash ? combo.Substring(combo.LastIndexOf("-") + 1) : combo;
                Enum.TryParse(key, true, out Key keyVal);
                return Keyboard.IsKeyDown(keyVal);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("error: " + ex);
                return false;
            }
        }

        private static double Lerp(double firstFloat, double secondFloat, double by)
        {
            return firstFloat * by + secondFloat * (1 - by);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            this.applicationRunning = false;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public readonly int left;
            public readonly int top;
            public readonly int right;
            public readonly int bottom;
        }
    }
}