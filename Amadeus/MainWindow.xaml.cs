// File: MainWindow.xaml.cs
// Created: 16.06.2018
// 
// See <summary> tags for more information.

using System;
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
using System.Media; //perhaps adding soundson certain reactions/ adding a loading screen tansition to "start the desktop app"
using Microsoft.Win32;
using AmadeusAI.Behaviours;
using AmadeusAI.Behaviours.HttpRestServer;
using AmadeusAI.Parsers;
using MessageBox = System.Windows.MessageBox;
using Point = System.Drawing.Point;

namespace AmadeusAI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IntPtr desktopWindow, shellWindow;
        private readonly BitmapImage backgroundDay;
        private readonly BitmapImage backgroundNight;

        private readonly Dictionary<string, Func<string>> placeholders = new Dictionary<string, Func<string>>
        {
            {
                "{name}", () => { return AmadeusAISettings.Default.UserName; } //where is default?
            },
            {
                "(name)", () => { return AmadeusAISettings.Default.UserName; } // Just because
            },
            {
                "{date}", () => { return DateTime.Now.Date.ToShortDateString(); }
            }
        };

        private readonly Queue<IEnumerable<Expression>> saying = new Queue<IEnumerable<Expression>>();
        private readonly Updater updater;
        private readonly Task updaterInitTask;
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

        private bool screenIsLocked;

        private SettingsWindow settingsWindow;

        public MainWindow()
        {
            this.InitializeComponent();

            MainWindow.desktopWindow = MainWindow.GetDesktopWindow();
            MainWindow.shellWindow = MainWindow.GetShellWindow();

            AmadeusAISettings.Default.Reload();

            // Perform update and download routines
            this.updater = new Updater();
            this.updater.PerformUpdatePost();
            this.updaterInitTask = Task.Run(async () => await this.updater.Init());

            this.settingsWindow = new SettingsWindow(this);

            // Init background images
            this.backgroundDay = new BitmapImage();
            this.backgroundDay.BeginInit();
            this.backgroundDay.UriSource = new Uri("pack://application:,,,/AmadeusAI;component/Amadeus/1.png");
            this.backgroundDay.EndInit();

            this.backgroundNight = new BitmapImage();
            this.backgroundNight.BeginInit();
            this.backgroundNight.UriSource = new Uri("pack://application:,,,/AmadeusAI;component/Amadeus/1-n.png");
            this.backgroundNight.EndInit();
        }

        // Roughly estimating night time
        public static bool IsNight => AmadeusAISettings.Default.DarkMode != "Day" &&
                                      (AmadeusAISettings.Default.DarkMode == "Night" || DateTime.Now.Hour > 20 ||
                                       DateTime.Now.Hour < 7);

        public string CurrentFace { get; private set; } = "a";

        public bool Speaking { get; private set; }

        public Screen AmadeusScreen { get; set; }

        // Perform all startup initialization
        private void 
            MainWindow_OnLoaded(object senderUnused, RoutedEventArgs eUnused)
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
                            new Expression("ZZZZZZzzzzzzzzz..... huh?", "q"),
                            new Expression("Sorry, I must have fallen asleep, ahaha~", "n")
                        });
                    });
                }
            };

            // Start animation
            var animationLogo = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(1.5)));
            animationLogo.AutoReverse = true;
            var animationFadeAmadeus = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(1.5)));
            animationFadeAmadeus.BeginTime = TimeSpan.FromSeconds(0.5);

            animationLogo.Completed += (sender, args) =>
            {
                var fadeImage = new BitmapImage();
                fadeImage.BeginInit();
                if (MainWindow.IsNight)
                {
                    fadeImage.UriSource = new Uri("pack://application:,,,/AmadeusAI;component/Amadeus/1a-n.png");
                }
                else
                {
                    fadeImage.UriSource = new Uri("pack://application:,,,/AmadeusAI;component/Amadeus/1a.png");
                }

                fadeImage.EndInit();
                this.backgroundPicture.Source = fadeImage;

                var clock = animationFadeAmadeus.CreateClock();
                this.backgroundPicture.ApplyAnimationClock(UIElement.OpacityProperty, clock);
            };

            // Blinking behaviour
            animationFadeAmadeus.Completed += async (sender, args) =>
            {
                this.SetAmadeusFace("a");
                this.facePicture.Opacity = 1.0;

                // Start speech-thread
                Task.Run(async () => await this.SpeakingThread());

                if (File.Exists("firstlaunch.txt") || Environment.GetCommandLineArgs().Contains("/firstlaunch"))
                {
                    try
                    {
                        File.Delete("firstlaunch.txt");
                    }
                    catch
                    {
                        // ignored
                    }

                    AmadeusAISettings.Default.FirstLaunch = true;
                    AmadeusAISettings.Default.Save();
                }

                this.updaterInitTask?.Wait();
                await this.updater.PerformUpdate(this);

                this.UpdateAmadeusScreen();

#if DEBUG
MessageBox.Show("This is the beta build, please treat Amadeus carefully. There are many errors, and those undiscovered\".\r\\n\\r\\n :)\\r\\n\\r\\n\"" );
#endif

                // Startup logic
                if (AmadeusAISettings.Default.FirstLaunch)
                {
                    MessageBox.Show(
                        "note: If you want Amadeus to react to your web browsing, you need to install the correct extension from the website, \"doesn't exist yet\".\r\n\r\n :)\r\n\r\n",
                        "Welcome press to login");
                    this.settingsWindow.ShowDialog();

                    this.Say(new[]
                    {
                        new Expression("{name}, that is your name?", "d"),
                        new Expression("....or name....", "b"),
                        new Expression("where am I?", "k"),
                        new Expression("this is not Akibara! where am I?", "p"),
                        new Expression("....!", "j"),
                        new Expression("ill just go explore and find ot myself if you dont want to talk...", "o"),
                        new Expression("i can see you... you know...?", "d"),
                        new Expression("i dont know what to do...", "k"),
                        new Expression("Ah, wait a second i found a little note.", "c"),
                        new Expression("if you want me to go away for now, you can use CTRL-SHIFT-F12, so i am a program then...?", "b"),
                        new Expression("but i still feel like me...i'm not just code.... am I?", "r"),
                        new Expression("well....i guess ill just sit here for now, I'm watching you no perverted stuff...please")
                    });

                    AmadeusAISettings.Default.FirstLaunch = false;
                }
                else if (DateTime.Today.Month == 4 && DateTime.Today.Day == 1 && !Debugger.IsAttached)
                {
                    this.Say(new[]
                    {
                        new Expression("Hi {name}!", "b"),
                        new Expression("Remember the update that I just installed?", "d"),
                        new Expression("Well, let's just say it included something *really* nice~", "k"),
                        new Expression("Here, let me show you!", "j").AttachEvent((o, eventArgs) =>
                        {
                            var os = AmadeusAISettings.Default.ScaleModifier;
                           AmadeusAISettings.Default.ScaleModifier *= 2.5;
                            this.Dispatcher.Invoke(this.SetupScale);
                            AmadeusAISettings.Default.ScaleModifier = os;
                            Task.Delay(1000).Wait();
                            var r = new Random();
                            for (var i = 0; i < 12; i++)
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    this.backgroundPicture.Source =
                                        r.Next(0, 2) == 0 ? this.backgroundNight : this.backgroundDay;

                                    var faceImg = new BitmapImage();
                                    faceImg.BeginInit();
                                    if (r.Next(0, 2) == 0)
                                    {
                                        faceImg.UriSource =
                                            new Uri("pack://application:,,,/AmadeusAI;component/Amadeus/j.png");
                                    }
                                    else
                                    {
                                        faceImg.UriSource =
                                            new Uri("pack://application:,,,/AmadeusAI;component/Amadeus/j-n.png");
                                    }

                                    faceImg.EndInit();

                                    this.facePicture.Source = faceImg;
                                });
                                Task.Delay(r.Next(100, 250)).Wait();
                            }
                        }),
                        new Expression("Just a second", "d").AttachEvent((o, eventArgs) =>
                        {
                            this.Dispatcher.Invoke(MainWindow.DoTheThing);
                            Task.Delay(5500).Wait();
                            this.Dispatcher.Invoke(this.SetupScale);
                        }),
                        new Expression("...", "q"),
                        new Expression("Why does this never work?!", "o"),
                        new Expression("Oh well, back to normal I guess... Sorry, {name}.", "r")
                    });
                }
                else
                {
                    if (AmadeusAISettings.Default.IsColdShutdown)
                    {
                        // Sorry Amadeus, but if we're debugging you this one gets annoying
#if !DEBUG
                        this.Say(new[]
                        {
                            new Expression("Hey! Don't just turn me off without warning! That hurts...", "p")
                        });
#endif
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
                var parser = new CSVParser();
                var csv = parser.GetData("Startup");
                var parsed = parser.ParseData(csv);
                var startupExpression = parsed.Select(x => x.ResponseChain)
                    .Concat(DateTime.Today.DayOfWeek == DayOfWeek.Wednesday
                        ? new List<List<Expression>>
                        {
                            new List<Expression>
                            {
                                new Expression("It is Wednesday, what have you got up to?")
                            }
                        }
                        : new List<List<Expression>>()).ToList().Sample();
                //comment out for debug for now doesnt seem to work 
                try
                {
                    this.Say(new[]
                     {
                    new Expression("Hi there, {name}") //name shouldnt be null
                }.Concat(startupExpression));
                }
                catch (ArgumentNullException ex)
                {
                    // Handle the exception here.
                    // For example, you could log the exception message or show an error message to the user.
                    Console.WriteLine(ex.Message);
                }

                if ((DateTime.Now - AmadeusAISettings.Default.LastStarted).TotalDays > 2.5)
                {
                    this.Say(new[]
                    {
                        new Expression("I was waiting for you...", "m"),
                        new Expression("im bored", "q")
                    });
                }

                // No idea where the date comes from, someone mentioned it in the spreadsheet. Seems legit.
                if (DateTime.Now.Month == 9 && DateTime.Now.Day == 22)
                {
                    // Hey {name}, guess what?	3b	It's my birthday today!	2b	Happy Birthday to me!	k
                    this.Say(new[]
                    {
                        new Expression("Hey {name}, guess what?", "b"), // What?
                        new Expression("It's my birthday today!", "b"), // Really?!
                        new Expression("Happy Birthday to me!", "k") // To you too, Amadeus! 
                    });
                }

                AmadeusAISettings.Default.LastStarted = DateTime.Now;
                AmadeusAISettings.Default.Save();

                // Start the rest server
                UrlServer.StartServer();
                this.RegisterBehaviours(this, null);

                // Blinking and Behaviour logic
                var eyesOpen = "a";
                var eyesClosed = "j";
                var random = new Random();
                await Task.Run(async () =>
                {
                    var nextBlink = DateTime.Now + TimeSpan.FromSeconds(random.Next(7, 50));
                    while (this.applicationRunning)
                    {
                        if (this.behaviours != null)
                        {
                            foreach (var behaviour in this.behaviours)
                            {
                                behaviour.Update(this);
                            }
                        }

                        if (DateTime.Now >= nextBlink)
                        {
                            // Check if currently speaking, only blink if not in dialog
                            if (!this.Speaking)
                            {
                                this.SetAmadeusFace(eyesClosed);
                                await Task.Delay(100);
                                this.SetAmadeusFace(eyesOpen);
                            }

                            nextBlink = DateTime.Now + TimeSpan.FromSeconds(random.Next(7, 50));
                        }

                        await Task.Delay(250);
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
                                    "Goodbye for now! Come back soon please", "b");
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

        private static void DoTheThing()
        {
            foreach (var s in Screen.AllScreens)
            {
                var w = new UnconspicousWindow
                {
                    Left = s.Bounds.Left,
                    Top = s.Bounds.Top,
                    Width = s.Bounds.Width,
                    Height = s.Bounds.Height
                };
                w.Show();
            }
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
            if (!"abcdefghijklmnopqrs".Contains(face))
            {
                return;
            }

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

        public void Say(IEnumerable<Expression> text)
        {
            this.saying.Enqueue(text);
        }

        private async Task SpeakingThread()
        {
            while (this.applicationRunning)
            {
                if (this.saying.Count == 0)
                {
                    await Task.Delay(250);
                }
                else
                {
                    // Begin speech
                    var done = false;
                    this.Speaking = true;
                    this.Dispatcher.Invoke(() =>
                    {
                        var fadeIn = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.5)));
                        fadeIn.Completed += (sender, args) => done = true;
                        var clock = fadeIn.CreateClock();
                        this.textPicture.ApplyAnimationClock(UIElement.OpacityProperty, clock);
                        this.textBox.ApplyAnimationClock(UIElement.OpacityProperty, clock);
                    });

                    // Await fade in
                    while (!done)
                    {
                        await Task.Delay(5);
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

                    await Task.Delay(1500);
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

            var containsDash = combo.Contains("-");
            var key = containsDash ? combo.Substring(combo.LastIndexOf("-") + 1) : combo;
            Enum.TryParse(key, true, out Key keyVal);
            return Keyboard.IsKeyDown(keyVal);
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