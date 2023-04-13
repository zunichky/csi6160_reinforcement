import matplotlib.pyplot as plt
import os
from statistics import mean 
import pandas as pd

results_root = "results"
#models = ['000191']
models = ['000181','000185','000191','000195','000197','000203','000210','000215','000219','000370','000382','000419']
MIN_RESULTS = 15
MAX_RESULTS = 101
TOP_RESULTS_MEAN = .7
results = {}

for model in models:
    cur_dir = os.path.join(os.getcwd(), results_root, model , "Robotarm")
    subfolders = [ f.path for f in os.scandir(cur_dir) if f.is_dir() ]
    
    int_folders = []
    for folder in list(subfolders):    
        int_folders.append(int(os.path.basename(folder).split('-')[1]))

    int_folders.sort()

    for cur_file in int_folders:
        folder = os.path.join(os.getcwd(), results_root, model , "Robotarm", "Robotarm-" + str(cur_file))
        cur_result = os.path.basename(folder)

        if (not os.path.isfile(os.path.join(folder,cur_result + ".csv" ))):
            # bug to move csv files without .csv
            if (os.path.isfile(os.path.join(folder,cur_result + "csv" ))):
                os.rename(os.path.join(folder,cur_result + "csv" ), os.path.join(folder,cur_result + ".csv" ))
            else:
                print(str(folder) + ": Couldn't get results")
                continue

        df  = pd.read_csv(os.path.join(folder,cur_result + ".csv"))

        if (df.shape[0] < MIN_RESULTS or df.shape[0] > MAX_RESULTS):
            print(print(str(folder) + ": Have too little/many results"))
            continue

        accuracy = (df.ObjectHit).sum() / df.shape[0]

        epoch = int(cur_result.split('-')[1])
        if model in results.keys():
            results[model][0].append(accuracy)
            results[model][1].append(epoch)
        else:
            results[model] = [[accuracy],[epoch]]

'''
result = results['000195']
plt.plot(result[1], result[0])
plt.title('model accuracy')
plt.ylabel('accuracy')
plt.xlabel('epoch')
#plt.xlim((5000,10000000))
plt.legend(['Train', 'Validation'])
plt.show()
print(result)
'''
legends = []
averages = []

for key, value in results.items():
    legends.append(key)
    averages.append(round(mean(value[0]),2))
plt.bar(legends,averages)
plt.legend()

# The following commands add labels to our figure.
plt.xlabel('Config')
plt.ylabel('Accuracy')
plt.title('Mean accuracy per config')
plt.ylim(0,1)

plt.show()

legends = []
averages = []
for key, value in results.items():
    if (mean(value[0]) > TOP_RESULTS_MEAN):
        plt.plot(value[1], value[0])
        legends.append(key)
        print(str(key) + ": " + str(round(mean(value[0]),2)))
        averages.append(round(mean(value[0]),2))
plt.title('Accuracy per model')
plt.ylabel('accuracy')
plt.xlabel('epoch')
plt.legend(legends)
plt.show()


'''
# summarize history for accuracy
plt.plot(history.history['accuracy'])
plt.plot(history.history['val_accuracy'])
plt.title('model accuracy')
plt.ylabel('accuracy')
plt.xlabel('epoch')
plt.legend(['Train', 'Validation'], loc='upper left')
plt.show()
'''