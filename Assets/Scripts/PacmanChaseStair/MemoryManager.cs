using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MemoryManager : MonoBehaviour
{
	[SerializeField] GameObject fadeOut;

	public static MemoryManager Instance;

	int collected = 0;

	void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void CollectOrb(int id)
	{
		collected++;

		if (collected >= 4)
		{
			Debug.Log("All memories collected!");

			StartCoroutine(TransitionToScene());
		}
	}

	IEnumerator TransitionToScene()
	{
		Time.timeScale = 1f; // 👈 ADD THIS

		fadeOut.SetActive(true);

		CanvasGroup fadeCanvas = fadeOut.GetComponent<CanvasGroup>();
		fadeCanvas.alpha = 0;

		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime; // 👈 switch BACK to normal time
			fadeCanvas.alpha = t;
			yield return null;
		}

		yield return new WaitForSeconds(0.5f);

		SceneManager.LoadScene("StaircaseScene01");
	}
}