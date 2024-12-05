using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private TextMeshProUGUI correctsText; // Texto para mostrar os acertos
    [SerializeField] private TextMeshProUGUI errorsText;   // Texto para mostrar os erros
    [SerializeField] private GameObject resultsCanvas;     // Canvas de resultados
    [SerializeField] private GameObject goodResultsCanvas; // Canvas de bons resultados
    [SerializeField] private GameObject badResultsCanvas;  // Canvas de maus resultados

    private int correctAnswers = 0; // Contador de acertos
    private int incorrectAnswers = 0; // Contador de erros

    public void AddCorrectAnswer()
    {
        correctAnswers++; // Incrementa os acertos
    }

    public void AddIncorrectAnswer()
    {
        incorrectAnswers++; // Incrementa os erros
    }

    public void DisplayResults()
    {
        correctsText.text = $"Acertos: {correctAnswers}"; // Atualiza a quantidade de acertos
        errorsText.text = $"Erros: {incorrectAnswers}";   // Atualiza a quantidade de erros

        // Exibe o canvas de resultados
        resultsCanvas.SetActive(true);

        // Verifica se o jogador atingiu a quantidade de acertos necessária
        if (correctAnswers >= 6)
        {
            goodResultsCanvas.SetActive(true); // Exibe o painel de bons resultados
            badResultsCanvas.SetActive(false); // Oculta o painel de maus resultados
        }
        else
        {
            goodResultsCanvas.SetActive(false); // Oculta o painel de bons resultados
            badResultsCanvas.SetActive(true);   // Exibe o painel de maus resultados
        }
    }

    public void ResetScore()
    {
        correctAnswers = 0;  // Zera os acertos
        incorrectAnswers = 0; // Zera os erros

        // Atualiza os textos para 0
        correctsText.text = "Acertos: 0";
        errorsText.text = "Erros: 0";

        resultsCanvas.SetActive(false); // Oculta a tela de resultados
        goodResultsCanvas.SetActive(false); // Oculta o painel de bons resultados
        badResultsCanvas.SetActive(false); // Oculta o painel de maus resultados
    }

}
