using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class RewardsBox : MonoBehaviour
{
    public GameObject fishGO;
    public List<Food> foods = new List<Food>();
 //   public List<GameObject> rewards = new List<GameObject>();
    GameInstance gameInstance = new GameInstance();
    public Mesh meshFilter;
    // Start is called before the first frame update
    void Start()
    {
        
         
    /*    for (int i = 0; i < 100; i++)
        {
            GameObject go = Instantiate(fishGO);
            go.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            go.SetActive(false);
            gameObjects.Enqueue(go);
        }
        */
    }

    float fallSpeed = 0.5f;
    float lastTimer = 0f;
    bool removes=false;
    private void Update()
    {
        if (removes)
        {
            Vector3 testScale = new Vector3(transform.localScale.x - Time.deltaTime * 10f, transform.localScale.y - Time.deltaTime * 10f, transform.localScale.z - Time.deltaTime * 10f);
            //   transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * 10f, transform.localScale.y - Time.deltaTime * 10f, transform.localScale.z - Time.deltaTime * 10f);
            if (testScale.x < 0)
            {
                transform.localScale = new Vector3(2, 2, 2);
                //  removeCoroutine = null;
                fallSpeed = 0.5f;
                removes = false;
                if (gameInstance.GameIns.applianceUIManager.currentBox == this) gameInstance.GameIns.applianceUIManager.currentBox = null;
                gameInstance.GameIns.applianceUIManager.rewardsBoxes.Remove(this);
                FoodManager.RemoveRewardsBox(this);

            }
            else
            {
                transform.localScale = testScale;
            }
        }
     
        //  gameObject.SetActive(false);
       
    }


    public bool GetFish(bool touchdown = false)
    {
        bool result = false;
        if (lastTimer + fallSpeed < Time.time || touchdown)
        {

            lastTimer = Time.time;
            if (foods.Count < 30)
            {
                Food go = FoodManager.GetFood(meshFilter, FoodMachine.MachineType.None, true);
                foods.Add(go);
                go.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                go.transform.rotation = Quaternion.Euler(new Vector3(90, 45, 0));
                go.transform.position = transform.position + Vector3.up * 10f;
                // StartCoroutine(DownFish());
                StartCoroutine(Falling(go));
                result = true;
            }
        }

        fallSpeed -= Time.deltaTime * 2f;
        if (fallSpeed < 0.1f) fallSpeed = 0.1f;

        return result;
    }

    public void StopFish()
    {
        fallSpeed = 0.5f;
    }

    Coroutine r;
    IEnumerator Falling(Food go)
    {
       
        float timer = 0;
        while(timer < 0.4f)
        {
            go.transform.Translate(Vector3.down * Time.deltaTime * 100f, Space.World);

            timer += Time.deltaTime;
            if(go.transform.position.y <= transform.position.y + 0.4f + 0.2f * foods.Count - 1)
            {
                go.transform.position = new Vector3(go.transform.position.x, transform.position.y + 0.4f + 0.2f * foods.Count, go.transform.position.z);

                if (r != null) StopCoroutine(r);
                 r = StartCoroutine(Rebound(go));
                break;
            }

            yield return null;
        }
    }

    IEnumerator Rebound(Food go)
    {
        float a = 2;
        while(true)
        {
            a+= Time.deltaTime * 10f;
            transform.localScale = new Vector3(a, 2, a);
            for (int i = 0; i < foods.Count; i++)
            {
                if (foods[i] != go)
                {
                    foods[i].transform.localScale = new Vector3(2.5f + (a - 2) * 2, 2.5f, 2.5f + (a - 2) * 2);
                }
            }

            yield return null;
            if (a > 2.2f)
            {
                a = 2.2f;
                break;
            }
        }


        while (true)
        {
            a -= Time.deltaTime*10;
            transform.localScale = new Vector3(a, 2, a);
            for (int i = 0; i < foods.Count; i++)
            {
                if (foods[i] != go)
                {
                    foods[i].transform.localScale = new Vector3(2.5f + (a - 2) * 2, 2.5f, 2.5f + (a - 2) * 2);
                }
            }
            yield return null;
            if (a < 2)
            {
                a = 2f;
                transform.localScale = new Vector3(a, 2, a);
                for (int i = 0; i < foods.Count; i++)
                {
                    if (foods[i] != go)
                    {
                        foods[i].transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                    }
                }
                break;
            }
        }
    }

    Coroutine removeCoroutine;
    public bool ClearFishes()
    {
        Debug.Log("Clear");
        if(foods.Count ==0)
        {
            removes = true;
           // if (removeCoroutine == null)
            {
              //  removeCoroutine = StartCoroutine(RemoveRewardBox());
                return true;
            }
        }
        return false;
    }

    public void EatFish(Food go)
    {
        foods.Remove(go);
        FoodManager.EatFood(go, true);
    }
    public IEnumerator RemoveRewardBox()
    {
        while(true)
        {
            Vector3 testScale = new Vector3(transform.localScale.x - Time.deltaTime * 10f, transform.localScale.y - Time.deltaTime * 10f, transform.localScale.z - Time.deltaTime * 10f);
         //   transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * 10f, transform.localScale.y - Time.deltaTime * 10f, transform.localScale.z - Time.deltaTime * 10f);
            if(testScale.x < 0)
            {
                transform.localScale = new Vector3(2, 2, 2);
                break;
            }
            else
            {
                transform.localScale = testScale;
            }

            yield return null;
        }
      //  gameObject.SetActive(false);
        removeCoroutine = null;

        fallSpeed = 0.5f;
        if (gameInstance.GameIns.applianceUIManager.currentBox == this) gameInstance.GameIns.applianceUIManager.currentBox = null;
        gameInstance.GameIns.applianceUIManager.rewardsBoxes.Remove(this);
        FoodManager.RemoveRewardsBox(this);
    }
    
}
