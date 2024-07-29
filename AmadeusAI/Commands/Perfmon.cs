using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace AmadeusAI.Commands
{
  public partial class Perfmon
    {
        private Dictionary<string, Func<bool>> executableActions;

        public Perfmon()
        {
            // Initialize the dictionary with executable names and corresponding methods
            executableActions = new Dictionary<string, Func<bool>>
        {
            { "Taskmgr", OpenTaskManager },
            { "CrystalDisk", OpenCrystalDisk },
            { "Resource Monitor", OpenResmon },
            { "NewExecutable", OpenNewExecutable }
            // Add more executables and methods as needed
        };
        }
        public void LaunchExecutablesOnStartup()
        {
            // Get the executables setting
            string executablesSetting = AmadeusAISettings.Default.Executables;
            if (executablesSetting == null)
            {
                System.Windows.MessageBox.Show("The executable Setting is null");
                return;
            }
            else
            {
                // Split the setting by commas to get an array of executable names
                string[] executables = executablesSetting.Split(',');

                // Loop through the array of executable names
                foreach (string executable in executables)
                {
                    // Trim any leading or trailing whitespace
                    string trimmedExecutable = executable.Trim();

                    // Check if there's a method for this executable
                    if (executableActions.ContainsKey(trimmedExecutable))
                    {
                        // Call the method for this executable
                        //executableActionstrimmedExecutable;
                    }
                }
            }
        }
        public void ExecuteExecutable(string executableName)
        {
            // Check if the executable is in the dictionary
           // if (executableActions.ContainsKey(executableName))
           try //avoiding checks overhead
            {
                // Execute the corresponding method
                executableActions[executableName].Invoke();
            }
           catch(KeyNotFoundException) //else
            {
                // MessageBox.Show($"Executable '{executableName}' not supported.");
                return;
            }
        }
        public bool OpenNewExecutable()
        {
            // Implementation for opening NewExecutable
            return true;
        }
        public bool OpenTaskManager()
        {
            // Start Task Manager process
          
            if (!IsProcessRunning("Taskmgr"))
            { //:(
                Process.Start("taskmgr.exe");
               // System.Threading.Thread.Sleep(1000);
                // have a look at this code and check if it works 
                // Get the Task Manager window
                var taskManagerWindow = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Task Manager"));

                // Get the tab control
             var tabControl = taskManagerWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Tab));

                // Get the Performance tab
             var performanceTab = tabControl.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Performance"));

                // Select the Performance tab
               var selectionItemPattern = performanceTab.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
              selectionItemPattern.Select();
                return true;

            }
            else
            {

               return false;
                // Wait for Task Manager to start (adjust the sleep time as needed)


                // Send keys to navigate to the Performance tab
              //  SendKeys.SendWait("%{TAB}"); // Alt + Tab to switch to the Performance tab
                                             // Wait for the switch to complete
               // SendKeys.SendWait("%V"); // Alt + V to open the View menu

             //  SendKeys.SendWait("P"); // Press P for Performance
                //:)
            }

        }
       public bool OpenCrystalDisk()
        {
            // Implementation for opening CrystalDisk
            // Add your logic here
            return true;
        }
        public bool OpenResmon()
        {
            return true;

        }

            private bool IsProcessRunning(string processName)
        {
            // Check if a process with the specified name is running
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }
    }
}

