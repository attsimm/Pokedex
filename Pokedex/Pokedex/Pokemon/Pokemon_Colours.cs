using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;

namespace Pokedex.Pokemon
{
    public class Pokemon_Colours
    {
        public static ColourScheme GetColourScheme(int pokemon_id)
        {
            string url_pokemon_img = Constant.ImgUrl + pokemon_id.ToString("000") + ".png";

            List<Color> MostUsedColors = new List<Color>();
            try
            {
                var request = WebRequest.Create(url_pokemon_img);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    Image bmp = Bitmap.FromStream(stream);
                    Bitmap bitmap = new Bitmap(bmp);
                    MostUsedColors = GetAllColours(bitmap);
                }
                ColourScheme cs = new ColourScheme();
                cs.HexColours = new List<string>();
                cs.RgbaColours = new List<string>();
                foreach (var colour in MostUsedColors)
                {
                    cs.HexColours.Add(HexConverter(colour));
                    cs.RgbaColours.Add(RGBConverter(colour));
                }
                List<string> MostUsed = cs.HexColours.OrderBy(i => i.Count()).ToList();
                List<string> FinalHex = new List<string>();
                foreach (var colours in MostUsed)
                {
                    if (colours != "#000000")
                    {
                        FinalHex.Add(colours);
                    }
                }
                cs.HexColours = FinalHex.OrderBy(i => i.Count()).Distinct().Take(10).ToList();
                return cs;
            }
            catch
            {
                return new ColourScheme();
            }
        }
        public static List<Color> GetAllColours(Bitmap theBitMap)
        {
            List<Color> TenMostUsedColors;
            List<int> TenMostUsedColorIncidences;

            Color MostUsedColor;
            int MostUsedColorIncidence;

            int pixelColor;

            Dictionary<int, int> dctColorIncidence;
            TenMostUsedColors = new List<Color>();
            TenMostUsedColorIncidences = new List<int>();

            MostUsedColor = Color.Empty;
            MostUsedColorIncidence = 0;

            dctColorIncidence = new Dictionary<int, int>();

            for (int row = 0; row < theBitMap.Size.Width; row++)
            {
                for (int col = 0; col < theBitMap.Size.Height; col++)
                {
                    pixelColor = theBitMap.GetPixel(row, col).ToArgb();

                    if (dctColorIncidence.Keys.Contains(pixelColor))
                    {
                        dctColorIncidence[pixelColor]++;
                    }
                    else
                    {
                        dctColorIncidence.Add(pixelColor, 1);
                    }
                }
            }

            var dctSortedByValueHighToLow = dctColorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            foreach (KeyValuePair<int, int> kvp in dctSortedByValueHighToLow)
            {
                TenMostUsedColors.Add(Color.FromArgb(kvp.Key));
                TenMostUsedColorIncidences.Add(kvp.Value);
            }

            MostUsedColor = Color.FromArgb(dctSortedByValueHighToLow.First().Key);
            MostUsedColorIncidence = dctSortedByValueHighToLow.First().Value;
            return TenMostUsedColors;
        }
        private static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
        private static String RGBConverter(System.Drawing.Color c)
        {
            return "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        }
    }
}