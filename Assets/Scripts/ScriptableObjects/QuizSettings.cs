using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    public int correctPointsOther;
    public int wrongPoints;
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

    public bool shuffleCategories;
    public bool infiniteMode;
    public bool explainRules;
    public bool enableBuzzerServer;
    public string buzzerRoomCode;

    public Quiz() {
        presetName = "";
        buzzerRoomCode = "";
        players = new List<Player>();
        questionTypeSettingsList = new List<QuestionTypeSettings>();
        foreach (QuestionType questionType in Enum.GetValues(typeof(QuestionType)))
        {
            QuestionTypeSettings qts = new QuestionTypeSettings() { type = questionType };
            qts.excludedGens = new List<PokemonGen>();
            questionTypeSettingsList.Add(qts);
        }
    }

    public List<string> GetPlayerNames()
    {
        return players.ConvertAll(player => player.name);
    }
}

[CreateAssetMenu(fileName = "QuizSettings", menuName = "QuizSettings", order = 1)]
public class QuizSettings : ScriptableObject
{
    public int selectedQuiz = 0;
    public List<Quiz> quizzes;

    public Quiz quiz
    {
        get
        {
            return (0 <= selectedQuiz && selectedQuiz < quizzes.Count) ? quizzes[selectedQuiz] : null;
        }
    }

    public List<string> GetQuizNames()
    {
        return quizzes.ConvertAll(quiz => quiz.presetName);
    }

    public Quiz GetNewSelectedQuiz(int index)
    {
        selectedQuiz = index;
        return quiz;
    }

    public void CreateNewQuiz()
    {
        Quiz newQuiz = new Quiz();
        quizzes.Insert(0, newQuiz);
        selectedQuiz = 0;
    }

    public string AddQuiz(Quiz quiz, int index = -1)
    {
        int position = index == -1 ? quizzes.Count : index;

        if (quiz.presetName.Length <= 0)
            return "Nicht gespeichert: Quizname darf nicht leer sein!";

        if (GetQuizNames().Contains(quiz.presetName)) 
            return "Nicht gespeichert: Quizname existiert bereits!";

        quizzes.Insert(position, quiz);
        selectedQuiz = position;

        if (index < 0)
            return "Neues Quiz gespeichert!";

        return "Quiz wurde überschrieben!";
    }

    public string DeleteSelectedQuiz()
    {
        string deletedQuizName = quizzes[selectedQuiz].presetName;
        quizzes.RemoveAt(selectedQuiz);
        return $"Quiz '{deletedQuizName}' gelöscht!";
    }

    public string DeleteQuiz(int index)
    {
        if (0 <= selectedQuiz && selectedQuiz < quizzes.Count)
        {
            string deletedQuizName = quizzes[index].presetName;
            quizzes.RemoveAt(index);
            return $"Quiz '{deletedQuizName}' gelöscht!";
        }
        return $"Error: Quiz existiert nicht.";
    }
}
