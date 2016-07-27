using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Net;

public class NetMan1 : MonoBehaviour {

	private int myReliableChannelId;
	private string strSocketConnOpen = "Socket open. SocketID: ";
	private string strConnToServ = "Connected to server. ConnectionID: ";
	//private string msg = "hello";

	public int maxConnections = 10;
	public int socketId;
	public int socketPort = 8888;
	public string ipAddress = "169.254.169.175";
	public int connectionId;
	public int bufferSize = 1024;

	public GameObject cam;
	private int count = 0;
	private Boolean isVector = false;
	Vector3 oldV = Vector3.zero;
	Quaternion oldQ = Quaternion.identity;
	Vector3 newV = Vector3.zero;
	Quaternion newQ = Quaternion.identity;
	public static Vector3 dataV;
	public static Quaternion dataQ;
	

	// Use this for initialization
	void Start () 
	{
		NetworkTransport.Init ();
		ConnectionConfig config = new ConnectionConfig ();
		myReliableChannelId = config.AddChannel (QosType.Unreliable);
		HostTopology topology = new HostTopology (config, maxConnections);
		socketId = NetworkTransport.AddHost (topology, socketPort);
		Debug.Log (strSocketConnOpen + socketId.ToString());
		DebugConsole.Log (strSocketConnOpen + socketId.ToString ());

		Connect();

	}

	public void Connect() 
	{
		byte error;
		connectionId = NetworkTransport.Connect (socketId, ipAddress, socketPort, 0, out error);
		Debug.Log (strConnToServ + connectionId);
		DebugConsole.Log (strConnToServ + connectionId);
	}

	/*
	public void OnGUI()
	{
		GUILayout.BeginHorizontal ();
		ipAddress = GUILayout.TextField (ipAddress);
		GUILayout.Label("IP Address");
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal ();
		string tempPort;
		tempPort = GUILayout.TextField (socketPort.ToString ());
		socketPort = int.Parse (tempPort);
		GUILayout.Label ("Port");
		GUILayout.EndHorizontal ();

		if (GUILayout.Button ("Connect")) 
		{
			Debug.Log("Connecting....");
			DebugConsole.Log ("Connecting....");
			Connect();
		}


		GUILayout.BeginHorizontal ();
		/*string tempMsg;
		tempMsg = GUILayout.TextField("Enter message here");
		msg = tempMsg;
		GUILayout.Label("Message");
		GUILayout.EndHorizontal ();

		if (GUILayout.Button ("Send Message")) 
		{
			Debug.Log("Sending message...");
			Debug.Log (isVector);
			DebugConsole.Log("Sending message...");
			//SendVector3 (cam.transform.position);
			//SendQuaternion (cam.transform.rotation);
		}
		 
	}
	*/

	/*
	public void SendSocketMessage(string msg) 
	{
		byte error;
		byte[] buffer = new byte[bufferSize];
		Stream stream = new MemoryStream (buffer);
		BinaryFormatter formatter = new BinaryFormatter ();
		formatter.Serialize (stream, msg);

		NetworkTransport.Send (socketId, 
		                       connectionId, 
		                       myReliableChannelId, 
		                       buffer, 
		                       bufferSize, 
		                       out error);

	}*/

	public void SendVector3(Vector3 msg) 
	{
		byte error;
		byte[] buffer = new byte[bufferSize];
		Stream stream = new MemoryStream (buffer);
		BinaryFormatter formatter = new BinaryFormatter ();
		int netx = IPAddress.HostToNetworkOrder ((int)(msg.x*10000));
		int nety = IPAddress.HostToNetworkOrder ((int)(msg.y*10000));
		int netz = IPAddress.HostToNetworkOrder ((int)(msg.z*10000));
		int netw = IPAddress.HostToNetworkOrder (10000*10000);
		this.isVector = true;
		int[] data = {netx,nety,netz,netw};
		//DebugConsole.Log (data[1].ToString());
		formatter.Serialize (stream, data);
		
		NetworkTransport.Send (socketId, 
		                       connectionId, 
		                       myReliableChannelId, 
		                       buffer, 
		                       bufferSize, 
		                       out error);


	}

