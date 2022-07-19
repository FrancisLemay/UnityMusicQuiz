using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeScreen : MonoBehaviour
{
    public RectTransform background;

    public ScrollRect playlistScrollRect;

    public GameObject playlistPrefab;

    private void Awake()
    {
        EventManager.OnPlaylistsFetched += RefreshPlaylistsList;
        EventManager.OnQuizGameStateChanged += OnQuizGameStateChanged;
    }

    private void OnDestroy()
    {
        EventManager.OnPlaylistsFetched -= RefreshPlaylistsList;
        EventManager.OnQuizGameStateChanged -= OnQuizGameStateChanged;
    }

    private void RefreshPlaylistsList()
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

            temp.TryGetComponent(out PlaylistSelector tempPlaylistSelector);

            tempPlaylistSelector.LoadPlaylistInfo(i, QuizGameManager.Instance.Playlists.playlists[i].playlist);
        }
    }

    private void OnQuizGameStateChanged(QuizGameManager.QuizGameState quizGameState)
    {
        switch (quizGameState)
        {
            case QuizGameManager.QuizGameState.WelcomeScreen:
                background.gameObject.SetActive(true);
                background.transform.DOScale(1, 0.5f);
                break;
            case QuizGameManager.QuizGameState.PreparingQuiz:
                background.transform.DOScale(0, 0.5f).OnComplete(delegate { background.gameObject.SetActive(false); });
                break;
            default:
                break;
        }
    }
}
