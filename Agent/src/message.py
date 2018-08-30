class Message:
    """Represents the format a message must conform to for it to work in the
    QCog/PyCog Platforms"""

    def __init__(self, id):
        self.id = id    # id that describes how to interpret the data
        self.data = []  # list of strings

    def add_data(self, new_data):
        self.data.append(new_data)
