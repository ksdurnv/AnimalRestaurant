using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GatcharManager : MonoBehaviour
{
    public enum MapType { town, forest, winter }

    public MapType mapType;
    public GameObject[] gameObjects;
    public GameObject popup;
    public GameObject popup_NewCustomer;
    public GameObject popup_TierUp;
    public Image backGlow;

    public float price;
    public Sprite[] sprites;
    public Image NewAnimalImage;
    public Image TierUpAnimalImage;
    public bool isSpawning;
    public float spawnTerm = 0.2f;
    public float rollingSpeed = 10;

    private bool isFast;
    private int mapInt;
    private int x = 5;
    private List<GameObject> spawnedAnimals = new List<GameObject>();
    private List<int> spawnedAnimalTypes = new List<int>();

    private PlayerAnimalDataManager playerAnimalDataManager;
    private GameInstance gameInstance = new GameInstance();

    private void OnEnable()
    {
        gameInstance.GameIns.gatcharManager = this;
        isSpawning = false;
        ClearAnimals();
    }
    private void Start()
    {
        playerAnimalDataManager = gameInstance.GameIns.playerAnimalDataManager;

        if (mapType == MapType.town) mapInt = 0;
        else if (mapType == MapType.forest) mapInt = 6;
        else if (mapType == MapType.winter) mapInt = 12;
    }

    public void Purchase()
    {
        gameInstance.GameIns.restaurantManager.playerData.money -= price;
        gameInstance.GameIns.uiManager.UpdateMoneyText(gameInstance.GameIns.restaurantManager.playerData.money);
        SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_PLAYERDATA);
    }

    public void SpownAnimal()
    {
        int i = Random.Range(0, gameObjects.Length);
        GameObject animal = Instantiate(gameObjects[i], new Vector3(x, 0, -980), Quaternion.Euler(0, 180, 0));
        spawnedAnimals.Add(animal);
        spawnedAnimalTypes.Add(i);
        x -= 5;
    }

    public void SpownAnimal3()
    {
        if (isSpawning) return;

        popup_NewCustomer.SetActive(false);
        popup_TierUp.SetActive(false);
        popup.SetActive(false);

        Purchase();
        isSpawning = true;
        ClearAnimals();

        x = 5;
        StartCoroutine(SpownAnimalsWithDelay());
    }

    private IEnumerator SpownAnimalsWithDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            SpownAnimal();
            yield return new WaitForSecondsRealtime(spawnTerm);
          
        }

        yield return StartCoroutine(WaitUntilAnimalsReachTarget());
        CheckForDuplicateAnimals();
        isSpawning = false;
    }

    private IEnumerator WaitUntilAnimalsReachTarget()
    {
        while (true)
        {
            bool allReached = true;

            foreach (GameObject animal in spawnedAnimals)
            {
                if (animal != null && animal.GetComponent<Rolling>()?.IsMoving() == true)
                {
                    allReached = false;
                    break;
                }
            }

            if (allReached) break;

            yield return null;
        }
    }

    private void CheckForDuplicateAnimals()
    {
        Dictionary<int, int> animalCounts = new Dictionary<int, int>();

        foreach (int type in spawnedAnimalTypes)
        {
            if (animalCounts.ContainsKey(type))
            {
                animalCounts[type]++;
            }
            else
            {
                animalCounts[type] = 1;
            }
        }

        foreach (var pair in animalCounts)
        {
            if (pair.Value >= 2)
            {
                popup.SetActive(true);

                HaveAnimal animal = playerAnimalDataManager.AddAnimal(pair.Key + mapInt);
                if(animal.tier == 1)
                {
                    popup_NewCustomer.SetActive(true);

                    NewAnimalImage.sprite = this.sprites[pair.Key];

                    gameInstance.GameIns.playerAnimalDataManager.PlayerAnimalUpdate();
                }
                else
                {
                    popup_TierUp.SetActive(true);

                    TierUpAnimalImage.sprite = this.sprites[pair.Key];
                                        
                    if (animal.tier == 2) backGlow.color = Color.blue;
                    else if (animal.tier == 3) backGlow.color = new Color(0.5f, 0f, 0.5f);
                    else if (animal.tier == 4) backGlow.color = Color.yellow;
                    else
                    {
                        popup_TierUp.SetActive(false);
                        popup.SetActive(false);
                    }
                }

            }
            
        }
    }

    private void ClearAnimals()
    {
        foreach (GameObject animal in spawnedAnimals)
        {
            if (animal != null) Destroy(animal);
        }

        spawnedAnimals.Clear();
        spawnedAnimalTypes.Clear();
    }

    private void OnDestroy()
    {
        ClearAnimals();
    }

    public void GatcharSpeedUp()
    {
        if (!isFast)
        {
            spawnTerm = 0.04f;            
            rollingSpeed = 50;
            isFast = true;
        }
        else
        {
            spawnTerm = 0.2f;
            rollingSpeed = 10;
            isFast = false;
        }
        
    }
}
