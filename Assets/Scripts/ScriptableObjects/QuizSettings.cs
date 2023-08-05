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
    public List<PokemonGen> excludedGens;
}

[System.Serializable]
public enum PlayerColor
{
    Yellow,
    Orange,
    Red,
    Pink,
    Purple,
    Blue,
    Green,
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
    public bool explainRules;

    public Quiz() {
        questionTypeSettingsList = new List<QuestionTypeSettings>();
        foreach (QuestionType questionType in Enum.GetValues(typeof(QuestionType)))
        {
            QuestionTypeSettings qts = new QuestionTypeSettings() { type = questionType };
            questionTypeSettingsList.Add(qts);
        }
    }
}

[CreateAssetMenu(fileName = "QuizSettings", menuName = "QuizSettings", order = 1)]
public class QuizSettings : ScriptableObject
{
    public Quiz quiz;
}
