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
    [SerializeField] private TextMeshProUGUI questionFillInTheBlank;
    [SerializeField] private Transform fillInTheBlankPanelContainer; // Container para elementos da lacuna
    [SerializeField] private Transform fillInTheBlankQuestionContainer;

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
    private string options;

    private void Start()
    {
        // Certifique-se que questionText não é nulo
        if (questionText == null)
        {
            Debug.LogError("questionText is null. Please check the Inspector.");
            return;
        }

        // Atribuir o texto inicial
        questionText.text = "Texto inicial da pergunta";
    }

    public void LoadThemeQuestions(Theme theme)
    {
        if (theme == null || theme.questions == null || theme.questions.Length == 0)
        {
            Debug.LogError("O tema selecionado não possui perguntas ou está nulo!");
            return;
        }


        themesSelectionCanvas.SetActive(false); // Oculta o canvas de seleção de temas
        gameplayCanvas.SetActive(true);        // Ativa o canvas de gameplay
        questionText.gameObject.SetActive(true);
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

        // Define os textos dos botões
        trueFalseButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = "Verdadeiro";
        trueFalseButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = "Falso";

        // Adiciona os listeners de clique para os botões
        for (int i = 0; i < trueFalseButtons.Length; i++)
        {
            trueFalseButtons[i].onClick.RemoveAllListeners(); // Remove listeners antigos
            int index = i; // Captura o índice do botão
            trueFalseButtons[i].onClick.AddListener(() => OnTrueFalseSelected(index, question));
        }
    }

    private void ShowFillInTheBlank(Question question)
    {
        fillInTheBlankPanel.SetActive(true);

        // 1. Defina o texto da pergunta
        string fillInTheBlankQuestionText = question.FillInTheBlankText;

        if (string.IsNullOrEmpty(fillInTheBlankQuestionText))
        {
            Debug.LogError("Campo de perguntas vazio. Verifique a lógica.");
            return;
        }

        // 2. Substituir o marcador [BLANK] por uma tag `<link>`
        string formattedText = fillInTheBlankQuestionText.Replace("[BLANK]", "<link=\"InputField\">______</link>");
        questionFillInTheBlank.text = formattedText;

        // 3. Habilitar suporte ao RichText
        questionFillInTheBlank.richText = true;

        // 4. Atualizar a renderização do texto
        questionFillInTheBlank.ForceMeshUpdate();

        // 5. Localizar a posição do marcador no texto
        TMP_TextInfo textInfo = questionFillInTheBlank.textInfo;
        int linkIndex = questionFillInTheBlank.text.IndexOf("<link=\"InputField\">");

        if (linkIndex == -1)
        {
            Debug.LogError("Marcador [BLANK] não encontrado no texto formatado.");
            return;
        }

        TMP_CharacterInfo firstChar = textInfo.characterInfo[linkIndex];

        // 6. Ajustar dimensões para corresponder ao espaço do marcador
        float widthBeforeBlank = firstChar.topRight.x - firstChar.bottomLeft.x;

        // 7. Limpa o container de palavras arrastáveis
        foreach (Transform child in fillInTheBlankPanelContainer)
        {
            if (child.CompareTag("DraggableWord")) // Se você tiver palavras arrastáveis
            {
                Destroy(child.gameObject);
            }
        }

        // 8. Cria as DraggableWords
        foreach (string option in question.Options)
        {
            GameObject draggableWordObject = Instantiate(draggableWordPrefab, fillInTheBlankPanelContainer);
            DraggableWord draggableWord = draggableWordObject.GetComponent<DraggableWord>();

            if (draggableWord == null)
            {
                Debug.LogError("DraggableWord component is missing on the DraggableWord prefab.");
                return;
            }

            // Inicializa a DraggableWord com o texto da opção
            draggableWord.Initialize(option, null, true); // O segundo parâmetro pode ser ajustado conforme necessário

            // RectTransform draggableWordRectTransform = draggableWordObject.GetComponent<RectTransform>();
            // Vector3 worldPosition = questionFillInTheBlank.transform.TransformPoint(firstChar.bottomLeft);
            // draggableWordRectTransform.position = worldPosition;

            // // 10. Ajustar a largura da DraggableWord para corresponder ao espaço do marcador
            // draggableWordRectTransform.sizeDelta = new Vector2(widthBeforeBlank, draggableWordRectTransform.sizeDelta.y);
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

    private void OnTrueFalseSelected(int selectedIndex, Question question)
    {
        // Define 0 como "Verdadeiro" e 1 como "Falso"
        bool isCorrect = (selectedIndex == question.CorrectOptionIndex);

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

        // Chama a próxima pergunta
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

    private void CopyTextSettings(TextMeshProUGUI source, TextMeshProUGUI target)
{
    if (source == null || target == null)
    {
        Debug.LogError("Source ou Target está nulo na função CopyTextSettings.");
        return;
    }

    // Copia as propriedades principais
    target.font = source.font;
    target.fontSize = source.fontSize;
    target.color = source.color;
    target.alignment = source.alignment;
    target.textWrappingMode = source.textWrappingMode;
    target.overflowMode = source.overflowMode;

    // Copia margens e espaçamentos, se necessário
    target.margin = source.margin;
    target.characterSpacing = source.characterSpacing;
    target.wordSpacing = source.wordSpacing;
    target.lineSpacing = source.lineSpacing;
    target.paragraphSpacing = source.paragraphSpacing;

    // Habilite/Desabilite efeitos, como sublinhado, itálico, negrito
    target.fontStyle = source.fontStyle;

    // Se precisar copiar mais configurações, adicione aqui
}
}

