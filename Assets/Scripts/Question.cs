using UnityEngine;

[System.Serializable]
public class Question
{
    [SerializeField] private string questionText; // Enunciado da pergunta
    [SerializeField] private string fillInTheBlankText; // Texto específico para FillInTheBlank
    [SerializeField] private string[] options;    // Opções de resposta
    [SerializeField] private int correctOptionIndex;   // Índice da resposta correta
    [SerializeField] private QuestionType type;   // Tipo de exercício
    [SerializeField] private string[] correctMatches;   // Matches para perguntas do tipo drag and drop

    [SerializeField] private GameObject inputFieldPrefab; // Prefab do InputField para as lacunas
    [SerializeField] private GameObject draggablePrefab; // Prefab das opções arrastáveis


    public string QuestionText => questionText;
    public string FillInTheBlankText => fillInTheBlankText; 
    public QuestionType Type => type;
    public string[] Options => options;
    public int CorrectOptionIndex => correctOptionIndex;
    public string[] CorrectMatches => correctMatches;

    public GameObject InputFieldPrefab => inputFieldPrefab;
    public GameObject DraggablePrefab => draggablePrefab; 

    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse,
        FillInTheBlank,
        DragAndDrop
    }
}
