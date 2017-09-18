using FANNCSharp;
using FANNCSharp.Float;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using DataType = System.Single;

public class StickthingNeuralNet : MonoBehaviour {

    public NeuralNet net;

    float last_result;

    uint last_from_neuron;
    uint last_to_neuron;
    float last_weight;

    bool first_time_modifying = true;

    // Use this for initialization
    void Awake () {
        Init();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Init()
    {
        const uint num_layers = 3;
        const uint num_neurons_hidden = 3;

        // Inputs: Angle AB, height difference
        net = new NeuralNet(NetworkType.LAYER, num_layers, 3, num_neurons_hidden, 2);

        net.RandomizeWeights(-1.0f, 1.0f);

        print("Connection count: " + net.ConnectionArray.Length);
        for (int i = 0; i < net.ConnectionArray.Length; i++)
        {
            //print("Weight " + i.ToString() + ": " + net.ConnectionArray[i].Weight);
        }
    }

    public float[] GetTorques(float angle_left, float angle_right, float height_difference)
    {
        return net.Run(new float[] { angle_left, angle_right, height_difference });
    }

    public void ModifyFromResult(float result)
    {
        // If it's not first time modifying & result is worse than last result, return modifications
        if (!first_time_modifying && result < last_result)
        {
            print("Worse result.");
            net.SetWeight(last_from_neuron, last_to_neuron, last_weight);
        }
        else
        {
            print("Better result.");
        }

        // Randomize some weight.
        uint random_connection = (uint)UnityEngine.Random.Range(0, (int)net.ConnectionArray.Length);

        last_from_neuron = net.ConnectionArray[random_connection].FromNeuron;
        last_to_neuron = net.ConnectionArray[random_connection].ToNeuron;
        last_weight = net.ConnectionArray[random_connection].Weight;

        net.SetWeight(net.ConnectionArray[random_connection].FromNeuron, net.ConnectionArray[random_connection].ToNeuron, UnityEngine.Random.Range(-1.0f, 1.0f));

        last_result = result;

        first_time_modifying = false;
    }
    /*
    static StreamReader trainingFile = null;
    static StreamReader testFile = null;
    static void TrainingDataCallback(uint number, uint inputCount, uint outputCount, DataType[] input, DataType[] output)
    {
        print("input count: " + inputCount);

        float[] data = new float[] {UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)};

        for (int i = 0; i < 2; i++)
        {
            input[i] = data[i];
        }

        output[0] = 1.0f;
    }

    static void TestDataCallback(uint number, uint inputCount, uint outputCount, DataType[] input, DataType[] output)
    {
        print("input count: " + inputCount);

        float[] data = new float[] { UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f) };

        for (int i = 0; i < 2; i++)
        {
            input[i] = data[i];
        }

        output[0] = 1.0f;
    }

    static int TrainingCallback(NeuralNet net, TrainingData data, uint maxEpochs, uint epochsBetweenReports, float desiredError, uint epochs, object userData)
    {
        print(String.Format("CAAAAAAAAALLLBAAAAAAAACK: MSE error on train data: {0}", net.TestData(data)));
        System.GC.Collect(); // Make sure nothing's getting garbage-collected prematurely
        GC.WaitForPendingFinalizers();
        print(String.Format("Callback: Last neuron weight: {0}, Last data input: {1}, Max epochs: {2}\nEpochs between reports: {3}, Desired error: {4}, Current epoch: {5}\nGreeting: \"{6}\"",
                            net.ConnectionArray[net.TotalConnections - 1].Weight, data.InputAccessor.Get((int)data.TrainDataLength - 1, (int)data.InputCount - 1),
                            maxEpochs, epochsBetweenReports, desiredError, epochs, userData));
        return 1;
    }
    */
}
