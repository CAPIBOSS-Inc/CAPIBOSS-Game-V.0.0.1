using UnityEngine;

[CreateAssetMenu(fileName = "New Theme", menuName = "Quizz/Theme")]
public class Theme : ScriptableObject
{
    public string themeName;        // Nome do tema
    public Question[] questions;    // Lista de perguntas do tema
}
