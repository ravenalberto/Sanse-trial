using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene01Events : MonoBehaviour
{

    public GameObject fadeScreenIn;
    public GameObject charKuh;
    public GameObject charCristel;
    public GameObject textBox;


    [SerializeField] string textToSpeak;
    [SerializeField] int currentTextLength;
    [SerializeField] int textLength;
    [SerializeField] GameObject mainTextObject;
    [SerializeField] GameObject nextButton;
    [SerializeField] int eventPos = 0;
    [SerializeField] GameObject charName;
	[SerializeField] GameObject fadeOut;



    void Update()
    {
        textLength = TextCreator.charCount;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EventStarter());
    }

    IEnumerator EventStarter()
    {
        yield return new WaitForSeconds(2);
        fadeScreenIn.SetActive(false);
        charCristel.SetActive(true);
        yield return new WaitForSeconds(2);
		//text funtion will go here
        mainTextObject.SetActive(true);
		textToSpeak = "…Where is everyone?";
		TextCreator.fullText = textToSpeak;
		TextCreator.charCount = 0;
		TextCreator.runTextPrint = true;
		currentTextLength = textToSpeak.Length;
        yield return new WaitForSeconds(0.05f);


		yield return new WaitUntil(() => textLength >= currentTextLength);
		yield return new WaitForSeconds(0.05f);
        nextButton.SetActive(true);
        eventPos = 1;


    }

    IEnumerator EventOne()
    {

        // event 1

        nextButton.SetActive(false);
		textBox.SetActive(true);
        charName.GetComponent<TMPro.TMP_Text>().text = "Kuh";

		textToSpeak = "Psst. Tetel!";
		TextCreator.fullText = textToSpeak;
		TextCreator.charCount = 0;
		TextCreator.runTextPrint = true;

		currentTextLength = textToSpeak.Length;
		yield return new WaitForSeconds(0.05f);
		yield return new WaitForSeconds(1);

		yield return new WaitUntil(() => textLength >= currentTextLength);
		yield return new WaitForSeconds(0.05f);
		//yield return new WaitForSeconds(2);
		charKuh.SetActive(true);
		nextButton.SetActive(true);
        eventPos = 2;


	}

	IEnumerator EventTwo()
	{

		// event 2

		nextButton.SetActive(false);
		textBox.SetActive(true);
		charName.GetComponent<TMPro.TMP_Text>().text = "Cristel";

		textToSpeak = "Ah! Kuh, andiyan ka pala.";
		TextCreator.fullText = textToSpeak;
		TextCreator.charCount = 0;
		TextCreator.runTextPrint = true;

		currentTextLength = textToSpeak.Length;

		yield return new WaitForSeconds(0.05f);
		yield return new WaitForSeconds(1);

		yield return new WaitUntil(() => textLength >= currentTextLength);
		yield return new WaitForSeconds(0.05f);
		//yield return new WaitForSeconds(2);
		charKuh.SetActive(true);
		nextButton.SetActive(true);
		eventPos = 3;


	}

	IEnumerator EventThree()
	{

		// event 3 

		nextButton.SetActive(false);
		textBox.SetActive(true);
		charName.GetComponent<TMPro.TMP_Text>().text = "Kuh";

		textToSpeak = "San ka naman nanggaling ngayon? Nasa room sila.";
		TextCreator.fullText = textToSpeak;
		TextCreator.charCount = 0;
		TextCreator.runTextPrint = true;
		currentTextLength = textToSpeak.Length;

		yield return new WaitForSeconds(0.05f);
		yield return new WaitForSeconds(1);

		yield return new WaitUntil(() => textLength >= currentTextLength);
		yield return new WaitForSeconds(0.05f);
		//yield return new WaitForSeconds(2);
		charKuh.SetActive(true);
		nextButton.SetActive(true);

		eventPos = 4;


	}

	IEnumerator EventFour()
	{

		// event 4 

		nextButton.SetActive(false);
		textBox.SetActive(true);
		
		charKuh.SetActive(true);
		fadeOut.SetActive(true);
		yield return new WaitForSeconds(2);

		eventPos = 4;
		SceneManager.LoadScene(1);


	}



	public void NextButton()
    {
        if (eventPos == 1)
        {
			StartCoroutine(EventOne());
		}
		if (eventPos == 2)
		{
			StartCoroutine(EventTwo());
		}
		if (eventPos == 3)
		{
			StartCoroutine(EventThree());
		}
		if (eventPos == 4)
		{
			StartCoroutine(EventFour());
		}
	}
}
