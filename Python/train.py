import numpy as np
import torch
import torch.nn as nn
import pfrl

from othello_env_wrapper import OthelloGymEnv  # DLL初期化はここで行われる
from Othello.States import Player
from model import OthelloNet


# ---- pfrl 用 Actor-Critic ラッパー ----

class OthelloAC(nn.Module):
    def __init__(self, net: OthelloNet):
        super().__init__()
        self.net = net

    def forward(self, obs):
        logits, value = self.net(obs)
        dist = torch.distributions.Categorical(logits=logits)
        return dist, value


# ---- 関数 ----

def build_agent(lr=3e-4, update_interval=2048, minibatch_size=256, epochs=10):
    model = OthelloAC(OthelloNet())
    opt   = torch.optim.Adam(model.parameters(), lr=lr)
    agent = pfrl.agents.PPO(
        model,
        opt,
        gpu=-1,
        gamma=0.99,
        lambd=0.95,
        value_func_coef=0.5,
        entropy_coef=0.01,
        update_interval=update_interval,
        minibatch_size=minibatch_size,
        epochs=epochs,
        clip_eps=0.2,
        clip_eps_vf=None,
        standardize_advantages=True,
        act_deterministically=False,
        recurrent=False,
    )
    return model, agent


def train(model, agent, env, steps=2_000_000, log_interval=50_000):
    print("start training...")
    obs, _ = env.reset()
    for step in range(1, steps + 1):
        action = agent.act(obs)

        legal = env.legal_mask()
        if not legal[action]:
            action = int(np.random.choice(np.where(legal)[0]))

        obs, reward, terminated, truncated, _ = env.step(action)
        reset = terminated or truncated
        agent.observe(obs, reward, reset, reset)

        if reset:
            obs, _ = env.reset()

        if step % log_interval == 0:
            print(f"[step {step:>8}] {agent.get_statistics()}")

    torch.save(model.net.state_dict(), "othello_policy.pth")
    print("saved: othello_policy.pth")


if __name__ == "__main__":
    env          = OthelloGymEnv(agent_color=Player.Black)
    model, agent = build_agent(
        lr=3e-4,
        update_interval=50,
        minibatch_size=10,
        epochs=3,
    )
    train(
        model, agent, env,
        steps=50,
        log_interval=50,
    )
