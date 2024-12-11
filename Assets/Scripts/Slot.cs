using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public string CorrectWordOrPhrase; // Palavra ou frase correta para este slot
    private List<GameObject> draggedItems = new List<GameObject>(); // Lista de objetos arrastados para o slot

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Draggable"))
        {
            draggedItems.Add(other.gameObject); // Adiciona o arrastável à lista
            Debug.Log($"Objeto {other.name} foi colocado no slot {name}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Draggable"))
        {
            draggedItems.Remove(other.gameObject); // Remove o arrastável da lista
            Debug.Log($"Objeto {other.name} foi removido do slot {name}");
        }
    }

    public bool ValidateSlot()
    {
        // Verifica se todos os itens no slot têm a palavra/frase correta
        foreach (var item in draggedItems)
        {
            var draggable = item.GetComponent<DraggableWord>();
            if (draggable != null && draggable.WordOrPhrase != CorrectWordOrPhrase)
            {
                Debug.Log($"Item {item.name} está incorreto no slot {name}");
                return false;
            }
        }

        Debug.Log($"Slot {name} está correto!");
        return true;
    }

    public bool ValidateWord(string word)
    {
        return word == CorrectWordOrPhrase;
    }

    public void OnDrop(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}

