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
    }

    private void OnDestroy()
    {
        EventManager.OnQuizGameStateChanged -= OnQuizGameStateChanged;
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
        UpdateQuizTitleText("Preparing Quiz...");
    }

    private void OnQuizStarted()
    {
        // Update the title text to the playlist name
        UpdateQuizTitleText("Guess the song | " + QuizGameManager.Instance.CurrentQuizPlaylist.playlist);

        ApplyChoiceTexts();

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
        Texture2D tempTexture2D = QuizAssetsManager.Instance.QuizSongThumbnails[QuizGameManager.Instance.CurrentQuizQuestionId];
        songClipThumbnail.sprite = Sprite.Create(tempTexture2D, new Rect(0f, 0f, tempTexture2D.width, tempTexture2D.height), new Vector2(0.5f, 0.5f), 100);

        // TODO Update choice that was selected and go through the next question
        _choiceButtons[0].transform.GetSiblingIndex();
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
    #endregion
}