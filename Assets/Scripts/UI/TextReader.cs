using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextReader : MonoBehaviour
{
	static void ReadString()
	{
		string path = "Assets/AlphaAssets/Text/test.rtf";

		//Read the text from directly from the test.rtf file
		StreamReader reader = new StreamReader(path);
		Debug.Log(reader.ReadToEnd());
		reader.Close();
	}

}
