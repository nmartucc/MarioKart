/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VR = UnityEngine.VR;
using System.IO.Ports;
using System.Threading;
//-------------------------------------------------------------------------------------
/// <summary>
/// Shows debug information on a heads-up display.
/// </summary>
public class MYOVRDebugInfo : MonoBehaviour
{
	public KartController kart;

    #region GameObjects for Debug Information UIs   
    GameObject debugUIManager;
    GameObject debugUIObject;
	GameObject img;
	GameObject item;
	GameObject warn;
	GameObject imgManager;
	GameObject itemManager;
	GameObject warning1Manager;
	GameObject warning2Manager;
	GameObject warning3Manager;
    GameObject riftPresent;    
    GameObject fps;    
    GameObject ipd;
    GameObject fov;
    GameObject height;
	GameObject depth;
	GameObject resolutionEyeTexture;
    GameObject latencies;
    GameObject texts;
	GameObject sensor1; // FRONT
	GameObject sensor2; // LEFT
	GameObject sensor3; // RIGHT 
	GameObject warning1;
	GameObject warning2;
	GameObject warning3;
	public Image caution;
	public Image mushroom;
    #endregion

    public static double[] sensordata;
	double dist1 = double.PositiveInfinity;
	double dist2 = double.PositiveInfinity;
	double dist3 = double.PositiveInfinity;
	// distance below which warning displayed, in meters
	double mindist = 1;
	double mindistFront = 2;

    #region Debug strings
	string strRiftPresent            = null; // "VR DISABLED"
    string strFPS                    = null; // "FPS: 0";
    string strIPD                    = null; // "IPD: 0.000";
    string strFOV                    = null; // "FOV: 0.0f";
    string strHeight                 = null; // "Height: 0.0f";
	string strDepth                  = null; // "Depth: 0.0f";
	string strResolutionEyeTexture   = null; // "Resolution : {0} x {1}"
    string strLatencies              = null; // "R: {0:F3} TW: {1:F3} PP: {2:F3} RE: {3:F3} TWE: {4:F3}"
	string strSensor1 = null;
	string strSensor2 = null;
	string strSensor3 = null;
    #endregion

	bool shouldexit = false;
    /// <summary>
    /// Variables for FPS
    /// </summary>
    float updateInterval = 0.5f;
    float accum          = 0.0f;
    int   frames         = 0;
    float timeLeft       = 0.0f;

    /// <summary>
    /// Managing for UI initialization
    /// </summary>
    bool  initUIComponent = false;
    bool  isInited        = false;

	bool initImage = false;
	bool isInitedImage = false;

	bool initItem = false;
	bool isInitedItem = false;

	bool initWarning1 = false;
	bool isInitedWarning1 = false;

	bool initWarning2 = false;
	bool isInitedWarning2 = false;

	bool initWarning3 = false;
	bool isInitedWarning3 = false;

    /// <summary>
    /// UIs Y offset
    /// </summary>
    float offsetY = 55.0f;

    /// <summary>
    /// Managing for rift detection UI
    /// </summary>
    float riftPresentTimeout = 0.0f;

    /// <summary>
    /// Turn on / off VR variables
    /// </summary>
    bool showVRVars = false;
	bool showImg = false;
	bool showWarn1 = false;
	bool showWarn2 = false;
	bool showWarn3 = false;
	bool showIt = false;

	public static bool showMushroom = false;

	int sensorCount;

    #region MonoBehaviour handler

    /// <summary>
    /// Initialization
    /// </summary>
    void Awake()
    {
        // Create canvas for using new GUI
        debugUIManager = new GameObject();
        debugUIManager.name = "DebugUIManager";
        debugUIManager.transform.parent = GameObject.Find("LeftEyeAnchor").transform;

		imgManager = new GameObject ();
		imgManager.name = "ImageManager";
		imgManager.transform.parent = GameObject.Find("LeftEyeAnchor").transform;

		itemManager = new GameObject ();
		itemManager.name = "ItemManager";
		itemManager.transform.parent = GameObject.Find("LeftEyeAnchor").transform;

		warning1Manager = new GameObject ();
		warning1Manager.name = "Warning1Manager";
		warning1Manager.transform.parent = GameObject.Find("LeftEyeAnchor").transform;

		warning2Manager = new GameObject ();
		warning2Manager.name = "Warning2Manager";
		warning2Manager.transform.parent = GameObject.Find("LeftEyeAnchor").transform;

		warning3Manager = new GameObject ();
		warning3Manager.name = "Warning3Manager";
		warning3Manager.transform.parent = GameObject.Find("LeftEyeAnchor").transform;

        RectTransform rectTransform = debugUIManager.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100f, 100f);
        rectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        rectTransform.localPosition = new Vector3(0.01f, 0.17f, 0.53f);
        rectTransform.localEulerAngles = Vector3.zero;

