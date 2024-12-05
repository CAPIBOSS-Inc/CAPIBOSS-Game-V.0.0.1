using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Texto da Pergunta")]
    [SerializeField] private TextMeshProUGUI questionText; // Texto da pergunta principal

    [Header("Painéis de Exercícios")]
    [SerializeField] private GameObject multipleChoicePanel; // Painel de múltipla escolha
    [SerializeField] private GameObject trueFalsePanel;      // Painel de verdadeiro ou falso
    [SerializeField] private GameObject fillInTheBlankPanel; // Painel de preencher a lacuna
    [SerializeField] private GameObject dragAndDropPanel;    // Painel de arrastar e soltar

    [Header("Múltipla Escolha")]
    [SerializeField] private Button[] multipleChoiceButtons; // Botões de múltipla escolha

    [Header("Verdadeiro ou Falso")]
    [SerializeField] private Button[] trueFalseButtons; // Botões de verdadeiro ou falso

    [Header("Preencher a Lacuna")]
    [SerializeField] private TMP_InputField fillInTheBlankField;    // Campo de entrada para preencher a lacuna
    [SerializeField] private Transform fillInTheBlankPanelContainer; // Container para elementos da lacuna
    [SerializeField] private GameObject draggablePrefab;            // Prefab para palavras arrastáveis (lacuna)

    [Header("Arrastar e Soltar")]
    [SerializeField] private Transform dragAndDropPanelContainer; // Container principal do painel de arrastar e soltar
    [SerializeField] private GameObject slotPrefab;               // Prefab dos slots de destino
    [SerializeField] private GameObject draggableWordPrefab;
    public Slot[] Slots;     // Prefab das palavras arrastáveis

    [Header("Configurações Gerais")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private GameObject themesSelectionCanvas;   // Canvas de seleção de temas
    [SerializeField] private GameObject gameplayCanvas;          // Canvas de gameplay

    [Header("Referências de UI")]
    [SerializeField] private ScoreManager scoreManager;

    private Question[] currentQuestions; // Perguntas carregadas
    private int currentQuestionIndex;    // Índice da pergunta atual
    private Coroutine feedbackCoroutine; // Gerencia o feedback
    private string correctAnswer;        // Resposta correta

    public void LoadThemeQuestions(Theme theme)
    {
        if (theme == null || theme.questions == null || theme.questions.Length == 0)
        {
            Debug.LogError("O tema selecionado não possui perguntas ou está nulo!");
            return;
        }

        themesSelectionCanvas.SetActive(false); // Oculta o canvas de seleção de temas
        gameplayCanvas.SetActive(true);        // Ativa o canvas de gameplay
        currentQuestions = theme.questions;    // Carrega as perguntas
        currentQuestionIndex = 0;              // Reseta o índice
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        if (currentQuestionIndex >= currentQuestions.Length)
        {
            Debug.Log("Fim das perguntas!");
            scoreManager.DisplayResults();    // Exibe o resultado ao final do quiz
            return;
        }

        Question question = currentQuestions[currentQuestionIndex];
        questionText.text = question.QuestionText;
        DisableAllPanels();

        switch (question.Type)
        {
            case Question.QuestionType.MultipleChoice:
                ShowMultipleChoice(question);
                break;
            case Question.QuestionType.TrueFalse:
                ShowTrueFalse(question);
                break;
            case Question.QuestionType.FillInTheBlank:
                ShowFillInTheBlank(question);
                break;
            case Question.QuestionType.DragAndDrop:
                ShowDragAndDrop(question);
                break;
        }
    }

    private void ShowMultipleChoice(Question question)
    {
        multipleChoicePanel.SetActive(true);
        for (int i = 0; i < multipleChoiceButtons.Length; i++)
        {
            if (i < question.Options.Length)
            {
                multipleChoiceButtons[i].gameObject.SetActive(true);
                multipleChoiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.Options[i];

                // Adiciona um Listener de clique para cada botão
                int index = i;  // Captura o índice da opção
                multipleChoiceButtons[i].onClick.RemoveAllListeners(); // Limpa os listeners antigos
                multipleChoiceButtons[i].onClick.AddListener(() => OnAnswerSelected(index, question));
            }
            else
            {
                multipleChoiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ShowTrueFalse(Question question)
    {
        trueFalsePanel.SetActive(true);
        trueFalseButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = "Verdadeiro";
        trueFalseButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = "Falso";
    }

    private void ShowFillInTheBlank(Question question)
    {
        fillInTheBlankPanel.SetActive(true);

        foreach (string option in question.Options)
        {
            GameObject draggable = Instantiate(draggablePrefab, fillInTheBlankPanelContainer);
            draggable.GetComponent<DraggableWord>().Initialize(option, fillInTheBlankField.transform, true);
        }
    }

    private void ShowDragAndDrop(Question question)
    {
        dragAndDropPanel.SetActive(true);

        // Limpa os slots e itens arrastáveis anteriores
        foreach (Transform child in dragAndDropPanelContainer)
        {
            Destroy(child.gameObject);
        }

        // Criação dos slots e arrastáveis
        for (int i = 0; i < question.Options.Length; i++)
        {
            // Cria um slot para cada opção
            GameObject slot = Instantiate(slotPrefab, dragAndDropPanelContainer);
            Slot slotScript = slot.GetComponent<Slot>();

            if (slotScript != null)
            {
                // Configura o texto ou a identificação do slot
                slotScript.CorrectWordOrPhrase = question.Options[i];
            }
            else
            {
                Debug.LogWarning("O prefab do Slot não tem o script Slot anexado.");
                continue;
            }

            // Para cada Option, cria os itens arrastáveis correspondentes
            foreach (string match in question.CorrectMatches)
            {
                // Verifica se o match pertence à Option atual
                if (match == question.Options[i]) // Ajuste a lógica caso o critério seja diferente
                {
                    GameObject draggable = Instantiate(draggableWordPrefab, dragAndDropPanelContainer);
                    DraggableWord draggableScript = draggable.GetComponent<DraggableWord>();

                    if (draggableScript != null)
                    {
                        // Inicializa o item arrastável com o texto correto
                        draggableScript.Initialize(match, slot.transform);
                    }
                    else
                    {
                        Debug.LogWarning("O prefab do DraggableWord não tem o script DraggableWord anexado.");
                    }
                }
            }
        }
    }

    private void OnAnswerSelected(int selectedOptionIndex, Question question)
    {
        bool isCorrect = (selectedOptionIndex == question.CorrectOptionIndex);

        // Atualiza a pontuação
        if (isCorrect)
        {
            Debug.Log("Resposta correta!");
            scoreManager.AddCorrectAnswer();
        }
        else
        {
            Debug.Log("Resposta errada.");
            scoreManager.AddIncorrectAnswer();
        }

        // Chama a função para ir para a próxima pergunta
        NextQuestion();
    }

    public void ValidateAnswers()
    {
        bool allCorrect = true;

        foreach (var slot in Slots)
        {
            if (!slot.ValidateSlot())
            {
                allCorrect = false;
            }
        }

        if (allCorrect)
        {
            Debug.Log("Todas as respostas estão corretas!");
            scoreManager.AddCorrectAnswer(); // Incrementa acerto
        }
        else
        {
            Debug.Log("Existem respostas incorretas. Tente novamente.");
            scoreManager.AddIncorrectAnswer(); // Incrementa erro
        }

        // Chama a próxima pergunta após a validação
        NextQuestion();
    }

    public void NextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex < currentQuestions.Length)
        {
            ShowQuestion();
        }
        else
        {
            Debug.Log("Fim das perguntas!");
            scoreManager.DisplayResults(); // Exibe o resultado ao final do quiz
        }
    }

    public void GoToNextTheme()
    {
        // Lógica para carregar o próximo tema
        scoreManager.ResetScore(); // Reseta a pontuação para o próximo tema
        // Carregar o próximo tema
    }

    public void GoBackToThemeSelection()
    {
        // Lógica para voltar à tela de seleção de temas
        scoreManager.ResetScore(); // Reseta a pontuação
        themesSelectionCanvas.SetActive(true); // Volta para a tela de seleção
        gameplayCanvas.SetActive(false); // Oculta a tela de gameplay
    }

    private void DisableAllPanels()
    {
        multipleChoicePanel.SetActive(false);
        trueFalsePanel.SetActive(false);
        fillInTheBlankPanel.SetActive(false);
        dragAndDropPanel.SetActive(false);
    }

    public bool AllSlotsFilled()
    {
        foreach (Transform child in dragAndDropPanelContainer)
        {
            if (child.CompareTag("Slot") && child.childCount == 0)
            {
                return false; // Slot ainda está vazio
            }
        }
        return true; // Todos os slots preenchidos
    }
}

