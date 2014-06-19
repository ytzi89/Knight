#pragma strict

var isQuit = false;

function OnMouseEnter(){
	// change text color
	renderer.material.color=Color.red;
}

function OnMouseExit(){
	//change text color
	renderer.material.color=new Color32(204, 167, 58, 1);
}

function OnMouseUp(){
	//is this quit
	if(isQuit==true){
		// quit the game
		Application.Quit();
	}else{
		//load level
		Application.LoadLevel("Home");
	}
}

function Update () {

}