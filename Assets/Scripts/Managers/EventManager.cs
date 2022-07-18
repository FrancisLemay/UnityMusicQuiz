using UnityEngine.Events;

public static class EventManager
{
    public static UnityAction OnPlaylistsFetched;

    public static UnityAction OnQuizAssetReady;

    public static UnityAction<QuizGameManager.QuizGameState> OnQuizGameStateChanged;

}
