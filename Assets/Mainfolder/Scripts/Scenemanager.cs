using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;

public class Scenemanager : MonoBehaviour
{
    public TransitionSettings transition;
    public float startDelay;

        
    public void LoadScene(string _sceneName)
    {
        TransitionManager.Instance().Transition(_sceneName, transition, startDelay);
    }
    
    public void LoadScene(SceneReference sceneReference)
    {
        TransitionManager.Instance().Transition(sceneReference.ScenePath, transition, startDelay);
    }
    
}
