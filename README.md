Deep Reinforcement Learning (DRL) for robot arm control, implemented in Unity and Python with tensorForce.

To set up:
    
    1. Go to the TensorForce Github page https://github.com/reinforceio/tensorforce.

    2. Follow the instructions to set up TensorForce.
       - Install the TensorFlow CPU version first and get it working.
       - If you really need the extra speed you can try get the GPU version working,
         but days of your life will disappear.
       - To make sure TensorForce works, follow their "quick start" example.

    3. Go to Unity and run the RobotBoxScene.
       - It will freeze while it waits for a client connection.

    4. In Agent/src run the arm_agents.py file.
       - By default it will run the DQN agent.
       - It always takes long for TensorFlow to initialize the Neural Network
       so give it a couple of minutes.

    5. Go back to Unity. The arm will eventually start moving.
       - Using my default DQN it will take between 1000 and 3000 iterations
       to learn to reach it (or about 8 hours on an i7).

To customize:

    To customize the robot arm, the main file you need to manage is RobotArm.cs
    on the Unity side. The comments should let you know what is happening.

    To customize the learning agent, the main file you need to manage is arm_agents.py
    on the Python side. The comments should let you know what is happening.

You can contact me at robbie.barnhoorn@gmail.com if you need help/advice. Enjoy!