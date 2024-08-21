using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UI_Base
{
    enum GameObjects
    {
        HPBar
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<GameObject>(typeof(GameObjects));
        return true;
    }

    public void SetInfo(CreatureController owner)
    {
        GetObject((int)GameObjects.HPBar).GetComponent<Slider>().maxValue = owner.Hp;
        SetHpRatio(owner.Hp);
    }

    public void SetHpRatio(float ratio)
    {
        StartCoroutine(CoSmoothHpChange(ratio));
    }

    IEnumerator CoSmoothHpChange(float ratio)
    {
        Slider slider = GetObject((int)GameObjects.HPBar).GetComponent<Slider>();
        float currentRatio = slider.value;
        while (Mathf.Abs(currentRatio - ratio) > 0.01f)
        {
            currentRatio = Mathf.Lerp(currentRatio, ratio, 0.5f);
            slider.value = currentRatio;
            yield return null;
        }

        slider.value = ratio;
    }
}
