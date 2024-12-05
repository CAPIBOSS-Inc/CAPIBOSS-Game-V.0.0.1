using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DragAndDropManager : MonoBehaviour
{
   [SerializeField] private Transform dragDropParent; // Área onde os elementos serão instanciados
   [SerializeField] private GameObject dragDropPrefab; // Prefab dos itens arrastáveis
   private string[] correctMatches;
   private List<string> playerMatches = new List<string>();

   /// <summary>
   /// Configura o Drag and Drop com os dados da pergunta
   /// </summary>
   
    //Cria novos itens Drag and Drop
    public void SetupDragAndDrop(Question question)
    {
        // Limpa elementos antigos
        foreach (Transform child in dragDropParent)
        {
            Destroy(child.gameObject);
        }

        correctMatches = question.CorrectMatches;
        playerMatches.Clear();

        foreach (string option in question.Options)
        {
            GameObject newDragItem = Instantiate(dragDropPrefab, dragDropParent);
            newDragItem.GetComponentInChildren<TextMeshProUGUI>().text = option;

            // Configura a lógica de arrastar e soltar
            var dragItem = newDragItem.GetComponent<DragItem>();
            dragItem.Initialize(option, this);
        }
    }

    /// <summary>
    /// Adiciona um match feito pelo jogador.
    /// </summary>
    public void AddPlayerMatch(string match)
    {
        playerMatches.Add(match);
    }

    /// <summary>
    /// Verifica se as configurações do jogador estão corretas.
    /// </summary>
    public bool VerifyMatches()
    {
        if (playerMatches.Count != correctMatches.Length) return false;

        for (int i=0; i < correctMatches.Length; i++)
        {
            if (!playerMatches.Contains(correctMatches[i]))
            {
                return false;
            }
        }

        return true;
    }
}
