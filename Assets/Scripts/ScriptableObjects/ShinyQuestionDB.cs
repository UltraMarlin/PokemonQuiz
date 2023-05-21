using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShinyQuestionDB", menuName = "QuestionDatabases/Shiny", order = 2)]
public class ShinyQuestionDB : ScriptableObject
{
    public List<ShinyQuestion> questions = new List<ShinyQuestion>();
}
