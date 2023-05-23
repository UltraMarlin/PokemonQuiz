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

public class QuizColors
{
    public static List<Color> main = new List<Color>()
    {
        new Color((float)255/255, (float)213/255, (float)75/255),
        new Color((float)255/255, (float)156/255, (float)79/255),
        new Color((float)215/255, (float)91/255, (float)107/255),
        new Color((float)144/255, (float)120/255, (float)208/255),
        new Color((float)107/255, (float)182/255, (float)255/255),
        new Color((float)116/255, (float)222/255, (float)131/255),
        new Color((float)255/255, (float)161/255, (float)212/255),
        new Color((float)255/255, (float)255/255, (float)255/255)
    };
    public static List<Color> dark = new List<Color>()
    {
        new Color((float)214/255, (float)165/255, (float)53/255),
        new Color((float)214/255, (float)114/255, (float)55/255),
        new Color((float)186/255, (float)64/255, (float)79/255),
        new Color((float)113/255, (float)84/255, (float)181/255),
        new Color((float)75/255, (float)142/255, (float)214/255),
        new Color((float)81/255, (float)191/255, (float)100/255),
        new Color((float)214/255, (float)113/255, (float)168/255),
        new Color((float)163/255, (float)163/255, (float)163/255)
    };
    public static List<Color> light = new List<Color>()
    {
        new Color((float)255/255, (float)235/255, (float)167/255),
        new Color((float)255/255, (float)195/255, (float)149/255),
        new Color((float)231/255, (float)156/255, (float)166/255),
        new Color((float)188/255, (float)173/255, (float)227/255),
        new Color((float)166/255, (float)211/255, (float)255/255),
        new Color((float)171/255, (float)235/255, (float)180/255),
        new Color((float)255/255, (float)198/255, (float)229/255),
        new Color((float)230/255, (float)230/255, (float)230/255)
    };
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
