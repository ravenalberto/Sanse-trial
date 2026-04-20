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

		// 👇 connect to your story system
		PuzzleState.scene3Result = "Lose";

		SceneManager.LoadScene("StaircaseScene01");
		// or wherever your bad path continues
	}
}