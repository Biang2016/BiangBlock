using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class IntroductionManager : MonoSingletion<IntroductionManager>
{
    [SerializeField] private Canvas IntroductionCanvas;
    [SerializeField] private Image[] IntroductionImages;

    void Awake()
    {
        IntroductionCanvas.gameObject.SetActive(false);
    }

    public void StartIntroduction()
    {
        IntroductionCanvas.gameObject.SetActive(true);
        ResetIntroduction();
        if (IntroductionImages.Length != 0) IntroductionImages[0].enabled = true;
    }

    int currentShowIndex = 0;
    public void GotItButtonClick()
    {
        IntroductionImages[currentShowIndex].enabled = false;
        currentShowIndex++;
        if (currentShowIndex >= IntroductionImages.Length)
        {
            CloseIntroduction();
        }
        else
        {
            IntroductionImages[currentShowIndex].enabled = true;
        }
    }

    public void PreviousButtonClick()
    {
        if (currentShowIndex == 0)
        {
            IntroductionImages[currentShowIndex].enabled = false;
            CloseIntroduction();
            return;
        }
        IntroductionImages[currentShowIndex].enabled = false;
        currentShowIndex--;
        IntroductionImages[currentShowIndex].enabled = true;
    }

    public void CloseIntroduction()
    {
        IntroductionCanvas.gameObject.SetActive(false);
        GameManager.Instance.EndIntroduction();
    }

    public void ResetIntroduction()
    {
        currentShowIndex = 0;
        foreach (Image img in IntroductionImages)
        {
            img.enabled = false;
        }
    }
}
