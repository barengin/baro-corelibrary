using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary;
using System.Drawing;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public enum KeyboardControl
    {
        Char,
        SpaceBar = 0x0020,
        NumericKeypad = 0x0021,
        Back = 0x0022,
        Backspace = 0x0023,
        Enter = 0x0024,
        AlphaKeypad = 0x0025
    }

    public delegate void KeyboardCharPressedDelegate(object sender, char c, KeyboardControl e);

    public class Keyboard : UIElement
    {
        public bool NumericKeyboard { get; set; }

        public Image MaskImage { get; set; }
        public bool AutoConnectFocusedTextbox { get; set; }

        public Gradient AlfanumericButtonsGradient { get; set; }
        public Gradient ControlButtonsGradient { get; set; }
        public CompundFont FontStyle { get; set; }

        #region Events
        public event KeyboardCharPressedDelegate OnCharPressedEvent;

        protected void OnCharPressed(char c, KeyboardControl e)
        {
            if (this.AutoConnectFocusedTextbox)
            {
                foreach (var t in UICanvas)
                {
                    Textbox txt = t as Textbox;

                    if (txt != null && txt.Focused)
                    {
                        txt.AddChar(c, e);
                        break;
                    }
                }
            }

            if (OnCharPressedEvent != null)
            {
                OnCharPressedEvent(this, c, e);
            }
        }

        #endregion

        private UICanvas _canvasAlfa = new UICanvas();
        private UICanvas _canvasNum = new UICanvas();

        private bool _layoutCreated = false;

        /*
         * \u0020 Space bar
         * \u0021 Sayısal klavye
         * \u0022 GERİ
         * \u0023 Backspace
         * \u0024 ENTER - İLERİ
         * \u0025 Alfabetik klavye
         * 
         * */

        private const string keyboardline1 = "QWERTYUIOPĞÜ";
        private const string keyboardline2 = "ASDFGHJKLŞİ";
        private const string keyboardline3 = "ZXCVBNMÖÇ\u0021";
        private const string keyboardline4 = "\u0022\u0020\u0023\u0024";

        private const string keyboardnumber1 = "12345";
        private const string keyboardnumber2 = "67890";
        private const string keyboardnumber3 = "./\u0025";
        private const string keyboardnumber4 = "\u0022\u0020\u0023\u0024";

        public int SPACE_BETWEEN_BUTTONS { get; set; }

        public Keyboard()
            : base()
        {
            SPACE_BETWEEN_BUTTONS = 5;
            AutoConnectFocusedTextbox = true;

            ControlButtonsGradient = new Gradient()
            {
                FromColor = G3Color.FromRGB(166, 52, 0),
                ToColor = G3Color.BLACK,
                UseAlpha = false
            };

            AlfanumericButtonsGradient = new Gradient()
            {
                FromColor = G3Color.FromRGB(247, 243, 247),
                ToColor = G3Color.FromRGB(156, 154, 156),
                UseAlpha = false
            };

            this.FontStyle = new CompundFont(null, G3Color.WHITE, G3Color.BLACK);
        }

        internal override void MouseDown(System.Drawing.Point p)
        {
            if (NumericKeyboard)
            {
                _canvasNum.MouseDown(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));
            }
            else
            {
                _canvasAlfa.MouseDown(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));
            }
        }

        internal override void MouseUp(System.Drawing.Point p)
        {
            if (NumericKeyboard)
            {
                _canvasNum.MouseUp(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));
            }
            else
            {
                _canvasAlfa.MouseUp(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));
            }
        }

        public override void Render(G3Canvas g)
        {
            if (!_layoutCreated)
            {
                _layoutCreated = true;
                CreateLayout();
            }

            g.BeginDrawing();
            g.FillRectangle(Location.X, Location.Y, Size.Width, Size.Height, G3Color.FromRGB(49, 52, 49));
            g.Rectangle(Location.X, Location.Y, Size.Width - 1, Size.Height, G3Color.GRAY);
            g.EndDrawing();

            if (NumericKeyboard)
            {
                _canvasNum.Render(g);
            }
            else
            {
                _canvasAlfa.Render(g);
            }
        }

        private void CreateLayout()
        {
            int y = Location.Y + SPACE_BETWEEN_BUTTONS;
            int h;

            h = CreateLine(y, keyboardline1, _canvasAlfa);
            CreateLine((int)((h + SPACE_BETWEEN_BUTTONS) + y), keyboardline2, _canvasAlfa);
            CreateLine((int)((h + SPACE_BETWEEN_BUTTONS) * 2 + y), keyboardline3, _canvasAlfa);
            CreateLine((int)((h + SPACE_BETWEEN_BUTTONS) * 3 + y), keyboardline4, _canvasAlfa);

            h = CreateLine(y, keyboardnumber1, _canvasNum);
            CreateLine((int)((h + SPACE_BETWEEN_BUTTONS) + y), keyboardnumber2, _canvasNum);
            CreateLine((int)((h + SPACE_BETWEEN_BUTTONS) * 2 + y), keyboardnumber3, _canvasNum);
            CreateLine((int)((h + SPACE_BETWEEN_BUTTONS) * 3 + y), keyboardnumber4, _canvasNum);
        }

        private int CreateLine(int y, string keyboard, UICanvas canvas)
        {
            int w = Size.Width - (((keyboard.Length) + 1) * SPACE_BETWEEN_BUTTONS);
            w = w / (keyboard.Length);

            int h = Size.Height - (SPACE_BETWEEN_BUTTONS * 5);
            h = h / 4;

            float x = Location.X + SPACE_BETWEEN_BUTTONS;

            foreach (var i in keyboard)
            {
                var button = canvas.Add<GradientButton>(() => new GradientButton()
                {
                    FontStyle = this.FontStyle,
                    Location = new System.Drawing.Point((int)x, y),
                    Size = new System.Drawing.Size((int)w, (int)h),
                    MaskImage = this.MaskImage,
                    Gradient = this.AlfanumericButtonsGradient
                });

                switch (i)
                {
                    case '\u0020':
                        button.Text = "Boşluk";
                        button.OnClickEvent += new EventHandler(delegate(object sender, EventArgs e)
                        {
                            OnCharPressed('\u0020', KeyboardControl.SpaceBar);
                        });
                        break;

                    case '\u0025':
                        button.Text = "abc";
                        button.Gradient = this.ControlButtonsGradient;
                        button.OnClickEvent += new EventHandler(delegate(object sender, EventArgs e)
                        {
                            this.NumericKeyboard = !this.NumericKeyboard;
                        });
                        break;

                    case '\u0021':
                        button.Gradient = this.ControlButtonsGradient;
                        button.Text = "123";
                        button.OnClickEvent += new EventHandler(delegate(object sender, EventArgs e)
                        {
                            this.NumericKeyboard = !this.NumericKeyboard;
                        });
                        break;

                    case '\u0022':
                        button.Text = "Geri";
                        button.Gradient = this.ControlButtonsGradient;
                        button.OnClickEvent += new EventHandler(delegate(object sender, EventArgs e)
                        {
                            OnCharPressed('\u0022', KeyboardControl.Back);
                        });
                        break;

                    case '\u0023':
                        button.Text = "Sil";
                        button.Gradient = this.ControlButtonsGradient;
                        button.OnClickEvent += new EventHandler(delegate(object sender, EventArgs e)
                        {
                            OnCharPressed('\u0023', KeyboardControl.Backspace);
                        });
                        break;

                    case '\u0024':
                        button.Text = "Ok";
                        button.Gradient = this.ControlButtonsGradient;
                        button.OnClickEvent += new EventHandler(delegate(object sender, EventArgs e)
                        {
                            OnCharPressed('\u0024', KeyboardControl.Enter);
                        });

                        break;

                    default:
                        button.Text = i.ToString();
                        button.OnClickEvent += new EventHandler(delegate(object sender, EventArgs e)
                        {
                            OnCharPressed(((GradientButton)sender).Text[0], KeyboardControl.Char);
                        });

                        break;
                }

                x += (SPACE_BETWEEN_BUTTONS + w);
            }

            return h;
        }

        protected override void Dispose(bool disposing)
        {
            _canvasAlfa.Clear();
            _canvasNum.Clear();
        }

        internal override void MouseMove(Point p)
        {
        }
    }
}
