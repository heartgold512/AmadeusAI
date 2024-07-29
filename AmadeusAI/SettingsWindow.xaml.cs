using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using Action = System.Action;
using MessageBox = System.Windows.MessageBox;
using Point = System.Drawing.Point;
using Task = System.Threading.Tasks.Task;

namespace AmadeusAI
{
    /// <summary>
    ///     Interaction logic for SettingsWindow.xaml
    /// </summary>
   
    public partial class SettingsWindow : Window
    {
        private Cmd commands;
        private const string AUTOSTART_TASK_NAME = "AmadeusAI_startup";
        private readonly MainWindow mainWindow;
        private Brush originalBackground;
        private bool settingsLoaded;
        public bool ON = false;


        //update method to introduce a button click event for communication between the two files
        public class Button4ClickedEventArgs : EventArgs
        {
            public bool IsClicked { get; set; }

        }

        public event EventHandler<Button4ClickedEventArgs> Button4Clicked;

        public void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {

                AmadeusAISettings.Default.Button4Clicked = !AmadeusAISettings.Default.Button4Clicked;
                AmadeusAISettings.Default.Save();
                if (AmadeusAISettings.Default.Button4Clicked == true)
                {
                    Nobackground.Content = "Background off";
                }
                else
                {
                    Nobackground.Content = "Background on";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in background: " + ex);
#if DEBUG
                Message messageWindow = new Message();

                // Set the message text
                messageWindow.Messager.Text = "Error in background: " + ex;

                // Show the Message window
                messageWindow.ShowDialog();
#endif
            }

        }
        public async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            await this.HotkeySetTask(this.txtBckg, () => AmadeusAISettings.Default.HotKeyBckgHide = this.txtBckg.Text);

        }
       
        public SettingsWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.InitializeComponent();
            originalBackground = Background;
            DataContext = this;
            
