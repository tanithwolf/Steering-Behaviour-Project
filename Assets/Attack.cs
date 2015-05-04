using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
	
	public Vector3 force; //force - can be applied in any direction right?
	public Vector3 velocity; //direction 
	public float mass = 1f; //weight of our agent
	public float maxSpeed = 5f; //how fast it can travel
	public GameObject target; //something to steer towards!
	public bool seekEnabled, fleeEnabled; //toggle which steering behaviour you want on in the inspector!
	//For measuring the distance between the attackers and the defender and also the finish
	public float distanceDefender;
	public float distanceFinish;
	Transform defender;
	Transform finish;
	
	void Start()
	{
		//Gets the transform for the defender and finish
		defender = GameObject.FindWithTag("Defender").transform;
		finish = GameObject.FindWithTag("Finish").transform;
	}
	
	void Update () 
	{
		//Works out the distance between the defender and attackers, attackers and finish
		distanceDefender = Vector3.Distance(defender.transform.position, transform.position);
		distanceFinish = Vector3.Distance(finish.transform.position, transform.position);
		//if defender gets too close changes seeking finsh to flee from defender
		if (distanceDefender <= 5f)
		{
			target = GameObject.FindWithTag("Defender");
			seekEnabled = false;
			fleeEnabled = true;
		}
		//otherwise if defender is suitable distance away stops flee and changes to seek on finish
		else if (distanceDefender >= 20f)
		{
			target = GameObject.FindWithTag("Finish");
			fleeEnabled = false;
			seekEnabled = true;
		}
		//Attackers go red when close to finish
		if (distanceFinish <= 1f)
		{
			GetComponent<Renderer>().material.color = Color.red;
		}
		//Otherwise are black
		else
		{
			GetComponent<Renderer>().material.color = Color.black;
		}
		//calls forceintegrator
		ForceIntegrator();
	}
	
	void ForceIntegrator()
	{
		Vector3 accel = force / mass; //our accell is the accumulated force / our mass, so 
		velocity = velocity + accel * Time.deltaTime; //add the accel to our velocity over time
		transform.position = transform.position + velocity * Time.deltaTime; //set our position to our position + the velocity over time
		force = Vector3.zero; //each frame, we find the difference between self and target, and add the different to the forceAcc. We don't want this adding up to itself, so we reset it each frame and only add force once each frame to the required direction
		if(velocity.magnitude > float.Epsilon) //if we have speed
		{
			transform.forward = Vector3.Normalize(velocity); //set our forward direction to use our velocity (difference between us and target) so to always point at target
		}
		velocity *= 0.99f; //damping! each frame set veloc to 0.99 of itself, so it can slow down 
		if(seekEnabled)
		{
			force += Seek(target.transform.position); //Seek is a functon that returns a Vector3, and we also pass down our targets position
		}
		if(fleeEnabled)
		{
			force += Flee (target.transform.position);
		}
	}
	
	Vector3 Seek(Vector3 target) //This is the Seek behaviour! It returns a Vector3 to the force, which is where we steer this agent towards
	{
		Vector3 desiredVel = target - transform.position; //first we find the desired Velocity - the different between the target and ourselves. This is the vector we need to add force to, which will steer us towards our target
		desiredVel.Normalize(); //let's normalize the vector, so that it keeps it's direction but is of max length 1, so we have control over it's speed (To normalize, divide each x, y, z of the vector by it's own magnitude. Magnitude is calculated by adding the squared values of x,y,z together, then getting the squared root of the result. So (5, 0, 5) = (25, 0, 25) = 50. Square root of 50 is 7.04. This is the magnitude. To normalize, (5 / 7.04, 0 / 7.04, 5 / 7.04) = (0.71, 0, 0.71) Try one yourself!
		desiredVel *= maxSpeed; //we then multiply our normalized vector by maxSpeed to make it move faster
		return desiredVel - velocity; //we return the difference of our desired velocity and velocity so that we are steered in the direction of the targets direction
	}
	
	Vector3 Flee(Vector3 target) //flee is just the opposite of Seek. We create an Vector3 to steer the agent in the opposite direction
	{
		Vector3 desiredVel = target - transform.position; //same as flee
		float distance = desiredVel.magnitude; //we now create a distance float, which tracks the distance between agent and target using the magnitude of the vector (distance)
		if(distance < 10f) //if the distance between the two is less than 4 units, Flee!
		{
			desiredVel.Normalize();
			desiredVel *= maxSpeed;
			return velocity - desiredVel; //we create our opposing force here, notice how we flipped the vectors compared to Seek
		}
		else //if our agent and target are greater than 4 units apart
		{
			return Vector3.zero; //return a 0,0,0 vector (we don't want to move!)
		}
	}
	
	
}