        Canvas canvas = debugUIManager.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
		canvas.pixelPerfect = false;

		RectTransform imgRectTransform = imgManager.AddComponent<RectTransform>();
		imgRectTransform.sizeDelta = new Vector2(100f, 100f);
		imgRectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		imgRectTransform.localPosition = new Vector3(0.01f, 0.17f, 0.53f);
		imgRectTransform.localEulerAngles = Vector3.zero;
		
		Canvas imgCanvas = imgManager.AddComponent<Canvas>();
		imgCanvas.renderMode = RenderMode.WorldSpace;
		imgCanvas.pixelPerfect = false;

		RectTransform itemRectTransform = itemManager.AddComponent<RectTransform>();
		itemRectTransform.sizeDelta = new Vector2(100f, 100f);
		itemRectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		itemRectTransform.localPosition = new Vector3(0.01f, 0.17f, 0.53f);
		itemRectTransform.localEulerAngles = Vector3.zero;

		Canvas itemCanvas = itemManager.AddComponent<Canvas>();
		itemCanvas.renderMode = RenderMode.WorldSpace;
		itemCanvas.pixelPerfect = false;

		RectTransform w1RectTransform = warning1Manager.AddComponent<RectTransform>();
		w1RectTransform.sizeDelta = new Vector2(100f, 100f);
		w1RectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		w1RectTransform.localPosition = new Vector3(0.01f, 0.17f, 0.53f);
		w1RectTransform.localEulerAngles = Vector3.zero;
		
		Canvas w1Canvas = warning1Manager.AddComponent<Canvas>();
		w1Canvas.renderMode = RenderMode.WorldSpace;
		w1Canvas.pixelPerfect = false;

		RectTransform w2RectTransform = warning2Manager.AddComponent<RectTransform>();
		w2RectTransform.sizeDelta = new Vector2(100f, 100f);
		w2RectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		w2RectTransform.localPosition = new Vector3(0.01f, 0.17f, 0.53f);
		w2RectTransform.localEulerAngles = Vector3.zero;
		
		Canvas w2Canvas = warning2Manager.AddComponent<Canvas>();
		w2Canvas.renderMode = RenderMode.WorldSpace;
		w2Canvas.pixelPerfect = false;

		RectTransform w3RectTransform = warning3Manager.AddComponent<RectTransform>();
		w3RectTransform.sizeDelta = new Vector2(100f, 100f);
		w3RectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		w3RectTransform.localPosition = new Vector3(0.01f, 0.17f, 0.53f);
		w3RectTransform.localEulerAngles = Vector3.zero;
		
