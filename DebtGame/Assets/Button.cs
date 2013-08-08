using UnityEngine;
using System.Collections;
 
public enum ButtonState {
    normal,
    hover,
    armed
}
 
[System.Serializable] // Required so it shows up in the inspector 
public class ButtonTextures {
    public Texture normal=null;
    public Texture hover=null;
    public Texture armed=null;
    public ButtonTextures() {}
 
    public Texture this [ButtonState state] {
        get {
            switch(state) {
                case ButtonState.normal:
                    return normal;
                case ButtonState.hover:
                    return hover;
                case ButtonState.armed:
                    return armed;
                default:
                    return null;
            }
        }
    }
}
 
 
[RequireComponent(typeof(GUITexture))]
[AddComponentMenu ("GUI/Button")]    
public class Button : MonoBehaviour {
 
    public GameObject messagee;
    public string message = "ButtonPressed";
    public ButtonTextures textures;
 
	static GameManager manager = null;
	
	public static GameManager getManager(GameObject gameObject) {
		if(manager == null) {
			Debug.Log("Creating game manager");
			manager = (GameManager) gameObject.AddComponent("GameManager");
		} else {
			Debug.Log("Found game manager");
			manager.startNewRound();
		}
		return manager;
	}
	
    protected int state = 0;
    protected GUITexture myGUITexture;
 
    protected virtual void SetButtonTexture(ButtonState state) {
        myGUITexture.texture=textures[state];
    }
 
    public virtual void Reset() {
        messagee = gameObject;
        message = "ButtonPressed";
    }
 
    public virtual void Start() {
        myGUITexture = GetComponent(typeof(GUITexture)) as GUITexture; 
        SetButtonTexture(ButtonState.normal);
    }
 
    public virtual void OnMouseEnter()
    {
        state++;
        if (state == 1)
            SetButtonTexture(ButtonState.hover);
		Debug.Log("button enter");
    }
 
    public virtual void OnMouseDown()
    {
        state++;
        if (state == 2)
            SetButtonTexture(ButtonState.armed);
		Debug.Log("button down");
    }
 
    public virtual void OnMouseUp()
    {
        if (state == 2)
        {
            state--;
//            if (messagee != null)
//                messagee.SendMessage(message, this);
			//GameMaster master = (GameMaster) gameObject.AddComponent("GameMaster");
//			master.startNewRound();
			getManager(gameObject);
        }
        else
        {
            state --;
            if (state < 0)
                state = 0;
        }
        SetButtonTexture(ButtonState.normal);
		Debug.Log("button up");
    }
 
    public virtual void OnMouseExit()
    {
        if (state > 0)
            state--;
        if (state == 0)
            SetButtonTexture(ButtonState.normal);
		Debug.Log("button exit");
    }
}