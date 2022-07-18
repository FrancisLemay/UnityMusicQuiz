using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class QuizAssetsManager : MonoBehaviour
{
    private static QuizAssetsManager _instance = null;
    public static QuizAssetsManager Instance { get { return _instance; } }

    [SerializeField]
    private List<AudioClip> _quizSongClips = new List<AudioClip>();
    public List<AudioClip> QuizSongClips { get { return _quizSongClips; } }

    [SerializeField]
    private List<Texture2D> _quizSongThumbnails = new List<Texture2D>();
    public List<Texture2D> QuizSongThumbnails { get { return _quizSongThumbnails; } }

    private void Awake()
    {
        _instance = this;

        EventManager.OnQuizGameStateChanged += OnQuizGameStateChanged;
    }

    private void OnDestroy()
    {
        EventManager.OnQuizGameStateChanged -= OnQuizGameStateChanged;
    }

    private void OnQuizGameStateChanged(QuizGameManager.QuizGameState quizGameState)
    {
        switch (quizGameState)
        {
            case QuizGameManager.QuizGameState.PreparingQuiz:
                StartCoroutine(DownloadQuizAssets());
                break;
            default:
                break;
        }
    }

    private IEnumerator DownloadQuizAssets()
    {
        // Get the urls of all the song clips for all the questions
        List<string> songClipUrls = new List<string>();
        // Get the urls of all the songs thumbnails for all the questions
        List<string> songThumbnailUrls = new List<string>();

        foreach (Question question in QuizGameManager.Instance.CurrentQuizPlaylist.questions)
        {
            songClipUrls.Add(question.song.sample);
            songThumbnailUrls.Add(question.song.picture);
        }

        // Start coroutines that will download all the song clips and thumbnails from the web and add them to a separate list
        yield return StartCoroutine(GetSongClips(songClipUrls));
        yield return StartCoroutine(GetSongThumbnails(songThumbnailUrls));

        EventManager.OnQuizAssetReady.Invoke();
    }

    #region Quiz Assets Functions
    public IEnumerator GetSongThumbnails(List<string> thumbnailUrls)
    {
        if (thumbnailUrls == null || thumbnailUrls.Count == 0)
        {
            yield break;
        }

        // First, clear the _songThumbnails list
        _quizSongThumbnails.Clear();

        foreach (string thumbnailUrl in thumbnailUrls)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(thumbnailUrl);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (myTexture != null)
                {
                    _quizSongThumbnails.Add(myTexture);
                }
            }
        }
    }

    public IEnumerator GetSongClips(List<string> songUrls)
    {
        if (songUrls == null || songUrls.Count == 0)
        {
            yield break;
        }

        // First, clear the _songClips list
        _quizSongClips.Clear();

        foreach (string songUrl in songUrls)
        {
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(songUrl, AudioType.WAV);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                if (myClip != null)
                {
                    _quizSongClips.Add(myClip);
                }
            }
        }
    }
    #endregion
}