		Canvas w3Canvas = warning3Manager.AddComponent<Canvas>();
		w3Canvas.renderMode = RenderMode.WorldSpace;
		w3Canvas.pixelPerfect = false;
    }

	void Start(){
		Thread mythread = new Thread (new ThreadStart (UpdateSensors));
		mythread.Start ();
	}

    /// <summary>
    /// Updating VR variables and managing UI present
    /// </summary>
    void Update()
    {

		if (initUIComponent && !isInited) {
			InitUIComponents ();
		}

		if (Input.GetKeyDown(KeyCode.Space) && riftPresentTimeout < 0.0f)
		{
			initUIComponent = true;
			showVRVars ^= true;
		}

		UpdateDeviceDetection();
		
		// Presenting VR variables
		if (showVRVars)
		{
			debugUIManager.SetActive(true);
			imgManager.SetActive(false);
			warning1Manager.SetActive(false);
			warning2Manager.SetActive(false);
			warning3Manager.SetActive(false);
			UpdateVariable();
			UpdateStrings();           
		}
		else
		{
			debugUIManager.SetActive(false);
		}

		if (!showVRVars) {
			if (initImage && !isInitedImage) {
				showImage ();
			}

			if (initItem && !isInitedItem) {
				showItem ();
			}

			if (initWarning1 && !isInitedWarning1) {
				showWarning1 ();
			}

			if (initWarning2 && !isInitedWarning2) {
				showWarning2 ();
			}

			if (initWarning3 && !isInitedWarning3) {
				showWarning3 ();
			}

			// if any of the distaces are below the minimums, display the caution image
			if ((dist1 < mindistFront || dist2 < mindist || dist3 < mindist) && riftPresentTimeout < 0.0f) {
				initImage = true;
				showImg = true;
			} else {
				initImage = false;
				showImg = false;
			}

			if (dist1 < mindistFront) {
				initWarning1 = true;
				showWarn1 = true;
			} else {
				initWarning1 = false;
				showWarn1 = false;
			}

			if (dist2 < mindist) {
				initWarning2 = true;
				showWarn2 = true;
			} else {
				initWarning2 = false;
				showWarn2 = false;
			}

			if (dist3 < mindist) {
				initWarning3 = true;
				showWarn3 = true;
			} else {
				initWarning3 = false;
				showWarn3 = false;
			}

			if (showMushroom) {
				initItem = true;
				showIt = true;
			} else {
				initItem = false;
				showIt = false;
			}

			if (showWarn1) {
				warning1Manager.SetActive (true);
			} else
				warning1Manager.SetActive (false);

			if (showWarn2) {
				warning2Manager.SetActive (true);
			} else
				warning2Manager.SetActive (false);

			if (showWarn3) {
				warning3Manager.SetActive (true);
			} else
				warning3Manager.SetActive (false);

			if (showImg) {
				imgManager.SetActive (true);
			} else
				imgManager.SetActive (false);

			if (showIt) {
				itemManager.SetActive (true);
			} else
				itemManager.SetActive (false);

		}
    }

    /// <summary>
    /// Initialize isInited value on OnDestroy
    /// </summary>
    void OnDestroy()
    {
        isInited = false;
		isInitedImage = false;
		isInitedItem = false;
    }
    #endregion

    #region Private Functions

	// Caution image
	void showImage(){
		img = new GameObject();
		img.name = "Image";
		img.transform.parent = GameObject.Find("ImageManager").transform;
		img.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		img.transform.localEulerAngles = Vector3.zero;
		img.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);

		img.AddComponent<Image> ();
		img.GetComponent<Image> ().sprite = caution.sprite;
		Color c = caution.color;
		c.a = 0.7f;
		img.GetComponent<Image> ().color = c;

		initImage = false;
		isInitedImage = true;

	}

	// Item image
	void showItem(){
		item = new GameObject();
		item.name = "Item";
		item.transform.parent = GameObject.Find("ItemManager").transform;
		item.transform.localPosition = new Vector3(-200.0f, 100.0f, 0.0f);
		item.transform.localEulerAngles = Vector3.zero;
		item.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);

		item.AddComponent<Image> ();
		// If the item is a mushroom
		if (showMushroom) {
			item.GetComponent<Image> ().sprite = mushroom.sprite;
			Color c = mushroom.color;
			c.a = 0.8f;
			item.GetComponent<Image> ().color = c;
		}

		initItem = false;
		isInitedItem = true;

	}

	void showWarning1(){
		warn = new GameObject();
		warn.name = "Warning";
		warn.transform.parent = GameObject.Find("Warning1Manager").transform;
		warn.transform.localPosition = new Vector3(0.0f, -140.0f, 0.0f);
		warn.transform.localEulerAngles = Vector3.zero;
		warn.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

		warning1 = VariableObjectManager (warning1, "Warning", 0f, "Object Ahead", 30);

		initWarning1 = false;
		isInitedWarning1 = true;
	}

	void showWarning2(){
		warn = new GameObject();
		warn.name = "Warning";
		warn.transform.parent = GameObject.Find("Warning2Manager").transform;
		warn.transform.localPosition = new Vector3(0.0f, -140.0f, 0.0f);
		warn.transform.localEulerAngles = Vector3.zero;
		warn.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

		warning2 = VariableObjectManager (warning2, "Warning", -55f, "Object on Left", 30);
		
		initWarning2 = false;
		isInitedWarning2 = true;
	}

	void showWarning3(){
		warn = new GameObject();
		warn.name = "Warning";
		warn.transform.parent = GameObject.Find("Warning3Manager").transform;
		warn.transform.localPosition = new Vector3(0.0f, -140.0f, 0.0f);
		warn.transform.localEulerAngles = Vector3.zero;
		warn.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		
		warning3 = VariableObjectManager (warning3, "Warning", -110f, "Object on Right", 30);
		
		initWarning3 = false;
		isInitedWarning3 = true;
	}
    /// <summary>
    /// Initialize UI GameObjects
    /// </summary>
    void InitUIComponents()
    {
        float posY = 0.0f;
        int fontSize = 20;

        debugUIObject = new GameObject();
        debugUIObject.name = "DebugInfo";
        debugUIObject.transform.parent = GameObject.Find("DebugUIManager").transform;
        debugUIObject.transform.localPosition = new Vector3(0.0f, 240.0f, 0.0f);
        debugUIObject.transform.localEulerAngles = Vector3.zero;
        debugUIObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Print out for FPS
        if (!string.IsNullOrEmpty(strFPS))
        {
            fps = VariableObjectManager(fps, "FPS", posY -= offsetY, strFPS, fontSize);
        }

        // Print out for IPD
        if (!string.IsNullOrEmpty(strIPD))
        {
            ipd = VariableObjectManager(ipd, "IPD", posY -= offsetY, strIPD, fontSize);
        }

        // Print out for FOV
        if (!string.IsNullOrEmpty(strFOV))
        {
            fov = VariableObjectManager(fov, "FOV", posY -= offsetY, strFOV, fontSize);
        }

        // Print out for Height
        if (!string.IsNullOrEmpty(strHeight))
        {
            height = VariableObjectManager(height, "Height", posY -= offsetY, strHeight, fontSize);
        }
		
		// Print out for Depth
		if (!string.IsNullOrEmpty(strDepth))
		{
			depth = VariableObjectManager(depth, "Depth", posY -= offsetY, strDepth, fontSize);
		}

		// Print out for Resoulution of Eye Texture
        if (!string.IsNullOrEmpty(strResolutionEyeTexture))
        {
            resolutionEyeTexture = VariableObjectManager(resolutionEyeTexture, "Resolution", posY -= offsetY, strResolutionEyeTexture, fontSize);
        }

        // Print out for Latency
        if (!string.IsNullOrEmpty(strLatencies))
        {
            latencies = VariableObjectManager(latencies, "Latency", posY -= offsetY, strLatencies, 17);
        }

		if (!string.IsNullOrEmpty (strSensor1)) {
			sensor1 = VariableObjectManager(sensor1, "Sensor1", posY -= offsetY, strSensor1, fontSize);
		}

		if (!string.IsNullOrEmpty (strSensor2)) {
			sensor2 = VariableObjectManager(sensor2, "Sensor2", posY -= offsetY, strSensor2, fontSize);
		}

		if (!string.IsNullOrEmpty (strSensor3)) {
			sensor3 = VariableObjectManager(sensor3, "Sensor3", posY -= offsetY, strSensor3, fontSize);
			posY = 0.0f;
		}

        initUIComponent = false;
        isInited = true;

    }

    /// <summary>
    /// Update VR Variables
    /// </summary>
    void UpdateVariable()
    {        
        UpdateIPD();
        UpdateEyeHeightOffset();
		UpdateEyeDepthOffset();
		UpdateFOV();
        UpdateResolutionEyeTexture();
        UpdateLatencyValues();
        UpdateFPS();
    }

	void UpdateSensors(){
		while (shouldexit == false) {
			// Read line from Arduino
            double[] dist = PortMan.Read();
            if (dist != null)
            {
                sensordata = dist;
                double distance1 = dist[0] / 100;
                double distance2 = dist[2] / 100;
                double distance3 = dist[1] / 100;
				// Update respective sensors with distance data
                if (distance1 != 0)
                    UpdateSensor1(distance1);
                if (distance2 != 0)
                    UpdateSensor2(distance2);
                if (distance3 != 0)
                    UpdateSensor3(distance3);
            }
		}
	}

    /// <summary>
    /// Update Strings
    /// </summary>
    void UpdateStrings()
    {
        if (debugUIObject == null)
            return;       
                
        if (!string.IsNullOrEmpty(strFPS))
            fps.GetComponentInChildren<Text>().text = strFPS;
        if (!string.IsNullOrEmpty(strIPD))
            ipd.GetComponentInChildren<Text>().text = strIPD;
        if (!string.IsNullOrEmpty(strFOV))
            fov.GetComponentInChildren<Text>().text = strFOV;
        if (!string.IsNullOrEmpty(strResolutionEyeTexture))
            resolutionEyeTexture.GetComponentInChildren<Text>().text = strResolutionEyeTexture;
        if (!string.IsNullOrEmpty(strLatencies))
		{
            latencies.GetComponentInChildren<Text>().text = strLatencies;
			latencies.GetComponentInChildren<Text>().fontSize = 14;
		}
        if (!string.IsNullOrEmpty(strHeight))
            height.GetComponentInChildren<Text>().text = strHeight;
		if (!string.IsNullOrEmpty(strDepth))
			depth.GetComponentInChildren<Text>().text = strDepth;
		if (!string.IsNullOrEmpty (strSensor1))
			sensor1.GetComponentInChildren<Text>().text = strSensor1;
		if (!string.IsNullOrEmpty (strSensor2))
			sensor2.GetComponentInChildren<Text>().text = strSensor2;
		if (!string.IsNullOrEmpty (strSensor3))
			sensor3.GetComponentInChildren<Text>().text = strSensor3;
	}
	
	/// <summary>
    /// It's for rift present GUI
    /// </summary>
    void RiftPresentGUI(GameObject guiMainOBj)
    {
        riftPresent = ComponentComposition(riftPresent);
        riftPresent.transform.SetParent(guiMainOBj.transform);
        riftPresent.name = "RiftPresent";
        RectTransform rectTransform = riftPresent.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        rectTransform.localEulerAngles = Vector3.zero;

        Text text = riftPresent.GetComponentInChildren<Text>();
        text.text = strRiftPresent;
        text.fontSize = 20;
    }

    /// <summary>
    /// Updates the device detection.
    /// </summary>
    void UpdateDeviceDetection()
    {
        if (riftPresentTimeout >= 0.0f)
        {
            riftPresentTimeout -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Object Manager for Variables
    /// </summary>
    /// <returns> gameobject for each Variable </returns>
    GameObject VariableObjectManager(GameObject gameObject, string name, float posY, string str, int fontSize)
    {
		gameObject = ComponentComposition(gameObject);
        gameObject.name = name;
		if (debugUIObject != null && name != "Warning")
			gameObject.transform.SetParent (debugUIObject.transform);
		else if (warn != null)
			gameObject.transform.SetParent (warn.transform);

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0.0f, posY -= offsetY, 0.0f);

        Text text = gameObject.GetComponentInChildren<Text>();
        text.text = str;
        text.fontSize = fontSize;
        gameObject.transform.localEulerAngles = Vector3.zero;

        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        return gameObject;
    }

    /// <summary>
    /// Component composition
    /// </summary>
    /// <returns> Composed gameobject. </returns>
    GameObject ComponentComposition(GameObject GO)
    {
        GO = new GameObject();
		GO.AddComponent<RectTransform> ();
		GO.AddComponent<CanvasRenderer> ();
		GO.AddComponent<Image> ();
		GO.GetComponent<RectTransform> ().sizeDelta = new Vector2 (350f, 50f);
		GO.GetComponent<Image> ().color = new Color (7f / 255f, 45f / 255f, 71f / 255f, 200f / 255f);
		texts = new GameObject ();
		texts.AddComponent<RectTransform> ();
		texts.AddComponent<CanvasRenderer> ();
		texts.AddComponent<Text> ();
		texts.GetComponent<RectTransform> ().sizeDelta = new Vector2 (350f, 50f);
		texts.GetComponent<Text> ().font = Resources.GetBuiltinResource (typeof(Font), "Arial.ttf") as Font;
		texts.GetComponent<Text> ().alignment = TextAnchor.MiddleCenter;
		texts.transform.SetParent (GO.transform);
		texts.name = "TextBox";
		return GO;
    }
    #endregion

    #region Debugging variables handler
    /// <summary>
    /// Updates the IPD.
    /// </summary>

	void UpdateSensor1(double dist)
	{
		dist1 = dist;
		strSensor1 = "Front Sensor: "+dist1+"m";
	}

	void UpdateSensor2(double dist)
	{
		dist2 = dist;
		strSensor2 = "Left Sensor: "+dist2+"m";
	}

	void UpdateSensor3(double dist)
	{
		dist3 = dist;
		strSensor3 = "Right Sensor: "+dist3+"m";
	}

    void UpdateIPD()
    {
        strIPD = System.String.Format("IPD (mm): {0:F4}", OVRManager.profile.ipd * 1000.0f);
    }

    /// <summary>
    /// Updates the eye height offset.
    /// </summary>
    void UpdateEyeHeightOffset()
    {
        float eyeHeight = OVRManager.profile.eyeHeight;
        strHeight = System.String.Format("Eye Height (m): {0:F3}", eyeHeight);
	}
	
	/// <summary>
	/// Updates the eye depth offset.
	/// </summary>
	void UpdateEyeDepthOffset()
	{
		float eyeDepth = OVRManager.profile.eyeDepth;
		strDepth = System.String.Format("Eye Depth (m): {0:F3}", eyeDepth);
	}
	
	/// <summary>
	/// Updates the FOV.
    /// </summary>
    void UpdateFOV()
    {
        OVRDisplay.EyeRenderDesc eyeDesc = OVRManager.display.GetEyeRenderDesc(VR.VRNode.LeftEye);
        strFOV = System.String.Format("FOV (deg): {0:F3}", eyeDesc.fov.y);   
    }

    /// <summary>
    /// Updates resolution of eye texture
    /// </summary>
    void UpdateResolutionEyeTexture()
    {
		OVRDisplay.EyeRenderDesc leftEyeDesc = OVRManager.display.GetEyeRenderDesc(VR.VRNode.LeftEye);
		OVRDisplay.EyeRenderDesc rightEyeDesc = OVRManager.display.GetEyeRenderDesc(VR.VRNode.RightEye);

		float scale = VR.VRSettings.renderScale;
        float w = (int)(scale * (float)(leftEyeDesc.resolution.x + rightEyeDesc.resolution.x));
        float h = (int)(scale * (float)Mathf.Max(leftEyeDesc.resolution.y, rightEyeDesc.resolution.y));

        strResolutionEyeTexture = System.String.Format("Resolution : {0} x {1}", w, h);
    }

    /// <summary>
    /// Updates latency values
    /// </summary>
    void UpdateLatencyValues()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
            OVRDisplay.LatencyData latency = OVRManager.display.latency;
            if (latency.render < 0.000001f && latency.timeWarp < 0.000001f && latency.postPresent < 0.000001f)
                strLatencies = System.String.Format("Latency values are not available.");
            else
                strLatencies = System.String.Format("Render: {0:F3} TimeWarp: {1:F3} Post-Present: {2:F3}\nRender Error: {3:F3} TimeWarp Error: {4:F3}",
                    latency.render,
                    latency.timeWarp,
                    latency.postPresent,
                    latency.renderError,
                    latency.timeWarpError);      
#endif
    }

    /// <summary>
    /// Updates the FPS.
    /// </summary>
    void UpdateFPS()
    {
        timeLeft -= Time.unscaledDeltaTime;
        accum += Time.unscaledDeltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeLeft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = frames / accum;

            strFPS = System.String.Format("FPS: {0:F2}", fps);

            timeLeft += updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
    #endregion

    void OnApplicationQuit()
    {
        shouldexit = true;
    }

}