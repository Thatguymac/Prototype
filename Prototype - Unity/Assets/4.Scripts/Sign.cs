using UnityEngine;

//Creates a menu in the Unity Editor so that Signs can be created
[CreateAssetMenu(fileName = "New Sign", menuName = "Sign")] 
public class Sign : ScriptableObject
{
    [Header("Sign Parameters")]
    public string Name;
    public int Index;
    
    [Header("Animation Parameters")]
    public AnimationClip Animation;
    public float FadeDuration;
    public int Speed;

    public Sign (string name, int index, AnimationClip animation, float fadeDuration, int speed)
    {
        Name = name;
        Index = index;
        Animation = animation;
        FadeDuration = fadeDuration;
        Speed = speed;
    }

    public Sign(string name) 
    {
        Name = name;
    }
}
