using UnityEngine;
using UnityEngine.UI;

public class WelcomeScreen : MonoBehaviour
{
    public ScrollRect playlistScrollRect;

    public GameObject playlistPrefab;

    public void RefreshPlaylistsList()
    {
        if (QuizGameManager.Instance == null)
        {
            Debug.LogWarning("QuizGameManager static instance is NULL. Cannot refresh playlists list.");
            return;
        }

        if (playlistScrollRect.content.childCount > 0)
        {
            foreach (Transform transform in playlistScrollRect.content)
            {
                Destroy(transform.gameObject);
            }
        }

        for (int i = 0; i < QuizGameManager.Instance.Playlists.playlists.Length; i++)
        {
            // For each playlist inside the Playlists array, instantiate a playlist selector prefab inside the scroll view content component.
            GameObject temp = Instantiate(playlistPrefab, playlistScrollRect.content);

            // Load the playlist info on the instantiated prefab by passing the index of the playlist in the array + the title.
            temp.TryGetComponent(out PlaylistSelector tempPlaylistSelector);
            tempPlaylistSelector.LoadPlaylistInfo(i, QuizGameManager.Instance.Playlists.playlists[i].playlist);
        }
    }
}
