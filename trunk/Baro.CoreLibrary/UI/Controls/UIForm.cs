using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Baro.CoreLibrary;
using Baro.CoreLibrary.Extensions;
using Baro.CoreLibrary.UI.Activities;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public partial class UIForm : Form
    {
        private G3Canvas _offScreen;
        private readonly UICanvas _canvas = new UICanvas();
        private Activity _mainActivity;
        private bool _activityExecuted = false;

        public Activity Activity
        {
            get { return _mainActivity; }
            set
            {
                if (_mainActivity != null)
                {
                    _mainActivity.ExecuteExit(this);
                }

                _mainActivity = value;
                _canvas.Clear();
                _canvas.IncRefCounter();

                if (_mainActivity != null)
                {
                    _mainActivity.Form = this;
                    _mainActivity.Create(this);
                    _activityExecuted = false;
                }

                this.Invalidate();
            }
        }

        public UICanvas UICanvas
        {
            get { return _canvas; }
        }

        public UIForm()
        {
            InitializeComponent();
        }

        public Image BackgroundImage { get; set; }
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

            // Ekrana çiz
            _offScreen.Surface.DrawCanvas(e.Graphics, e.ClipRectangle);

            // ExecuteOnce
            if (Activity != null)
            {
                if (!_activityExecuted)
                {
                    Activity.ExecuteOnce(this);
                    _activityExecuted = true;
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Bu satıra artık gerek yok.
            // base.OnPaintBackground(e);
        }

        private void UIForm_MouseDown(object sender, MouseEventArgs e)
        {
            this.UICanvas.MouseDown(e);
            this.Invalidate();
        }

        private void UIForm_MouseUp(object sender, MouseEventArgs e)
        {
            this.UICanvas.MouseUp(e);
            this.Invalidate();
        }

        private void UIForm_MouseMove(object sender, MouseEventArgs e)
        {
            this.UICanvas.MouseMove(e);
            this.Invalidate();
        }
    }
}
