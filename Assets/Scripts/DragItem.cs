using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI text;  // Referência ao texto exibido no item
    private CanvasGroup canvasGroup;               // Usado para controlar a visibilidade durante o arraste
    private RectTransform rectTransform;           // Transformação do item
    private Transform originalParent;              // Parent original para restaurar se necessário
    private string itemText;                       // Texto associado ao item
    private DragAndDropManager dragManager;        // Referência ao gerenciador de Drag and Drop

    /// <summary>
    /// Inicializa o item com texto e uma referência ao gerenciador de Drag and Drop.
    /// </summary>
    public void Initialize(string text, DragAndDropManager manager)
    {
        this.itemText = text;
        this.dragManager = manager;
        if (this.text != null)
            this.text.text = text;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;  // Permite que o item seja ignorado por objetos abaixo
        originalParent = transform.parent;  // Salva o parent atual
        transform.SetParent(dragManager.transform);  // Move o item para o topo na hierarquia visual
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / dragManager.GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Verifica se o item foi solto em um alvo válido
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("DropTarget"))
        {
            // Move para o novo parent e registra o match no DragAndDropManager
            transform.SetParent(eventData.pointerEnter.transform);
            dragManager.AddPlayerMatch(itemText);
        }
        else
        {
            // Retorna ao parent original se o arraste foi inválido
            transform.SetParent(originalParent);
        }
    }
}

