using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmadeusAI.Commands
{
    internal class ScreenManipulation
    {
        private static void Angry()
        {//the screen crashed one sec
            foreach (var s in Screen.AllScreens)
            {
                var w = new Crash
                {
                    Left = s.Bounds.Left,
                    Top = s.Bounds.Top,
                    Width = s.Bounds.Width,
                    Height = s.Bounds.Height

                };
                w.Show();
            }
        }
    }
}
