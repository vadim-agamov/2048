using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public class HandModel
    {
        private TileModel[] _tiles;
        public int Size => _tiles.Length;
        public IReadOnlyList<TileModel> Tiles => _tiles;
        
        public HandModel(int size) => _tiles = Enumerable.Repeat<TileModel>(null, size).ToArray();
        public void SetTile(int index, TileModel tileType) => _tiles[index] = tileType;
        public void RemoveTile(int index) => _tiles[index] = null;
        public void IncreaseSize() => _tiles = Enumerable.Repeat<TileModel>(null, _tiles.Length + 1).ToArray(); 
    }
}