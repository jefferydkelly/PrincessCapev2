using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCLParser {
    static string structStart = "{\n";
    static string structEnd = "},\n";
    static string lineEnding = ",\n";

    /// <summary>
    /// Creates an attribute string with the given name and value
    /// </summary>
	/// <returns>An attribute string with the given name and value.</returns>
    /// <param name="atrName">The name of the attribute.</param>
    /// <param name="val">V\The value of the attribute.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static string CreateAttribute<T>(string atrName, T val) {
       return string.Format("\"{0}\": \"{1}\"", atrName, val) + lineEnding;
    }

    /// <summary>
    /// Creates an array with the given name.
    /// </summary>
    /// <returns>The first line of the array in the file.</returns>
    /// <param name="atrName">The name of the array.</param>
    public static string CreateArray(string atrName) {
        return string.Format("\"{0}\": [\n", atrName);
    }

    /// <summary>
    /// Creates an array with the given name and populates it with the given values
    /// </summary>
    /// <returns>The string representing the array in the file.</returns>
    /// <param name="atrName">The name of the array.</param>
    /// <param name="data">The data that fills the array.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
	public static string CreateArray<T>(string atrName, List<T> data)
	{
		string info = string.Format("\"{0}\": [\n", atrName);
        foreach(T s in data) {
            info += CreateAttribute("El", s);
        }
        info += ArrayEnding;
        return info;
	}

    /// <summary>
    /// Gets the array ending.
    /// </summary>
    /// <value>The closing line of the array.</value>
    public static string ArrayEnding {
        get {
            return "]" + lineEnding;
        }
    }

    /// <summary>
    /// Parses an enum value from the line.
    /// </summary>
    /// <returns>The enum value.</returns>
    /// <param name="json">The json file line.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T ParseEnum<T>(string json) {
        string enu = ParseLine(json);
        return (T)(System.Enum.Parse(typeof(T), enu));
    }
    /// <summary>
    /// Parses a Vector3 from the given line of json.
    /// </summary>
    /// <returns>The vector3.</returns>
    /// <param name="json">The Json line.</param>
	public static Vector3 ParseVector3(string json) {

        string line = ParseLine(json);
        string firstSub = line.Split('(')[1];
        firstSub = firstSub.Split(')')[0];
        string[] xyz = firstSub.Split(',');
        return new Vector3(float.Parse(xyz[0].Trim()), float.Parse(xyz[1].Trim()), float.Parse(xyz[2].Trim()));
    }

    /// <summary>
    /// Parses an int from the given line of json.
    /// </summary>
    /// <returns>The int parsed from the line.</returns>
    /// <param name="json">Json line.</param>
	public static int ParseInt(string json) {
        return int.Parse(ParseLine(json));
    }

    /// <summary>
    /// Parses a float from the given json line
    /// </summary>
    /// <returns>The float parsed from the line.</returns>
    /// <param name="json">The line of json.</param>
	public static float ParseFloat(string json) {
        return float.Parse(ParseLine(json));
    }

    /// <summary>
    /// Parses a boolean from the given json line
    /// </summary>
    /// <returns><c>true</c>, if bool was parsed, <c>false</c> otherwise.</returns>
    /// <param name="json">Json.</param>
	public static bool ParseBool(string json) {
        return bool.Parse(ParseLine(json));
    }

    /// <summary>
    /// Parses a dictionary from the given lines of json.
    /// </summary>
    /// <returns>The dictionary.</returns>
    /// <param name="json">The lines of json.</param>
	public static Dictionary<string, string> ParseDictionary(string json) {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        string[] lines = json.Split('\n');
        for (int i = 1; i < lines.Length - 2; i++) {
            KeyValuePair<string, string> kvp = ParseKVP(lines[i]);
            dict.Add(kvp.Key, kvp.Value);
        }
        return dict;
    }

    /// <summary>
    /// Parses a Key Value pair from the given line of json
    /// </summary>
    /// <returns>The key-value pair.</returns>
    /// <param name="json">The line of json.</param>
	static KeyValuePair<string, string> ParseKVP(string json) {
        string key = json.Substring(1, json.IndexOf(':') - 2);
		string val = json.Substring(json.IndexOf(':') + 3);
		val = val.Substring(0, val.Length - 2);

        return new KeyValuePair<string, string>(key, val);
    }

	public static MapFile ParseMapFile(string json) {
		int connectionsStart = -1;
        List<TileStruct> tiles = ParseTiles(json, out connectionsStart);
		List<ConnectionStruct> connections = ParseConnectionsList(json.Substring(connectionsStart));
		return new MapFile(tiles, connections);
	}
    
	static int FindEndOfArray(string s, int startInd) {
		int numOpens = 0;
		int numCloses = 0;
		for (int i = startInd; i < s.Length; i++) {
			if (s[i] == '[') {
				numOpens++;
			} else if (s[i] == ']') {
				numCloses++;
				if (numOpens == numCloses) {
					return i;
				}
			}
		}

		return -1;
	}
	public static List<TileStruct> ParseTiles(string json, out int nextInd) {
        List<TileStruct> tiles = new List<TileStruct>();
        int ind = json.IndexOf('[');
		int lastInd = FindEndOfArray(json, ind);
		nextInd = lastInd + 1;
		ind += 2;//json.LastIndexOf(']');
        json = json.Substring(ind, lastInd - ind);
	
        string[] tilesList = json.Split('\n');

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
        ts.instanceName = "";
        ts.id = -1;
        for (int i = 0; i < tile.Count; i++) {
            string s = tile[i];
         
            if (s.Contains("\"Prefab\": ") || s.Contains("\"Name\": "))
			{
				ts.tileName = ParseLine(s);
			} else if (s.Contains("\"Instance Name\": ")) {
                ts.instanceName = ParseLine(s);
			} else if (s.Contains("\"ID\":")) {
                ts.id = ParseInt(s);
            }
			else if (IsLine(s))
			{
                ts.AddInfo(s);
			}
        }

        if (ts.instanceName.Length == 0) {
            ts.instanceName = ts.tileName;
        }
        return ts;
    }

	public static List<ConnectionStruct> ParseConnectionsList(string json) {
		List<ConnectionStruct> connections = new List<ConnectionStruct>();
        int ind = json.IndexOf('[');
        int lastInd = FindEndOfArray(json, ind);
       
        ind += 2;//json.LastIndexOf(']');
        json = json.Substring(ind, lastInd - ind);
        string[] tilesList = json.Split('\n');

        for (int i = 0; i < tilesList.Length; i++)
        {
            if (tilesList[i] == "{")
            {
				List<string> toParse = new List<string>() { tilesList[i + 1], tilesList[i + 2], tilesList[i + 3] };

				connections.Add(ParseConnection(toParse));
                i+=4;
            }
        }
		return connections;
	}

	public static ConnectionStruct ParseConnection(List<string> connections) {
		int tor = ParseInt(connections[0]);
		int ted = ParseInt(connections[1]);
		bool inv = ParseBool(connections[2]);
		return new ConnectionStruct(tor, ted, inv);
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
	public string tileName;
    public string instanceName;
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

public class MapFile {
	List<TileStruct> tiles;
	List<ConnectionStruct> connections;

	public MapFile(List<TileStruct> tileStructs, List<ConnectionStruct> activatorConnections) {
		tiles = tileStructs;
		connections = activatorConnections;
	}
	public List<TileStruct> Tiles {
		get {
			return tiles;
		}
	}

	public List<ConnectionStruct> Connections {
		get {
			return connections;
		}
	}
}

public struct ConnectionStruct {
	int activatorID;
	int activatedID;
	bool inverted;

	public ConnectionStruct(int tor, int ted, bool inv) {
		activatorID = tor;
		activatedID = ted;
		inverted = inv;
	}

	public int Activator {
		get {
			return activatorID;
		}
	}

	public int Activated {
		get {
			return activatedID;
		}
	}

	public bool Inverted {
		get {
			return inverted;
		}
	}
}
