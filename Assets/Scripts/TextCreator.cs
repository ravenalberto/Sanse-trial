using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class TextCreator : MonoBehaviour
{
    public static TMPro.TMP_Text viewText;
    public static bool runTextPrint;
    public static int charCount;



	public static string fullText;




	[SerializeField] string transferText;
    [SerializeField] int internalCount;





	// Update is called once per frame
	void Update()
	{
		if (runTextPrint == true)
		{
			runTextPrint = false;

			viewText = GetComponent<TMPro.TMP_Text>();

			transferText = fullText;

			StopAllCoroutines(); // 🔥 VERY IMPORTANT
			StartCoroutine(RollText());
		}
	}

	IEnumerator RollText()
	{
		charCount = 0;
		viewText.text = "";

		foreach (char c in transferText)
		{
			viewText.text += c;
			charCount++; // 🔥 THIS IS THE IMPORTANT PART
			yield return new WaitForSeconds(0.03f);
		}
	}

	public static void CompleteText()
	{
		if (viewText != null)
		{
			viewText.text = viewText.text; // force full text
			charCount = viewText.text.Length;
		}
	}

}
