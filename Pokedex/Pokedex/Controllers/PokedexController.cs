using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace Pokedex.Controllers
{
    public class PokedexController : Controller
    {
        public const string ApiUrl = "http://pokeapi.co/api/v1/";
        public const string ImgUrl = "http://assets13.pokemon.com/assets/cms2/img/pokedex/detail/";

        public ActionResult Index()
        {
            List<PokemonData> md = new List<PokemonData>();
            JObject json = GetJsonPokemon(ApiUrl + "pokedex/");
            foreach (var pokemon in json["objects"][0]["pokemon"])
            {
                PokemonData Pk = new PokemonData();
                Pk.ID = Convert.ToInt32(pokemon["resource_uri"].ToString().Replace("api/v1/pokemon/", "").Replace("/", ""));
                Pk.Name = pokemon["name"].ToString();
                Pk.ColourScheme = new ColourScheme();
                md.Add(Pk);
            }
            md = md.OrderBy(m => m.ID).ToList();
            return View(md);
        }

        public ActionResult Pokemon(int id = 0, string pokemonname = "")
        {
            if (id <= 0)
            {
                return Redirect("~/");
            }
            PokemonData md = new PokemonData();
            JObject json = GetJsonPokemon(ApiUrl + "pokemon/" + id);
            md.Name = json["name"].ToString();
            md.ID = id;
            md.Image = ImgUrl + md.ID.ToString("000") + ".png";
            md.Json = json.ToString();
            foreach (var type in json["types"])
            {
                md.Type.Add(type["name"].ToString());
            }
            md.Attack = Convert.ToInt32(json["attack"]);
            md.Defense = Convert.ToInt32(json["defense"]);
            md.Special_Attack = Convert.ToInt32(json["sp_atk"]);
            md.Special_Defense = Convert.ToInt32(json["sp_def"]);
            md.Speed = Convert.ToInt32(json["speed"]);
            md.HP = Convert.ToInt32(json["hp"]);
            md.ColourScheme = GetColourScheme(id);

            return View(md);
        }
        public static string sep(string s)
        {
            int l = s.IndexOf("-");
            if (l > 0)
            {
                return s.Substring(0, l);
            }
            return "";

        }
        public JObject GetJsonPokemon(string url)
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString(url);
                JObject json_deserialize = new JObject();
                return JsonConvert.DeserializeObject<JObject>(json);
            }
        }
        public ColourScheme GetColourScheme(int pokemon_id)
        {
            string url_pokemon_img = ImgUrl + pokemon_id.ToString("000") + ".png";
            
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
        public List<Color> GetAllColours(Bitmap theBitMap)
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