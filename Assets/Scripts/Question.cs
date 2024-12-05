using UnityEngine;

[System.Serializable]
public class Question
{
    [SerializeField] private string questionText; // Enunciado da pergunta
    [SerializeField] private string[] options;    // Opções de resposta
    [SerializeField] private int correctOptionIndex;   // Índice da resposta correta
    [SerializeField] private QuestionType type;   // Tipo de exercício
    [SerializeField] private string[] correctMatches;   // Matches para perguntas do tipo drag and drop


    public string QuestionText => questionText;
    public QuestionType Type => type;
    public string[] Options => options;
    public int CorrectOptionIndex => correctOptionIndex;
    public string[] CorrectMatches => correctMatches;

    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse,
        FillInTheBlank,
        DragAndDrop
    }
}
