import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import pickle

import re
import nltk
import fuzzyset


def add_keyword(word, fuzzy_set):
    fuzzy_set.add(word)


def get_area_type(area_name, keyword_dict, fuzzy_set):
    # find top-k categories for area name
    k = 3
    res = fuzzy_set.get(area_name)
    if res is not None:
        # print(area_name, ':')
        for i in range(min(k, len(res))):
            score, match_keyword = res[i]
            # print(match_keyword)
            area_cat_pred = keyword_dict[match_keyword]
            print(area_cat_pred)
            # print('     [%s, %s, %3.2f]' % (match_keyword, area_cat_pred, score))
    else:
        print('No match found.')


if __name__ == "__main__":
    command = ""
    keyword_dict = {}
    fs = fuzzyset.FuzzySet(rel_sim_cutoff=.8)
    filename = 'keyword_dict_tf_30p.pickle'
    with open(filename, 'rb') as handle:
        keyword_dict = pickle.load(handle)
    for w, cat in keyword_dict.items():
        add_keyword(w, fs)
    while command != "quit":
        command = input("prompt: ")
        prompt = command.split(" ")
        if len(prompt) > 1:
            command = prompt[0]
            arg = prompt[1]
            if command == "add":
                add_keyword(arg, fs)
            if command == "get":
                get_area_type(arg, keyword_dict, fs)
        # default of 'relative similarity cutoff' is 1.0 which will always return top match. lower it to allow multiple matches
        # for w, cat in keyword_dict.items():
        #     add_keyword(w, fs)

