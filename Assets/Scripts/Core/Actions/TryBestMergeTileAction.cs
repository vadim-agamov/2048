using Core.Models;
using Core.Views;
using Cysharp.Threading.Tasks;
using Modules.Actions;
using UnityEngine;

namespace Core.Actions
{
    public class TryBestMergeTileAction : IAction
    {
        private readonly LogicActionBase _logicAction;
        private readonly VisualActionBase _visualAction;
        private readonly BoardModel _model;
        private readonly BoardView _view;
        private readonly Context _context;

        public TryBestMergeTileAction(BoardModel model, BoardView view, TileModel other)
        {
            _model = model;
            _view = view;
            _context = new Context();
            
            _logicAction = new Logic(_model, other, _context);
            if (view != null)
            {
                _visualAction = new Visual(_model, _view, _context);
            }
        }
        
        public async UniTask<bool> Do()
        {
            var success = _logicAction.Do();
            if (success)
            {
                await _visualAction.Do();
                var other = _model.Tiles[_context.ToPosition.x, _context.ToPosition.y];
                success |= await new TryBestMergeTileAction(_model, _view, other).Do();
            }

            return success;
        }

        private class Context
        {
            public Vector2Int ToPosition;
            public Vector2Int FromPosition;
        }
        
        private class Logic : LogicActionBase
        {
            private readonly Context _context;
            private readonly TileModel _tileModel;
            private readonly BoardModel _model;

            public Logic(BoardModel model, TileModel tileModel, Context context)
            {
                _model = model;
                _tileModel = tileModel;
                _context = context;
            }

            public override bool Do()
            {
               var success = new BestMergeTileFinder(_model, _tileModel).TryFind(out _context.FromPosition, out _context.ToPosition);
               if (success)
               {
                   _model.Tiles[_context.FromPosition.x, _context.FromPosition.y] = null;
                   var toTileModel = _model.Tiles[_context.ToPosition.x, _context.ToPosition.y];
                   _model.Tiles[_context.ToPosition.x, _context.ToPosition.y] = new TileModel(toTileModel.Type.Next(), _context.ToPosition);
               }

               return success;
            }
        }

        private class Visual : VisualActionBase
        {
            private readonly BoardView _view;
            private readonly Context _context;
            private readonly BoardModel _model;

            public Visual(BoardModel model, BoardView view, Context context)
            {
                _model = model;
                _view = view;
                _context = context;
            }

            public override async UniTask Do()
            {
                var tileViewFrom = _view.TileCells[_context.FromPosition.x, _context.FromPosition.y];
                var tileViewTo = _view.TileCells[_context.ToPosition.x, _context.ToPosition.y];

                await tileViewFrom.MoveTo(tileViewTo.transform.position);

                _view.RemoveTile(_context.FromPosition);
                _view.RemoveTile(_context.ToPosition);
                _view.CreateTile(_model.Tiles[_context.ToPosition.x, _context.ToPosition.y]);
            }
        }
    }
}