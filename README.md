# Robot Arm ML using Reinforcement 

## Environment Setup
ml-agents release 5 is used in the training. This exact version is needed. https://github.com/Unity-Technologies/ml-agents/releases/tag/release_5  
Do not clone the following items in the same directory
```
git clone git@github.com:zunichky/csi6160_reinforcement.git
git clone -b release_5 git@github.com:Unity-Technologies/ml-agents.git
```
python setup (I have python 3.8.10 installed. I believe minimum req is 3.7)
```
cd csi6160_reinforcement
python -m venv venv
source venv/bin/activate
pip install mlagents==0.18.1
pip install protobuf==3.20.*
pip install mxnet-mkl==1.6.0 numpy==1.23.1
```
## Unity Setup
- open csi6160_reinforcement in unity hub
- Most likely will have to enter safemode since we have a custom ml-agent repo
- Open package manager
- Click plus sign -> add package from disk
- navigate to the ml-agents that you cloned previously. ml-agents/com.unity.ml-agents and select package.json. The defualt ml-agents is too new 
- Unity crashed but after a restart, the project loaded. 
- File -> open scene; ai is the nomral interface, training has a farm of robots.
- Run the scene
<br> Credit: https://github.com/rkandas/RobotArmMLAgentUnity

## Training  
In a terminal window, start a learning session  
```
source venv/bin/activate
mlagents-learn trainer_config.yaml --run-id custom_train
```
In robotcontroller, make sure train is selected. There is also a scene that will train a farm of robots
