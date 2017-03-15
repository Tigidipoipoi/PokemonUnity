using System.Collections.Generic;
using UnityEngine;

public class Pokeball
{
    public string Name { get; set; }

    // ToDo: Clamp catchRate between 0.0f and 255.0f.
    public List<Condition> CatchConditions { get; set; }
}
