using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void ChangeScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SelectActiveButton(GameObject firstSelectedButton)
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

    }
}