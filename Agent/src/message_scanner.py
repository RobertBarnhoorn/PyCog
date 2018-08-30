from message import Message


class MessageScanner:
    """Converts between ascii text (for sending data over the socket) and
    message objects (used to interpret instructions)"""

    def ascii_to_msg(self, data):
        """Given an ascii string input, outputs a new message object"""
        instructions = []  # list of strings representing instructions
        instr = ''

        for i in range(len(data)):
            c = data[i]
            if c == '$':
                instructions.append(instr)
                instr = ''
            elif i == len(data) - 1:
                instr += c
                instructions.append(instr)
            else:
                instr += c

        id = instructions.pop(0)

        msg = Message(id)
        for i in range(len(instructions)):
            msg.add_data(instructions[i])

        return msg

    def msg_to_ascii(self, msg):
        """Returns an ascii string representation of a message object, which can be
        sent with the socket"""
        data = ''
        for i in range(len(msg.data)):
            data += '$' + msg.data[i]
        return str(msg.id) + data
