using UnityEngine;
using System.Collections;
using System;

// TODO: Fix snapping at end of move. Currently, if the customer is not at the
//       very bottom of their arc when they reach the destination X position,
//       they snap from their current offset to the final position with no
//       adjustment or smoothing in-between.

public class CustomerMovement : MonoBehaviour
{
    //how many times the customer should go up and down
    private float _bobs = 3;
    //how high the character should bob
    private float _bobHeight = 10f;

    public void WalkTo(Vector3 pos, float duration, Action callback)
    {
        StopAllCoroutines();
        StartCoroutine(Walk(pos, duration, callback));
    }

    //How I make it wait in between every frame
    //No I do not understand IEnumerator I'm just trying things out praying it works
    private IEnumerator Walk(Vector3 targetPos, float duration, Action callback)
    {
        Vector3 startPos = transform.position;
        yield return BasicAnimations.Interpolate(
            null,
            (t) => {
                float newX = Mathf.Lerp(startPos.x, targetPos.x, t);
                float yOffset = Mathf.Cos(t * Mathf.PI * 2 * _bobs) * _bobHeight;
                transform.position = new Vector3(newX, startPos.y + yOffset, startPos.z);
            },
            () => {
                transform.position = new Vector3(targetPos.x, startPos.y, startPos.z);
                callback?.Invoke();
            },
            duration
        );
    }
}
