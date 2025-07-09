using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultyDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private void Start()
    {
        // Set dropdown value based on saved difficulty in GameSessionData
        if (GameSessionData.Instance != null)
        {
            dropdown.value = (int)GameSessionData.Instance.selectedDifficulty;
        }

        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnDropdownChanged(int index)
    {
        GameSessionData.Instance.selectedDifficulty = (GameSessionData.Difficulty)index;
    }
}
