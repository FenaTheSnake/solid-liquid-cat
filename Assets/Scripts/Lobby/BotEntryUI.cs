using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotEntryUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text difficultyText;
    public Button removeButton;

    public void Initialize(string botName, string difficulty, System.Action onRemove)
    {
        nameText.text = botName;
        difficultyText.text = difficulty;
        removeButton.onClick.AddListener(() => onRemove());
    }
}