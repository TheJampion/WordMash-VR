using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private float scoreValue;

    private void Start()
    {
        WordInteractable.CompletedWordAction += AddScoreForCompletedWord;
    }

    private void AddScoreForCompletedWord(WordInteractable word, WordInteractable otherWord)
    {
        float scoreAttemptValue = (2 - word.attempts) * 10f;
        Mathf.Clamp(scoreAttemptValue, -90, 20);
        scoreValue += 100 + scoreAttemptValue;
        scoreText.text = scoreValue.ToString();
    }
}
