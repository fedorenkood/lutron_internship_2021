import os
import csv
from typing import Tuple

import matplotlib
from matplotlib import pyplot as plt
import numpy as np
import re


def mat_lab_operator(tempdirectory, failed_emitter_boards, questionable_emitter_boards):
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
    plt.gca().invert_yaxis()

    # Draw horizontal grids
    for y in [0.5, 1.5]:
        plt.axline((0, y), (5.5, y), linestyle='-', color='k', linewidth=2)

    for x in [0.5, 1.5, 2.5, 3.5, 4.5]:
        plt.axline((x, 0), (x, 2.5), linestyle='-', color='k', linewidth=2)

    plt.xticks([])
    plt.yticks([])

    # save image of grid in selected folder and give it name - not sure if this is actually necessary
    save = os.path.join(tempdirectory, "grid_image.png")

    # open figure as full screen
    plt.get_current_fig_manager().window.state('zoomed')

    plt.savefig(save)
    plt.show()
