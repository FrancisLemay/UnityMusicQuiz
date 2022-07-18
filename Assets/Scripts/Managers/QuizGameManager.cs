using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QuizGameManager : MonoBehaviour
{
    public enum QuizGameState
    {
        WelcomeScreen,
        PreparingQuiz,
        QuizScreen,
        ResultScreen
    }

    private static QuizGameManager _instance = null;
    public static QuizGameManager Instance { get { return _instance; } }

    [SerializeField]
    private Playlists _playlists = null;
    public Playlists Playlists { get { return _playlists; } }

    private QuizGameState _currentQuizGameState = QuizGameState.WelcomeScreen;

    private int _currentQuizQuestionId = 0;
    public int CurrentQuizQuestionId { get { return _currentQuizQuestionId; } }

    private List<int> _currentQuizAnswerIds = new List<int>();

    private Playlist _currentQuizPlaylist = null;
    public Playlist CurrentQuizPlaylist { get { return _currentQuizPlaylist; } }


    private void Awake()
    {
        _instance = this;

        EventManager.OnQuizAssetReady += OnQuizAssetReady;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Update the _currentQuizGameState to WelcomeScreen
        UpdateCurrentQuizGameState(QuizGameState.WelcomeScreen);

        FetchPlaylistsFromLocalJSON();
    }

    private void OnDestroy()
    {
        EventManager.OnQuizAssetReady -= OnQuizAssetReady;
    }

    private void FetchPlaylistsFromLocalJSON()
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, "Playlists.json");

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);

            // Pass the json to JsonUtility, and tell it to create a Playlists object from it
            _playlists = JsonUtility.FromJson<Playlists>("{\"playlists\":" + dataAsJson + "}");
            Debug.Log(_playlists.playlists.Length + " playlists were successfully loaded!");

            // Invoke the OnPlaylistsFetched event
            EventManager.OnPlaylistsFetched.Invoke();
        }
        else
        {
            Debug.LogError("Cannot load the playlists from the local JSON file!");
        }
    }

    public void OnPlaylistSelected(int playlistId)
    {
        if (_currentQuizGameState != QuizGameState.WelcomeScreen)
        {
            Debug.LogWarning("Current Quiz Game State is not WelcomeScreen, skipping OnPlaylistSelected() call...");
            return;
        }

        if (_playlists == null || _playlists.playlists[playlistId] == null)
        {
            Debug.LogWarning("The selected playlist is not valid!");
            return;
        }

        // Prepare the quiz after updating _currentQuizGameState value
        PrepareQuiz(playlistId);
    }

    private void PrepareQuiz(int playlistId)
    {
        if (_currentQuizGameState == QuizGameState.PreparingQuiz)
        {
            return;
        }

        // Set the _currentQuizPlaylist using the given playlistId
        _currentQuizPlaylist = _playlists.playlists[playlistId];
        Debug.Log("Playlist: " + _currentQuizPlaylist.playlist + " was selected. Preparing Quiz...");

        _currentQuizQuestionId = 0;
        _currentQuizAnswerIds = new List<int>();

        // Update the _currentQuizGameState to PreparingQuiz
        UpdateCurrentQuizGameState(QuizGameState.PreparingQuiz);
    }

    private void UpdateCurrentQuizGameState(QuizGameState quizGameState)
    {
        _currentQuizGameState = quizGameState;

        EventManager.OnQuizGameStateChanged.Invoke(_currentQuizGameState);
    }

    private void OnQuizAssetReady()
    {
        // If the quiz assets are ready to use, update the current quiz game state to QuizScreen
        UpdateCurrentQuizGameState(QuizGameState.QuizScreen);
    }
}

[System.Serializable]
public class Playlists
{
    public Playlist[] playlists;
}

[System.Serializable]
public class Playlist
{
    public string id;
    public Question[] questions;
    public string playlist;
}

[System.Serializable]
public class Question
{
    public string id;
    public int answerIndex;
    public Choice[] choices;
    public Song song;
}

[System.Serializable]
public class Choice
{
    public string artist;
    public string title;
}

[System.Serializable]
public class Song
{
    public string id;
    public string title;
    public string artist;
    public string picture;
    public string sample;
}