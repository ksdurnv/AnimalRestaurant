using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class LevelUpExpant : MonoBehaviour
{
    public GameObject[] hideOBject;
    public GameObject[] visibleObject;


    // Start is called before the first frame update
    void Start()
    {
        GameInstance gameInstance = new GameInstance();
        for(int i=0; i<hideOBject.Length; i++)
        {
            hideOBject[i].SetActive(false);
        }
        for (int i = 0; i < visibleObject.Length; i++)
        {
            visibleObject[i].SetActive(true);
        }
        MoveCalculator.CheckArea(gameInstance.GameIns.calculatorScale);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
