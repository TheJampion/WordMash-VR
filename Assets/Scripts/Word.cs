using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WordCategory
{
    Fruits, Animals
}
[CreateAssetMenu(fileName = "Word", menuName = "Create Word")]
public class Word : ScriptableObject
{
    public Sprite Icon;
    public string englishWord;
    public string spanishWord;
}
