/**
* JSONWriter.cs
* Created by: João Borks [joao.borks@gmail.com]
* Created on: 10/7/2020 (en-US)
*/

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class JSONWriter
{
    public static void WriteToFile<T>(T[] collection, string filename)
    {
        var content = new Dictionary<int, T>();
        for (int i = 0; i < collection.Length; i++)
            content.Add(i, collection[i]);
        var wrapper = new Dictionary<string, Dictionary<int, T>>()
        {
            { filename, content }
        };
        var json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
        var path = Path.Combine(Application.dataPath, "..", "Output", $"{filename}.json");
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();
    }
}