            //this.Button4Clicked += HandleButton4Clicked; //
        }

        public bool IsPositioning { get; private set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (AmadeusAISettings.Default.FirstLaunch)
            {
                AmadeusAISettings.Default.AutoUpdate = true;
            }

            if (!string.IsNullOrWhiteSpace(AmadeusAISettings.Default.LastUpdateConfig))
            {
                var localConfig =
                    JsonConvert.DeserializeObject<UpdateConfig>(AmadeusAISettings.Default.LastUpdateConfig);
                this.textBlockVersion.Text = "Version p" + localConfig.ProgramVersion + "t" + localConfig.ResponsesVersion
#if DEBUG
                    + "d" + localConfig; //detailed solution will be given, error improvments and catch mechanisms will be improved in the future
#endif
                ;
            }

            // Settings window initialization code
            this.textBoxName.Text = string.IsNullOrWhiteSpace(AmadeusAISettings.Default.UserName)
                ? Environment.UserName
                : AmadeusAISettings.Default.UserName;
            this.checkBoxPotatoPC.IsChecked = AmadeusAISettings.Default.PotatoPC;
            this.checkBoxAutoUpdate.IsChecked = AmadeusAISettings.Default.AutoUpdate;
            this.sliderScale.Value = AmadeusAISettings.Default.ScaleModifier;
            this.sliderTextScale.Value = AmadeusAISettings.Default.ScaleTextModifier;
            this.txtSettings.Text = AmadeusAISettings.Default.HotkeySettings;
            this.txtExit.Text = AmadeusAISettings.Default.HotkeyExit;
            this.txtHide.Text = AmadeusAISettings.Default.HotkeyHide;
            this.txtBckg.Text = AmadeusAISettings.Default.HotKeyBckgHide;
            this.comboBoxLanguage.Text = AmadeusAISettings.Default.Language;

            if (AmadeusAISettings.Default.LeftAlign)
            {
                this.radioLeft.IsChecked = true;
            }
            else
            {
                this.radioRight.IsChecked = true;
            }

            if (AmadeusAISettings.Default.ManualPosition)
            {
                this.radioManual.IsChecked = true;
            }

            if (!string.IsNullOrEmpty(AmadeusAISettings.Default.AutoStartTask))
            {
                this.buttonAutostart.Content = "Disable starting with Windows";
            }

            this.comboBoxNightMode.SelectedItem = AmadeusAISettings.Default.DarkMode;

            var index = 0;
            this.comboBoxScreen.Items.Clear();
            foreach (var screen in Screen.AllScreens)
            {
                this.comboBoxScreen.Items.Add($"{screen.DeviceName} ({screen.Bounds.Width}x{screen.Bounds.Height})");

                if (string.IsNullOrWhiteSpace(AmadeusAISettings.Default.Screen) && screen.Primary ||
                    screen.DeviceName == AmadeusAISettings.Default.Screen)
                {
                    this.comboBoxScreen.SelectedIndex = index;
                }
                index++;
            }

            object selObj = null;
            foreach (string item in this.comboBoxIdle.Items)
            {
                if (item.ToLower() == AmadeusAISettings.Default.IdleWait.ToLower())
                {
                    selObj = item;
                    break;
                }
            }

            // For some reason it's broken. Did they manually edit the settings? Lets force regular.
            if (selObj == null)
            {
                selObj = this.comboBoxIdle.Items[2];
                AmadeusAISettings.Default.IdleWait = "Regular (120-300s)";
            }

            this.comboBoxIdle.SelectedItem = selObj;

            // Focus window
            this.Focus();
            this.Activate();

            this.settingsLoaded = true;
        }

        private void comboBoxScreen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.comboBoxScreen.SelectedItem == null)
            {
                return;
            }

            this.mainWindow.AmadeusScreen =
                Screen.AllScreens.First(x => this.comboBoxScreen.SelectedItem.ToString().Contains(x.DeviceName));
            this.mainWindow.SetupScale();
            this.mainWindow.SetPosition(this.mainWindow.AmadeusScreen);
            this.mainWindow.SetupScale();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.textBoxName.Text != null)
            {
                AmadeusAISettings.Default.Save();
                Telegramsettings.Default.Save(); //saves the telegram settings need to fix the binding issues and null
                this.Close();
            }

            else
            {
                Message messageWindow = new Message();

                // Set the message text
                messageWindow.Messager.Text = "I know you have a name, you could just use a username too... please just something";

                // Show the Message window
                messageWindow.ShowDialog();
            
                return;

            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var restart = false;
            if ((string)this.comboBoxLanguage.SelectedItem != AmadeusAISettings.Default.Language) //default japanese will be added and improved in future
            {
                MessageBox.Show("Language settings have been changed or No CSV is selected. AmadeusAI will now attempt to restart.", "Note");
                restart = true;
            }

            AmadeusAISettings.Default.AutoUpdate = this.checkBoxAutoUpdate.IsChecked.GetValueOrDefault(true);
            AmadeusAISettings.Default.PotatoPC = this.checkBoxPotatoPC.IsChecked.GetValueOrDefault(false);
            AmadeusAISettings.Default.DarkMode = (string)this.comboBoxNightMode.SelectedItem;
            AmadeusAISettings.Default.UserName = this.textBoxName.Text;
           
            AmadeusAISettings.Default.Language = (string)this.comboBoxLanguage.SelectedItem;
            if (this.comboBoxScreen.SelectedItem != null && Screen.AllScreens != null)
            {
                AmadeusAISettings.Default.Screen =
                    Screen.AllScreens.First(x => this.comboBoxScreen.SelectedItem.ToString().Contains(x.DeviceName))
                        .DeviceName;
            }

            AmadeusAISettings.Default.Save();

            if (restart)
            {
                Process.Start(Assembly.GetEntryAssembly().Location);

                AmadeusAISettings.Default.IsColdShutdown = false;
                AmadeusAISettings.Default.Save();
                Environment.Exit(1); //sett to 1
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(ushort virtualKeyCode);

        private void radio_checked_changed(object sender, RoutedEventArgs e)
        {
            if (!this.settingsLoaded)
            {
                return;
            }

            AmadeusAISettings.Default.LeftAlign = this.radioLeft.IsChecked.GetValueOrDefault(false);

            if (this.radioManual.IsChecked.GetValueOrDefault(false))
            {
                if (this.IsPositioning)
                {
                    return;
                }

                AmadeusAISettings.Default.ManualPosition = true;
                AmadeusAISettings.Default.ManualPositionX = 0;
                AmadeusAISettings.Default.ManualPositionY = 0;
                this.IsPositioning = true;
                this.Dispatcher.Invoke(() => this.IsEnabled = false);

                MessageBox.Show(
                    "AmadeusAI will now follow your mouse cursor so you can position her wherever you want. Click the LEFT MOUSE BUTTON once you're satisfied with her position.",
                    "Manual Position");

                Task.Run(async () =>
                {
                    var mouseDown = SettingsWindow.GetAsyncKeyState(0x01) != 0;

                    do
                    {
                        var pos = Point.Empty;
                        SettingsWindow.GetCursorPos(ref pos);

                        AmadeusAISettings.Default.ManualPositionX = pos.X;
                        AmadeusAISettings.Default.ManualPositionY = pos.Y;

                        var prevMouseDown = mouseDown;
                        mouseDown = SettingsWindow.GetAsyncKeyState(0x01) != 0; // 0x01 is code for LEFT_MOUSE

                        if (mouseDown && !prevMouseDown)
                        {
                            break;
                        }

                        await Task.Delay(1);
                    } while (true);

                    this.Dispatcher.Invoke(() => this.IsEnabled = true);
                    this.IsPositioning = false;
                });
            }
            else
            {
                AmadeusAISettings.Default.ManualPosition = false;
                this.mainWindow.SetupScale();
                this.mainWindow.SetPosition(this.mainWindow.AmadeusScreen);
                this.mainWindow.SetupScale();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show("Are you sure? This will reset all your settings.", "Confirm reset",
                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                AmadeusAISettings.Default.Reset();
                this.Window_Loaded(this, null);
            }
        }

        // Settings hotkey
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await this.HotkeySetTask(this.txtSettings,
                () => AmadeusAISettings.Default.HotkeySettings = this.txtSettings.Text);
        }

        // Hide hotkey
        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
        await this.HotkeySetTask(this.txtHide, () => AmadeusAISettings.Default.HotkeyHide = this.txtHide.Text);
        }

        // Exit hotkey
        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
        await this.HotkeySetTask(this.txtExit, () => AmadeusAISettings.Default.HotkeyExit = this.txtExit.Text);
        }
        //Hide Bckg
       
        private async Task HotkeySetTask(TextBlock output, Action callback)
        {
            output.Dispatcher.Invoke(() => output.Text = "Press and HOLD any key combination");

            await this.WaitForKeyChange();

            var timer = DateTime.Now;
            var state = SettingsWindow.GetKeyboardState().ToList();
            var invalid = true;
            while ((DateTime.Now - timer).TotalSeconds < 0.75)
            {
                var newState = SettingsWindow.GetKeyboardState().ToList();

                var ctrlPressed = newState.Where(x => x.Item1 == "LeftCtrl" || x.Item1 == "RightCtrl").Any(x => x.Item2);
                var altPressed = newState.Where(x => x.Item1 == "LeftAlt" || x.Item1 == "RightAlt").Any(x => x.Item2);
                var shiftPressed =
                    newState.Where(x => x.Item1 == "LeftShift" || x.Item1 == "RightShift").Any(x => x.Item2);
                var otherKeysPressed =
                    newState.Where(
                        x =>
                            x.Item2 &&
                            !new[] { "LeftCtrl", "RightCtrl", "LeftAlt", "RightAlt", "LeftShift", "RightShift" }.Contains(
                                x.Item1)).ToList();
                invalid = otherKeysPressed.Count != 1;

                if (invalid || !state.SequenceEqual(newState))
                {
                    timer = DateTime.Now;
                }

                output.Dispatcher.Invoke(() =>
                {
                    if (invalid)
                    {
                        output.Text = "Invalid combination";
                    }
                    else
                    {
                        output.Text = otherKeysPressed.Single().Item1;
                        if (shiftPressed)
                        {
                            output.Text = "SHIFT-" + output.Text;
                        }
                        if (altPressed)
                        {
                            output.Text = "ALT-" + output.Text;
                        }
                        if (ctrlPressed)
                        {
                            output.Text = "CTRL-" + output.Text;
                        }
                    }
                });

                state = newState;

                await Task.Delay(10);
            }

            output.Dispatcher.Invoke(() => output.Foreground = Brushes.GreenYellow);
            await Task.Delay(500);
            output.Dispatcher.Invoke(() => output.Foreground = Brushes.Black);

            output.Dispatcher.Invoke(callback);
        }

        private async Task WaitForKeyChange()
        {
            var state = SettingsWindow.GetKeyboardState().ToList();
            while (state.SequenceEqual(SettingsWindow.GetKeyboardState()))
            {
                await Task.Delay(10);
            }
        }

        private static IEnumerable<Tuple<string, bool>> GetKeyboardState()
        {
            return Enum.GetNames(typeof(Key)).Select(x =>
            {
                var key = (Key)Enum.Parse(typeof(Key), x);
                return new Tuple<string, bool>(x, key != Key.None && Keyboard.IsKeyDown(key));
            });
        }

        /*
        // I disabled this because the workaround wasn't doing anything, but let's leave it in, maybe it will become useful at some point in the future
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            // Set DPI awareness or something
            if (AmadeusAISettings.Default.DpiWorkaround)
            {
                MessageBox.Show("Workaround has been disabled! AmadeusAI will now exit, please restart it manually.", "Workaround");
                AmadeusAISettings.Default.DpiWorkaround = false;
                AmadeusAISettings.Default.Save();
                Environment.Exit(0);
            }
            else
            {
                if (MessageBox.Show(
                        "If you don't see Amadeus on one of your screens right now, AmadeusAI can activate a workaround that *might* fix your issue - your milage may vary however. Do you want to try the fix?",
                        "Workaround", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    AmadeusAISettings.Default.DpiWorkaround = true;
                    AmadeusAISettings.Default.Save();
                    MessageBox.Show("Workaround enabled. AmadeusAI will now exit, please restart it manually.", "Workaround");
                    Environment.Exit(0);
                }
            }
        }*/

        private void sliderScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!this.settingsLoaded)
            {
                return;
            }

            // Scale modifier
            AmadeusAISettings.Default.ScaleModifier = this.sliderScale.Value;
            this.mainWindow.SetupScale();
        }
        private void sliderText_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!this.settingsLoaded)
            {
                return;
            }

            double newValue = e.NewValue;
            AmadeusAISettings.Default.ScaleTextModifier = e.NewValue;
            AmadeusAISettings.Default.Save();



            // Scale modifier
            // AmadeusAISettings.Default.ScaleTextModifier = this.sliderTextScale.Value;
            this.mainWindow.AdjustTextLocation();

        }

        private void comboBoxIdle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.comboBoxIdle.SelectedItem == null)
            {
                return;
            }

            AmadeusAISettings.Default.IdleWait = (string)this.comboBoxIdle.SelectedItem;
        }

        private void Button_Autostart_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AmadeusAISettings.Default.AutoStartTask))
            {
                this.buttonAutostart.Content = "Disable starting with Windows";
                // Enable autostart
                // NOTE: We use the task scheduler to circumnavigate the UAC dialog which would get really annoying over time
                using (var ts = new TaskService())
                {
                    var definition = ts.NewTask();
                    definition.RegistrationInfo.Description = "Automatically starts AmadeusAI at startup.";
                    // Add delay to compensate for taskbar weirdness, probably not a good idea but hey
                    definition.Triggers.Add(new LogonTrigger { Delay = TimeSpan.FromSeconds(2) });
                    definition.Actions.Add(new ExecAction(Assembly.GetEntryAssembly().Location));
                    definition.Principal.RunLevel = TaskRunLevel.Highest;
                    ts.RootFolder.RegisterTaskDefinition(SettingsWindow.AUTOSTART_TASK_NAME, definition);
                }
                AmadeusAISettings.Default.AutoStartTask = SettingsWindow.AUTOSTART_TASK_NAME;
                MessageBox.Show(
                    "Autostart has been enabled! Note that it points to this very executable this AmadeusAI.exe (probably), so if you move or delete it the autostart will stop working until you re-enable it",
                    "Note:" + "info: ");
            }
            else
            {
                this.buttonAutostart.Content = "Start with Windows Note: Can be disabled in Taskmanager";
                using (var ts = new TaskService())
                {
                    ts.RootFolder.DeleteTask(SettingsWindow.AUTOSTART_TASK_NAME, false);
                }

                AmadeusAISettings.Default.AutoStartTask = string.Empty;
            }

            // We need to save here, otherwise the user might cancel the dialog without saving and we end up in an invalid state, out of sync with the task scheduler
            AmadeusAISettings.Default.Save();
        }

        private void comboBoxNightMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AmadeusAISettings.Default.DarkMode = (string)this.comboBoxNightMode.SelectedItem;
            this.mainWindow.SetAmadeusFace("a");
        }
        private void Settingscolour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var colorDialog = new System.Windows.Forms.ColorDialog();
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var selectedColor = colorDialog.Color;
                    string colorHash = "#" + selectedColor.R.ToString("X2") + selectedColor.G.ToString("X2") + selectedColor.B.ToString("X2");
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHash));
                    AmadeusAISettings.Default.SettingsbckgColour = colorHash;
                    AmadeusAISettings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: " + ex);
            }
        }

        private void Commands_Click(object sender, RoutedEventArgs e)
        {
           commands= new Cmd();
           commands.Show();

        }

        private void Ringtune_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".mp3";
            dlg.Filter = "MP3 Files (*.mp3)|*.mp3|WAV Files (*.wav)|*.wav|All Files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;

                // Save the ringtone path to settings
                AmadeusAISettings.Default.RingtonePath = filename;
                AmadeusAISettings.Default.Save();
            }
        }

     
    }
}