using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollisionController : MonoBehaviour
{
    public GameObject gameOverPanel;
    public CanvasGroup gameOverCanvasGroup; 
    public float fadeDuration = 1.0f;

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 0;
            gameOverCanvasGroup.interactable = false; 
            gameOverCanvasGroup.blocksRaycasts = false;
        }
        Time.timeScale = 1f; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mule"))
        {
            GameOver();
        }
    }

    void GameOver()
    {

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); 
        }

        if (gameOverCanvasGroup != null)
        {
            StartCoroutine(FadeInGameOverScreen());
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    IEnumerator FadeInGameOverScreen()
    {
        float timer = 0f;
        float startAlpha = gameOverCanvasGroup.alpha;
        float targetAlpha = 1.0f; 

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            gameOverCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }
        gameOverCanvasGroup.alpha = targetAlpha;

        Time.timeScale = 0f; 
        gameOverCanvasGroup.interactable = true; 
        gameOverCanvasGroup.blocksRaycasts = true; 
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
