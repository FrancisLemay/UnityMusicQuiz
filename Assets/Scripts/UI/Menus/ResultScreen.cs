using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    public RectTransform background = null;

    public TMP_Text titleText = null;
    public TMP_Text totalScoreText = null;

    public ScrollRect choiceResultScrollRect = null;

    public GameObject choiceResultPrefab = null;

    private void Awake()
    {
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
            case QuizGameManager.QuizGameState.ResultScreen:
                ShowResults();
                break;
            default:
                break;
        }
    }

    private void ShowResults()
    {
        CreateChoiceResults();

        if (background != null)
        {
            background.transform.localScale = Vector3.zero;

            background.gameObject.SetActive(true);

            background.transform.DOScale(1, 0.5f);
        }
    }

    private void CreateChoiceResults()
    {
        if (choiceResultScrollRect.content.childCount > 0)
        {
            foreach (Transform transform in choiceResultScrollRect.content)
            {
                Destroy(transform.gameObject);
            }
        }

        int correctAnswersAmount = 0;
        // Create a choice result gameobject for each choices of the quiz and display if it was right or wront using the outline
        for (int i = 0; i < QuizGameManager.Instance.CurrentQuizAnswersIds.Count; i++)
        {
            GameObject temp = Instantiate(choiceResultPrefab, choiceResultScrollRect.content);
            Question tempQuestion = QuizGameManager.Instance.CurrentQuizPlaylist.questions[i];
            int tempAnswerId = QuizGameManager.Instance.CurrentQuizAnswersIds[i];
            int tempCorrectAnswerId = tempQuestion.answerIndex;

            if (tempAnswerId == tempCorrectAnswerId)
            {
                correctAnswersAmount++;
            }

            temp.TryGetComponent(out ChoiceButton tempChoiceResult);

            tempChoiceResult.choiceValidityOutlineImage.color = tempAnswerId == tempCorrectAnswerId ? QuizGameManager.Instance.validChoiceColor : QuizGameManager.Instance.invalidChoiceColor;

            tempChoiceResult.choiceText.text = "Question " + (i + 1) + " | " + tempQuestion.choices[tempAnswerId].artist + " / " + tempQuestion.choices[tempAnswerId].title;
        }

        choiceResultScrollRect.DONormalizedPos(Vector2.one, 0.1f);

        totalScoreText.text = "Congratulations! Your total score is: " + correctAnswersAmount + "/" + QuizGameManager.Instance.CurrentQuizPlaylist.questions.Length;
    }

    public void CloseResultScreen()
    {
        if (background != null)
        {
            background.transform.DOScale(0, 0.5f);
        }

        EventManager.OnQuizResultsClosed.Invoke();
    }
}
