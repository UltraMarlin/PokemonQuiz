using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Player
{
    public string name;
    public PlayerColor color;
}

[System.Serializable]
public struct QuestionTypeSettings
{
    public QuestionType type;
    public int questionAmount;
    public int correctPoints;
    public int wrongPointsOther;
}

[System.Serializable]
public enum PlayerColor
{
    Yellow,
    Orange,
    Red,
    Purple,
    Blue,
    Green,
    Pink,
    White
}

[System.Serializable]
public class Quiz
{
    public string presetName;

    public List<Player> players;

    public List<QuestionTypeSettings> questionTypeSettingsList;

    // TODO: category order if not shuffle

    public bool shuffleCategories;
    public bool infiniteMode;

    public Quiz() {
        questionTypeSettingsList = new List<QuestionTypeSettings>();
        foreach (QuestionType questionType in Enum.GetValues(typeof(QuestionType)))
        {
            questionTypeSettingsList.Add(new QuestionTypeSettings() { type = questionType });
        }
    }
}

[CreateAssetMenu(fileName = "QuizSettings", menuName = "QuizSettings", order = 1)]
public class QuizSettings : ScriptableObject
{
    public Quiz quiz;
}
