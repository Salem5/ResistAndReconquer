using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class TextToTextureBehaviour : MonoBehaviour {
		public Font customFont;
		public int fontCountX;
		public int fontCountY;
		public string text;
		public int textPlacementY;
		public PerCharacterKerning[] perCharacterKerning; 
		public float lineSpacing = 1;
		public bool useSharedMaterial = true;
		public int decalTextureSize = 1024;
		public float characterSize = 1; //1 = the exact size in pixels that the font appears in the texture
		public string textureID = "_DecalTex"; //ex: using decal=_DecalTex, when using diffuse=_MainTex
	public int maxTextWidth;

		private const string TEXT_TEXTURE_ID = "_DecalTex";
		private Material mat;




	// Use this for initialization
	void Start () {

		drawOnTexture ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void drawOnTexture()
	{
		Material mat = gameObject.renderer.material;
		TextToTexture textToTexture = new TextToTexture (customFont, fontCountX, fontCountY, perCharacterKerning, false);
		int textWidthPlusTrailingBuffer = textToTexture.CalcTextWidthPlusTrailingBuffer(text, decalTextureSize, characterSize);
		int posX = (decalTextureSize - (textWidthPlusTrailingBuffer + 1)) -  Mathf.Clamp(((maxTextWidth-textWidthPlusTrailingBuffer)/2),0,decalTextureSize);
		mat.SetTexture(TEXT_TEXTURE_ID, textToTexture.CreateTextToTexture(text, posX, textPlacementY, decalTextureSize, characterSize, lineSpacing));

	}
}
