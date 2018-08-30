import socket
from collections import deque
from threading import Thread
from message_scanner import MessageScanner
from time import sleep
from message import Message


class TestServer:
    """Used to test messaging protocol with client"""

    def __init__(self, scanner, host=socket.gethostname(), port=1302):
        self.scanner = scanner          # converts stream data to messages
        self.send_buffer = deque()      # queue of message objects to be sent
        self.recv_buffer = deque()      # queue of message objects received
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

        self.server_socket.bind((host, port))
        self.server_socket.listen(1)  # how many agents we are expecting
        print('Listening to ', host, ' on port ', port)

        self.client_socket, address = self.server_socket.accept()
        print('Connected to ', str(address))
        Thread(target=self._send_messages).start()
        Thread(target=self._receive_messages).start()
        # self.client_socket.close()
        # self.server_socket.close()

    def _send_messages(self):
        """Continuously checks buffer for messages and sends them"""
        print('Ready to send...')
        while (True):
            if len(self.send_buffer) > 0:
                msg = self.send_buffer.popleft()  # get first message in buffer
                text = self.scanner.msg_to_ascii(msg)  # convert it to text
                data = text.encode()       # convert it to byte-data
                self.client_socket.send(data)   # send it through socket
                sleep(0.05)

    def _receive_messages(self):
        """Continuously listens for messages and adds them to the buffer"""
        print('Ready to receive...')
        while (True):
            data = self.client_socket.recv(4096)  # receive byte data
            print('Message received')
            text = data.decode()           # convert data to text
            msg = self.scanner.ascii_to_msg(text)      # build message object
            self.recv_buffer.append(msg)      # add message to buffer
            sleep(0.05)

    def send(self, msg):
        """Adds a message to the send buffer. It will be sent when it reaches
        the front of the queue"""
        self.send_buffer.append(msg)

    def receive(self):
        """Returns the first message in the receive buffer, or None if the
        buffer is empty"""
        if len(self.recv_buffer) > 0:
            return self.recv_buffer.popleft()
        else:
            return None


if __name__ == '__main__':
    # Run this class directly to test the client-server connection
    # and messaging protocol without having to launch unity

    scanner = MessageScanner()
    interface = TestServer(scanner)

    while(True):
        msg_recv = interface.receive()
        while (msg_recv is None):
            msg_recv = interface.receive()

        print('received: ', scanner.msg_to_ascii(msg_recv))
        text = '1$received message'
        print('sent: ', text)
        msg_send = scanner.ascii_to_msg(text)
        interface.send(msg_send)

    # i = 0
    # while(i < 10000):
    #     msg_recv = interface.receive()
    #     while (msg_recv is None):
    #         msg_recv = interface.receive()

    #     print('received: ', scanner.msg_to_ascii(msg_recv))

    #     text = str(i) + '$hello client'
    #     print('sent: ', text)
    #     msg_send = scanner.ascii_to_msg(text)
    #     interface.send(msg_send)
    #     i += 1
