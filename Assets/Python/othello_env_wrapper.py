import sys
import os
import numpy as np
import gym
from gym import spaces

from pythonnet import load
load("coreclr")
import clr

dll_dir = os.path.abspath(os.path.join(os.path.dirname(__file__),
    "../OthelloEnv/bin/Release/net10.0"))
sys.path.append(dll_dir)
clr.AddReference("OthelloEnv")

from Othello.Systems import EnvSystem
from Othello.States  import Player


class OthelloGymEnv(gym.Env):
    """
    observation : float32 (2, 8, 8)
        ch0 = 自分の石, ch1 = 相手の石
    action      : int 0-63  (index = x + y*8)
    """
    metadata = {"render_modes": []}

    def __init__(self, agent_color=Player.Black):
        super().__init__()
        self.agent_color       = agent_color
        self.observation_space = spaces.Box(0.0, 1.0, shape=(2, 8, 8), dtype=np.float32)
        self.action_space      = spaces.Discrete(64)
        self._state            = None

    # ---- gymnasium API ----

    def reset(self, *, seed=None, options=None):
        super().reset(seed=seed)
        self._state = EnvSystem.CreateInitial()
        return self._obs(), {}

    def step(self, action: int):
        x, y       = int(action % 8), int(action // 8)
        next_state = EnvSystem.Apply(self._state, x, y)

        if not next_state.IsActionSuccess:
            return self._obs(), -1.0, False, False, {}

        self._state = next_state
        terminated  = self._is_terminal()
        reward      = self._reward() if terminated else 0.0
        return self._obs(), reward, terminated, False, {}

    def legal_mask(self) -> np.ndarray:
        legal = self._state.LegalMoves
        mask  = np.zeros(64, dtype=bool)
        for x in range(8):
            for y in range(8):
                if legal[x, y]:
                    mask[x + y * 8] = True
        return mask

    # ---- helpers ----

    def _obs(self) -> np.ndarray:
        board = self._state.Board
        me    = int(self.agent_color) + 1
        opp   = 3 - me
        obs   = np.zeros((2, 8, 8), dtype=np.float32)
        for x in range(8):
            for y in range(8):
                v = board[x, y]
                if v == me:  obs[0, y, x] = 1.0
                if v == opp: obs[1, y, x] = 1.0
        return obs

    def _is_terminal(self) -> bool:
        return not np.any(self.legal_mask())

    def _reward(self) -> float:
        board     = self._state.Board
        me        = int(self.agent_color) + 1
        opp       = 3 - me
        score_me  = sum(board[x, y] == me  for x in range(8) for y in range(8))
        score_opp = sum(board[x, y] == opp for x in range(8) for y in range(8))
        if score_me > score_opp: return  1.0
        if score_me < score_opp: return -1.0
        return 0.0
