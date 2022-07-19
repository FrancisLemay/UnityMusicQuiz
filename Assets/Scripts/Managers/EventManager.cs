using UnityEngine.Events;

public static class EventManager
{
    /// <summary>
    /// Triggered when the playlists are fetched from the local JSON file.
    /// </summary>
    public static UnityAction OnPlaylistsFetched;

    /// <summary>
    /// Triggered when a playlist is selected from the scrolling list in the welcome screen UI.
    /// </summary>
    public static UnityAction<int> OnPlaylistSelected;

    /// <summary>
    /// Triggered when both the quiz song clips and thumbnails are downloaded and available to use.
    /// </summary>
    public static UnityAction OnQuizAssetsReady;

    /// <summary>
    /// Triggered when the quiz game state changes. Useful for triggering UI events.
    /// </summary>
    public static UnityAction<QuizGameManager.QuizGameState> OnQuizGameStateChanged;

    /// <summary>
    /// Triggered once a choice is selected by pressing one of the four choices button.
    /// </summary>
    public static UnityAction<int> OnChoiceSelected;

    /// <summary>
    /// Triggered when the next question of the quiz is reached.
    /// </summary>
    public static UnityAction LoadNextQuestion;

    /// <summary>
    /// Triggered when the quiz results screen UI is closed.
    /// </summary>
    public static UnityAction OnQuizResultsClosed;
}
