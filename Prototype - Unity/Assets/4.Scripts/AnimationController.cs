using Animancer;
using UnityEngine;

public sealed class AnimationController : MonoBehaviour
{
    [SerializeField] private NamedAnimancerComponent _Animancer;
    [SerializeField] private GameObject _Utility; //GameObject that holds the Signs, IncomingWord list and CurrentWord

    [SerializeField] private AnimationClip _DefaultAnimation; //Idle Animation
    [SerializeField] private float _DefaultFadeDuration = 0.5f;
    [SerializeField] private int _DefaultSpeed = 2;

    private AnimationClip[] _Animations;
    private string _CurrentWord;
    private bool _IsPlaying; //True or False depending on if the animation is playing or not

    private void Start()
    {
        InitialiseAnimations(); //Register the animations
    }

    private void Update()
    {
        PlayAnimations(); //Play the animations
    }

    //Gets the animations and registers them to the NamedAnimancerComponent, Sets the Default animation to Idle if IncomingResponses list is empty
    private void InitialiseAnimations() 
    {
        Debug.Log("Registering Animations...");

        _Animations = _Utility.GetComponent<Animations>().GetAllAnimations();
        _Animancer.Animations = _Animations;
        
        var state = _Animancer.Play(_DefaultAnimation);
        state.Speed = _DefaultSpeed;
    }

    //If the current word is not null then the animations will play depending on the current word
    public void PlayAnimations() 
    {
        _CurrentWord = _Utility.GetComponent<IncomingResponses>().GetLatestResponse();

        if (_CurrentWord != null)
        {
            Sign _SignParameters = _Utility.GetComponent<Animations>().GetSignByName(_CurrentWord);

            if (_SignParameters.Name != "Not_Found" && !_IsPlaying) //If the animation clip is not null and no animation is currently playing
            {
                _IsPlaying = true; //Animation started playing

                var state = _Animancer.Play(_SignParameters.Animation, _SignParameters.FadeDuration); //play the animation from the start with a blend amount
                state.Speed = _SignParameters.Speed; //adjust the speed of the animation

                state.Events.OnEnd = OnAnimationEnd; //On end of animation, run OnAnimationEnd method
            }
            else if (_SignParameters.Name == "Not_Found")
            {
                Debug.Log(_CurrentWord + " was not found.");
                RemoveCurrentWord();
            }
        }
    }

    //On the end of an animation, this method removes the first word from the wordList and allows for the next animation to play unless current word is null
    private void OnAnimationEnd() 
    {
        RemoveCurrentWord();
        _IsPlaying = false; //animation stopped

        if (_Utility.GetComponent<IncomingResponses>().GetLatestResponse() == null)  //If current word is null then there is no more words in the list, set the default animation
        { 
            var state = _Animancer.Play(_DefaultAnimation, _DefaultFadeDuration);
            state.Speed = _DefaultSpeed;
        }
    }

    //ADD FUNCTION TO CHECK IF AN SIGN IS AVAILABLE FOR THE LATEST RESPONSE

    private void RemoveCurrentWord() 
    {
        Debug.Log("Removing " + _Utility.GetComponent<IncomingResponses>().GetLatestResponse());
        _Utility.GetComponent<IncomingResponses>().RemoveLatestResponse(); //Remove the first word to allow the next animation to play
    }
}