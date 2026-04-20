using UnityEngine;

public class ServerTime
{
    private const string TIME_API_URL = "";
}


[System.Serializable]
public struct ServerTimeResponse
{
    public int _unixTime;
}