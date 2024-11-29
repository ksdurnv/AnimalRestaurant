using System.Collections.Generic;
using System.IO; // 파일 입출력
using UnityEngine;

[System.Serializable]
public class HaveAnimal
{
    public int id;         // 동물 ID
    public string name;    // 동물 이름
    public int tier;      // 동물 등급
    public float speed;
    public float eatSpeed;
    public int minOrder;
    public int maxOrder;
}

[System.Serializable]
public class PlayerAnimalData
{
    public List<HaveAnimal> animals = new List<HaveAnimal>(); // 동물 리스트
}

public class PlayerAnimalDataManager : MonoBehaviour
{
    private string saveFilePath;
    private GameInstance gameInstance = new GameInstance();

    public PlayerAnimalData playerAnimalData;
    public Animal[] lockAnimals;

    private void Awake()
    {
        gameInstance.GameIns.playerAnimalDataManager = this;
        saveFilePath = Path.Combine(Application.persistentDataPath, "PlayerAnimalData.json");
        LoadPlayerData();
        InitializeAnimalData(); // 동물 데이터를 초기화
        PlayerAnimalUpdate();

    }

    private void InitializeAnimalData()
    {
        // 동물 데이터가 비어 있을 경우 초기화
        if (playerAnimalData.animals.Count == 0)
        {
            playerAnimalData.animals = new List<HaveAnimal>
            {
                new HaveAnimal { id = 0, name = "Cat", tier = 1, speed = 5, eatSpeed = 1, minOrder = 15, maxOrder = 20 },
                new HaveAnimal { id = 1, name = "Crow", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 2, name = "Dog", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 3, name = "Parrot", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 4, name = "Rabbit", tier = 0, speed = 10, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 5, name = "Tortoise", tier = 0, speed = 2, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 6, name = "Chick", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 7, name = "Cow", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 8, name = "Donkey", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 9, name = "Duck", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 10, name = "Elephant", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 11, name = "Flamingo", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 12, name = "Fox", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 13, name = "Gazelle", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 14, name = "Hen", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 15, name = "Hog", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 16, name = "Hornbill", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 17, name = "Hyena", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 18, name = "Ostrich", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 19, name = "Zebra", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 20, name = "Musk Ox", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 21, name = "Owl", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 22, name = "Polar Bear", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 23, name = "Reindeer", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 24, name = "Sea Lion", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 25, name = "Sheep", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 26, name = "Walrus", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 },
                new HaveAnimal { id = 27, name = "Wolf", tier = 0, speed = 5, eatSpeed = 5, minOrder = 1, maxOrder = 5 }
            };

            SavePlayerData();
        }
    }

    public HaveAnimal AddAnimal(int animalId)
    {
        HaveAnimal animal = playerAnimalData.animals.Find(a => a.id == animalId);
        if (animal != null)
        {
            animal.tier++;
            SavePlayerData();
        }

        return animal;
    }

    public HaveAnimal PlayerAnimal(int animalId)
    {
        HaveAnimal animal = playerAnimalData.animals.Find(a => a.id == animalId);                

        return animal;
    }

    private void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerAnimalData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Player data saved to {saveFilePath}");
    }

    private void LoadPlayerData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerAnimalData = JsonUtility.FromJson<PlayerAnimalData>(json);
      //      Debug.Log("Player data loaded.");
        }
        else
        {
            playerAnimalData = new PlayerAnimalData(); // 데이터 초기화
            Debug.Log("No player data found. Creating new data.");
        }
    }

    public void PlayerAnimalUpdate()
    {
        for (int i = 0; i < lockAnimals.Length; i++)
        {
            HaveAnimal animal = PlayerAnimal(i);

            if (animal.tier > 0)
            {
                lockAnimals[i].speed = animal.speed;
                lockAnimals[i].eatSpeed = animal.eatSpeed;
                lockAnimals[i].minOrder = animal.minOrder;
                lockAnimals[i].maxOrder = animal.maxOrder;

                gameInstance.GameIns.animalManager.AddNewAnimal(lockAnimals[i]);           

            }
        }

    }
}
