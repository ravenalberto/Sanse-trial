using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
	public static ChoiceManager Instance;

	public bool ravenRed;
	public bool darleneRed;
	public bool kuhRed;
	public bool cristelRed;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}