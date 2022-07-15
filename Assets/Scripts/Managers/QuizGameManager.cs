using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

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

    [SerializeField]
    private WelcomeScreen _welcomeScreen = null;

    [SerializeField]
    private QuizScreen _quizScreen = null;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentQuizGameState = QuizGameState.WelcomeScreen;

        // Find the WelcomeScreen component and assign it to _welcomeScreen
        if (_welcomeScreen == null)
        {
            _welcomeScreen = FindObjectOfType<WelcomeScreen>();
        }
        // Same for the Quiz Screen
        if (_quizScreen == null)
        {
            _quizScreen = FindObjectOfType<QuizScreen>();
        }

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

        // Update the _currentQuizGameState to PreparingQuiz
        _currentQuizGameState = QuizGameState.PreparingQuiz;
        // Prepare the quiz after updating _currentQuizGameState value
        StartCoroutine(PrepareQuiz(playlistId));
    }

    private IEnumerator PrepareQuiz(int playlistId)
    {
        if (_currentQuizGameState != QuizGameState.PreparingQuiz)
        {
            yield break;
        }

        // Set the _currentQuizPlaylist using the given playlistId
        _currentQuizPlaylist = _playlists.playlists[playlistId];
        Debug.Log("Playlist: " + _currentQuizPlaylist.playlist + " was selected. Preparing Quiz...");

        _currentQuizQuestionId = 0;
        _currentQuizAnswerIds = new List<int>();

        // Get the urls of all the song clips for all the questions
        List<string> songClipUrls = new List<string>();
        // Get the urls of all the songs thumbnails for all the questions
        List<string> songThumbnailUrls = new List<string>();

        foreach (Question question in _currentQuizPlaylist.questions)
        {
            songClipUrls.Add(question.song.sample);
            songThumbnailUrls.Add(question.song.picture);
        }
        
        // Start a coroutine that will download all the song clips and thumbnails from the web and add them to a separate list
        yield return StartCoroutine(_quizScreen.GetSongClips(songClipUrls));
        yield return StartCoroutine(_quizScreen.GetSongThumbnails(songThumbnailUrls));

        // Update the _currentQuizGameState value to QuizScreen now that both the song clips and thumbnails have been fetched
        _currentQuizGameState = QuizGameState.QuizScreen;

        // Make the Welcome Screen UI shrink with a delay using Tweening. Then disable the welcome screen gameobject once the animation is completed
        _welcomeScreen.background.transform.DOScale(0, 0.25f).OnComplete(delegate { _welcomeScreen.background.gameObject.SetActive(false); });

        _quizScreen.OnQuizStarted();
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