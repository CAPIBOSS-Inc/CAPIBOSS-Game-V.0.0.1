using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] 
    private string levelGameName1;

    [SerializeField] 
    private string levelGameName2;

    [SerializeField]
    private GameObject MainMenuPanel;

    [SerializeField]
    private GameObject OptionsPanel;

    public void StoryMode()
    {
        SceneManager.LoadScene(levelGameName1);
    }

    public void QuizzMode()
    {
        SceneManager.LoadScene(levelGameName2);
    }

    public void OpenOptions()
    {
        MainMenuPanel.SetActive(false);
        OptionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        OptionsPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Saiu do Jogo!");
        Application.Quit();
    }
}
