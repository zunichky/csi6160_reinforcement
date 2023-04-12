from pathlib import Path
import shutil
import os
import subprocess
import csv

REBUILD=False
RUNMODEL=True
project_root = "/home/dev/code/csi6160_reinforcement/"
results_root = "results/"
build_folder = "builds/linux_AI/"
models=["000181"]
#models = ['000181','000185','000191','000195','000197','000203','000210','000215','000219','000370','000382','000419']
build_script = os.path.join(project_root, "build.sh")


def main():
    for model in models:
        cur_dir = os.path.join(os.getcwd(), results_root, model , "Robotarm")
        print("----------------")
        print("Model: " + model)
        print("----------------")
        for file in sorted(os.listdir(cur_dir)):
            if not file.endswith(".onnx"):
                continue       
            run = os.path.splitext(file)[0]
            if (not os.path.exists(os.path.join(cur_dir, run)) or REBUILD):
                #print("Building: "+ run)
                #copy current model
                shutil.copyfile(os.path.join(cur_dir, file), os.path.join(project_root, "Assets", "NNModels", "Robotarm.onnx"))
                process = subprocess.Popen(build_script, shell=False, stdout=subprocess.PIPE)
                process.wait()
                
                #Remove previous build
                if os.path.exists(os.path.join(cur_dir, run)):
                    shutil.rmtree(os.path.join(cur_dir, run))
                
                os.makedirs(os.path.join(cur_dir, run), exist_ok=True)
                shutil.move(os.path.join(project_root, build_folder), os.path.join(cur_dir, run))
            

            if (RUNMODEL):
                #print("Running: "+ run)
                if os.path.exists(os.path.join(cur_dir, "results.csv")):
                    os.remove(os.path.join(cur_dir, "results.csv"))
                if ( os.path.exists(os.path.join(cur_dir, run, run + "csv"))):
                    print("Already have results")
                    continue
                process = subprocess.Popen(os.path.join(cur_dir, run, "linux_AI", "build.X86_64"),cwd=cur_dir, shell=True, stdout=subprocess.PIPE)
                process.wait()
                shutil.move(os.path.join(cur_dir, "results.csv"), os.path.join(cur_dir, run, run + "csv"))
                with open(os.path.join(cur_dir, run, run + "csv")) as file:
                    headerline = next(file)
                    total = 0
                    success = 0
                    for row in csv.reader(file):
                        total += 1
                        if row[4] == "True":
                            success += 1
                    print(run + ": " + str(success) + '/' + str(total) + ': ' + str(success/total) )

if __name__ == "__main__":
    main()
