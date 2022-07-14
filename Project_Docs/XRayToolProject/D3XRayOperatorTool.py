import os
import csv
from pprint import pprint
from typing import Tuple

import matplotlib
from matplotlib import pyplot as plt
import numpy as np
from enum import Enum

import tkinter
import tkinter.filedialog as fd


# TODO: if three

def convert_filename_to_board_xy(filename) -> Tuple[int, int]:
    """Converts the XRay's file naming pattern into a xy coordinate of the board.
    The origin point (0,0) is in the top-left corner of the board.
    The furthest point (5, 2) is the bottom-right corner of the board.

    E.g.,
        >>> convert_filename_to_board_xy('BGA_B1P3S2.csv')
        (0, 0)
    """
    name = os.path.split(filename)[1]
    mapping = {
        "BGA_B1P1S2.csv": (0, 2),
        "BGA_B1P2S2.csv": (0, 1),
        "BGA_B1P3S2.csv": (0, 0),
        "BGA_B1P1S4.csv": (1, 2),
        "BGA_B1P2S4.csv": (1, 1),
        "BGA_B1P3S4.csv": (1, 0),
        "BGA_B2P1S2.csv": (2, 2),
        "BGA_B2P2S2.csv": (2, 1),
        "BGA_B2P3S2.csv": (2, 0),
        "BGA_B2P1S4.csv": (3, 2),
        "BGA_B2P2S4.csv": (3, 1),
        "BGA_B2P3S4.csv": (3, 0),
        "BGA_B3P1S2.csv": (4, 2),
        "BGA_B3P2S2.csv": (4, 1),
        "BGA_B3P3S2.csv": (4, 0),
        "BGA_B3P1S4.csv": (5, 2),
        "BGA_B3P2S4.csv": (5, 1),
        "BGA_B3P3S4.csv": (5, 0)
    }

# error detection
    try:
        return mapping[name]
    except KeyError:
        exit(f'Error: Cannot recognize {name}, update the filename map')


root = tkinter.Tk()
root.withdraw()  # use to hide tkinter window


# browse file path
currentdirectory = os.getcwd()
tempdirectory = fd.askdirectory(parent=root, initialdir=currentdirectory, title='Please select a directory')
if len(tempdirectory) > 0:
    print ("You chose %s" % tempdirectory)

# hard coded file path used in development: tempdirectory = r'C:\Users\iadams\Downloads\001-D3 Production Boards-00'
files = [os.path.join(tempdirectory, f) for f in os.listdir(tempdirectory) if f.endswith('.csv') and f.startswith('BGA')]

failed_emitter_boards = set()  # The set of boards which have outright failed
questionable_emitter_boards = set()  # The set of boards where 3 void measurements are > 28%

for file in files:
    with open(file, 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        data = list(reader)

        overspec_void_count = 0

        for row in data:
            if row['Result'] != 'OK':
                failed_emitter_boards.add(convert_filename_to_board_xy(file))

            if float(row['Overall Area Void (%)']) >= 28.0:
                overspec_void_count += 1

        if overspec_void_count >= 3:
            questionable_emitter_boards.add(convert_filename_to_board_xy(file))
            # questionable_emitter_boards.add(file)

print(f'Failed: {failed_emitter_boards}')
print(f'Check: {questionable_emitter_boards}')

# Display a 6x3 grid with red/green/yellow squares
grid = np.zeros((6, 3))

for x, y in questionable_emitter_boards:
    grid[x][y] = 1

for x, y in failed_emitter_boards:
    grid[x][y] = 2

grid = grid.transpose()

fig, ax = plt.subplots()

cmap = matplotlib.colors.ListedColormap(['green', 'yellow', 'red'])
ax.imshow(grid, cmap=cmap)

# Draw horizontal grids
for y in [0.5, 1.5]:
    plt.axline((0, y), (5.5, y),  linestyle='-', color='k', linewidth=2)

for x in [0.5, 1.5, 2.5, 3.5, 4.5]:
    plt.axline((x, 0), (x, 2.5),  linestyle='-', color='k', linewidth=2)

plt.xticks([])
plt.yticks([])

# save image of grid in selected folder and give it name - not sure if this is actually necessary
save = os.path.join(tempdirectory, "grid_image.png")

# open figure as full screen
plt.get_current_fig_manager().window.state('zoomed')

plt.savefig(save)
plt.show()
