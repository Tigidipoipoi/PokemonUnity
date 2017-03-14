//Original Scripts by IIColour (IIColour_Spectrum)

using UnityEngine;
using System.Collections;

public class PokemonDatabase : Singleton<PokemonDatabase>
{
    private DatabaseController _databaseController;

    public PokemonDatabase()
    {
        _databaseController = new DatabaseController();
    }

    public PokemonSpecies GetPokemonSpeciesByGameId(string pGameId)
    {
        PokemonSpecies pokemon = _databaseController.DBConnection
            .Find<PokemonSpecies>(pkmn => pkmn.GameId == pGameId);

        if (pokemon != null)
            pokemon.LoadForeignData(_databaseController.DBConnection);

        return pokemon;
    }

    public PokemonSpecies GetPokemonSpeciesByName(string pName)
    {
        PokemonSpecies pokemon = _databaseController.DBConnection
            .Find<PokemonSpecies>(pkmn => pkmn.Name == pName);

        if (pokemon != null)
            pokemon.LoadForeignData(_databaseController.DBConnection);

        return pokemon;
    }
}
