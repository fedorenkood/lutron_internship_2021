from enum import Enum
import tkinter
from PIL import ImageTk, Image

class Color(Enum):
    GREEN = 'green'
    YELLOW = 'yellow'
    RED = 'red'


class Status(Enum):
    OK = 'green'
    CHECK = 'yellow'
    FAIL = 'red'

def image_window(image):
    win = tkinter.Toplevel()
    win.wm_title("Window")

    l = tkinter.Label(win, text="Input", image=image)
    l.grid(row=0, column=0)

    b = tkinter.Button(win, text="Okay", command=win.destroy)
    b.grid(row=1, column=0)

class EmitterButton(tkinter.Button):
    def set_image(self, image):
        self['image'] = image

    def set_status(self, new_status):
        self.status = new_status
        self['bg'] = new_status.value

    def get_status(self):
        return self.status

    def set_coordinates(self, x_coordinate, y_coordinate):
        # self['row'] = y_coordinate
        # self['column'] = x_coordinate
        self.grid(row=y_coordinate, column=x_coordinate)

    def set_command(self, image):
        my_img = image.resize((800, 600), Image.ANTIALIAS)
        tk_img = ImageTk.PhotoImage(my_img)
        self.config(command=lambda img=tk_img: image_window(img))

    def __init__(self, *args, **kwargs):
        tkinter.Button.__init__(self, *args, **kwargs)
        self['bg'] = Status.OK.value
        self.status = Status.OK
        self['bd'] = 4
