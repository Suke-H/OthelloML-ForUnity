import sys
import os
from pythonnet import load
load("coreclr")
import clr

# DLLのパスを追加
dll_dir = os.path.abspath(os.path.join(os.path.dirname(__file__),
    "../OthelloEnv/bin/Release/net10.0"))
sys.path.append(dll_dir)

clr.AddReference("OthelloEnv")

from Othello.Systems import EnvSystem
from Othello.States import Player

# 初期化
state = EnvSystem.CreateInitial()
print(f"初期盤面 turn={state.CurrentTurn}")