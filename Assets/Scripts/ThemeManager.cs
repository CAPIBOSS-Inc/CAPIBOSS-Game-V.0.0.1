using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    [SerializeField] private GameObject themeSelectionCanvas; // Canvas de seleção de temas
    [SerializeField] private GameObject gameplayCanvas;      // Canvas de gameplay
    [SerializeField] private UIManager uiManager;            // Referência ao UIManager
    [SerializeField] private Theme[] themes;      // Array com os ScriptableObjects dos temas
    [SerializeField] private Theme economicsBasicsTheme; // ScriptableObject do tema "Economics Basics"
    [SerializeField] private Theme householdEconomyTheme; // ScriptableObject do tema "Household Economy"
    [SerializeField] private Theme financialBasicsTheme; // ScriptableObject do tema "Financial Basics"


    private void Start()
    {
        // Inicialmente, o Canvas de temas está ativo, e o de gameplay está desativado
        themeSelectionCanvas.SetActive(true);
        gameplayCanvas.SetActive(false);
    }

    public void SelectTheme(int themeIndex)
    {
        if (themeIndex >= 0 && themeIndex < themes.Length)
        {
            Theme selectedTheme = themes[themeIndex];
            uiManager.LoadThemeQuestions(selectedTheme); // Envia o tema selecionado para o UIManager
        }
        else
        {
            Debug.LogError("Índice de tema inválido!");
        }
    }

    /// <summary>
    /// Método chamado pelo botão para carregar o tema "Economics Basics".
    /// </summary>
    public void SelectEconomicsBasics()
    {
        uiManager.LoadThemeQuestions(economicsBasicsTheme);
        Debug.Log("Tema Economia e Finanças Básica");
    }

    /// <summary>
    /// Método chamado pelo botão para carregar o tema "Household Economy".
    /// </summary>
    public void SelectHouseholdEconomy()
    {
        uiManager.LoadThemeQuestions(householdEconomyTheme);
    }

    /// <summary>
    /// Método chamado pelo botão para carregar o tema "Financial Basics".
    /// </summary>
    public void SelectFinancialBasics()
    {
        uiManager.LoadThemeQuestions(financialBasicsTheme);
    }
}
