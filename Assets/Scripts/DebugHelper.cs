using UnityEngine;
using System.Collections;
using System.Text;
using System;

public static class DebugHelper {

	public static StringBuilder messageAppender = new StringBuilder ();

	public static void MakeSingleMessage(string messageString)
	{
		string resMessage = messageString;
		Debug.Log (resMessage);
		//Console.WriteLine (resMessage);
	}

	public static string ConnectedMessage(string nextMessageBit)
	{
		messageAppender.Append (nextMessageBit);
		return messageAppender.ToString();
	}

	public static void ClearConnectedMessages()
	{
		messageAppender = new StringBuilder ();
	}

	public static String IterateCollectionToMessage(object incommingCollection)
	{
		messageAppender.Append ("##########################INCOMMING COLLECTION OF TYPE \"" + incommingCollection.GetType()+ "\"!!!##########################");
	
		IEnumerable myTest = incommingCollection as IEnumerable;
			
			if (myTest != null) {

		//if (incommingCollection.GetType() is IEnumerable) {
			int counter = 0;
						foreach (var incollection in (IEnumerable)incommingCollection) {
				messageAppender.Append(String.Format("*Entry {0} :  {1}  *{2}", counter,incollection.ToString(), Environment.NewLine ));
				counter++;
						}
			messageAppender.Append ("##########################COLLECTION END!!!##########################");

			Debug.Log ( messageAppender.ToString());
			//Console.WriteLine ( messageAppender.ToString());

			return  messageAppender.ToString();
				}
		else {
			messageAppender.Append ("##########################COLLECTION FAILED!!!##########################");
			Debug.Log ( messageAppender.ToString());
			//Console.WriteLine ( messageAppender.ToString());

			return  messageAppender.ToString();
				}
	}
}