using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelAnimalsController : MonoBehaviour
{
    public AnimalCard[] animalCard;
    public Sprite[] cardBg;
    public Color[] backGlowColor;
    public Color[] glowColor;
    public Animal[] lockAnimals;
    

    private PlayerAnimalDataManager playerAnimalDataManager;
    private GameInstance gameInstance = new GameInstance();

    private void OnEnable()
    {
        playerAnimalDataManager = gameInstance.GameIns.playerAnimalDataManager;

        CardUpdate();
    }

    // Update is called once per frame
    public void CardUpdate()
    {
        for(int i = 0; i < animalCard.Length; i++)
        {
            HaveAnimal animal = playerAnimalDataManager.PlayerAnimal(i);

            if (animal.tier > 0)
            {
                animalCard[i].gameObject.SetActive(true);

                animalCard[i].id = animal.id;
                animalCard[i].name = animal.name;
                animalCard[i].speed = animal.speed;
                animalCard[i].eatSpeed = animal.eatSpeed;
                animalCard[i].minOrder = animal.minOrder;
                animalCard[i].maxOrder = animal.maxOrder;
                animalCard[i].likeFood = animal.likeFood;
                animalCard[i].hateFood = animal.hateFood;

                for (int j = 0; j < 4; j++)
                {
                    if (animal.tier == j + 1)
                    {
                        animalCard[i].cardBg.sprite = this.cardBg[j];
                        animalCard[i].backGlow.color = this.backGlowColor[j];
                        animalCard[i].glow.color = this.glowColor[j];
                    }
                }
                
            }
        }        
    }


}
