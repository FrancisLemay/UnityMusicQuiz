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

    private Playlists _playlists = null;
    public Playlists Playlists { get { return _playlists; } }

    private QuizGameState _currentQuizGameState;

    private Playlist _currentQuizPlaylist = null;

    private WelcomeScreen _welcomeScreen = null;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentQuizGameState = QuizGameState.WelcomeScreen;

        FetchPlaylistsFromLocalJSON();
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

            // Find the WelcomeScreen component and assign it to _welcomeScreen
            if (_welcomeScreen == null)
            {
                _welcomeScreen = FindObjectOfType<WelcomeScreen>();
            }

            // Refresh the playlists list on the WelcomeScreen UI component
            _welcomeScreen.RefreshPlaylistsList();

            Debug.Log(_playlists.playlists.Length + " playlists were successfully loaded!");
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

        // Set the _currentQuizPlaylist using the given playlistId
        _currentQuizPlaylist = _playlists.playlists[playlistId];
        Debug.Log("Playlist: " + _currentQuizPlaylist.playlist + " was selected. Preparing Quiz...");

        // Update the _currentQuizGameState to PreparingQuiz
        _currentQuizGameState = QuizGameState.PreparingQuiz;
        // Call PrepareQuiz()
        PrepareQuiz();
    }

    private async void PrepareQuiz()
    {
        if (_currentQuizGameState != QuizGameState.PreparingQuiz)
        {
            return;
        }


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
