using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Grid gridReference;
    public float MoveSpeed = 8;
    Coroutine MoveIE;
    Coroutine MoveRoutine;

    private bool isMoving = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isMoving)
            {
                MoveRoutine = StartCoroutine(moveObject());
                isMoving = true;
            }
            else
            {
                StopCoroutine(MoveRoutine);
                StopCoroutine(MoveIE);
                isMoving = false;
            }
        }
    }

    IEnumerator moveObject()
    {
        for (int i = 0; i < gridReference.FinalPath.Count - 1; i++)
        {
            MoveIE = StartCoroutine(Moving(i));
            yield return MoveIE;
        }
    }

    IEnumerator Moving(int currentPosition)
    {
        while (transform.position != gridReference.FinalPath[currentPosition].vPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, gridReference.FinalPath[currentPosition].vPosition, MoveSpeed * Time.deltaTime);
            yield return null;
        }

    }
}
