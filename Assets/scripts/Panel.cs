using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//한글

public class Panel : MonoBehaviour
{
    public RectTransform rectTransform;

    public Button thisButton;
    public float height;
    public float speed;

    bool spread = false;
    public void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            Spread(spread);
        });
    }

    void Spread(bool bSpread)
    {
        if(bSpread)
        {
            spread = false;
            StartCoroutine(GatheringRectTranform());
        }
        else
        {
            spread = true;
            StartCoroutine(SpreadRectTranform());
        }
       
    }

    IEnumerator SpreadRectTranform()
    {
        RectTransform rectTransforms = rectTransform;
        Vector2 currentVec = rectTransform.sizeDelta;
        Vector3 currentVec2 = rectTransform.localPosition;
        while (true)
        {
            currentVec += new Vector2(0, 500 * Time.unscaledDeltaTime * 4);
            currentVec2 += new Vector3(0, -250f * Time.unscaledDeltaTime * 4, 0);
            if(currentVec.y > 600)
            {
                currentVec.y = 600;
                currentVec2.y = -250f;
                rectTransform.sizeDelta = currentVec;
                rectTransforms.localPosition = currentVec2;
                break;
            }
                rectTransform.sizeDelta = currentVec;
                rectTransform.localPosition = currentVec2;
            yield return null;
        }
    }

    IEnumerator GatheringRectTranform()
    {
        RectTransform rectTransforms = rectTransform;
        Vector2 currentVec = rectTransform.sizeDelta;
        Vector3 currentVec2 = rectTransform.localPosition;
        while (true)
        {
          //  currentVec -= new Vector2(0, 500 * Time.unscaledDeltaTime );
            currentVec2 -= new Vector3(0, -250f * Time.unscaledDeltaTime * 4 , 0);
            if (currentVec2.y > 0)
            {
             //   currentVec.y = 100;
                currentVec2.y = 0f;
              //  rectTransform.sizeDelta = currentVec;
                rectTransforms.localPosition = currentVec2;
                break;
            }
           // rectTransform.sizeDelta = currentVec;
            rectTransform.localPosition = currentVec2;
            yield return null;
        }
    }
}
