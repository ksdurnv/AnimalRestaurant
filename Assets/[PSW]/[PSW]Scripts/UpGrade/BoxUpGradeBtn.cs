using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxUpGradeBtn : MonoBehaviour
{
    [SerializeField] public Button upButton;
    FoodList FoodList;
    FoodMachine.MachineType machineType;
    GameInstance gameInstance;
    FoodMachine foodMachinesType;



    public float moneyUpGrade = 100f;

    private void Start()
    {
        gameInstance = new GameInstance();
        FoodList = GetComponent<FoodList>();
        foodMachinesType = gameInstance.GameIns.workSpaceManager.foodMachines[0];

        upButton.onClick.AddListener(()=> OnBtnFoodUpGrade(moneyUpGrade));
    }
    public void OnBtnFoodUpGrade(float moneyUpGrade)
    {
        switch(foodMachinesType.machineType)
        {
            case FoodMachine.MachineType.BurgerMachine:
                FoodList.humbugerPrice += moneyUpGrade;
                Debug.Log("ÇÜ¹ö°Å °¡°ÝÀÌ ÀÛµ¿µÊ");
                break;
            case FoodMachine.MachineType.DonutMachine:
                FoodList.dountPrice += moneyUpGrade;
                break;
            case FoodMachine.MachineType.CokeMachine:
                FoodList.cokePrice += moneyUpGrade;
                break;
            case FoodMachine.MachineType.CoffeeMachine:
                FoodList.CuffePrice += moneyUpGrade;
                break;
        }
        
    }
}
