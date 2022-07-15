using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuizScreen : MonoBehaviour
{
    public RectTransform background;

    public Image songClipThumbnail;

    public AudioSource audioSource;

    public Color validChoiceColor;
    public Color invalidChoiceColor;

    [SerializeField]
    private List<AudioClip> _songClips = new List<AudioClip>();
    
    [SerializeField]
    private List<Texture2D> _songThumbnails = new List<Texture2D>();

    [SerializeField]
    private List<ChoiceButton> _choiceButtons = new List<ChoiceButton>();

    public IEnumerator GetSongThumbnails(List<string> thumbnailUrls)
    {
        if (thumbnailUrls == null || thumbnailUrls.Count == 0)
        {
            yield break;
        }

        // First, clear the _songThumbnails list
        _songThumbnails.Clear();

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
                    _songThumbnails.Add(myTexture);
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
        _songClips.Clear();

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
                    _songClips.Add(myClip);
                }
            }
        }
    }

    public void OnQuizStarted()
    {
        background.transform.localScale = Vector3.zero;
        background.gameObject.SetActive(true);
        // Start a growing animation for the Quiz Screen UI and once its finished, play the current question song clip
        background.transform.DOScale(1, 1f).OnComplete(delegate { PlayCurrentQuestionSongClip(); });

        ApplyChoiceTexts();
    }

    private void ApplyChoiceTexts()
    {
        if (_choiceButtons == null)
        {
            return;
        }

        Question currentQuestion = QuizGameManager.Instance.CurrentQuizPlaylist.questions[QuizGameManager.Instance.CurrentQuizQuestionId];
        for (int i = 0; i < currentQuestion.choices.Length; i++)
        {
            _choiceButtons[i].choiceText.text = currentQuestion.choices[i].artist + " / " + currentQuestion.choices[i].title;
            
            // update the outline color of the choice button depending if its the right answer choice or not.
            _choiceButtons[i].choiceValidityOutlineImage.color = i == currentQuestion.answerIndex ? validChoiceColor : invalidChoiceColor;
            _choiceButtons[i].choiceValidityOutlineImage.gameObject.SetActive(false);
        }
    }

    public void OnChoiceButtonPressed()
    {
        // Make all the choice buttons outlines visible to display which choice was the correct answer.
        for (int i = 0; i < _choiceButtons.Count; i++)
        {
            _choiceButtons[i].choiceValidityOutlineImage.gameObject.SetActive(true);
        }

        // Display the clip thumbnail for the question
        Texture2D tempTexture2D = _songThumbnails[QuizGameManager.Instance.CurrentQuizQuestionId];
        songClipThumbnail.sprite = Sprite.Create(tempTexture2D, new Rect(0f, 0f, tempTexture2D.width, tempTexture2D.height), new Vector2(0.5f, 0.5f), 100);

        // TODO Update choice that was selected and go through the next question
    }

    private void PlayCurrentQuestionSongClip()
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.clip = _songClips[QuizGameManager.Instance.CurrentQuizQuestionId];
        audioSource.Play();
    }
}