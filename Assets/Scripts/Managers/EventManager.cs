using UnityEngine.Events;

public static class EventManager
{
    public static UnityAction OnPlaylistsFetched;

    public static UnityAction<int> OnPlaylistSelected;

    public static UnityAction OnQuizAssetReady;

    public static UnityAction<QuizGameManager.QuizGameState> OnQuizGameStateChanged;

    public static UnityAction<int> OnChoiceSelected;

    public static UnityAction OnNextQuestionRequested;
}
