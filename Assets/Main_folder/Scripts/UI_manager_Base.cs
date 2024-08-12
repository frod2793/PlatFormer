using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UI_manager_Base : MonoBehaviour
{
    
    
    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init();
    }
    
    public virtual void init_Popup(GameObject target)
    {
        target.transform.localScale = Vector3.zero;
        target.SetActive(false);
    }
    
    public void Hide(GameObject target)
    {
        target.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            target.SetActive(false);
        });

    }
    
    public void Show(GameObject target)
    {
        target.SetActive(true);
        target.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
    }
 
}
