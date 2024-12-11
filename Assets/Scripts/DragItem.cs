using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup; // Usado para controlar a visibilidade durante o arraste
    private RectTransform rectTransform; // Transformação do item
    private Transform originalParent; // Parent original para restaurar se necessário
    private bool isDragging; // Indica se o item está sendo arrastado

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Iniciando arraste");
        isDragging = true; // Marca que o arraste começou
        originalParent = transform.parent; // Salva o parent atual
        transform.SetParent(transform.root); // Move o item para o nível superior na hierarquia visual
        canvasGroup.blocksRaycasts = false; // Desativa o bloqueio de raycasts para permitir que o item seja arrastado
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Arrastando");
        if (isDragging)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform.parent, 
                eventData.position, 
                eventData.pressEventCamera, 
                out localPoint);
            rectTransform.anchoredPosition = localPoint; // Atualiza a posição do objeto arrastável
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Finalizando arraste");
        isDragging = false; // Marca que o arraste terminou
        canvasGroup.blocksRaycasts = true; // Restaura a capacidade de receber cliques

        // Verifica se o objeto foi solto em um espaço válido
        if (eventData.pointerEnter != null)
        {
            // Aqui você pode adicionar lógica para verificar se o objeto foi solto em um slot ou área válida
            // Por exemplo, se você tiver um slot, você pode verificar se o slot aceita o item
        }
        else
        {
            // Retorna ao parent original se o arraste foi inválido
            transform.SetParent(originalParent);
        }
    }
}

