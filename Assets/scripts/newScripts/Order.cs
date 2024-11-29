using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//한글

public class Order : MonoBehaviour
{
    public Image foodImage1;
    public Image foodImage2;

    public GameObject secondFood;
    public TMP_Text foodNum1;
    public TMP_Text foodNum2;

    public AnimalController animalController;
    public Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     /*   if(animalController != null)
        {
            transform.position = animalController.transform.position + new Vector3(0,10,0);
            transform.rotation = Quaternion.Euler(new Vector3(60, 45, 0));
            foodImage1.rectTransform.localPosition =  new Vector3(-0.8f,0,0);
            foodNum1.rectTransform.localPosition = new Vector3(0.95f, -0.2f, 0f);
            
            if(foodImage2.gameObject.activeSelf)
            {
                foodImage1.rectTransform.localPosition = new Vector3(-0.8f, -0.8f, 0);
                foodNum1.rectTransform.localPosition = new Vector3(0.95f, -1f, 0f);
            }
        }*/
    }
    public void ShowOrder(FoodMachine.MachineType type, int num, int count)
    {
        transform.position = animalController.transform.position + new Vector3(0, 10, 0);
        transform.rotation = Quaternion.Euler(new Vector3(60, 45, 0));
       
        if (count == 0)
        {
            foodImage1.rectTransform.localPosition = new Vector3(-0.8f, 0, 0);
            foodNum1.rectTransform.localPosition = new Vector3(0.95f, -0.2f, 0f);
          
            foodImage1.sprite = sprites[(int)type];
            foodNum1.text = $" {num}";
            foodImage2.gameObject.SetActive(false);
            foodNum2.gameObject.SetActive(false);
        }
        if (count == 1)
        {
            foodImage1.rectTransform.localPosition = new Vector3(-0.8f, -0.8f, 0);
            foodNum1.rectTransform.localPosition = new Vector3(0.95f, -1f, 0f);
            foodImage2.sprite = sprites[(int)type];
            foodNum2.text = $" {num}";
            foodImage2.gameObject.SetActive(true);
            foodNum2.gameObject.SetActive(true);
        }

  
    }
}
