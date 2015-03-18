using System;
using System.Collections.Generic;
using System.IO;
using Baro.CoreLibrary.Text;

namespace Baro.CoreLibrary.G3
{
    public struct FontCharData
    {
        public int width;
        public byte[] data;
    }

    public sealed unsafe class G3Font : G3Object
    {
        public readonly int SpaceBetweenChars;
        public readonly int FontHeight;
        public readonly int SpaceSize;
        public readonly FontCharData[] Chars = new FontCharData[256];
        public byte Index;

        public const byte TRANSPARENT = 0;
        public const byte FONT = 1;
        public const byte HALO = 2;

        public G3Font(string fontFile, Encoding encoding)
        {
            Dictionary<string, string> config = new Dictionary<string, string>();

            using (StreamReader sr = new StreamReader(fontFile, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith(";"))
                        continue;

                    string[] s = line.Split('=');

                    if (s.Length == 2)
                        config.Add(s[0], s[1]);
                }
            }

            SpaceBetweenChars = int.Parse(config["spacebetweenchars"]);
            config.Remove("spacebetweenchars");

            FontHeight = int.Parse(config["fontheight"]);
            config.Remove("fontheight");

            SpaceSize = int.Parse(config["space"]);
            config.Remove("space");

            foreach (KeyValuePair<string, string> kvp in config)
            {
                int charcode1254 = encoding[kvp.Key[0]];

                string[] s = kvp.Value.Split(',');

                Chars[charcode1254].width = int.Parse(s[0]);
                Chars[charcode1254].data = new byte[s.Length - 1];

                for (int x = 1; x < s.Length; x++)
                {
                    byte d;

                    if (s[x][0] == 'T')
                    {
                        d = TRANSPARENT;
                    }
                    else
                        if (s[x][0] == 'F')
                        {
                            d = FONT;
                        }
                        else
                            if (s[x][0] == 'H')
                            {
                                d = HALO;
                            }
                            else
                                throw new Exception("hatalı fn dosyası");

                    Chars[charcode1254].data[x - 1] = d;
                }
            }

            Chars[32].width = SpaceSize;
        }

        public int TextWidth(byte* encodedByteCharArray, int len)
        {
            int r = 0;

            for (int k = 0; k < len; k++)
                r += (this.Chars[encodedByteCharArray[k]].width);

            return r + ((this.SpaceBetweenChars * (len - 1)) / 2);
        }

        public int TextWidth(byte[] encodedByteCharArray)
        {
            int r = 0;

            for (int k = 0; k < encodedByteCharArray.Length; k++)
                r += (this.Chars[encodedByteCharArray[k]].width);

            return r + ((this.SpaceBetweenChars * (encodedByteCharArray.Length - 1)) / 2);
        }
    }
}
