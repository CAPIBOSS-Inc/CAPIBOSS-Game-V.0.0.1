using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DraggableWord : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private Transform assignedSlot;
    private bool isForBlank; // Indica se é para FillInTheBlank
    private UIManager uiManager;
    public string WordOrPhrase; // Palavra ou frase associada a este arrastável
    private bool isDragging;


    public void Initialize(string text, Transform slot, bool isForBlankExercise = false)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = text; // Define o texto
        assignedSlot = slot; // Define o slot associado
        isForBlank = isForBlankExercise; // Define se é exercício de FillInTheBlank
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform.parent, 
                mousePosition, 
                null, 
                out localPoint);
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Iniciando o arraste");
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Arrastando");
        // Converte a posição do mouse para coordenadas locais do Canvas
        if (isDragging)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform.parent, 
                eventData.position, 
                eventData.pressEventCamera, 
                out localPoint);

            // Atualiza a posição do objeto arrastável
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Finalizando o arraste");
        canvasGroup.blocksRaycasts = true; // Restaura a capacidade de receber cliques

        // Verifica se o objeto foi solto em um espaço válido
        if (eventData.pointerEnter != null)
        {
            // Verifica se o objeto foi solto em um marcador
            if (eventData.pointerEnter.CompareTag("DraggableWord"))
            {
                // Aqui você pode adicionar a lógica para substituir o marcador
                // Por exemplo, você pode destruir este objeto ou desativá-lo
                Destroy(gameObject); // Remove o objeto arrastável
            }
        }
        else
        {
            // Retorna ao parent original se o arraste foi inválido
            transform.SetParent(originalParent);
        }
    }

    private void ValidateFillInTheBlank(InputField inputField)
    {
        // Lógica de validação para FillInTheBlank (já implementada anteriormente)
        bool isCorrect = inputField.text == WordOrPhrase;
        ProvideFeedback(isCorrect);
        inputField.text = "";
    }

    private void ValidateDragAndDrop(Slot slot)
    {
        // Lógica de validação para DragAndDrop
        bool isCorrect = slot.ValidateWord(WordOrPhrase); // Supondo que o Slot tenha um método ValidateWord
        ProvideFeedback(isCorrect);

        // Opcional: Remover o objeto arrastável do Slot após a validação
        if (isCorrect)
        {
            Destroy(gameObject);
        }
    }

    internal void ProvideFeedback(bool isCorrect)
    {
        // Altera a cor do texto para vermelho ou verde
        GetComponentInChildren<TextMeshProUGUI>().color = isCorrect ? Color.green : Color.red;

        // Mostra o feedback por alguns segundos
        StartCoroutine(ClearFeedback(isCorrect));
    }

    private IEnumerator ClearFeedback(bool isCorrect)
    {
        yield return new WaitForSeconds(2f);

        // Reseta a cor do texto
        GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        if (isCorrect && uiManager.AllSlotsFilled())
        {
            uiManager.NextQuestion(); // Avança para a próxima pergunta se tudo estiver preenchido
        }
    }
}


