using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BlockPuzzleManager : MonoBehaviour
{

	public GameObject cellPrefab;
	public Transform gridParent;

	private GameObject[,] cellObjects;


	public PieceShape[] shapes;

	public PieceShape[] currentShapes = new PieceShape[3];
	public int selectedShapeIndex = -1;

	public Transform[] previewParents; // size = 3


	public Transform previewParent;
	public GameObject previewCellPrefab;

	public int remainingTime = 317; // example: 5:17
	public TMPro.TextMeshProUGUI scoreText;

	public int width = 8;
	public int height = 8;

	public int[,] board;

	public TMPro.TextMeshProUGUI messageText;

	public Image fadePanel;
	public float fadeSpeed = 0.5f;

	bool isFading = false;
	bool winTriggered = false;

	public bool puzzleFinished = false;

	void Start()
	{
		messageText.gameObject.SetActive(false);
		GenerateShapes();

		UpdateTimeUI();

	}

	void UpdateTimeUI()
	{
		int minutes = remainingTime / 60;
		int seconds = remainingTime % 60;



		scoreText.text = $"{minutes:00}:{seconds:00}";
	}

	void GenerateShapes()
	{
		for (int i = 0; i < 3; i++)
		{
			currentShapes[i] = shapes[Random.Range(0, shapes.Length)];
		}

		ShowAllPreviews();
	}

	public void PreviewShape(int baseX, int baseY)
	{
		ClearPreview();

		if (selectedShapeIndex == -1) return;

		PieceShape shape = currentShapes[selectedShapeIndex];

		bool canPlace = CanPlace(shape, baseX, baseY);

		foreach (var offset in shape.offsets)
		{
			int x = baseX + offset.x;
			int y = baseY + offset.y;

			if (x >= 0 && x < width && y >= 0 && y < height)
			{
				var img = cellObjects[x, y].GetComponent<UnityEngine.UI.Image>();

				img.color = canPlace
				? new Color(0.3f, 1f, 0.3f, 1f)
				: new Color(1f, 0.3f, 0.3f, 1f);
			}
		}
	}

	void ShowAllPreviews()
	{
		for (int i = 0; i < previewParents.Length; i++)
		{
			ShowShapeInPreview(currentShapes[i], previewParents[i]);
		}
	}

	public void ClearPreview()
	{
		UpdateGridVisual(); // reset colors
	}

	public void SelectShape(int index)
	{
		selectedShapeIndex = index;

		HighlightSelected();
	}

	public void HighlightSelected()
	{
		for (int i = 0; i < previewParents.Length; i++)
		{
			UnityEngine.UI.Image img = previewParents[i].GetComponent<UnityEngine.UI.Image>();

			if (img != null)
			{
				if (i == selectedShapeIndex)
					img.color = Color.white; // selected
				else
					img.color = new Color(1f, 1f, 1f, 0.4f); // dim others
			}
		}
	}


	void ShowShapeInPreview(PieceShape shape, Transform parent)
	{
		foreach (Transform child in parent)
		{
			Destroy(child.gameObject);
		}

		float cellSize = 30f;
		float spacing = 3f;
		float step = cellSize + spacing;

		Vector2 center = Vector2.zero;

		foreach (var offset in shape.offsets)
			center += offset;

		center /= shape.offsets.Length;

		foreach (var offset in shape.offsets)
		{
			GameObject cell = Instantiate(previewCellPrefab, parent);

			RectTransform rt = cell.GetComponent<RectTransform>();

			rt.sizeDelta = new Vector2(cellSize, cellSize);

			Vector2 centeredOffset = offset - center;

			rt.anchoredPosition = new Vector2(
				centeredOffset.x * step,
				-centeredOffset.y * step
			);
		}
	}


	public void OnCellClicked(int x, int y)
	{
		if (selectedShapeIndex == -1)
		{
			Debug.Log("No shape selected");
			return;
		}

		PieceShape shape = currentShapes[selectedShapeIndex];

		if (CanPlace(shape, x, y))
		{
			PlaceShape(shape, x, y);

			// replace used shape FIRST
			currentShapes[selectedShapeIndex] = shapes[Random.Range(0, shapes.Length)];

			selectedShapeIndex = -1;

			ShowAllPreviews();

			// NOW check properly
			if (!CanPlaceAnyShape())
			{
				GameOver();
			}

		}
		else
		{
			Debug.Log("Can't place here");
		}
	}

	void GameOver()
	{
		Debug.Log("No more moves.");

		isFading = true;

		PuzzleState.scene3Result = "Lose";
		PuzzleState.currentResult = PuzzleState.scene3Result;

		puzzleFinished = true;
	}




	public string[] messages =
	{
		"it’s marc’s fault",
		"you didn’t stop it",
		"you saw it",
		"you let it happen",
		"it was just a joke",
		"six-seven",
		"bading haha",
		"naiwan mo bukas ung ilaw",
		"armando",
		"fyn shi frfr",
		"no one remembers correctly"
	};

	void Awake()
	{
		Debug.Log("Creating grid...");
		board = new int[width, height];
		cellObjects = new GameObject[width, height];

		CreateGridVisual();
	}

	void UpdateGridVisual()
	{
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				var img = cellObjects[x, y].GetComponent<UnityEngine.UI.Image>();

				if (board[x, y] == 1)
					img.color = Color.white;
				else
					img.color = Color.gray;
			}
		}
	}

	public bool IsCellEmpty(int x, int y)
	{
		return board[x, y] == 0;
	}

	public void SetCell(int x, int y, int value)
	{
		board[x, y] = value;
	}

	public bool CanPlace(PieceShape shape, int baseX, int baseY)
	{
		foreach (var offset in shape.offsets)
		{
			int x = baseX + offset.x;
			int y = baseY + offset.y;

			if (x < 0 || x >= width || y < 0 || y >= height)
				return false;

			if (board[x, y] != 0)
				return false;
		}

		return true;
	}

	public void PlaceShape(PieceShape shape, int baseX, int baseY)
	{
		foreach (var offset in shape.offsets)
		{
			int x = baseX + offset.x;
			int y = baseY + offset.y;

			board[x, y] = 1;
		}

		CheckLines();
		UpdateGridVisual(); // 👈 ADD THIS
	}

	void CheckLines()
	{
		int linesCleared = 0;

		// Rows
		for (int y = 0; y < height; y++)
		{
			bool full = true;

			for (int x = 0; x < width; x++)
			{
				if (board[x, y] == 0)
				{
					full = false;
					break;
				}
			}

			if (full)
			{
				ClearRow(y);
				linesCleared++;
				TriggerMessage();
			}
		}

		// Columns
		for (int x = 0; x < width; x++)
		{
			bool full = true;

			for (int y = 0; y < height; y++)
			{
				if (board[x, y] == 0)
				{
					full = false;
					break;
				}
			}

			if (full)
			{
				ClearColumn(x);
				linesCleared++;
				TriggerMessage();
			}
		}

		// 🔥 APPLY SCORE ONCE
		if (linesCleared > 0)
		{
			ReduceScore(linesCleared * 30);
		}
	}

	void ReduceScore(int amount)
	{
		remainingTime -= amount;

		if (remainingTime < 0)
			remainingTime = 0;

		UpdateTimeUI();

		if (remainingTime == 0)
		{
			WinGame();
		}
	}

	void WinGame()
	{
		Debug.Log("You reached 0 👁");

		isFading = true;

		winTriggered = true; // 👈 add this flag

		PuzzleState.scene3Result = "Win";
		PuzzleState.currentResult = PuzzleState.scene3Result;
		puzzleFinished = true;
	}

	void OnFadeComplete()
	{
		messageText.gameObject.SetActive(true);

		if (winTriggered)
		{
			string[] winMessages =
			{
			"…it’s gone.",
			"…you fixed it.",
			"…for now.",
			"…silence.",
			"…nothing left."
		};

			string msg = winMessages[Random.Range(0, winMessages.Length)];
			messageText.text = msg;

			Invoke(nameof(GoToNextScene), 2f);
		}
		else
		{
			string[] failMessages =
			{
			"…you ran out.",
			"…you couldn’t fix it.",
			"…it’s still there.",
			"…too late.",
			"…you saw it happen."
		};

			string msg = failMessages[Random.Range(0, failMessages.Length)];
			messageText.text = msg;

			Invoke(nameof(RestartGame), 2f);
		}
	}

	void RestartGame()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(
			UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
		);
	}

	bool CanPlaceAnyShape()
	{
		for (int s = 0; s < currentShapes.Length; s++)
		{
			PieceShape shape = currentShapes[s];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (CanPlace(shape, x, y))
						return true;
				}
			}
		}

		return false;
	}

	void CreateGridVisual()
	{
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				GameObject cell = Instantiate(cellPrefab, gridParent);

				cellObjects[x, y] = cell;

				// 🔥 ADD THIS PART
				GridCellUI cellUI = cell.AddComponent<GridCellUI>();
				cellUI.x = x;
				cellUI.y = y;
				cellUI.manager = this;
			}
		}
	}

	void ClearRow(int y)
	{
		for (int x = 0; x < width; x++)
		{
			board[x, y] = 0;
		}
	}

	void ClearColumn(int x)
	{
		for (int y = 0; y < height; y++)
		{
			board[x, y] = 0;
		}
	}

	public void TriggerMessage()
	{
		string msg = messages[Random.Range(0, messages.Length)];

		messageText.gameObject.SetActive(true);
		messageText.text = msg;

		CancelInvoke();
		Invoke(nameof(ClearMessage), 1.5f);
	}

	void ClearMessage()
	{
		messageText.gameObject.SetActive(false);
	}

	void Update()
	{
		if (isFading)
		{
			Color c = fadePanel.color;
			float dynamicSpeed = winTriggered
			? Mathf.Lerp(0.2f, 0.6f, fadePanel.color.a) // slower, calm
			: Mathf.Lerp(0.3f, 1.2f, fadePanel.color.a); // faster, intense
			c.a += Time.deltaTime * dynamicSpeed;
			fadePanel.color = c;


			if (c.a >= 1f)
			{
				isFading = false;
				OnFadeComplete();
			}
		}
	}

	void GoToNextScene()
	{
		PlayerPrefs.SetInt("Scene03_Event", 22);
		SceneManager.LoadScene("StaircaseScene01");// change to your next scene name
	}

	public void DebugSkipWin()
	{
		Debug.Log("DEBUG SKIP → WIN");

		PuzzleState.scene3Result = "Win";
		PuzzleState.currentResult = PuzzleState.scene3Result;

		puzzleFinished = true;

		// optional: trigger fade immediately
		isFading = true;
		winTriggered = true;
	}


}

[System.Serializable]
public class PieceShape
{
	public Vector2Int[] offsets;
}

public static class PuzzleState
{
	public static string scene2Result = "";
	public static string scene3Result = "";

	// optional: last played puzzle
	public static string currentResult = "";
}

