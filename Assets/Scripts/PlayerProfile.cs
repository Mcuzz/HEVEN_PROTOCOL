using UnityEngine;

public static class PlayerProfile
{
    private const string NameKey = "HEVEN_PROTOCOL.PlayerName";

    public static string PlayerName => PlayerPrefs.GetString(NameKey, "Jugador");

    public static void SaveName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;
        PlayerPrefs.SetString(NameKey, name.Trim());
        PlayerPrefs.Save();
    }
}
