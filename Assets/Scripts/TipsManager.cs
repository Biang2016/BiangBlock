using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TipsManager : MonoSingletion<TipsManager>
{
    private List<string> tips = new List<string>{
        "Breaker group can be used in several places! You can still control the remain after it crashes, unless it is broken into pieces.",
        "If you want to make the best of breakers, try to trigger a fall.",
        "Sometimes abandoning is also important, especially when you are facing failure. Break those deadly big blocks.",
        "When there is only one color left, there would be no line added from the bottom anymore.",
        "If you feel it too simple, please have a try on the Nightmare level!"
    };

    [SerializeField] private Text[] TipsText;

    private void Awake()
    {
        tipIndex = Random.Range(0, tips.Count);
    }

    int tipIndex = 0;
    public void RefreshText()
    {
        tipIndex++;
        if (tipIndex >= tips.Count)
        {
            tipIndex = 0;
        }
        foreach (Text t in TipsText)
        {
            t.text = "<color=\"#FFFFFF\">Tips</color>: " + tips[tipIndex];
        }
    }
}
