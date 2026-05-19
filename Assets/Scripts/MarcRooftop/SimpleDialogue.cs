using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimpleDialogue : MonoBehaviour
{
	[Header("Choice UI")]
	public GameObject choiceUI;

	public Button redButton;
	public Button blueButton;

	public TMP_Text redButtonText;
	public TMP_Text blueButtonText;

	private bool hasChoice = false;

	private string redChoiceResult;
	private string blueChoiceResult;

	private string[] resultSpeakers;
	private string[] resultLines;

	private GameObject currentCharacter;

	private string currentCharacterName;
	string currentCharacterID;



	[Header("Prefab References")]
    public GameObject vnUI;            // The parent container (VN_ui)
    public GameObject textBox;         // Your TextBox prefab
    public GameObject nextButton;      // Your NextButton prefab
    public TMP_Text nameText;          // Text inside TextBox
    public TMP_Text dialogueText;      // Text inside TextBox

    [Header("Settings")]
    public float typingSpeed = 0.03f;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullLine;

	private int currentLine = 0;

	private string[] currentLines;
	private string[] currentSpeakers;

	void Start()
    {
        if (vnUI != null) vnUI.SetActive(false);
        if (nextButton != null) nextButton.SetActive(false);
    }


	public PlayerMovement playerMovement;
	public CameraFollow cameraFollow;
	public void ShowDialogue(
	string[] speakers,
	string[] lines,
	GameObject characterObject,
	string characterID
)
	{
		playerMovement.canMove = false;
		cameraFollow.canLook = false;

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		vnUI.SetActive(true);
		textBox.SetActive(true);

		currentSpeakers = speakers;
		currentLines = lines;
		currentCharacter = characterObject;

		currentCharacterID = characterID;

		currentCharacterName = characterID.ToLower();

		currentLine = 0;

		ShowCurrentLine();
	}

	public void ShowDialogue(string speaker, string line)
	{
		ShowDialogue(
			new string[] { speaker },
			new string[] { line },
			null,
			""
		);
	}

	void ShowCurrentLine()
	{
		if (currentLine >= currentSpeakers.Length ||
	currentLine >= currentLines.Length)
		{
			Debug.LogError("Dialogue arrays mismatch!");
			return;
		}
		nextButton.SetActive(false);

		nameText.text = currentSpeakers[currentLine];

		currentFullLine = currentLines[currentLine];

		if (typingCoroutine != null)
			StopCoroutine(typingCoroutine);

		typingCoroutine = StartCoroutine(TypeText(currentFullLine));
	}

	private IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        nextButton.SetActive(true); // Show the NextButton once finished
    }

	// Call this from the NextButton's OnClick event
	public void OnNextButtonClicked()
	{
		if (isTyping)
		{
			StopCoroutine(typingCoroutine);

			dialogueText.text = currentFullLine;

			isTyping = false;

			nextButton.SetActive(true);

			return;
		}

		currentLine++;

		if (currentLine < currentLines.Length)
		{
			ShowCurrentLine();
		}
		else
		{
			if (hasChoice)
			{
				ShowChoices();
			}
			else
			{
				EndDialogue();
			}
		}
	}
	void ShowChoices()
	{
		nextButton.SetActive(false);

		choiceUI.SetActive(true);
	}

	public void SetupChoices(
	string redText,
	string blueText
)
	{
		hasChoice = true;

		redButtonText.text = redText;
		blueButtonText.text = blueText;
	}

	public void OnRedChoice()
	{
		choiceUI.SetActive(false);
		hasChoice = false;

		if (currentCharacterID == "Raven")
			ChoiceManager.Instance.ravenRed = true;

		if (currentCharacterID == "Darlene")
			ChoiceManager.Instance.darleneRed = true;

		if (currentCharacterID == "Kuh")
			ChoiceManager.Instance.kuhRed = true;

		if (currentCharacterID == "Cristel")
			ChoiceManager.Instance.cristelRed = true;

		string name = currentCharacterName.ToLower();

		// 🔴 CRISTEL
		if (name.Contains("cristel"))
		{
			resultSpeakers = new string[]
			{
			"Marc",
			"Cristel",
			"Marc",
			"Cristel"
			};

			resultLines = new string[]
			{
			"Hindi mo naman kailangang manatili sa storya. Wala na eh, deads ka na dito. So... what if eto na yung afterlife?",

			"Pinagsasabi mo nanaman?",

			"May tiwala ka ba sakin Cristel?",

			"... Ok edi go."
			};
		}

		// 🔴 RAVEN
		else if (name.Contains("raven"))
		{
			resultSpeakers = new string[]
			{
			"Marc",
			"Raven"
			};

			resultLines = new string[]
			{
			"Hahaha, bandang kaliwa, tapos may elevator don.",

			"ok thanks marc."
			};
		}

		// 🔴 DARLENE
		else if (name.Contains("darlene"))
		{
			resultSpeakers = new string[]
			{
			"Darlene",
			"Marc",
			"Darlene",
			"Marc"
			};

			resultLines = new string[]
			{
			"Edi... hindi pala totoo lahat ng nangyayari?",

			"Totoo naman, kaso nasa game nga lang",

			"Ano mangyayari kapag.. umalis ako dito?",

			"Subukan mo para malaman mo."
			};
		}

		// 🔴 KUH
		else if (name.Contains("kuh"))
		{
			resultSpeakers = new string[]
			{
			"Kuh"
			};

			resultLines = new string[]
			{
			"Wait omg pwede nako umalis legit?"
			};
		}

		// 🔴 MARC
		else if (name.Contains("marc"))
		{
			resultSpeakers = new string[]
			{
			"Marc"
			};

			resultLines = new string[]
			{
			"Haha ayoko nga, tinatamad na nga ako magcode ng choice results eh papahirapan mo pa ako."
			};
		}

		StartResultDialogue();
	}


	public void OnBlueChoice()
	{
		choiceUI.SetActive(false);
		hasChoice = false;

		if (currentCharacterID == "Raven")
			ChoiceManager.Instance.ravenRed = false;

		if (currentCharacterID == "Darlene")
			ChoiceManager.Instance.darleneRed = false;

		if (currentCharacterID == "Kuh")
			ChoiceManager.Instance.kuhRed = false;

		if (currentCharacterID == "Cristel")
			ChoiceManager.Instance.cristelRed = false;

		string name = currentCharacterName.ToLower();

		// 🔵 CRISTEL
		if (name.Contains("cristel"))
		{
			resultSpeakers = new string[]
			{
			"Marc",
			"Cristel",
			"Marc"
			};

			resultLines = new string[]
			{
			"Huli man ang lahat, pero hindi nila sisirain ang pangako nila.",

			"Marc...",

			"Kahit ako. Hindi ko tanggap. Pero alam mong hinding hindi ka mawawala sa puso ko."
			};
		}

		// 🔵 RAVEN
		else if (name.Contains("raven"))
		{
			resultSpeakers = new string[]
			{
			"Marc",
			"Raven"
			};

			resultLines = new string[]
			{
			"Time heals.",

			"Real af."
			};
		}

		// 🔵 DARLENE
		else if (name.Contains("darlene"))
		{
			resultSpeakers = new string[]
			{
			"Marc",
			"Darlene",
			"Darlene"
			};

			resultLines = new string[]
			{
			"Wala na si Cristel, pero andito pa rin ang presensya nya satin. Hinding hindi mawawala yon.",

			"...",

			"Tama ka..."
			};
		}

		// 🔵 KUH
		else if (name.Contains("kuh"))
		{
			resultSpeakers = new string[]
			{
			"Kuh"
			};

			resultLines = new string[]
			{
			"Hindi ko kayang iwan tong mundong to.."
			};
		}

		// 🔵 MARC
		else if (name.Contains("marc"))
		{
			resultSpeakers = new string[]
			{
			"Marc"
			};

			resultLines = new string[]
			{
			"Haha ayoko nga, tinatamad na nga ako magcode ng choice results eh papahirapan mo pa ako."
			};
		}

		StartResultDialogue();
	}

	void StartResultDialogue()
	{
		if (resultSpeakers == null || resultLines == null)
		{
			Debug.LogError("RESULT DIALOGUE NOT SET");
			return;
		}

		currentSpeakers = resultSpeakers;
		currentLines = resultLines;

		currentLine = 0;

		ShowCurrentLine();
	}


	void EndDialogue()
	{
		Debug.Log("ENDING DIALOGUE");
		Debug.Log("CURRENT CHARACTER: " + currentCharacterName);

		// 🎬 FINAL MARC
		if (currentCharacterName.ToLower().Contains("marc"))
		{
			Debug.Log("LOADING EPILOGUE");

			SceneManager.LoadScene("EpilogueScene");
			return;
		}

		// COUNT INTERACTIONS
		if (
			!currentCharacterName.Contains("marc")
		)
		{
			InteractionProgress.Instance.AddInteraction();
		}

		// NORMAL CHARACTERS DISAPPEAR
		if (currentCharacter != null)
		{
			currentCharacter.SetActive(false);
		}

		vnUI.SetActive(false);

		playerMovement.canMove = true;
		cameraFollow.canLook = true;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}
