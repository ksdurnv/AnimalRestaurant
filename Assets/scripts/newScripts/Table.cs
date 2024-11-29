using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public bool isDirty = false;
    public AnimalController controller;
    public AnimalController employeeContoller;
  
    public List<FoodStack>foodStacks = new List<FoodStack>();
    //public Stack<Garbage> garbages = new Stack<Garbage>();
    public List<Garbage> garbageList = new List<Garbage>();
    public Garbage garbageInstance;
    public Transform animalSeat;
    public Transform cleanSeat;
    public Transform up;
    public int numberOfGarbage;
    public Seat[] seats;
    public GameObject trashPlate;
    public Vector3 plateLoc;
    public bool interacting;
    public int seatNum;
    public void Awake()
    {
        plateLoc = trashPlate.transform.position;
    }
    private void Start()
    {
        foodStacks.Add(new FoodStack());
    }

    private void OnMouseEnter()
    {
        if (isDirty)
        {
            GameInstance instance = new GameInstance();
            instance.GameIns.inputManager.clickedTable = this;
        }
    }

    public void CleanTableManually(Vector3 pos)
    {
        Debug.Log("A");
        StartCoroutine(Clean(pos));
    }
    
    IEnumerator Clean(Vector3 pos)
    {
        
        GameInstance instance = new GameInstance();
        TrashCan tc = instance.GameIns.workSpaceManager.trashCans[0];
   //     instance.GameIns.inputManager.inOtherAction = true;

        while (garbageList.Count > 0)
        {
            Garbage g = garbageList[garbageList.Count - 1];
            garbageList.Remove(g);

            numberOfGarbage--;
           // trashPlate.transform.DO
            g.transform.DOJump(tc.transform.position,2,1,  0.2f).OnComplete(() => {
                Destroy(g.gameObject);
               // GarbageManager.ClearGarbage(g);
            });


            yield return new WaitForSecondsRealtime(0.2f);
        }

        interacting = false;
        isDirty = false;
        trashPlate.transform.position = new Vector3(transform.position.x, transform.position.y + 0.44f, transform.position.z);
      //  yield return StartCoroutine(instance.GameIns.inputManager.BecomeToOrgin());
  //      instance.GameIns.inputManager.inOtherAction = false;
    }
}
