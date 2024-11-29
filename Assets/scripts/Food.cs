using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    public MeshFilter meshFilter;
    public FoodMachine.MachineType parentType;

    public float foodPrice = 0f;


    // Start is called before the first frame update
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
   // void Update()
   // {
      //  GameInstance gameInstance = new GameInstance();
       // gameInstance.GameIns.moneyManager.UserMoney(foodPrice);
    //}



}
