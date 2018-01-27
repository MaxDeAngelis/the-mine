using System;

public enum JOB_TYPE { None, Cancel, Move, Place, Build };

public enum BUILD_SUB_TYPE { None, Tunnel, Shaft };

public enum PLACE_SUB_TYPE { None, Miner, Lamp };

public enum RESOURCE_TYPE { None, Wood, Food, Stone, Iron, Gold };

public enum UNIT_STATE {Idle, Busy, Working, Moving};

public enum NODE_TYPE { Tunnel, Stone, Shaft };

public enum ITEM_TYPE { Lamp }

public enum GAME_TIME { Paused, Quarter, Half, Full, Double, Tripple, Quadrupal }