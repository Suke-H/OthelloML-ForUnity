using System;
using Cysharp.Threading.Tasks;
using Othello.States;
using Unity.Sentis;
using UnityEngine;

namespace Othello.Systems
{
    public class AIInputSystem : MonoBehaviour, IInputSystem
    {
        [SerializeField] private ModelAsset _modelAsset;
        [SerializeField] private float      _minThinkSeconds = 0.5f;

        private Worker _worker;
        private bool   _resultReady;
        private int    _resultX, _resultY;
        private bool   _predicting;

        private void Awake()
        {
            var model = ModelLoader.Load(_modelAsset);
            _worker = new Worker(model, BackendType.CPU);
        }

        private void OnDestroy() => _worker?.Dispose();

        public void StartPredict(EnvState env)
        {
            if (_predicting) return;
            PredictAsync(env, destroyCancellationToken).Forget();
        }

        public void Tick(InputState state)
        {
            state.ActionConfirmed = false;
            if (!_resultReady) return;

            state.ActionConfirmed = true;
            state.ActionX         = _resultX;
            state.ActionY         = _resultY;
            _resultReady          = false;
        }

        private async UniTaskVoid PredictAsync(EnvState env, System.Threading.CancellationToken ct)
        {
            _predicting = true;

            var thinkDelay  = UniTask.Delay(TimeSpan.FromSeconds(_minThinkSeconds), cancellationToken: ct);
            var predictTask = UniTask.RunOnThreadPool(() => Predict(env), cancellationToken: ct);

            await UniTask.WhenAll(thinkDelay, predictTask);

            (_resultX, _resultY) = predictTask.GetResult();
            _resultReady         = true;
            _predicting          = false;
        }

        private (int x, int y) Predict(EnvState env)
        {
            using var input  = BoardToTensor(env);
            _worker.Schedule(input);
            using var output = _worker.PeekOutput() as Tensor<float>;
            output.CompleteOperationsAndDownload();
            return ArgmaxLegal(output, env.LegalMoves);
        }

        private Tensor<float> BoardToTensor(EnvState env)
        {
            int me  = (int)env.CurrentTurn + 1;
            int opp = 3 - me;
            var t   = new Tensor<float>(new TensorShape(1, 2, 8, 8));
            for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                int v         = env.Board[x, y];
                t[0, 0, y, x] = v == me  ? 1f : 0f;
                t[0, 1, y, x] = v == opp ? 1f : 0f;
            }
            return t;
        }

        private static (int x, int y) ArgmaxLegal(Tensor<float> logits, bool[,] legal)
        {
            float best = float.NegativeInfinity;
            int bx = 0, by = 0;
            for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                if (!legal[x, y]) continue;
                float v = logits[0, x + y * 8];
                if (v <= best) continue;
                best = v; bx = x; by = y;
            }
            return (bx, by);
        }
    }
}