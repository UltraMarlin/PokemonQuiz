using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = Application.persistentDataPath;
    private string dataFileName = "quiz_settings.json";

    public class QuizSettingsData
    {
        public int selectedQuiz = 0;
        public List<Quiz> quizzes;
    }

    public QuizSettingsData Load()
    {
        Debug.Log("Loading Data!");
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        QuizSettingsData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<QuizSettingsData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        Debug.Log("Loading Done.");
        return loadedData;
    }

    public void Save(QuizSettings settings)
    {
        Debug.Log("Saving Data!");

        QuizSettingsData data = new QuizSettingsData();
        data.selectedQuiz = settings.selectedQuiz;
        data.quizzes = settings.quizzes;

        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(data, true);

            // write the serialized data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
        Debug.Log("Saving Done.");
    }
}