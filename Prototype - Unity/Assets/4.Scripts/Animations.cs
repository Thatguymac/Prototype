using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class Animations : MonoBehaviour
{
    //List of Signs that will hold all the signs and their information
    public List<Sign> Signs = new List<Sign>();

    //Array of animations for all the signs
    private AnimationClip[] _Animations;

    private void Awake()
    {
        //Create an array for all the available sign animations
        _Animations = new AnimationClip[GetSignCount()];
    }

    //Gets all the animations from the sign objects and populates the _Animations variable
    public AnimationClip[] GetAllAnimations()
    {
        for (int i = 0; i < GetSignCount(); i++)
        {
            _Animations[i] = Signs[i].Animation;
        }
        
        return _Animations;
    }

    //Finds an animation based on it's name from the availabe sign names
    public Sign GetSignByName(string _AnimationName) 
    {
        foreach (Sign _Sign in Signs) 
        {
            if (_Sign.Name == _AnimationName) 
            {
                return _Sign;
            }
        }

        Sign _nullSign = new Sign("Not_Found");

        return _nullSign;
    }

    //Gets the number of available signs
    private int GetSignCount() 
    {
        return Signs.Count;
    }
}
