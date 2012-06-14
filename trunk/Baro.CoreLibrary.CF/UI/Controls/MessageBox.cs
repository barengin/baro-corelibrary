using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class MessageBox : UIElement
    {
        public string Text { get; set; }
        public string Title { get; set; }
        
        public G3Font TitleFont { get; set; }
        public G3Font Font { get; set; }

        public Gradient Gradient { get; set; }
        public Gradient TitleGradient { get; set; }

        internal override void MouseDown(System.Drawing.Point p)
        {
        }

        internal override void MouseUp(System.Drawing.Point p)
        {
        }

        public MessageBox()
            : base()
        {
            Gradient = new Gradient()
            {
                FromColor = new G3Color(107, 162, 182),
                ToColor = new G3Color(201, 224, 232),
                UseAlpha = false
            };

            TitleGradient = new Gradient()
            {
                FromColor = new G3Color(18, 73, 130),
                ToColor = new G3Color(37, 111, 158),
                UseAlpha = false
            };
        }

        public override void Render(G3Canvas g)
        {
            g.BeginDrawing();

            g.DarkBox(0, 0,
                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, 10);

            int w = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 5 * 3;
            int h = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2;

            int x = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 5;
            int y = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 4;

            // MessageBox
            this.Gradient.Draw(g, x, y + 30, w, h);

            // Title
            this.TitleGradient.Draw(g, x, y, w, 30);

            // Title Text
            g.DrawTextCenter(Title, UICanvas.Encoding, TitleFont, x + (w / 2), y + (30 / 2), 0, 
                G3Color.BLACK, G3Color.WHITE);

            // Message Text
            g.DrawTextCenter(Text, UICanvas.Encoding, 
                Font, x + (w / 2), y + 60, 0, G3Color.BLACK, G3Color.BLACK);
            
            // Border
            g.Rectangle(x, y, w, h + 30, G3Color.GRAY);

            g.EndDrawing();
        }

        protected override void Dispose(bool disposing)
        {
        }

        internal override void MouseMove(System.Drawing.Point p)
        {
        }
    }
}
