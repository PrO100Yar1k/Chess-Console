using Chess_Console.Core.Board;
using Chess_Console.Core.Pieces.Base;
using Chess_Console.Core.Common.Enums;
using Chess_Console.Infrastructure.Common;

namespace Chess_Console.Views
{
    internal class BoardRenderer
    {
        private readonly BoardModel _boardModel;

        private const int _emptySpacesY = 1;
        private const int _emptySpacesX = 4;

        public BoardRenderer(BoardModel boardModel)
        {
            _boardModel = boardModel;
        }

        public void DisplayBoard()
        {
            for (int i = 0; i < _emptySpacesY; i++)
                DisplayView.WriteLine();

            for (int y = 0; y < BoardModel._boardHeight; y++)
            {
                int rowNumber = BoardModel._boardHeight - y;

                DisplayView.Write($"{new string(' ', _emptySpacesX)}{rowNumber}| ");

                for (int x = 0; x < BoardModel._boardWidth; x++)
                {
                    Vector2 targetPosition = new Vector2(x, y);

                    ChessPiece chessPiece = _boardModel.GetBoardField(targetPosition);

                    if (chessPiece != null)
                    {
                        string initialSymbolString = chessPiece.ChessPieceChar.ToString();

                        char finalSymbol = chessPiece.ChessSide == ChessSide.Player
                            ? initialSymbolString.ToUpper()[0]
                            : initialSymbolString.ToLower()[0];

                        DisplayView.Write($"{finalSymbol} ");
                    }
                    else
                    {
                        DisplayView.Write($"{GameConstants.EmptyChar} ");
                    }
                }

                DisplayView.WriteLine();
            }

            DisplayView.WriteLine(new string(' ', _emptySpacesX + 2) + new string('-', BoardModel._boardWidth * 2));
            DisplayView.Write(new string(' ', _emptySpacesX + 3));

            for (int i = 0; i < BoardModel._boardWidth; i++)
            {
                DisplayView.Write($"{(char)('a' + i)} ");
            }

            DisplayView.WriteLine();
        }
    }
}
