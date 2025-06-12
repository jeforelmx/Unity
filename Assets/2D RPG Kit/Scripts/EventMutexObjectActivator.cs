using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventMutexObjectActivator : MonoBehaviour
{
    [Tooltip("Drag and drop the game object that should be activated or deactivated")]
    public GameObject objectToActivate;
    [Tooltip("Choose the event whose completion should be checked from the Event Manager")]
    public string eventToCheck;
    public string mutexeventToCheck;
    [Tooltip("Activate the game object when the chosen event was completed. Leave unchecked if you want to deactivate the game object instead")]
    public bool activeIfComplete;
    [Tooltip("Activate a delay before the activation")]
    public bool waitBeforeActivate;
    [Tooltip("Enter the duration for the delay in seconds")]
    public float waitTime;

    private bool initialCheckDone;

    public UnityEvent onActivate;



    // Update is called once per frame
    void Update()
    {
        if (!initialCheckDone)
        {
            //initialCheckDone = true;

            CheckCompletion();
        }
    }

    public void CheckCompletion()
    {
        bool eventchk = EventManager.instance.CheckIfComplete(eventToCheck);
        bool eventchk_mutex = EventManager.instance.CheckIfComplete(mutexeventToCheck);
        if (eventchk && !eventchk_mutex)
        {
            if (waitBeforeActivate)
            {
                StartCoroutine(waitCo());
            }
            else
            {
                objectToActivate.SetActive(activeIfComplete);
            }

        }
        if(eventchk && eventchk_mutex)
        {
            if (waitBeforeActivate)
            {
                StartCoroutine(waitCo());
            }
            else
            {
                objectToActivate.SetActive(!activeIfComplete);
            }
        }
    }

    IEnumerator waitCo()
    {
        yield return new WaitForSeconds(waitTime);
        objectToActivate.SetActive(activeIfComplete);
        onActivate?.Invoke();
    }
}
