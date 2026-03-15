import torch
from model import OthelloNet


def export(weights_path="othello_policy.pth", output_path="othello_policy.onnx"):
    model = OthelloNet()
    model.load_state_dict(torch.load(weights_path, weights_only=True))
    model.eval()

    dummy = torch.zeros(1, 2, 8, 8)

    torch.onnx.export(
        model,
        dummy,
        output_path,
        input_names=["board"],
        output_names=["policy_logits", "value"],
        dynamic_axes={"board": {0: "batch"}},
        opset_version=17,
    )
    print(f"exported: {output_path}")


if __name__ == "__main__":
    export()
