import os
import time

startConfig=int(input("Start Config: "))
endConfig=int(input("End Config: "))

for i in range(startConfig, endConfig):
    config = "Configs/" + f'{i:06}' + ".yaml"
    print("Config: " + config)
    os.system("mlagents-learn " + config + " --run-id " + f'{i:06}' )
    #os.system("mlagents-learn " + config + " --run-id " + f'{i:06}'  + " --resume")
    time.sleep(10)


