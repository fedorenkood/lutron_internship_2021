import csv
import tkinter
import tkinter.filedialog as fd
from PIL import ImageTk, Image
from EmitterButton import *
from FilenameProcessing import *
from OperatorTool import *




if __name__ == '__main__':
    root = tkinter.Tk()
    # root.withdraw()  # use to hide tkinter window

    # browse file path
    currentdirectory = os.getcwd()
    tempdirectory = fd.askdirectory(parent=root, initialdir=currentdirectory, title='Please select a directory')
    if len(tempdirectory) > 0:
        print("You chose %s" % tempdirectory)

    # hard coded file path used in development: tempdirectory = r'C:\Users\iadams\Downloads\001-D3 Production Boards-00'
    csv_files = [os.path.join(tempdirectory, f) for f in os.listdir(tempdirectory) if
                 f.endswith('.csv') and f.startswith('BGA')]

    # read img files
    jpg_files = [os.path.join(tempdirectory, f) for f in os.listdir(tempdirectory) if
                 f.endswith('.jpg')]

    img_dict = dict()
    status_dict = dict()
    coordinates_set = set()

    for file in csv_files:
        with open(file, 'r', encoding='utf-8') as f:
            reader = csv.DictReader(f)
            data = list(reader)
            overspec_void_count = 0

            # Coordinates for the emitter
            coordinates = filename_to_board_xy(file)
            coordinates_set.add(coordinates)
            status_dict[coordinates] = Status.OK

            # add properties to the EmitterButton
            for row in data:
                if row['Result'] != 'OK':
                    status_dict[coordinates] = Status.FAIL
                # Count the number of Area Voids above the spec
                if float(row['Overall Area Void (%)']) >= 28.0:
                    overspec_void_count += 1

            # This emitter should be manually inspected
            if overspec_void_count >= 3 and status_dict[coordinates] != Status.FAIL:
                status_dict[coordinates] = Status.CHECK

    for img_path in jpg_files:
        try:
            # Creates a Tkinter-compatible photo image, which can be used everywhere Tkinter expects an image object.
            img_coordinates = img_filename_to_xy(img_path)
            img_dict[img_coordinates] = Image.open(img_path)
        except Exception as error:
            print('Caught this error: ' + repr(error))

    app = OperatorTool(root)
    app.add_buttons(coordinates_set=coordinates_set, status_dict=status_dict, img_dict=img_dict)

    root.mainloop()
