using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public List<QuestionAndAnswers> QnA;
    public GameObject[] optioins;
    public int currentQuestion;

    public Text QuestionText;

    private void Start()
    {
        makeQuestion();
    }

    void makeQuestion()
    {
        currentQuestion = Random.Range(0, QnA.Count);
        QuestionText.text = QnA[currentQuestion].Question;
        SetAnswers();
        QnA.RemoveAt(currentQuestion);
    }

    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].transform.Getchild(0).GetComponent<QuestionText>().text = QnA[currentQuestion].Answers[i];

            if (QnA[currentQuestion].CorrectAnswer == i + 1)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = true;
            }

        }
    }

    public void correct()
    {
        QnA.RemoveAt(currentQuestion);
        makeQuestion
    }

    public void wrong()
    {

    }



}