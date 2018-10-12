# PyCog

**Deep Reinforcement Learning for robotic arm control**

## About

PyCog is a framework for evaluating Deep Reinforcement Learning algorithms by training them to control a simulated
robotic arm.

The neural networks are trained using [TensorForce](https://github.com/tensorforce/tensorforce), *a TensorFlow library
for applied reinforcement learning*.
The robotic arm is implemented in the [Unity Engine](https://unity.com), *a real-time 3D development platform*.
Communication between TensorFlow and Unity is done with a domain-specific communication protocol over TCP and is
implemented using Python and C#.

## Installation
    
1. [Install TensorFlow](https://github.com/tensorflow/tensorflow)
2. [Install TensorForce](https://github.com/tensorforce/tensorforce)
3. [Install Unity](https://store.unity.com/download)
4. Open Unity and run the simulation
5. Run the controller
