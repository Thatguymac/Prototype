using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TCPServer : MonoBehaviour
{
    private TcpClient _Client;
    private TcpListener _Server;
    private Thread _Thread;
    private NetworkStream _Stream;
    private enum _Commands 
    {
        Start,
        Stop,
        Quit
    }
    private IncomingResponses _IncomingResponses;
    
    [Header("Server Settings")]
    [SerializeField] private int _ConnectionPort = 25001;
    [SerializeField] private bool _IsConnected;

    [Header("Functional Information")]
    [SerializeField] private bool _IsRecording;
    [SerializeField] private Image _RecordingButtonImage;
    [SerializeField] private Sprite _StartSprite;
    [SerializeField] private Sprite _StopSprite;

    //Iniatilses the thread and waits for connection from python
    private void Start()
    {
        _IncomingResponses = this.GetComponent<IncomingResponses>();
        ThreadStart _ThreadStart = new ThreadStart(Listener);
        _Thread = new Thread(_ThreadStart);
        _Thread.Start();
    }

    private void Listener() 
    {
        //Start the server with _ConnectionPort
        _Server = new TcpListener(IPAddress.Any, _ConnectionPort);
        _Server.Start();
        Debug.Log("Waiting for Connection...");

        //Wait for python to connect to _Server
        _Client = _Server.AcceptTcpClient();
        Debug.Log("Connection Established with " + _Client);
        _IsConnected = true; // Server Connected

        //While connected, process incoming data
        while (_IsConnected) 
        {
            ProcessIncomingData();
        }

        //Otherwise, close the connection
        _Server.Stop();
    }

    private void ProcessIncomingData()
    {
        //Read incoming data from stream
        _Stream = _Client.GetStream(); 
        byte[] _Buffer = new byte[_Client.ReceiveBufferSize];
        int _BytesRead = _Stream.Read(_Buffer, 0, _Client.ReceiveBufferSize);

        //Decode the bytes into a string
        string _DataReceived = Encoding.UTF8.GetString(_Buffer, 0, _BytesRead);

        //Make sure the data retreived is not empty
        if(!string.IsNullOrEmpty(_DataReceived)) 
        {
            _IncomingResponses._LatestResponse = _DataReceived;
            _IncomingResponses.AddResponse(_IncomingResponses._LatestResponse);
        }
    }

    private bool ConnectionEstablished() 
    {
        if (_Client != null && _Client.Connected) 
        {
            return true;
        }

        return false;
    }

    private void SendToPython(string message)
    {
        if (ConnectionEstablished())
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            _Stream.Write(data, 0, data.Length);
        }
        else
        {
            Debug.Log("No connection established or client is null.");
        }
    }

    private string CommandToString(_Commands _Command) 
    {
        switch (_Command) 
        {
            case _Commands.Start:
                return "Start_Recording";

            case _Commands.Stop:
                return "Stop_Recording";

            case _Commands.Quit:
                return "Quit_From_Unity";

            default:
                throw new ArgumentException("Unsupported command", nameof(_Command));
        }
    }

    public void StartStopRecording() 
    {
        if (ConnectionEstablished())
        {
            _IsRecording = !_IsRecording;

            _RecordingButtonImage.sprite = _IsRecording ? _StopSprite : _StartSprite;

            if (_IsRecording)
            {
                SendToPython(CommandToString(_Commands.Start));
            }

            if (!_IsRecording)
            {
                SendToPython(CommandToString(_Commands.Stop));
            }
        }
        else
        {
            Debug.Log("No connection established or client is null.");
        }
    }

    public void QuitApplication()
    {
        //Check if there is a current connection to Python
        if (ConnectionEstablished())
        {
            SendToPython(CommandToString(_Commands.Quit)); //Tell Python to close the server
            _Server.Stop(); // Stop the connection

            //Stop the Unity Editor from running
            UnityEditor.EditorApplication.isPlaying = false;
            Debug.Log("Shutting Down Server...");
            Debug.Log("Quitting Unity...");
        }
        else
        {
            //Otherwise, only stop the Unity Editor from running
            UnityEditor.EditorApplication.isPlaying = false;
            Debug.Log("Quitting Unity...");
        }
    }
}
