import os

current_directory = os.getcwd()

caseta_txt = os.path.join(current_directory, "caseta_area_list.txt")
ml_txt = os.path.join(current_directory, "ml_area_list.txt")
final_txt = os.path.join(current_directory, "final_area_list.txt")

enum_room_name = dict()

with open(caseta_txt, 'r', encoding='utf-8') as f:
    line_list = f.readlines()
    for l in line_list:
        l_list = l.strip('\n').split(",'")
        enum_room_name[int(l_list[0])] = l_list[1]

print(enum_room_name)
reverse_enum_room_name = dict((v, k) for k, v in enum_room_name.items())
last_index = len(enum_room_name.keys())
print(reverse_enum_room_name)

ml_room_dict = dict()
ml_index = 0

with open(ml_txt, 'r', encoding='utf-8') as f:
    line_list = f.readlines()
    for l in line_list:
        l_list = l.strip('\n')
        ml_room_dict[ml_index] = l_list
        ml_index += 1
        if l_list not in reverse_enum_room_name:
            enum_room_name[last_index] = l_list
            last_index += 1

print(ml_room_dict)
print(enum_room_name)

with open(final_txt, 'w', encoding='utf-8') as f:
    for k, v in enum_room_name.items():
        # f.write(f"\t({k}, \'{v}\'),\n")
        # f.write(f"\t\t\t\t{{{k}, \"{v}\"}},\n")
        enum_value = v
        enum_value = enum_value.replace("/", "_")
        enum_value = enum_value.replace(" ", "")
        f.write(f"\t\t/// <summary>\n")
        f.write(f"\t\t/// Enum for {v} AreaType\n")
        f.write(f"\t\t/// </summary>\n")
        # f.write(f"\t\t[Description(\"{v}\")]\n")
        f.write(f"\t\t{enum_value} = {k},\n\n")
