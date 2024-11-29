using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalCard : MonoBehaviour
{
    public Image cardBg;
    public Image backGlow;
    public Image glow;

    // Start is called before the first frame update
    void OnEnable()
    {
        // Card 오브젝트의 Image 가져오기
        cardBg = gameObject.GetComponent<Image>();

        // BackGlow 오브젝트의 Image 가져오기
        backGlow = transform.Find("Mask/BackGlow").GetComponent<Image>();

        // Glow 오브젝트의 Image 가져오기
        glow = transform.Find("Mask/Glow").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
