using System.Globalization;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public PokemonDatabase PokemonDb;

    public GameController()
    {
        PokemonDb = new PokemonDatabase();

        // Init localization.
        InitCulture();

        string path = string.Format("lang/{0}/Localisation.txt", LocalizationManager.Language);

        // Avoid to load outside the game.
        if (Application.isPlaying)
        {
            var localizationFile = Resources.Load<TextAsset>(path);
            if (localizationFile != null)
            {
                var allText = localizationFile.text.Split('\n');
                LocalizationManager.InitLocalization(allText, false);
            }
        }
    }

    /// <summary>
    /// Set Language to systemLanguage. If systemLanguage are not supported, set English.
    /// Also : init DataLanguage in <see cref="LocalizationManager"/>
    /// </summary>
    public void InitCulture()
    {
        LocalizationManager.Language = "en";
        switch (Application.systemLanguage)
        {
            case SystemLanguage.French:
                LocalizationManager.Language = "fr";
                LocalizationManager.CultureInfo = new CultureInfo("fr-FR");//CultureInfo.GetCultureInfo("fr-FR");
                break;
            case SystemLanguage.German:
                LocalizationManager.Language = "ge";
                LocalizationManager.CultureInfo = new CultureInfo("de-DE");//CultureInfo.GetCultureInfo("de-DE");
                break;
            case SystemLanguage.Spanish:
                LocalizationManager.Language = "sp";
                LocalizationManager.CultureInfo = new CultureInfo("es-ES");//CultureInfo.GetCultureInfo("es-ES");
                break;
            default:
                LocalizationManager.Language = "en";
                LocalizationManager.CultureInfo = new CultureInfo("en-US");//CultureInfo.GetCultureInfo("en-US");
                break;
        }
    }
}
