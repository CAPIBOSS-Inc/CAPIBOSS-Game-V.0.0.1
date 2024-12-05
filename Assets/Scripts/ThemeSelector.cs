using UnityEngine;
using UnityEngine.SceneManagement;

public class ThemeSelector : MonoBehaviour
{
    [SerializeField] private Theme[] availableThemes; // Temas disponíveis no seletor

    public void SelectTheme(int themeIndex)
    {
        if (themeIndex >= 0 && themeIndex < availableThemes.Length)
        {
            GameManager.Instance.SetCurrentTheme(availableThemes[themeIndex]);
            Debug.Log("Tema " + availableThemes[themeIndex].themeName + " selecionado!");

            // Carregar a cena do jogo
            SceneManager.LoadScene("QuizzModeScene");
        }
        else
        {
            Debug.LogError("Índice de tema inválido!");
        }
    }
}
