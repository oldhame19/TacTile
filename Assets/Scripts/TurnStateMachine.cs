public static class TurnStateMachine
{
    public static bool Advance(GameState state)
    {
        if (state == null)
        {
            return false;
        }

        switch (state.CurrentPhase)
        {
            case TurnPhase.BeginTurn:
                state.SelectedTile = null;
                state.CurrentPhase = TurnPhase.SelectTile;
                return true;

            case TurnPhase.SelectTile:
                if (state.SelectedTile == null)
                {
                    return false;
                }

                state.CurrentPhase = TurnPhase.PlaceTile;
                return true;

            case TurnPhase.PlaceTile:
                if (state.SelectedTile == null ||
                    !state.SelectedTile.IsPlaced)
                {
                    return false;
                }

                state.CurrentPhase = TurnPhase.EndTurn;
                return true;

            case TurnPhase.EndTurn:
                state.CompletedTurns++;

                state.CurrentPlayer =
                    state.CurrentPlayer == PlayerId.Player1
                        ? PlayerId.Player2
                        : PlayerId.Player1;

                state.SelectedTile = null;
                state.CurrentPhase = TurnPhase.BeginTurn;
                return true;

            default:
                return false;
        }
    }
}