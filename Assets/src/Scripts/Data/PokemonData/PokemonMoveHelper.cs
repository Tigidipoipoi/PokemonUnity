using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PokemonMoveHelper
{
    public static string GetMoveCategoryName(PokemonMoveCategory pCategory)
    {
        switch (pCategory)
        {
            case PokemonMoveCategory.Physical:
                return "Physical";
            case PokemonMoveCategory.Special:
                return "Special";
            case PokemonMoveCategory.Status:
                return "Status";

            default:
                return null;
        }
    }
}
