using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnagramQuestionDB", menuName = "QuestionDatabases/Anagram", order = 4)]
public class AnagramQuestionDB : ScriptableObject
{
    public List<AnagramQuestion> questions = new List<AnagramQuestion>();
}
