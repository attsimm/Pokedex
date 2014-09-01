using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Pokedex.Pokemon
{
    public class Pokemon
    {
        /// <summary>
        /// Get the information concerning a particuliar pokemon
        /// </summary>
        /// <param name="id">Id of the pokemon (ex: 1 = Bulbasaur)</param>
        /// <returns>Data concerning the pokemon </returns>
        public static PokemonData GetPokemonData(int id)
        {
            PokemonData md = new PokemonData();
            JObject json = GetJsonPokemon(Constant.ApiUrl + "pokemon/" + id);
            md.Name = json["name"].ToString();
            md.ID = id;
            md.Image = Constant.ImgUrl + md.ID.ToString("000") + ".png";
            md.Json = json.ToString();
            foreach (var type in json["types"])
            {
                md.Type.Add(type["name"].ToString());
            }
            //Stats
            md.Attack = Convert.ToInt32(json["attack"]);
            md.Defense = Convert.ToInt32(json["defense"]);
            md.Special_Attack = Convert.ToInt32(json["sp_atk"]);
            md.Special_Defense = Convert.ToInt32(json["sp_def"]);
            md.Speed = Convert.ToInt32(json["speed"]);
            md.HP = Convert.ToInt32(json["hp"]);
            //Size
            md.Weight = json["weight"].ToString();
            md.Height = json["height"].ToString();
            //Moves
            List<Move> Moves = new List<Move>();
            foreach (var move in json["moves"])
            {
                if (move["learn_type"].ToString() == "level up")
                {
                    Move active_move = new Move();

                    active_move.Move_Id = Convert.ToInt32(move["resource_uri"].ToString().Replace("api/v1/move/", "").Replace("/", ""));
                    JObject pokemon_move_details = GetJsonPokemon(Constant.ApiUrl + "move/" + active_move.Move_Id);
                    active_move.Name = move["name"].ToString();
                    active_move.LearnType = move["learn_type"].ToString();
                    active_move.Level = Convert.ToInt32(move["level"]);
                    active_move.Accuracy = (double?)pokemon_move_details["accuracy"];
                    active_move.Power = (double?)pokemon_move_details["power"];
                    active_move.PP = (double?)pokemon_move_details["pp"];
                    active_move.Category = pokemon_move_details["category"].ToString();
                    active_move.Description = pokemon_move_details["description"].ToString().Replace("Ã©", "é");
                    Moves.Add(active_move);
                }
            }
            md.Moves = Moves;
            //Abilities
            foreach (var ability in json["abilities"])
            {
                md.Abilities.Add(ability["name"].ToString());
            }
            //Colour_Scheme
            md.ColourScheme = Pokemon_Colours.GetColourScheme(id);
            return md;
        }
        /// <summary>
        /// Get a list of all the pokemons
        /// </summary>
        /// <returns></returns>
        public static List<PokemonData> GetPokemons()
        {
            List<PokemonData> md = new List<PokemonData>();
            JObject json = GetJsonPokemon(Constant.ApiUrl + "pokedex/");
            foreach (var pokemon in json["objects"][0]["pokemon"])
            {
                PokemonData Pk = new PokemonData();
                Pk.ID = Convert.ToInt32(pokemon["resource_uri"].ToString().Replace("api/v1/pokemon/", "").Replace("/", ""));
                Pk.Name = pokemon["name"].ToString();
                Pk.ColourScheme = new ColourScheme();
                md.Add(Pk);
            }
            md = md.OrderBy(m => m.ID).ToList();
            return md;
        }
        private static JObject GetJsonPokemon(string url)
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString(url);
                JObject json_deserialize = new JObject();
                return JsonConvert.DeserializeObject<JObject>(json);
            }
        }
    }
}