﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using DND.Common;

namespace DND.Controls
{
    public class ResultsControl : ZenControl, IMessageFilter
    {
        // TEMP standard scroll bar
        private VScrollBar sb;
        private Size contentRectSize;
        private List<OneResultControl> resCtrls = new List<OneResultControl>();
        private readonly System.Timers.Timer timer;

        public ResultsControl(float scale, ZenControl owner)
            : base(scale, owner)
        {
            Application.AddMessageFilter(this);

            sb = new VScrollBar();
            AddWinFormsControl(sb);
            sb.Height = Height - 2;
            sb.Top = 1;
            sb.Left = Width - 1 - sb.Width;
            sb.Enabled = false;
            sb.Scroll += sb_Scroll;
            sb.ValueChanged += sb_ValueChanged;

            contentRectSize = new Size(Width - 2 - sb.Width, Height - 2);

            timer = new System.Timers.Timer(40);
            timer.AutoReset = true;
            timer.Start();
            timer.Elapsed += onScrollTimerEvent;
        }

        void sb_ValueChanged(object sender, EventArgs e)
        {
            int y = 1 - sb.Value;
            SuspendPaint();
            foreach (OneResultControl orc in resCtrls)
            {
                orc.AbsTop = y;
                y += orc.Height;
            }
            ResumePaint(false, RenderMode.Invalidate);
        }

        void sb_Scroll(object sender, ScrollEventArgs e)
        {
            MakeMePaint(false, RenderMode.Invalidate);
        }

        public static ushort HIWORD(IntPtr l) { return (ushort)((l.ToInt64() >> 16) & 0xFFFF); }
        public static ushort LOWORD(IntPtr l) { return (ushort)((l.ToInt64()) & 0xFFFF); }

        public bool PreFilterMessage(ref Message m)
        {
            bool r = false;
            if (m.Msg == 0x020A) //WM_MOUSEWHEEL
            {
                Point p = new Point((int)m.LParam);
                int delta = (Int16)HIWORD(m.WParam);
                MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, p.X, p.Y, delta);
                m.Result = IntPtr.Zero; //don't pass it to the parent window
                onMouseWheel(e);
            }
            return r;
        }

        private void onMouseWheel(MouseEventArgs e)
        {
            subscribeOrAddScrollTimer(-((float)e.Delta) * ((float)sb.LargeChange) / 1500.0F);

            //float diff = ((float)sb.LargeChange) * ((float)e.Delta) / 240.0F;
            //int idiff = -(int)diff;
            //int newval = sb.Value + idiff;
            //if (newval < 0) newval = 0;
            //else if (newval > sb.Maximum - sb.LargeChange) newval = sb.Maximum - sb.LargeChange;
            //sb.Value = newval;
        }

        private float scrollSpeed;
        private object scrollTimerLO = new object();

        private void subscribeOrAddScrollTimer(float diffMomentum)
        {
            lock (scrollTimerLO)
            {
                scrollSpeed += diffMomentum;
            }
        }

        private void onScrollTimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            float speed = 0;
            lock (scrollTimerLO)
            {
                speed = scrollSpeed;
                scrollSpeed *= 0.9F;
                if (scrollSpeed > 3.0F) scrollSpeed -= 3.0F;
                else if (scrollSpeed < -3.0F) scrollSpeed += 3.0F;
                if (Math.Abs(scrollSpeed) < 1.0F) scrollSpeed = 0;
            }
            if (speed == 0) return;
            Invoke((MethodInvoker)delegate
            {
                int scrollVal = sb.Value;
                scrollVal += (int)speed;
                if (scrollVal < 0)
                {
                    scrollVal = 0;
                }
                else if (scrollVal > sb.Maximum - contentRectSize.Height)
                {
                    scrollVal = sb.Maximum - contentRectSize.Height;
                }
                sb.Value = scrollVal;
            });
        }

        protected override void DoDispose()
        {
            foreach (OneResultControl orc in resCtrls) orc.Dispose();
            timer.Dispose();
        }

        protected override void OnSizeChanged()
        {
            contentRectSize = new Size(Width - 2 - sb.Width, Height - 2);
            sb.Height = Height - 2;
            sb.Top = AbsTop + 1;
            sb.Left = AbsLeft + Width - 1 - sb.Width;
            sb.LargeChange = contentRectSize.Height;
            SuspendPaint();
            foreach (OneResultControl orc in resCtrls) orc.Width = contentRectSize.Width;
            ResumePaint(true, RenderMode.Invalidate);
        }

        public void SetResults(ReadOnlyCollection<CedictResult> results, int pageSize)
        {
            if (scale == 0) throw new InvalidOperationException("Scale must be set before setting results to show.");
            // Dispose old results controls
            foreach (OneResultControl orc in resCtrls) orc.Dispose();
            resCtrls.Clear();
            // Create new result controls
            SuspendPaint();
            int y = 0;
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (CedictResult cr in results)
                {
                    OneResultControl orc = new OneResultControl(scale, this, cr);
                    orc.Analyze(g, contentRectSize.Width);
                    orc.AbsLocation = new Point(1, y + 1);
                    y += orc.Height;
                    resCtrls.Add(orc);
                }
            }
            sb.Maximum = y;
            sb.LargeChange = contentRectSize.Height;
            sb.Value = 0;
            sb.Enabled = true;
            ResumePaint(false, RenderMode.Invalidate);
        }

        public override void DoPaint(Graphics g)
        {
            Region oldClip = g.Clip;
            Matrix oldTransform = g.Transform;
            Rectangle rect = AbsRect;
            g.Clip = new Region(new Rectangle(AbsLeft, AbsTop, Width, Height));
            g.TranslateTransform(rect.X, rect.Y);

            // Background
            using (Brush b = new SolidBrush(Color.White))
            {
                g.FillRectangle(b, new Rectangle(0, 0, Width, Height));
            }
            // Border
            using (Pen p = new Pen(SystemColors.ControlDarkDark))
            {
                g.DrawLine(p, 0, 0, Width, 0);
                g.DrawLine(p, Width - 1, 0, Width - 1, Height);
                g.DrawLine(p, Width - 1, Height - 1, 0, Height - 1);
                g.DrawLine(p, 0, Height - 1, 0, 0);
            }
            // Results
            g.Clip = new Region(new Rectangle(1, 1, contentRectSize.Width, contentRectSize.Height));
            foreach (OneResultControl orc in resCtrls)
            {
                if ((orc.AbsBottom < contentRectSize.Height && orc.AbsBottom >= 0) ||
                    (orc.AbsTop < contentRectSize.Height && orc.AbsTop >= 0))
                {
                    orc.DoPaint(g);
                }
            }

            g.Transform = oldTransform;
            g.Clip = oldClip;
        }

    }
}
