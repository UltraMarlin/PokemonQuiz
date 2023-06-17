using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionDB", menuName = "QuestionDB", order = 4)]
public class QuestionDB : ScriptableObject
{
    public List<IQuestion> questions = new List<IQuestion>();
}
