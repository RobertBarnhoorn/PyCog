from network_interface import NetworkInterface
from message_scanner import MessageScanner

if __name__ == '__main__':
    """ This script simulates the behaviour of the unity simulation
    -- use for easier network debugging """

    scanner = MessageScanner()
    interface = NetworkInterface(scanner, server=True)

    state = get_state()
    interface.send(state)
    # receive the chosen action from the agent
    action = interface.receive()
    while (action == null):
        action = server.receive()
 
    # Execute the action, receive reward and send it to agent
    TCPMessage reward = Execute(action);
    server.SendMessage (reward);

    while(True):
        msg_recv = interface.receive()
        while (msg_recv is None):
            msg_recv = interface.receive()

        print('received: ', scanner.msg_to_ascii(msg_recv))
        text = '1$received message'
        print('sent: ', text)
        msg_send = scanner.ascii_to_msg(text)
        interface.send(msg_send)
