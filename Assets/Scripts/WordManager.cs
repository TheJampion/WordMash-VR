using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public List<Word> wordDatabase;
    public GameObject wordPrefab;
    public List<GameObject> activeWords;
    public List<Word> correctWords;
    [SerializeField] private float wordRadius = 1.5f;
    [SerializeField] private float wordSpeed = 1.5f;
    private static WordManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        GameManager.startGame += SpawnWords;
        GameManager.stopGame += DestroyWords;

        WordInteractable.CompletedWordAction += AddCorrectWord;
    }

    public void SpawnWords(GameMode mode)
    {
        wordDatabase.ForEach(word =>
        {
            Vector3 randomPos1 = Vector3.forward * 1f + (Vector3)(wordRadius * Random.insideUnitCircle);
            Vector3 randomPos2 = Vector3.forward * 1f + (Vector3)(wordRadius * Random.insideUnitCircle);
            Vector3 randomForce1 = wordSpeed * Random.insideUnitSphere;
            Vector3 randomForce2 = wordSpeed * Random.insideUnitSphere;
            WordInteractable word1 = Instantiate(wordPrefab, randomPos1, Quaternion.identity).GetComponent<WordInteractable>();
            WordInteractable word2 = Instantiate(wordPrefab, randomPos2, Quaternion.identity).GetComponent<WordInteractable>();
            activeWords.Add(word1.gameObject);
            activeWords.Add(word2.gameObject);

            SetupWord(word, true, word1, mode);
            SetupWord(word, false, word2, mode);

            word1.wordData = word;
            word2.wordData = word;
            word1.rb.AddForce(randomForce1, ForceMode.Impulse);
            word2.rb.AddForce(randomForce2, ForceMode.Impulse);
        });
    }

    public void DestroyWords(GameMode mode)
    {
        foreach (GameObject word in activeWords)
        {
            Destroy(word);
        }
        activeWords.Clear();
        correctWords.Clear();
    }
    private void SetupWord(Word word, bool isEnglish, WordInteractable wordInteractable, GameMode mode)
    {
        if (isEnglish)
        {
            wordInteractable.textMeshPro.text = word.englishWord;
        }
        else
        {
            wordInteractable.textMeshPro.text = word.spanishWord;
        }
        wordInteractable.spriteRenderer.sprite = word.Icon;
        wordInteractable.grabIcon.SetActive(false);

        if (mode == GameMode.Text)
        {
            wordInteractable.spriteRenderer.sprite = null;
        }
        else if (mode == GameMode.Icon)
        {
            if (isEnglish)
            {
                wordInteractable.textMeshPro.text = "";
                wordInteractable.grabIcon.SetActive(true);
            }
            else
            {
                wordInteractable.spriteRenderer.sprite = null;
            }
        }
    }

    public void AddCorrectWord(WordInteractable word, WordInteractable otherWord)
    {
        //Need to sort lists before comparing
        if (!correctWords.Contains(word.wordData))
        {
            correctWords.Add(word.wordData);
        }
        if (activeWords.Contains(word.gameObject))
        {
            activeWords.Remove(word.gameObject);
        }
        if (activeWords.Contains(otherWord.gameObject))
        {
            activeWords.Remove(otherWord.gameObject);
        }
        if (wordDatabase.Count == correctWords.Count)
        {
            GameManager.instance.EndGame();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.forward * 1f, wordRadius);
    }
}
