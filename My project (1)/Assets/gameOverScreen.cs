using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI waveText;

    public void Setup(int wave)
    {
        gameObject.SetActive(true);
        waveText.text = "U SURVIVE TILL WAVE " + wave.ToString() + "!!";
    }
    public void restartButton()
    {
        SceneManager.LoadScene("levelDemo");
    }
    public void quitButton()
    {
        Application.Quit();
    }
}
