using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System;

/// <summary>
/// Localization Manager
/// </summary>
public static class LocalizationManager
{
    public static CultureInfo CultureInfo;

    /// <summary>
    /// Regex used for the text translation
    /// </summary>
    static readonly Regex validChar = new Regex(@"\$[a-zA-Z0-9_]+");

    /// <summary>
    /// Dictionary who content all txt at language
    /// </summary>
    static Dictionary<string, string> _allDataLanguage = new Dictionary<string, string>();

    /// <summary>
    /// static access to the language suffix
    /// </summary>
    public static string Language = "en";

    /// <summary>
    /// Deserializes pAllText and fills <see cref="_allDataLanguage"/> with key and translated value
    /// </summary>
    /// <param name="pAllText">all entries of the localization file</param>
    /// <param name="pDoClear">Clears current data if true.</param>
    /// <exception cref="BrainbugCustomException">thrown when a duplicate entry were found</exception>
    public static void InitLocalization(string[] pAllText, bool pDoClear = true)
    {
        string[] rawKVPaire;
        int i = 0;
        if (pDoClear)
            _allDataLanguage.Clear();
        List<string> duplicates = new List<string>();
        while (i < pAllText.Length)
        {
            rawKVPaire = pAllText[i].Split('\t');

            if (rawKVPaire.Length >= 2)
            {
                string key = rawKVPaire[0];
                string value = rawKVPaire[1];

                if (_allDataLanguage.ContainsKey(key))
                    duplicates.Add(key);
                else if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    _allDataLanguage.Add(key, value);
            }

            i++;
        }

        if (duplicates.Count != 0)
            throw new Exception(string.Format("[LocalizationManager] the following key already exists and second values were skipped:\n{0}", string.Join(" - ", duplicates.ToArray())));
    }


    /// <summary>
    /// Set a new localization key
    /// </summary>
    /// <param name="pKey">the key id (no '$' at the begining</param>
    /// <param name="pValue">the key value</param>
    /// <remarks>Prefer to use UPPER CASE for key added from runtime to avoid confusing key loc from csv</remarks>
    public static void SetLocalizationKey(string pKey, string pValue)
    {
        if (_allDataLanguage.ContainsKey(pKey))
            _allDataLanguage[pKey] = pValue;
        else
            _allDataLanguage.Add(pKey, pValue);
    }

    /// <summary>
    /// Check if a spcefic key exists in the language data.
    /// </summary>
    /// <param name="pKey">The key id (no $ at the begining).</param>
    /// <returns></returns>
    public static bool HasLocalizationKey(string pKey)
    {
        return _allDataLanguage.ContainsKey(pKey);
    }

    private static string toLocalized_Internal(Match pMatch)
    {
        var substring = pMatch.ToString().Substring(1);
        if (_allDataLanguage.ContainsKey(substring))
            return _allDataLanguage[substring].ToLocalized();
        return pMatch.ToString();
    }

    /// <summary>
    /// translate the string using regex' replace function return this corresponding if it's find or this string if is not find
    /// </summary>
    /// <returns>the translated code</returns>
    public static string ToLocalized(this string pName)
    {
        if (string.IsNullOrEmpty(pName))
            return pName;

        string result;
        string translate = pName;

        do
        {
            result = translate;
            translate = validChar.Replace(translate, toLocalized_Internal);
        }
        while (result != translate);

        return result;
    }
}
