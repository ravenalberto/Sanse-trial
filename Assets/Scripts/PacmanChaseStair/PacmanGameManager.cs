using UnityEngine;
using UnityEngine.SceneManagement;

public class PacmanGameManager : MonoBehaviour
{
	public static PacmanGameManager Instance;

	public int lives = 3;

	public GameObject gameOverUI;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	} 
	public void PlayerHit()
	{
		lives--;

		Debug.Log("Lives left: " + lives);

		if (lives > 0)
		{
			ShowRetry();
		}
		else
		{
			FailSequence();
		}
	}

	void ShowRetry()
	{
		Time.timeScale = 0f;
		gameOverUI.SetActive(true);
	}

	public void Retry()
	{
		Time.timeScale = 1f;
		gameOverUI.SetActive(false);

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

    void FailSequence()
    {
        Time.timeScale = 1f;

        // Connect to your story progression state
        PuzzleState.scene3Result = "Lose";

        // Reset the transition flag so Scene02 starts from the beginning instead of the rooftop
        Scene02.CameFromPacman = false;

        // Reload Scene02 from the beginning
        SceneManager.LoadScene("Scene02");
    }

    // --- ESCAPE SUCCESS / WIN SEQUENCE ---
    // Trigger this function when the player successfully reaches the exit/wins the Pacman chase
    public void WinSequence()
    {
        Time.timeScale = 1f;

        // 1. Alert Scene02 that we are returning from the chase
        Scene02.CameFromPacman = true;

        // 2. Load Scene02 to continue the story from the rooftop
        SceneManager.LoadScene("Scene02");
    }
}