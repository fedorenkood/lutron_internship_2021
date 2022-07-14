import os
from typing import Tuple
import re


def filename_to_board_xy(filename) -> Tuple[int, int]:
    """Converts the XRay's file naming pattern into a xy coordinate of the board.
    The origin point (0,0) is in the top-left corner of the board.
    The furthest point (5, 2) is the bottom-right corner of the board.

    E.g.,
        >>> filename_to_board_xy('BGA_B1P3S2.csv')
        (0, 0)
    """
    name = os.path.split(filename)[1]
    # error detection
    try:
        groups = re.findall('[A-Z][0-9]+', name)
        # print(groups)
        position_dict = {}
        for g in groups:
            identifier_tuple = re.findall('[A-Z]|[0-9]+', g)
            position_dict[identifier_tuple[0]] = int(identifier_tuple[1])
            # print(identifier_tuple)
        # print(position_dict)
        xy_coordinates = dict_to_xy(position_dict)
        print(xy_coordinates)
        return xy_coordinates
    except KeyError:
        exit(f'Error: Cannot recognize {name}, update the filename map')


def dict_to_xy(position_dict) -> Tuple[int, int]:
    x_coordinate = 0
    y_coordinate = 0
    board = "B"
    pattern = "P"
    step = "S"
    x_coordinate = (position_dict[board] - 1) * 2 + int(position_dict[step]/2 - 1)
    y_coordinate = 3 - position_dict[pattern]
    return (x_coordinate, y_coordinate)


def img_filename_to_xy(filename) -> Tuple[int, int]:
    name = os.path.split(filename)[1]
    # error detection
    try:
        identifier_tuple = re.findall('[0-9]+', name)

        if len(identifier_tuple) < 2:
            raise ValueError(f'Error reading the file {name}')

        dict_names = ["B", "P", "S"]
        position_dict = {}
        for i in range(0, 3):
            position_dict[dict_names[i]] = int(identifier_tuple[i])
        print(position_dict)
        xy_coordinates = dict_to_xy(position_dict)
        print(xy_coordinates)
        return xy_coordinates
    except KeyError:
        exit(f'Error: Cannot recognize {name}, update the filename map')