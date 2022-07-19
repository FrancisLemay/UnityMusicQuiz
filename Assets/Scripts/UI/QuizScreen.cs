using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizScreen : MonoBehaviour
{
    public RectTransform background;

    public TMPro.TMP_Text title;

    public Image songClipThumbnail;

    public AudioSource audioSource;

    public Color validChoiceColor;
    public Color invalidChoiceColor;

    [SerializeField]
    private List<ChoiceButton> _choiceButtons = new List<ChoiceButton>();

    private void Awake()
    {
        EventManager.OnQuizGameStateChanged += OnQuizGameStateChanged;
        EventManager.OnChoiceSelected += OnChoiceSelected;
        EventManager.OnNextQuestionRequested += LoadCurrentQuestion;
    }

    private void OnDestroy()
    {
        EventManager.OnQuizGameStateChanged -= OnQuizGameStateChanged;
        EventManager.OnChoiceSelected -= OnChoiceSelected;
        EventManager.OnNextQuestionRequested -= LoadCurrentQuestion;
    }

    #region Quiz Game State Functions
    private void OnQuizGameStateChanged(QuizGameManager.QuizGameState quizGameState)
    {
        switch (quizGameState)
        {
            case QuizGameManager.QuizGameState.PreparingQuiz:
                OnPreparingQuiz();
                break;
            case QuizGameManager.QuizGameState.QuizScreen:
                OnQuizStarted();
                break;
            case QuizGameManager.QuizGameState.ResultScreen:
                background.transform.DOScale(0, 0.5f).OnComplete(delegate { background.gameObject.SetActive(false); });
                break;
            default:
                break;
        }
    }

    private void OnPreparingQuiz()
    {
        if (background != null)
        {
            background.transform.localScale = Vector3.zero;

            background.gameObject.SetActive(true);

            background.transform.DOScale(1, 0.5f);
        }

        // Update the title text to: Preparing Quiz...
        UpdateQuizTitleText("Preparing Quiz... / " + QuizGameManager.Instance.CurrentQuizPlaylist.playlist);
    }

    private void OnQuizStarted()
    {
        // Update the title text to the playlist name
        UpdateQuizTitleText("Guess the song / " + QuizGameManager.Instance.CurrentQuizPlaylist.playlist);

        LoadCurrentQuestion();
    }

    private void LoadCurrentQuestion()
    {
        DisplaySongThumbnail();

        ApplyChoiceTexts();

        UpdateChoiceValidityOutlineVisibility(false);

        PlayCurrentQuestionSongClip();
    }
    #endregion

    #region Quiz UI Functions
    private void UpdateQuizTitleText(string text)
    {
        if (title == null)
        {
            return;
        }

        title.text = text;
    }

    private void DisplaySongThumbnail(Texture2D texture2D = null)
    {
        songClipThumbnail.sprite = texture2D != null ? Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100) : null;
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
        }
    }

    private void UpdateChoiceValidityOutlineVisibility(bool isVisible)
    {
        if (_choiceButtons == null)
        {
            return;
        }

        Question currentQuestion = QuizGameManager.Instance.CurrentQuizPlaylist.questions[QuizGameManager.Instance.CurrentQuizQuestionId];

        // Make all the choice buttons outlines visible to display which choice was the correct answer.
        for (int i = 0; i < _choiceButtons.Count; i++)
        {
            if (isVisible)
            {
                // update the outline color of the choice button depending if its the right answer choice or not.
                _choiceButtons[i].choiceValidityOutlineImage.color = i == currentQuestion.answerIndex ? validChoiceColor : invalidChoiceColor;
            }

            _choiceButtons[i].choiceValidityOutlineImage.gameObject.SetActive(isVisible);
        }
    }

    private void PlayCurrentQuestionSongClip()
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.clip = QuizAssetsManager.Instance.QuizSongClips[QuizGameManager.Instance.CurrentQuizQuestionId];
        audioSource.Play();
    }

    private void OnChoiceSelected(int choiceIndex)
    {
        // Make all the choice buttons outlines visible to display which choice was the correct answer.
        UpdateChoiceValidityOutlineVisibility(true);

        // Display the song thumbnail for the question
        DisplaySongThumbnail(QuizAssetsManager.Instance.QuizSongThumbnails[QuizGameManager.Instance.CurrentQuizQuestionId]);
    }
    #endregion
}