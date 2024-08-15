using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    
    public List<EnemyBase> Enemy_targetsList = new List<EnemyBase>();
    // Start is called before the first frame update
    void Start()
    {
        Enemy_targetsList.AddRange(FindObjectsOfType<EnemyBase>());

        
    }
    
    
    public void RemoveEnemy(GameObject obj)
    {
        for (int i = 0; i < Enemy_targetsList.Count; i++)
        {
            if (Enemy_targetsList[i].gameObject==obj)
            {
                Enemy_targetsList.RemoveAt(i);
                Debug.Log("적 제거");
            }
        }
        
    }
    
}
