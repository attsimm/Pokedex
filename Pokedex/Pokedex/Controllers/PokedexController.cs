using Newtonsoft.Json.Linq;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Pokedex.Pokemon;

namespace Pokedex.Controllers
{
    public class PokedexController : Controller
    {
        public ActionResult Index()
        {
            List<PokemonData> md = Pokedex.Pokemon.Pokemon.GetPokemons();
            return View(md);
        }

        public ActionResult Pokemon(int id = 0, string pokemonname = "")
        {
            if (id <= 0)
            {
                return Redirect("~/");
            }
            PokemonData md = Pokedex.Pokemon.Pokemon.GetPokemonData(id);
            return View(md);
        }
    }
}