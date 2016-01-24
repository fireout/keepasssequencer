using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sequencer.Forms
{
    /* The ENTIRE purpose of this class is to put a string on top of the
     * standard progress bar. It turns out this is actually not very
     * straightforward. We must override the message handler and act only on
     * WM_PAINT messages, because OnPaint is not reliably called, and if you
     * force it to be called, then you need to actually do the drawing commands
     * for the progress bar itself. Nope!
     */
    public partial class StrengthBar : ProgressBar
    {
        public StrengthBar()
        {
          InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0F) // This is WM_PAINT. For some reason C# doesn't seem to provide NAMES for these stupid things.
            {
                string readout_str = " (" + Value.ToString() + " bits)";

                int i = 0;
                while (StrengthValues[i+1].threshold < Value)
                {
                  i+=1;
                }
                readout_str = StrengthValues[i].name + readout_str;

                /* based on
                 * http://www.codeproject.com/Articles/31406/Add-the-Percent-or-Any-Text-into-a-Standard-Progre
                 */
                using (Graphics g = CreateGraphics())
                {
                    g.DrawString(
                            readout_str,
                            SystemFonts.DefaultFont, Brushes.Black,
                            new PointF(
                                Width  / 2 - (g.MeasureString(readout_str, SystemFonts.DefaultFont).Width / 2.0F),
                                Height / 2 - (g.MeasureString(readout_str, SystemFonts.DefaultFont).Height / 2.0F)));
                }
            }
        }

        private struct StrengthValType { public int threshold; public string name; };

        /* strength thresholds by entropy from http://keepass.info/help/kb/pw_quality_est.html */
        private readonly StrengthValType[] StrengthValues = {
          new StrengthValType {threshold = 0,   name = "Very weak"},
          new StrengthValType {threshold = 64,  name = "Weak"},
          new StrengthValType {threshold = 80,  name = "Moderate"},
          new StrengthValType {threshold = 112, name = "Strong"},
          new StrengthValType {threshold = 128, name = "Very strong"}
        };
    }
}
