using System.Collections;
using UnityEngine;

public class zzTransparencyCaptureExample : MonoBehaviour
{
    public Texture2D capturedImage; //캡처된 이미지 저장
    public Material saveCapture;

    public IEnumerator capture()
    {
        //전체 화면 캡처
        Rect lRect = new Rect(0f, 0f, Screen.width, Screen.height);
        if (capturedImage)
            Destroy(capturedImage);

        yield return new WaitForEndOfFrame();
        capturedImage = zzTransparencyCapture.capture(lRect);

        //material의 texture에 captureImage 저장
        saveCapture.SetTexture("_MainTex", capturedImage);
        byte[] bytes = capturedImage.EncodeToPNG();
        System.IO.File.WriteAllBytes("RenderTextureCapture.png", bytes);

    }

    Vector3 lastMousePosition;
    public void Update()
    {
        //c 눌렀을 때 캡처
        if (Input.GetKeyDown(KeyCode.C))
            StartCoroutine(capture());
        //s 눌렀을 때 캡처 지움
        if (Input.GetKeyDown(KeyCode.S))
            Destroy(capturedImage);
    }

    //gui화면에 보여줌.
    void OnGUI()
    {
        if (capturedImage)
        {
            GUI.DrawTexture(
                new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f),
                capturedImage,
                ScaleMode.ScaleToFit,
                true);
            GUI.color = Color.green;
        }

        GUI.color = Color.black;
    }
}