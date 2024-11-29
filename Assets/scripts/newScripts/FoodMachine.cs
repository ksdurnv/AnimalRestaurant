using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FoodMachine : MonoBehaviour
{

    public enum MachineType
    {
        None,
        BurgerMachine,
        CokeMachine,
        CoffeeMachine,
        DonutMachine,
        PackingTable
    }

    public MachineType machineType;

    public Mesh mesh;
    public Food food;
    public FoodStack foodStack = new FoodStack();
    int maxNum = 10;
    public float cookingTimer = 3f;
    public Transform foodTransform;
    public Transform workingSpot;
    public AnimalController employee;
    public FoodStack[] canBringTheFoods;
    AudioSource audioSource;
    public PlayData playData;
    public int id;
    public int level=1;
    public MachineData mData;
    public MachineData machineData {
        get { return mData; }

        set { mData = value;
            level = mData.level;
        }
    }

    private float foodHight;
    
    // Start is called before the first frame update
    void Start()
    {        
        audioSource = GetComponent<AudioSource>();
        foodStack.type = machineType;
        Cooking();
        //for(int i=0; i<200; i++) FoodManager.NewFood(Instantiate(food));
       
    }

    public void Cooking()
    {
        StartCoroutine(Cook());
    }

    float coroutineTimer = 0;
    IEnumerator Cook()
    {
        while (true)
        {
            if (mData.food_production_max_height > foodStack.foodStack.Count)
            {
                if (!audioSource.isPlaying) audioSource.Play();
                // yield return new WaitForSeconds(cookingTimer);
                  while (coroutineTimer <= mData.food_production_speed)
                  {
                      coroutineTimer += Time.deltaTime;
                      yield return null;
                  }
                //yield return new WaitForSecondsRealtime(cookingTimer);
                coroutineTimer = 0;
                Food f = FoodManager.GetFood(mesh, machineType);
                f.parentType = machineType;

                if (machineType == MachineType.BurgerMachine) foodHight = 0.7f;
                else if (machineType == MachineType.CokeMachine) foodHight = 1f;
                else if (machineType == MachineType.CoffeeMachine) foodHight = 1.2f;
                else if (machineType == MachineType.DonutMachine) foodHight = 0.5f;

                f.transform.position = foodTransform.position + new Vector3(0, foodStack.foodStack.Count * foodHight, 0);
                f.foodPrice = mData.sale_proceeds;

                foodStack.foodStack.Add(f);

            }
            else
            {
                if (audioSource.isPlaying) audioSource.Stop();
                yield return null;

                // FoodManager.NewFood(food);
            }

        }
    }    
}
