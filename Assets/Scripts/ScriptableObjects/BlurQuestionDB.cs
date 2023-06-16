using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlurQuestionDB", menuName = "QuestionDatabases/Blur", order = 3)]
public class BlurQuestionDB : ScriptableObject
{
    public List<BlurQuestion> questions = new List<BlurQuestion>();
}
