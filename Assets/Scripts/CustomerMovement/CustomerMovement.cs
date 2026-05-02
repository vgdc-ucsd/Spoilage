using UnityEngine;
using System.Collections;

public class CustomerMovement : MonoBehaviour
{
    //How many times the coroutine plays
    private float _walkDuration = 6f;
    //how many times the customer should go up and down
    private float _bobs = 3;
    //how high the character should bob
    private float _bobHeight = 0.5f;
    //end position
    private Vector3 _targetPos;
    //starting position
    private Vector3 _startPos;
    //elapsed time
    private float _elapsed = 0f;
    //how far into the walk cycle we are
    private float _percent = 0f;
    //the new X position of the customer
    private float _newX = 0f;
    //the y offset of the walk bobbing up and down
    private float _yOffset = 0f;
    //the X position the people want the customer to end at
    [SerializeField]private float XMainPosition = 0f;

    // Arbitrary position outside the camera (how should we account for whether or not the camera
    // isn't the same size? or do we not care?)
    private float XEndPosition = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Beautiful thing about this is it doesn't matter the intial position of
        //the off screen character it will still arrive at the same spot.
        _targetPos = new Vector3(XMainPosition,transform.position.y,transform.position.z);
        _startPos = transform.position;

        // Setting character position to Main position just to test walking off
        // transform.position = _targetPos;

        //for now I just put WalkOnScreen() here in order to test it
        WalkOnScreen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This method should take the off screen customer into place
    void WalkOnScreen()
    {
        StartCoroutine(Walk());
    }

    //this method should just basically be a copy of above just it goes off screen. so change start and target position
    void WalkOffScreen()
    {
        _targetPos = new Vector3(XEndPosition,transform.position.y,transform.position.z);
        _startPos = transform.position;
        StartCoroutine(Walk());
    }

    //How I make it wait in between every frame
    //No I do not understand IEnumerator I'm just trying things out praying it works
    private IEnumerator Walk()
    {
        _elapsed = 0f;
        while (_elapsed < _walkDuration)
        {
            _elapsed += Time.deltaTime;
            _percent = _elapsed/_walkDuration;

            //calculates the movement necessary
            _newX = Mathf.Lerp(_startPos.x,_targetPos.x,_percent);
            _yOffset = Mathf.Cos(_percent * Mathf.PI * 2 * _bobs) * _bobHeight;
            //moves the charcter to the right
            transform.position = new Vector3(_newX,_startPos.y + _yOffset,_startPos.z);
            //waits a frame
            yield return null;
        }

        //fixes any float offset since floats aren't perfect.
        transform.position = _targetPos;
    }
}
