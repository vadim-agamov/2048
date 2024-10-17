using System.Collections.Generic;
using System.Threading;
using Core.Actions;
using Core.Models;
using Core.Views;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;

namespace Core.Controller
{
    public class BoardController : IService
    {
        private readonly BoardView _boardView;
        private readonly BoardModel _boardModel;
        private bool _isReady;

        public BoardController(BoardView boardView)
        {
            _boardView = boardView;
            _boardModel = new BoardModel(new Vector2Int(6, 6))
            {
                Tiles =
                {
                    [1, 1] = new TileModel(TileType.Tile1),
                    [4, 4] = new TileModel(TileType.Tile2),
                    [4, 5] = new TileModel(TileType.Tile64)
                }
            };
            _boardModel.Hand.SetTile(0, new TileModel(TileType.Tile1));
            _boardModel.Hand.SetTile(1, new TileModel(TileType.Tile1));
            _boardModel.Hand.SetTile(2, new TileModel(TileType.Tile1));
        }
        
        void IService.Dispose()
        {
        }

        public void PutTileOnBoard(Vector2Int cell, TileView tileView)
        {
            Do().Forget();

            async UniTask Do()
            {
                var success = await new PutBlockOnBoardAction(_boardModel, _boardView, cell, tileView).Do();
                if(!success)
                {
                    tileView.RestorePosition();
                }
            }
        }

        public async UniTask Initialize(CancellationToken cancellationToken)
        {
            _boardView.Initialize(this, _boardModel);
            await UniTask.Delay(1000);
        }
    }
}