	public void SendQuaternion(Quaternion msg) 
	{
		byte error;
		byte[] buffer = new byte[bufferSize];
		Stream stream = new MemoryStream (buffer);
		BinaryFormatter formatter = new BinaryFormatter ();
		int netx = IPAddress.HostToNetworkOrder ((int)(msg.x*10000));
		int nety = IPAddress.HostToNetworkOrder ((int)(msg.y*10000));
		int netz = IPAddress.HostToNetworkOrder ((int)(msg.z*10000));
		int netw = IPAddress.HostToNetworkOrder ((int)(msg.w*10000));
		this.isVector = false;
		int[] data = {netx,nety,netz,netw};
		formatter.Serialize (stream, data);
		
		NetworkTransport.Send (socketId, 
		                       connectionId, 
		                       myReliableChannelId, 
		                       buffer, 
		                       bufferSize, 
		                       out error);


	}

	// Update is called once per frame
	void Update () 
	{
		/*

		#if UNITY_ANDROID && !UNITY_EDITOR
		count++;
		if (count % 60 == 0) {
			SendVector3 (cam.transform.position);
			SendQuaternion (cam.transform.rotation);
		}
		#endif
		*/

		#if UNITY_ANDROID && !UNITY_EDITOR

		SendVector3 (cam.transform.position);
		SendQuaternion (cam.transform.rotation);

		#endif

		int recHostId;
		int recConnectionId;
		int recChannelId;
		byte[] recBuffer = new byte[bufferSize];
		int dataSize;
		byte error;
		NetworkEventType recNetworkEvent = NetworkTransport.Receive (out recHostId, 
		                                                            out recConnectionId, 
		                                                            out recChannelId, 
		                                                            recBuffer, 
		                                                            bufferSize, 
		                                                            out dataSize,
		                                                            out error);

		switch (recNetworkEvent) 
		{
			case NetworkEventType.Nothing:
				break;
			case NetworkEventType.ConnectEvent:
				Debug.Log("Incoming conenction event received.");
				DebugConsole.Log("Incoming conenction event received.");
				break;
			case NetworkEventType.DataEvent:
				Stream stream = new MemoryStream(recBuffer);
				BinaryFormatter formatter = new BinaryFormatter();

				int[] message = formatter.Deserialize(stream) as int[];
				float tempx = IPAddress.NetworkToHostOrder (message[0])/10000.0f;
				float tempy = IPAddress.NetworkToHostOrder (message[1])/10000.0f;
				float tempz = IPAddress.NetworkToHostOrder (message[2])/10000.0f;
				float tempw = IPAddress.NetworkToHostOrder (message[3])/10000.0f;
				float boolCheck = tempw - 1;
			if( boolCheck == 9999 ){
				//Debug.Log ("Vector");
				//DebugConsole.Log ("Vector");
				newV = new Vector3(tempx, tempy, tempz);
				//dataV = newV - oldV;
				//oldV = newV;
				dataV = newV;	
			}else{
				//Debug.Log ("Quaternion");
				//DebugConsole.Log ("Quaternion");
				newQ = new Quaternion(tempx, tempy, tempz, tempw);
				dataQ = newQ;
				//oldQ = newQ;
			}

			/*
				Debug.Log (isVector);
				if(isVector){
					Debug.Log ("true");
					//Vector3 data = new Vector3(tempx, tempy, tempz);
				}else if(!isVector){
					Debug.Log ("but why");
					
					//Quaternion data = new Quaternion(tempx, tempy, tempz, tempw);
				}
				*/

				Debug.Log("Incoming message event received x:" + tempx +" y:"+ tempy +" z:"+ tempz +" w:"+ tempw);
				DebugConsole.Log("Incoming message event received x:" + tempx +" y:"+ tempy +" z:"+ tempz +" w:"+ tempw);
				break;
			case NetworkEventType.DisconnectEvent:
				Debug.Log("Remote client event disconnected");
				DebugConsole.Log("Remote client event disconnected");
				Connect();
				break;
		}

        dataV.y = 1.72f;
        transform.position = dataV;
   
        Vector3 dataEuler = dataQ.eulerAngles;
        dataEuler.x = dataEuler.z = 0f;
        dataQ = Quaternion.Euler(dataEuler);
        transform.rotation = dataQ;
        
    }
}