using UnityEngine;
using System.Collections;

public class Speed_Scr : MonoBehaviour {

    public Main_Scr mainScript; //основной скрипт
    public GameObject[] myNumbers; //массив с числами
	
	void Update () { //каждый кадр
        for (int i = 0; i < myNumbers.Length; i++) { //убираем все цифры
            myNumbers[i].SetActive(false);
        }
        myNumbers[mainScript.speed-1].SetActive(true); // показываем только ту цифру, которая равна скорость - 1
    }
}
