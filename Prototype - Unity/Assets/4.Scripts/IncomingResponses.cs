using System.Collections.Generic;
using UnityEngine;

public class IncomingResponses : MonoBehaviour
{
    public string _LatestResponse;
    public List<string> _AllResponses;

    private void Awake()
    {
        _AllResponses = new List<string>();
    }

    public void AddResponse(string response) 
    {
        _AllResponses.Add(response);
    }

    public string GetLatestResponse() 
    {
        if(_AllResponses.Count > 0) 
        {
            return _AllResponses[0];
        }

        return null;
    }

    public void RemoveLatestResponse() 
    {
        if (_AllResponses.Count > 0) 
        {
            _AllResponses.RemoveAt(0);
        }
    }
}
