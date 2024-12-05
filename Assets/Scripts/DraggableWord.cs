using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class DraggableWord : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private Transform assignedSlot;
    private bool isForBlank; // Indica se é para FillInTheBlank
    private UIManager uiManager;
    public string WordOrPhrase; // Palavra ou frase associada a este arrastável


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

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var hit = eventData.pointerEnter?.GetComponent<Slot>();
        if (hit != null)
        {
            transform.SetParent(hit.transform);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.SetParent(originalParent);
        }
    }

    private void ProvideFeedback(bool isCorrect)
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


