from message import Message
from network_interface import NetworkInterface
from message_scanner import MessageScanner
from random import randint
from time import sleep


class RandomAgent:
    """Sends random movements to robot arm"""

    # the ID of the message should indicate what it is for
    QUIT = 0
    ROTATE_JOINT = 1

    def rotate_joint(self, joint, degrees):
        msg = Message(RandomAgent.ROTATE_JOINT)
        msg.add_data(str(joint))
        msg.add_data(str(degrees) + '\n')
        return msg


if __name__ == '__main__':
    agent = RandomAgent()
    scanner = MessageScanner()
    interface = NetworkInterface(scanner)

    while(True):
        joint = round(randint(0, 4))  # Choose a joint between 0 and 4
        degrees = round(randint(-1, 1))  # Random angle is -1 or 1
        if degrees > 0:
            degrees = 1
        else:
            degrees = -1

        msg_send = agent.rotate_joint(joint, degrees)
        interface.send(msg_send)
        print('sent: ', scanner.msg_to_ascii(msg_send))
        sleep(1)

        # msg_recv = interface.receive()
        # while (msg_recv is None):
        #     msg_recv = interface.receive()

        # print('received: ', scanner.msg_to_ascii(msg_recv))
