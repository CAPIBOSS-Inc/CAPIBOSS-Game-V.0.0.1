using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get; private set; }
   [SerializeField] private Theme[] themes;
   private Theme currentTheme; // Tema selecionado pelo jogador

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persiste entre as cenas
    }

    public Theme[] GetThemes()
    {
        return themes;
    }

    public void SetCurrentTheme(Theme selectedTheme)
    {
        if (selectedTheme != null)
        {
            currentTheme = selectedTheme;
            Debug.Log("Tema selecionado: " + currentTheme.themeName);
        }
        else
        {
            Debug.LogError("Tema inválido!");
        }
    }

    public Theme GetCurrentTheme()
    {
        return currentTheme;
    }
}
