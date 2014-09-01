using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pokedex.Models
{
    public class PokemonData
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public string PokeApiUrl { get; set; }

        public string Image { get; set; }

        public string Json { get; set; }
        //Informations about the pokeman
        public List<string> Type { get; set; }
        //Stats
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Special_Attack { get; set; }
        public int Special_Defense { get; set; }
        public int Speed { get; set; }
        public int HP { get; set; }


        public ColourScheme ColourScheme { get; set; }
        public PokemonData()
        {
            Type = new List<string>();
            ColourScheme = new ColourScheme();
        }
    }
    public class ColourScheme
    {
        public List<string> HexColours { get; set; }
        public List<string> RgbaColours { get; set; }
        public ColourScheme()
        {
            HexColours = new List<string>();
            RgbaColours = new List<string>();
        }
    }
}