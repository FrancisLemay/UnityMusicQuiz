using UnityEngine;

public class PlaylistSelector : MonoBehaviour
{
    public TMPro.TMP_Text playlistTitleText = null;

    public void OnPlaylistPressed()
    {
        // Trigger action OnPlaylistSelected and pass the sibling index of the pressed playlist button
        EventManager.OnPlaylistSelected.Invoke(transform.GetSiblingIndex());
    }
}
