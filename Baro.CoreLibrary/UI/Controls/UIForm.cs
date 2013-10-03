using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Baro.CoreLibrary;
using Baro.CoreLibrary.Core;
using Baro.CoreLibrary.Extensions;
using Baro.CoreLibrary.G3;
using Baro.CoreLibrary.UI.Activities;
using Baro.CoreLibrary.Text;

namespace Baro.CoreLibrary.UI.Controls
{
    public partial class UIForm : Form
    {
        private int _refCounter = 0;
        private int _refMouseHolder = 0;
        private int _transitionMSec = 2;

        private G3Canvas _offScreen;
        private UICanvas _canvas = new UICanvas();
        private volatile Activity _mainActivity;
        private volatile bool _activityExecuted = false;
        private Encoding _encoding = null;

        public bool NewActivityLoaded { get { return _refCounter != _refMouseHolder; } }

        public int TransitionMSec { get { return _transitionMSec; } set { _transitionMSec = value; } }

        public Encoding Encoding
        {
            get
            {
                if (_encoding == null)
                {
                    _encoding = EncodingFactory.GetEncoding(12542);
                }

                return _encoding;
            }
        }

        public void BreakAllOtherEvents()
        {
            _refCounter++;
        }

        private void setNewActivity(Activity a)
        {
            if (_mainActivity != null)
            {
                _mainActivity.ExecuteExit(this);
            }

            _mainActivity = a;

            _canvas.Clear();
            BreakAllOtherEvents();

            if (_mainActivity != null)
            {
                _mainActivity.Form = this;
                _mainActivity.Create(this);
                _activityExecuted = false;
            }

            this.Invalidate();
        }

        public Activity Activity
        {
            get { return _mainActivity; }
            set
            {
                if (this.InvokeRequired)
                {
                    try
                    {
                        this.Invoke(new Action<Activity>(setNewActivity), value);
                    }
                    catch { }
                }
                else
                {
                    setNewActivity(value);
                }
            }
        }

        public UICanvas UICanvas
        {
            get { return _canvas; }
        }

        public UIForm()
        {
            InitializeComponent();

            this.UICanvas.Parent = this;
        }

        private Image _backgroundImage;
        
        public Image BackgroundImage {
            get { return _backgroundImage; }
            set
            {
                // AŞAĞIDAKİ KOD HATALI!

                //if (_backgroundImage != null)
                //{
                //    _backgroundImage.Dispose();
                //    _backgroundImage = null;
                //}

                _backgroundImage = value;
            }
        }

        public G3Color BackgroundColor { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Yoksa yarat
            if (_offScreen == null)
            {
                _offScreen = new G3Canvas(this.Width, this.Height);
            }

            // Resize oldu ise!
            if (_offScreen.Surface.Width != this.Width || _offScreen.Surface.Height != this.Height)
            {
                _offScreen.Dispose();
                _offScreen = new G3Canvas(this.Width, this.Height);
            }

            // Render
            Graphics g = _offScreen.Surface.WindowsGraphics;

            // Background
            if (this.BackgroundImage != null)
            {
                g.DrawImage(this.BackgroundImage, 0, 0);
            }
            else
            {
                _offScreen.BeginDrawing();
                _offScreen.Surface.Clear(BackgroundColor);
                _offScreen.EndDrawing();
            }

            // Render
            _canvas.Render(_offScreen);

            // ExecuteOnce
            // Activity varsa ve Execute edilmemişse.
            if (Activity != null && !_activityExecuted)
            {
                if (Activity.Transition != TransitionEffects.None)
                {
                    // Transition here
                    MakeTransition(Activity.Transition, e.Graphics);
                }
                else
                {
                    // Ekrana çiz
                    _offScreen.Surface.DrawCanvas(e.Graphics, e.ClipRectangle);
                }

                _activityExecuted = true;
                Activity.ExecuteOnce(this);
            }
            else
            {
                // Ekrana çiz
                _offScreen.Surface.DrawCanvas(e.Graphics, e.ClipRectangle);
            }
        }

        private void MakeTransition(TransitionEffects transitionEffects, Graphics g)
        {
            switch (transitionEffects)
            {
                case TransitionEffects.None:
                    _offScreen.Surface.DrawCanvas(g, 0, 0);
                    break;

                case TransitionEffects.FromBottom:
                    {
                        int h = Screen.PrimaryScreen.Bounds.Height;

                        for (int k = h; k > 0; k = k - 10)
                        {
                            _offScreen.Surface.DrawCanvas(g, 0, k);
                            SynchSleep.Go(_transitionMSec);
                        }

                        _offScreen.Surface.DrawCanvas(g, 0, 0);
                    }
                    break;

                case TransitionEffects.FromTop:
                    {
                        int h = Screen.PrimaryScreen.Bounds.Height;

                        for (int k = -h; k < 0; k = k + 10)
                        {
                            _offScreen.Surface.DrawCanvas(g, 0, k);
                            SynchSleep.Go(_transitionMSec);
                        }

                        _offScreen.Surface.DrawCanvas(g, 0, 0);
                    }
                    break;

                case TransitionEffects.FromLeft:
                    {
                        int w = Screen.PrimaryScreen.Bounds.Width;

                        for (int k = -w; k < 0; k = k + 10)
                        {
                            _offScreen.Surface.DrawCanvas(g, k, 0);
                            SynchSleep.Go(_transitionMSec);
                        }

                        _offScreen.Surface.DrawCanvas(g, 0, 0);
                    }
                    break;

                case TransitionEffects.FromRight:
                    {
                        int w = Screen.PrimaryScreen.Bounds.Width;

                        for (int k = w; k > 0; k = k - 10)
                        {
                            _offScreen.Surface.DrawCanvas(g, k, 0);
                            SynchSleep.Go(_transitionMSec);
                        }

                        _offScreen.Surface.DrawCanvas(g, 0, 0);
                    }
                    break;

                default:
                    _offScreen.Surface.DrawCanvas(g, 0, 0);
                    break;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Bu satıra artık gerek yok.
            // base.OnPaintBackground(e);
        }

        private void UIForm_MouseDown(object sender, MouseEventArgs e)
        {
            _refMouseHolder = _refCounter;

            this.UICanvas.MouseDown(e);
            this.Invalidate();
        }

        private void UIForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.NewActivityLoaded)
                return;

            this.UICanvas.MouseUp(e);
            this.Invalidate();
        }

        private void UIForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.NewActivityLoaded)
                return;

            this.UICanvas.MouseMove(e);
            this.Invalidate();
        }
    }
}
