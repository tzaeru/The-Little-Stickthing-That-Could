using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickthingController : MonoBehaviour {
    public GameObject middle;
    public GameObject left_leg;
    public GameObject right_leg;

    public float impulse_force;

    public bool ai_controlled = true;

    public StickthingNeuralNet neural;

    public float time_per_run = 10.0f;

    public float timescale = 100.0f;

    float last_impulsed = 0.0f;
    public float impulse_rate = 4.0f;

    Vector3[] original_positions = new Vector3[3];
    float[] original_rotations = new float[3];
    
    float start_time;

    float record = 0.0f;
    int generations = 0;

    public UnityEngine.UI.Text generations_text;
    public UnityEngine.UI.Text record_text;
    public UnityEngine.UI.Text last_result_text;

    // Use this for initialization
    void Start () {
        if (ai_controlled)
            Time.timeScale = timescale;

        start_time = Time.time;

        original_positions[0] = middle.transform.position;
        original_positions[1] = left_leg.transform.position;
        original_positions[2] = right_leg.transform.position;

        original_rotations[0] = middle.transform.rotation.eulerAngles.z;
        original_rotations[1] = left_leg.transform.rotation.eulerAngles.z;
        original_rotations[2] = right_leg.transform.rotation.eulerAngles.z;
    }
	
	// Update is called once per frame
	void Update () {
        if (ai_controlled && (Time.time - last_impulsed) > 1.0f/impulse_rate && Time.time - start_time > 1.0f)
        {
            float left_angle = left_leg.GetComponent<Rigidbody2D>().rotation;
            float right_angle = right_leg.GetComponent<Rigidbody2D>().rotation;

            float height_diff = left_leg.transform.position.y - right_leg.transform.position.y;

            float[] torques = neural.GetTorques(left_angle, right_angle, height_diff);

            left_leg.GetComponent<Rigidbody2D>().AddTorque((torques[0] - 0.5f) * 2.0f * impulse_force, ForceMode2D.Impulse);
            right_leg.GetComponent<Rigidbody2D>().AddTorque((torques[1] - 0.5f) * 2.0f * impulse_force, ForceMode2D.Impulse);

            last_impulsed = Time.time;
        }
        else
        {
            if (Input.GetKeyDown("a"))
            {
                left_leg.GetComponent<Rigidbody2D>().AddTorque(impulse_force, ForceMode2D.Impulse);
            }
            else if (Input.GetKeyDown("d"))
            {
                left_leg.GetComponent<Rigidbody2D>().AddTorque(-impulse_force, ForceMode2D.Impulse);
            }

            if (Input.GetKeyDown("left"))
            {
                right_leg.GetComponent<Rigidbody2D>().AddTorque(impulse_force, ForceMode2D.Impulse);
            }
            else if (Input.GetKeyDown("right"))
            {
                right_leg.GetComponent<Rigidbody2D>().AddTorque(-impulse_force, ForceMode2D.Impulse);
            }
        }

        // Reset position
        if (Time.time - start_time > time_per_run)
        {
            float result = middle.transform.localPosition.x;

            middle.transform.position = original_positions[0];
            left_leg.transform.position = original_positions[1];
            right_leg.transform.position = original_positions[2];

            middle.transform.rotation = Quaternion.Euler(0.0f, 0.0f, original_rotations[0]);
            left_leg.transform.rotation = Quaternion.Euler(0.0f, 0.0f, original_rotations[1]);
            right_leg.transform.rotation = Quaternion.Euler(0.0f, 0.0f, original_rotations[2]);

            middle.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
            left_leg.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
            right_leg.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);

            middle.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
            left_leg.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
            right_leg.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;

            start_time = Time.time;

            // Apply our amazing learning.
            neural.ModifyFromResult(result);

            if (result > record)
                record = result;
            generations++;

            generations_text.text = "Generations: " + generations;
            last_result_text.text = "Last Result: " + result;
            record_text.text = "Record: " + record;
        }
    }
}
