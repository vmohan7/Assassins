using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	public Texture texture;

	void OnGUI(){
		float crossWidth = Screen.width / 5.0F;
		float crossHeight = Screen.height / 5.0F;

		float lWidth = (Screen.width - crossWidth) / 2.0F;
		float tHeight = (Screen.height - crossHeight) / 2.0F;
		GUI.DrawTexture(new Rect(lWidth,tHeight,crossWidth,crossHeight), texture, ScaleMode.ScaleToFit, true);
	}
}
