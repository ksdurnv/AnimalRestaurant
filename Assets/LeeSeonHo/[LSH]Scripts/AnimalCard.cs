using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalCard : MonoBehaviour
{
    public Image cardBg;
    public Image backGlow;
    public Image glow;

    public int id;
    public string name;    // ���� �̸�
    public float speed;
    public float eatSpeed;
    public int minOrder;
    public int maxOrder;
    public int likeFood;
    public int hateFood;

    public CustomerInfoPopup customerInfoPopup;

    // Start is called before the first frame update
    void OnEnable()
    {
        // Card ������Ʈ�� Image ��������
        cardBg = gameObject.GetComponent<Image>();

        // BackGlow ������Ʈ�� Image ��������
        backGlow = transform.Find("Mask/BackGlow").GetComponent<Image>();

        // Glow ������Ʈ�� Image ��������
        glow = transform.Find("Mask/Glow").GetComponent<Image>();
    }

    public void OnClick()
    {
        customerInfoPopup.gameObject.SetActive(true);

        customerInfoPopup.id = id;
        customerInfoPopup.name = name;
        customerInfoPopup.speed = speed;
        customerInfoPopup.eatSpeed = eatSpeed;
        customerInfoPopup.minOrder = minOrder;
        customerInfoPopup.maxOrder = maxOrder;
        customerInfoPopup.likeFood = likeFood;
        customerInfoPopup.hateFood = hateFood;

        customerInfoPopup.SetCustomerInfo();
    }
}
