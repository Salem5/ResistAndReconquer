using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Group {

	public List<PlayerBehaviour> players = new List<PlayerBehaviour>();
	public string title;
	public bool teamLost;

	public Group(string aTitle)
	{
		title = aTitle;
	}
}
