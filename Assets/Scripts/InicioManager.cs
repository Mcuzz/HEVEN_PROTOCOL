using UnityEngine;
using UnityEngine.SceneManagement;

public class InicioManager : MonoBehaviour
{
    public void IniciarJuego()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("SampleScene");
    }
}