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
        // Card ������Ʈ�� Image ��������
        cardBg = gameObject.GetComponent<Image>();

        // BackGlow ������Ʈ�� Image ��������
        backGlow = transform.Find("Mask/BackGlow").GetComponent<Image>();

        // Glow ������Ʈ�� Image ��������
        glow = transform.Find("Mask/Glow").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
