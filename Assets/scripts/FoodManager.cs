using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static FoodMachine;

public static class FoodManager
{
   // public static Food food;

    static Queue<Food> deActivatedfoods = new Queue<Food>();
    static List<Food> activatedfoods = new List<Food>();

    static Queue<PackageFood> deActivatedPackageFoods = new Queue<PackageFood>();
    static List<PackageFood> activatedPackageFoods = new List<PackageFood>();

    //직원 먹이용 
    static Queue<Food> deActovatedRewards = new Queue<Food>();
    static List<Food> activatedRewards = new List<Food>();

    static Queue<RewardsBox> deActovatedRewardsBox = new Queue<RewardsBox>();
    static List<RewardsBox> activatedRewardsBox = new List<RewardsBox>();

    // Start is called before the first frame update
    static GameObject foodCollects;
    public static void NewFood(Food food,int count, bool rewards = false)
    {
        if (foodCollects == null)
        {
            foodCollects = new GameObject();
            foodCollects.name = "foodCollects";
            foodCollects.transform.position = Vector3.zero;
        }
        for (int i = 0; i < count; i++)
        {
            Food f = GameObject.Instantiate(food, foodCollects.transform);
            f.gameObject.SetActive(false);
            if (!rewards) deActivatedfoods.Enqueue(f);
            else deActovatedRewards.Enqueue(f);
        }
    }

    public static Food GetFood(Mesh meshFilter, FoodMachine.MachineType machineType, bool rewards = false)
    {
        if (!rewards)
        {
            Food f = deActivatedfoods.Dequeue();
            f.meshFilter.mesh = meshFilter;
            f.parentType = machineType;
            f.gameObject.SetActive(true);
            activatedfoods.Add(f);
            return f;
        }
        else
        {
            Food f = deActovatedRewards.Dequeue();
            f.meshFilter.mesh = meshFilter;
            f.parentType = machineType;
            f.gameObject.SetActive(true);
            activatedRewards.Add(f);
            return f;
        }
        
    }

    public static void EatFood(Food food, bool rewards = false)
    {
        if (!rewards)
        {
            food.gameObject.SetActive(false);
            deActivatedfoods.Enqueue(food);
            activatedfoods.Remove(food);
        }
        else
        {
            food.gameObject.SetActive(false);
            deActovatedRewards.Enqueue(food);
            activatedRewards.Remove(food);
        }

    }

    public static void NewPackageBox(PackageFood box, int num)
    {
        for (int i = 0; i < num; i++)
        {
            PackageFood f = GameObject.Instantiate(box, foodCollects.transform);

            f.gameObject.SetActive(false);
            deActivatedPackageFoods.Enqueue(f);
        }

    }

    public static PackageFood GetPackageBox()
    {
        PackageFood packageFood = deActivatedPackageFoods.Dequeue();
        activatedPackageFoods.Add(packageFood);
        packageFood.gameObject.SetActive(true);
        return packageFood;
    }

    public static void RemovePackageBox(PackageFood remove)
    {

        for (int i = 0; i < remove.packageTrans.Length; i++)
        {
            Food food = remove.packageTrans[i].GetComponentInChildren<Food>();
            food.transform.SetParent(foodCollects.transform);
            food.transform.position = Vector3.zero;
            EatFood(food);
        }

        remove.gameObject.SetActive(false);
        activatedPackageFoods.Remove(remove);
        deActivatedPackageFoods.Enqueue(remove);
    }



    //직원 보상 관리
    //

    //보상 상자 인스턴스화
    public static void NewRewardsBox(RewardsBox rewardsBox, int count)
    {
      
        for (int i = 0; i < count; i++)
        {
            RewardsBox r = GameObject.Instantiate(rewardsBox, foodCollects.transform);
            r.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            r.transform.position = Vector3.zero;
            r.gameObject.SetActive(false);
            deActovatedRewardsBox.Enqueue(r);
        }
    }


    //보상 상자 가져오기
    public static RewardsBox GetRewardsBox()
    {
        RewardsBox r = deActovatedRewardsBox.Dequeue();
      //  r.meshFilter.mesh = meshFilter;
       // f.parentType = machineType;
      //  r.gameObject.SetActive(true);
        activatedRewardsBox.Add(r);
        return r;
    }

    //보상 상자 제거
    public static void RemoveRewardsBox(RewardsBox rewardsBox)
    {
        activatedRewardsBox.Remove(rewardsBox);
        deActovatedRewardsBox.Enqueue(rewardsBox);
        rewardsBox.gameObject.SetActive(false);
    }
}
