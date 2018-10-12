from tensorforce.agents import DQNAgent
from network_interface import NetworkInterface
from message_scanner import MessageScanner
from message import Message
import pathlib


# These constants are used as IDs for messages
# They indicate what the data in the message means
QUIT = 0
STATE = 1
ACTION = 2
REWARD = 3

class RLAgent:
    """ A controller for the robot arm """

    def __init__(self, agent):
        self.interface = NetworkInterface(MessageScanner())
        self.results = open('results.txt', 'a')
        self.run(agent)
        self.results.close()
        agent.save_model('./saved/')

    def run(self, agent):
        """ RL feedback loop """
        max_av_r = -10
        min_tick = 300
        sum_r = 0
        iter_count = 0
        n = 0
        while(iter_count < 2000):
            n += 1
            state = self.get_state()                                # get state of arm
            action = agent.act(state)                               # choose action = joint 0,1,2,3,4
            reward = self.execute(action)                           # execute action and observe reward
            agent.observe(reward=reward[0], terminal=reward[1])     # add experience
            sum_r += reward[0]
            if reward[1]:
                iter_count += 1
                av = sum_r / n                                      # average reward received for that iteration
                if av > max_av_r:
                    max_av_r = av
                    pathlib.Path('./saved/reward/' + str(iter_count)).mkdir(parents=True, exist_ok=True)
                    agent.save_model('./saved/reward/' + str(iter_count) + '/' + str(iter_count), False)
                if n < min_tick:
                    min_tick = n
                    pathlib.Path('./saved/actions/' + str(iter_count)).mkdir(parents=True, exist_ok=True)
                    agent.save_model('./saved/actions/' + str(iter_count) + '/' + str(iter_count), False)

                print(iter_count, ': , ', n, ', ', av)              # print the average reward for that iteration
                self.results.write(str(n) + ',' + str(sum_r) + '\n')
                self.results.flush()
                sum_r = 0
                n = 0

    def get_state(self):
        """ Gets the current state of the arm """
        msg_recv = self.interface.blocking_receive()  # wait for message
        if msg_recv.id != STATE:
            print('ERROR: Expected message.id=', STATE, ' but got ', msg_recv.id)
            return None

        state = []
        for i in range(len(msg_recv.data)):
            state.append(float(msg_recv.data[i]))
        return state

    def execute(self, action):
        """ Send the action to the arm and receive the reward """
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

        msg_send = self.rotate_joint(joint, degrees)
        self.interface.send(msg_send)                 # send the action
        msg_recv = self.interface.blocking_receive()  # wait for message

        if msg_recv.id != REWARD:
            print('ERROR: Expected message.id=', REWARD, ' but got ', msg_recv.id)
            return None

        reward = (float(msg_recv.data[0]), bool(int(msg_recv.data[1])))
        return reward

    def rotate_joint(self, joint, degrees):
        """ Return a message corresponding to a rotation """
        msg = Message(ACTION)
        msg.add_data(str(joint))
        msg.add_data(str(degrees) + '\n')
        return msg


if __name__ == '__main__':
    # Create a DQN agent
    agent = DQNAgent(
        states=dict(type='float', shape=(10,)),
        actions=dict(type='int', num_actions=8),
        network=[
            dict(type='dense', size=100),
            dict(type='dense', size=100)
        ],
        batched_observe=False,
        batching_capacity=1,
        discount=0.98,
        double_q_model=True
    )

    controller = RLAgent(agent)
