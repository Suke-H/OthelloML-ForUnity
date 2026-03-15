import io
import torch
import onnx
from model import OthelloNet


def export(weights_path="othello_policy.pth", output_path="../Assets/Models/othello_policy.onnx"):
    model = OthelloNet()
    model.load_state_dict(torch.load(weights_path, weights_only=True))
    model.eval()

    dummy = torch.zeros(1, 2, 8, 8)

    buf = io.BytesIO()
    torch.onnx.export(
        model,
        dummy,
        buf,
        input_names=["board"],
        output_names=["policy_logits", "value"],
        opset_version=18,
    )
    buf.seek(0)
    onnx_model = onnx.load_model_from_string(buf.read())
    onnx.save(onnx_model, output_path)
    print(f"exported: {output_path}")


if __name__ == "__main__":
    export()
