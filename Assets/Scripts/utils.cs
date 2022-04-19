using System.Collections;
using System.Collections.Generic;

public enum Player { Player1, Player2 };
public enum GuardRouteActionType { Move, Turn, Wait };

public static class GameConfig
{
    public const int ITEM_COLLIDER_BUFFER_SIZE = 1;
}
