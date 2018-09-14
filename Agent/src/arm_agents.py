from tensorforce.agents import PPOAgent
from tensorforce.agents import DQNAgent
from random_agent import RandomAgent
from network_interface import NetworkInterface
from message_scanner import MessageScanner
from message import Message

QUIT = 0
STATE = 1
ACTION = 2
REWARD = 3


class PPOArmAgent:
    """A controller for the robot arm"""

    # These variables are used as IDs for messages.
    # They indicate what the data in the message means.

    def __init__(self):
        self.scanner = MessageScanner()
        self.interface = NetworkInterface(self.scanner)

        # Create a Proximal Policy Optimization agent
        # agent = PPOAgent(
        #     states=dict(type='float', shape=(10,)),
        #     actions=dict(type='int', num_actions=8),
        #     network=[
        #         dict(type='dense', size=100),
        #         dict(type='dense', size=100)
        #     ],
        #     batched_observe=False,
        #     batching_capacity=1,
        #     discount=0.98
        # )

        # agent = DQNAgent(
        #     states=dict(type='float', shape=(10,)),
        #     actions=dict(type='int', num_actions=8),
        #     network=[
        #         dict(type='dense', size=100),
        #         dict(type='dense', size=100)
        #     ],
        #     batched_observe=False,
        #     batching_capacity=1,
        #     discount=0.98
        # )

        agent = RandomAgent()

        results_file = open('results.txt', 'a')  # to record fitness
        # ------- RL feedback loop -------
        sum_r = 0
        iter_count = 0
        n = 0
        while(True and iter_count < 5000):
            n += 1
            state = self.get_state()                             # get state of arm
            action = agent.act(state)                            # choose action = joint 0,1,2,3,4
            reward = self.execute(action)                        # execute action and observe reward
            agent.observe(reward=reward[0], terminal=reward[1])  # add experience
            sum_r += reward[0]
            if reward[1]:
                iter_count += 1
                av = sum_r / n
                print(iter_count, ': , ', n, ', ', av)  # print the average reward for that iteration
                results_file.write(str(n) + ',' + str(sum_r) + '\n')
                results_file.flush()
                sum_r = 0
                n = 0

        # --------------------------------
        results_file.close()
        agent.save_model('./saved/')

    def get_state(self):
        """Gets the current state of the arm"""
        msg_recv = self.interface.blocking_recieve()  # wait for message
        if msg_recv.id != STATE:
            print('ERROR: Expected message.id=', STATE, ' but got ', msg_recv.id)
            return None

        state = []
        for i in range(len(msg_recv.data)):
            state.append(float(msg_recv.data[i]))
        return state

    def execute(self, action):
        """Send the action to the arm and receive the reward"""
        if action == 0:
            joint = 0
            degrees = 1
        elif action == 1:
            joint = 0
            degrees = -1
        elif action == 2:
            joint = 1
            degrees = 1
        elif action == 3:
            joint = 1
            degrees = -1
        elif action == 4:
            joint = 2
            degrees = 1
        elif action == 5:
            joint = 2
            degrees = -1
        elif action == 6:
            joint = 3
            degrees = 1
        elif action == 7:
            joint = 3
            degrees = -1
        else:
            joint = 0
            degrees = 1
            print('ERRO ACTION WAS: ', action)

        msg_send = self.rotate_joint(joint, degrees)
        self.interface.send(msg_send)                 # send the action
        msg_recv = self.interface.blocking_recieve()  # wait for message

        if msg_recv.id != REWARD:
            print('ERROR: Expected message.id=', REWARD, ' but got ', msg_recv.id)
            return None


        reward = (float(msg_recv.data[0]), bool(int(msg_recv.data[1])))
        return reward

    def rotate_joint(self, joint, degrees):
        msg = Message(ACTION)
        msg.add_data(str(joint))
        msg.add_data(str(degrees) + '\n')
        return msg


if __name__ == '__main__':
    ppo_agent = PPOArmAgent()
