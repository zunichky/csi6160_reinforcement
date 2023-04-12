from pathlib import Path
import shutil
import os
import subprocess

import traceback
import pandas as pd
from tensorboard.backend.event_processing.event_accumulator import EventAccumulator

REBUILD=False
RUNMODEL=True
project_root = "/home/dev/code/csi6160_reinforcement/"
results_root = "results/"
build_folder = "builds/linux_AI/"
models=["000001", "000400"]
build_script = os.path.join(project_root, "build.sh")


allResults = {}

allResultsList = []

def main():
    for x in range(1,550):
        cur_dir = os.path.join(os.getcwd(), results_root, "{:06d}".format(x), "Robotarm")
        allResults[x] = tflog2pandas(cur_dir)
    
    for log, step in allResults.items():
        for step_num, tags in step.items():
            try:
                allResultsList.append([log, step_num,tags["Environment/Cumulative Reward"], tags["Losses/Value Loss"]])
            except:
                print("ignore")
    
    y = sorted(allResultsList, key=lambda x: x[2], reverse=True)
    df = pd.DataFrame(y, columns=["log", "step", "Cumul Reward", "Loss" ])
    df_tmp = df[:100]
    #df_tmp.to_csv(r'all_logs_top_100.csv',  sep=',', mode='w')
    df = df.drop_duplicates(subset=['log'])
    df = df[:100]
    pd.set_option('display.max_rows', None)
    #df.to_csv(r'no_duplicates_logs_top_100.csv',  sep=',', mode='w')
    output = []
    for index, row in df.iterrows():
        output.append("{:06d}".format(int(row["log"])) )
    print(output)
# Extraction function

#tag: Environment/Cumulative Reward
#Losses/Value Loss
#Losses/Policy Loss

tags=["Environment/Cumulative Reward", "Losses/Value Loss" ]


def tflog2pandas(path):
    runlog_data = pd.DataFrame({"metric": [], "value": [], "step": []})
    data_list = {}
    try:
        event_acc = EventAccumulator(path)
        event_acc.Reload()

        #tags = event_acc.Tags()["scalars"]
        for tag in tags:
            event_list = event_acc.Scalars(tag)
            for x in event_list:
                if x.step not in data_list:
                    data_list[x.step] = {tag: x.value}
                else:
                    tmp = data_list[x.step]
                    tmp[tag] = x.value
                    data_list[x.step] = tmp


    # Dirty catch of DataLossError
    except Exception:
        print("Event file possibly corrupt: {}".format(path))
        traceback.print_exc()

    return data_list

if __name__ == "__main__":
    main()