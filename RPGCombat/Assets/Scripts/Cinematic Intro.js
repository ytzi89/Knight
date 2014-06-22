#pragma strict
private var waypoints : GameObject[];
var maxSize: int = 30;
private var currentIndex : int;
private var currentWaypoint : Transform;
var introCamera : Transform;
var done : boolean = false;
var thingToLookAt : Transform;

function Start () {
	currentIndex = 0;
	waypoints = new GameObject[maxSize];
	for (var child : Transform in transform)
	{
		if (currentIndex < maxSize)
		{
		waypoints[currentIndex] = child.gameObject;
		++currentIndex;
		}
	}
	currentWaypoint = GetNextWaypoint();
}

function Update () {
	if (!done)
	{
		if (currentWaypoint != null)
		{
			if (Vector3.Distance(introCamera.position, currentWaypoint.position) < .5)
			{
				currentWaypoint = GetNextWaypoint();
			}
			else
			{
				introCamera.position = Vector3.Lerp(introCamera.position, currentWaypoint.position, Time.deltaTime * 2);
				introCamera.transform.LookAt(thingToLookAt);
			}
		}
		else
			done = true;
	}
}

function GetNextWaypoint()
{
	if (currentIndex > 0)
	{
		var nextWaypoint : Transform = waypoints[currentIndex - 1].transform;
		--currentIndex;
		return nextWaypoint;
	}
	else
		return null;
}