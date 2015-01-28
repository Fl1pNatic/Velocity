using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class ObjectData
{
	[XmlAttribute("Name")]
	public string name;

	public Vector3 position;
	public Vector3 eulerRotation;
	public Vector3 scale;

	[XmlArray("Children")]
	[XmlArrayItem("ObjectData")]
	public ObjectData[] children;
}