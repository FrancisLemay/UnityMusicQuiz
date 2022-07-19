using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    // This image component will allow to display the user if the choice was the correct one or not
    public Image choiceValidityOutlineImage;

    public TMPro.TMP_Text choiceText;

    public void OnChoiceButtonPressed()
    {
        // Trigger action OnChoiceSelected and pass the sibling index of the pressed button
        EventManager.OnChoiceSelected.Invoke(transform.GetSiblingIndex());
    }
}
