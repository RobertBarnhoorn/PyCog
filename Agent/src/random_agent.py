from message import Message
from random import randint


# These variables are used as IDs for messages.
# They indicate what the data in the message means.
QUIT = 0
STATE = 1
ACTION = 2
REWARD = 3

class RandomAgent:
    """Sends random movements to robot arm"""

    def act(self, state):
        action = randint(0, 7)  # Choose an action 0 to 7
        return action

    def execute(self, action):
        """ Send the action to the arm and receive the reward """
        self.interface.send(action)                   # send the action
        msg_recv = self.interface.blocking_receive()  # wait for message

        if msg_recv.id != RandomAgent.REWARD:
            print('ERROR: Expected message.id=', RandomAgent.REWARD,
                  ' but got ', msg_recv.id)
            return None

        reward = msg_recv.data[0]
        return reward

    def observe(self, reward, terminal):
        pass

    def rotate_joint(self, joint, degrees):
        msg = Message(RandomAgent.ACTION)
        msg.add_data(str(joint))
        msg.add_data(str(degrees) + '\n')
        return msg


if __name__ == '__main__':
    arm = RandomAgent()
