using UnityEngine;
using System.Collections;

public class Score_Scr : MonoBehaviour {

    public Main_Scr mainScript;
    public GameObject[] myFirstNumbers;
    public GameObject[] mySecondNumbers;
    public GameObject[] myThirdNumbers;
    

    void Start () {
	
	}

	void Update () {
        if (mainScript.score > 1000) mainScript.score = 0;
        for (int i = 0; i < myFirstNumbers.Length; i++)
        {
            myFirstNumbers[i].SetActive(false);
            mySecondNumbers[i].SetActive(false);
            myThirdNumbers[i].SetActive(false);
        }

        for (int i = 0; i < myFirstNumbers.Length; i++)
        {
            myFirstNumbers[Mathf.RoundToInt(mainScript.score/100)].SetActive(true);
            mySecondNumbers[Mathf.RoundToInt(mainScript.score/10%10f)].SetActive(true);
            myThirdNumbers[Mathf.RoundToInt(mainScript.score % 100f % 10f)].SetActive(true);
        }
    }
}
