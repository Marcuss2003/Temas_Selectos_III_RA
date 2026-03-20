using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;
using NUnit.Framework;
using Unity.VisualScripting;


public class MOVE : MonoBehaviour
{
    public GameObject model;
    public ObserverBehaviour[] ImageTargets;
    public int currentTarjet;
    public float speed= 1.0f;
    private bool isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void moveToNextMarker()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveModel());
        }
    }

    private IEnumerator MoveModel()
    {
        isMoving = true;
        ObserverBehaviour target = GetNextDetectedTarget();
        if (target == null)
        {
          isMoving = false;
          yield break;
        }

        Vector3 startPosition = model.transform.position;
        Vector3 endPosition = target.transform.position;

        float journey = 0;

        while (journey <= 1f)
        {
            journey += Time.deltaTime * speed;
            model.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
            yield return null;
        }
        currentTarjet = (currentTarjet + 1) % ImageTargets.Length;
        isMoving = false;
    }

    private ObserverBehaviour GetNextDetectedTarget()
    {
       foreach (ObserverBehaviour target in ImageTargets)
        {
            if (target != null && (target.TargetStatus.Status == Status.TRACKED || target.TargetStatus.Status == Status.EXTENDED_TRACKED))
            {
                return target;
            }

        }
        return null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
