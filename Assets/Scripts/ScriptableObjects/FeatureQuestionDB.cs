using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatureQuestionDB", menuName = "QuestionDatabases/Feature", order = 1)]
public class FeatureQuestionDB : ScriptableObject
{
    public List<FeatureQuestion> questions = new List<FeatureQuestion>();
}
