using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class PortMan : MonoBehaviour {

    public static SerialPort sPort = new SerialPort("COM9", 115200);

    void Start()
    {
        sPort.Open();
    }

    public static void Write(int data)
    {
        if (sPort.IsOpen)
        {
            sPort.WriteLine(data + ",");
        }
    }

    public static double[] Read()
    {
        double[] dist = null;
        if (sPort.IsOpen)
        {
            try
            {
				// Clear buffer
                sPort.DiscardInBuffer();
                string rawData = sPort.ReadLine();
                if (rawData.Length > 2)
                {
                    char delimiter = ',';
					// Spit into array of Strings using ',' as the delimiter
                    string[] distancestrings = rawData.Split(delimiter);
                    dist = new double[distancestrings.Length];
                    for (int i = 0; i < distancestrings.Length; i++)
                    {   
						// converts each string to a double
                        dist[i] = double.Parse(distancestrings[i]);
                    }
                }
            }
            catch (System.Exception)
            {
            }
        }
        return dist;
    }

    void OnApplicationQuit()
    {
        sPort.Close();
    }
}
