import torch
import torch.nn as nn


class OthelloNet(nn.Module):
    """(1, 2, 8, 8) -> policy_logits (64), value (1)"""

    def __init__(self):
        super().__init__()
        self.conv = nn.Sequential(
            nn.Conv2d(2, 64, 3, padding=1), nn.ReLU(),
            nn.Conv2d(64, 128, 3, padding=1), nn.ReLU(),
            nn.Conv2d(128, 128, 3, padding=1), nn.ReLU(),
        )
        self.head = nn.Sequential(
            nn.Flatten(),
            nn.Linear(128 * 8 * 8, 256), nn.ReLU(),
        )
        self.policy_head = nn.Linear(256, 64)
        self.value_head  = nn.Linear(256, 1)

    def forward(self, x):           # x: (B, 2, 8, 8)
        h = self.head(self.conv(x))
        return self.policy_head(h), self.value_head(h)
