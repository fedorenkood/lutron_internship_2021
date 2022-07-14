import tkinter
from PIL import ImageTk, Image
from EmitterButton import *


class OperatorTool(tkinter.Frame):
    def add_buttons(self, coordinates_set, status_dict, img_dict):
        for coordinates in coordinates_set:
            self.dict_of_emitters[coordinates] = EmitterButton(self)
            self.dict_of_emitters[coordinates].set_coordinates(coordinates[0], coordinates[1])
            self.dict_of_emitters[coordinates].set_status(status_dict[coordinates])
            my_img = img_dict[coordinates]
            my_img = my_img.resize((200, 150), Image.ANTIALIAS)
            self.tk_img_dict[coordinates] = ImageTk.PhotoImage(my_img)
            self.dict_of_emitters[coordinates].set_image(self.tk_img_dict[coordinates])
            self.dict_of_emitters[coordinates].set_command(img_dict[coordinates])


    def __init__(self, master):
        tkinter.Frame.__init__(self, master)
        self.pack()
        self.dict_of_emitters = dict()
        self.tk_img_dict = dict()
