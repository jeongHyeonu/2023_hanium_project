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
    public Text ScoreText;

    public GameObject QuizPanel;
    public GameObject GoPanel;

    int totalQuestion = 0;
    public int score;

    private void Start()
    {
        totalQuestion = QnA.Count;
        GoPanel.SetActive(false);
        makeQuestion();
    }

    void makeQuestion()
    {
        if(QnA.Count > 0)
        {
            currentQuestion = Random.Range(0, QnA.Count);
            QuestionText.text = QnA[currentQuestion].Question;
            SetAnswers();
        }
        else
        {
            Debug.Log("문제를 다 풀었습니다")
                GameOver();
        }
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
        score += 1;
        QnA.RemoveAt(currentQuestion);
        makeQuestion();
    }

    public void wrong()
    {
        QnA.RemoveAt(currentQuestion);
        makeQuestion();
    }

    void GameOver()
    {
        QuizPanel.SetActive(false);
        GoPanel.SetActive(true);
        ScoreText.text = "Your Score is " + score + "/15.";
    }

    public void retry()
    {
        SceneManager.LoadScene(SceneManager.Get)
    }

}