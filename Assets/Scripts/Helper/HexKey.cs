//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;

	
	public struct HexKey
	{
		public readonly int Dimension1;
		public readonly int Dimension2;
		public HexKey(int p1, int p2)
		{
			Dimension1 = p1;
			Dimension2 = p2;
		}
		public override bool Equals(object obj)
		{
			
			if (obj.GetType() != typeof(HexKey))
			{
				return false;    
			}
			return (((HexKey)obj).Dimension1 == this.Dimension1 && ((HexKey)obj).Dimension2 == this.Dimension2);
		}
		
		public override int GetHashCode()
		{
			//TODO: Will fail one day.
			return Dimension1 * 1000 + Dimension2;
		}
		// Equals and GetHashCode ommitted
	}


