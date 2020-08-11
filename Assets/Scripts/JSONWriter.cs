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
    public static void Write(double[] timestamps, string label)
    {
        var content = new Dictionary<int, double>();
        for (int i = 0; i < timestamps.Length; i++)
            content.Add(i, timestamps[i]);
        var wrapper = new Dictionary<string, Dictionary<int, double>>()
        {
            { label, content }
        };
        var json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
        var path = Path.Combine(Application.dataPath, "Output", $"{label}.json");
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();
    }
}