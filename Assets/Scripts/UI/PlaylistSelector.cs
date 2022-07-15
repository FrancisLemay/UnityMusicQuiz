using UnityEngine;
using UnityEngine.UI;

public class PlaylistSelector : MonoBehaviour
{
    private int _playlistId = 0;

    public Button playlistSelectorButton = null;
    public TMPro.TMP_Text playlistTitleText = null;

    public void LoadPlaylistInfo(int playlistId, string playlistTitle)
    {
        _playlistId = playlistId;

        if (playlistSelectorButton != null)
        {
            playlistSelectorButton.onClick.RemoveAllListeners();
            playlistSelectorButton.onClick.AddListener(OnPlaylistPressed);
        }

        if (playlistTitleText != null)
        {
            playlistTitleText.text = playlistTitle;
        }
    }

    private void OnPlaylistPressed()
    {
        if (QuizGameManager.Instance == null)
        {
            return;
        }

        QuizGameManager.Instance.OnPlaylistSelected(_playlistId);
    }
}
