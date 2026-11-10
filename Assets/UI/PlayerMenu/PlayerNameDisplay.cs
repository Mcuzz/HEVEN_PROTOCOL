using UnityEngine;

public sealed class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _label;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        _label.text = PlayerProfile.PlayerName;
    }
}
