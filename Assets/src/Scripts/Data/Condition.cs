using System;

public enum ConditionType
{
    None,

    // Pokeball
    DateTimeIs,
    TargetIsOfType,
    // ... http://bulbapedia.bulbagarden.net/wiki/Pokeball

    // Ability

}

[Serializable]
public class Condition
{
    public ConditionType Type { get; set; }

    public string Target { get; set; }

    public string Args { get; set; }
}
