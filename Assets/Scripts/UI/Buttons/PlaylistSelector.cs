using UnityEngine;

public class PlaylistSelector : MonoBehaviour
{
    private int _playlistId = 0;

    public TMPro.TMP_Text playlistTitleText = null;

    public void LoadPlaylistInfo(int playlistId, string playlistTitle)
    {
        _playlistId = playlistId;

        if (playlistTitleText != null)
        {
            playlistTitleText.text = playlistTitle;
        }
    }

    public void OnPlaylistPressed()
    {
        EventManager.OnPlaylistSelected.Invoke(_playlistId);
    }
}
