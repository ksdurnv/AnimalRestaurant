using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerInfoPopup : MonoBehaviour
{
    public Sprite[] animalTextures;
    public Sprite[] foodTextures;
    public Image animalFace;
    public Image likeFoodImg;
    public Image hateFoodImg;
    public int id;
    public string name;    // 동물 이름
    public float speed;
    public float eatSpeed;
    public int minOrder;
    public int maxOrder;
    public int likeFood;
    public int hateFood;

    public TextMeshProUGUI name_text;
    public TextMeshProUGUI speed_text;
    public TextMeshProUGUI eatSpeed_text;
    public TextMeshProUGUI order_text;
    public TextMeshProUGUI likeFood_text;
    public TextMeshProUGUI hateFood_text;
    public Slider speedSlider;
    public Slider eatSpeedSlider;
    public Slider orderSlider;
    public TextMeshProUGUI speedSlider_text;
    public TextMeshProUGUI eatSpeedSlider_text;
    public TextMeshProUGUI orderSlider_text;

    public void SetCustomerInfo()
    {
        animalFace.sprite = animalTextures[id];
        name_text.text = name;
        speed_text.text = speed.ToString();
        eatSpeed_text.text = eatSpeed.ToString();
        order_text.text = maxOrder.ToString();

        speedSlider.value = speed;
        eatSpeedSlider.value = eatSpeed;
        orderSlider.value = maxOrder;

        speedSlider_text.text = $"{speed}/20";
        eatSpeedSlider_text.text = $"{eatSpeed}/20";
        orderSlider_text.text = $"{maxOrder}/20";

        if (maxOrder < 4) order_text.text = "적음";
        else if (maxOrder >= 4 && maxOrder < 7) order_text.text = "보통";
        else if (maxOrder >= 7) order_text.text = "많음";

        switch (likeFood)
        {
            case 1: likeFood_text.text = "햄버거"; likeFoodImg.sprite = foodTextures[1]; break;
            case 2: likeFood_text.text = "콜라"; likeFoodImg.sprite = foodTextures[2]; break;
            case 3: likeFood_text.text = "커피"; likeFoodImg.sprite = foodTextures[3]; break;
            case 4: likeFood_text.text = "도넛"; likeFoodImg.sprite = foodTextures[4]; break;

            default: likeFood_text.text = "없음"; likeFoodImg.sprite = foodTextures[0]; break;
        }

        switch (hateFood)
        {
            case 1: hateFood_text.text = "햄버거"; hateFoodImg.sprite = foodTextures[1]; break;
            case 2: hateFood_text.text = "콜라"; hateFoodImg.sprite = foodTextures[2]; break;
            case 3: hateFood_text.text = "커피"; hateFoodImg.sprite = foodTextures[3]; break;
            case 4: hateFood_text.text = "도넛"; hateFoodImg.sprite = foodTextures[4]; break;

            default: hateFood_text.text = "없음"; hateFoodImg.sprite = foodTextures[0]; break;
        }
    }

    public void ClosePopup()
    {
        this.gameObject.SetActive(false);
    }
}
