import os
import yaml

cur_count = 1

learn_rate=[0.0001,0.0005, 0.001]
batch_size=[256,1000,2000,4000]
time_horz=[32, 256, 512, 1028, 2048]
normalize=['false','true']
hiddden_layers=[128,256,512]
layers=[3,4,5]
#buff_size = batch_size * 11

with open('Configs/orig.yaml', 'r') as file:
    original_config = yaml.safe_load(file)
    cur_config = original_config
    for normalize in normalize:
        cur_config['behaviors']['Robotarm']['network_settings']['normalize'] = normalize
        for rate in learn_rate:
            cur_config['behaviors']['Robotarm']['hyperparameters']['learning_rate'] = rate
            for size in batch_size:
                cur_config['behaviors']['Robotarm']['hyperparameters']['batch_size'] = size
                cur_config['behaviors']['Robotarm']['hyperparameters']['buffer_size'] = size * 11
                for time in time_horz:
                    cur_config['behaviors']['Robotarm']['time_horizon'] = time
                    for hidden in hiddden_layers:
                        cur_config['behaviors']['Robotarm']['network_settings']['hidden_units'] = hidden
                        for layer in layers:
                            cur_config['behaviors']['Robotarm']['network_settings']['num_layers'] = layer
                            with open("Configs/" + f'{cur_count:06}' + ".yaml", 'w') as out_file:
                                yaml.dump(cur_config, out_file)
                            cur_count += 1

'''
for i in range(120):
    print(f'{i:03}')
os.system("mlagents-learn " + config + " --run-id build --force")
'''

