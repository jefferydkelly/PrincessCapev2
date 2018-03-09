﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCLParser {
    static string structStart = "{\n";
    static string structEnd = "},\n";
    static string lineEnding = ",\n";

    public static string CreateAttribute<T>(string atrName, T val) {
       return string.Format("\"{0}\": \"{1}\"", atrName, val) + lineEnding;
    }

    public static string CreateArray(string atrName) {
        return string.Format("\"{0}\": [\n", atrName);
    }

	public static string CreateArray<T>(string atrName, List<T> data)
	{
		string info = string.Format("\"{0}\": [\n", atrName);
        foreach(T s in data) {
            info += CreateAttribute("El", s);
        }
        info += ArrayEnding;
        return info;
	}

    public static string ArrayEnding {
        get {
            return "]" + lineEnding;
        }
    }

    public static T ParseEnum<T>(string pcl) {
        string enu = ParseLine(pcl);
        return (T)(System.Enum.Parse(typeof(T), enu));
    }
    public static Vector3 ParseVector3(string pcl) {

        string line = ParseLine(pcl);
        string firstSub = line.Split('(')[1];
        firstSub = firstSub.Split(')')[0];
        string[] xyz = firstSub.Split(',');
        return new Vector3(float.Parse(xyz[0].Trim()), float.Parse(xyz[1].Trim()), float.Parse(xyz[2].Trim()));
    }

    public static int ParseInt(string pcl) {
        return int.Parse(ParseLine(pcl));
    }

    public static float ParseFloat(string pcl) {
        return float.Parse(ParseLine(pcl));
    }

    public static bool ParseBool(string pcl) {
        return bool.Parse(ParseLine(pcl));
    }

    public static Dictionary<string, string> ParseDictionary(string pcl) {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        string[] lines = pcl.Split('\n');
        for (int i = 1; i < lines.Length - 2; i++) {
            KeyValuePair<string, string> kvp = ParseKVP(lines[i]);
            dict.Add(kvp.Key, kvp.Value);
        }
        return dict;
    }

    static KeyValuePair<string, string> ParseKVP(string pcl) {
        string key = pcl.Substring(1, pcl.IndexOf(':') - 2);
		string val = pcl.Substring(pcl.IndexOf(':') + 3);
		val = val.Substring(0, val.Length - 2);

        return new KeyValuePair<string, string>(key, val);
    }
    public static List<TileStruct> ParseTiles(string pcl) {
        List<TileStruct> tiles = new List<TileStruct>();
        int ind = pcl.IndexOf('[') + 2;
        int lastInd = pcl.LastIndexOf(']');
        pcl = pcl.Substring(ind, lastInd - ind);
        string[] tilesList = pcl.Split('\n');

        for (int i = 0; i < tilesList.Length; i++)
        {
            if (tilesList[i] == "{")
            {
                List<string> toParse = new List<string>();
                int j = i + 1;
             
                while (j < tilesList.Length && !tilesList[j].Contains("}")) {
                   
                    toParse.Add(tilesList[j]);
                    j++;
                }
              
                tiles.Add(ParseTileStruct(toParse));
                i = j;
            }
        }
        return tiles;
    }

    public static TileStruct ParseTileStruct (List<string> tile) {
        TileStruct ts = new TileStruct();
     
        ts.id = -1;
        for (int i = 0; i < tile.Count; i++) {
            string s = tile[i];
         
			if (s.Contains("\"Name\": "))
			{
				ts.name = ParseLine(s);
            } else if (s.Contains("\"ID\":")) {
                ts.id = ParseInt(s);
            }
			else if (IsLine(s))
			{
                ts.AddInfo(s);
			}
        }
     
        return ts;
    }

    public static string ParseLine(string line) {
        string tName = line.Substring(line.IndexOf(':') + 3);
		tName = tName.Substring(0, tName.Length - 2);
        return tName;
    }

    static bool IsLine(string s) {
        return !(s.Contains("{") || s.Contains("}"));
    }

    public static string StructStart {
        get {
            return structStart;
        }
    }

    public static string StructEnd {
        get {
            return structEnd;
        }
    }

    public static string LineEnd {
        get {
            return lineEnding;
        }
    }
}

public class TileStruct {
    public string name;
    public int id;
    public List<string> info;
    private int currentIndex;

    public TileStruct() {
        currentIndex = 0;
        info = new List<string>();
    }
    public void AddInfo(string s) {
        info.Add(s);
    }

    public string NextLine {
        get {
            if (currentIndex < info.Count)
            {
                string line = info[currentIndex];
                currentIndex++;
                return line;

            } else {
                return null;
            }
        }
    }

	public string Peek
	{
		get
		{
			if (currentIndex < info.Count)
			{
                return info[currentIndex];
			}
			else
			{
				return null;
			}
		}
	}

    public void TossLine() {
        currentIndex++;
    }

    public bool FullyRead {
        get {
            return currentIndex >= info.Count;
        }
    }
}
