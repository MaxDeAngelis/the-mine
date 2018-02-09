using System;

public enum GAME_TIME { Paused, Quarter, Half, Full, Double, Tripple, Quadrupal }

// Work related 
public enum JOB_TYPE { None, Cancel, Move, Place, Build, Need };

public enum BUILD_SUB_TYPE { None, Tunnel, Shaft, Room };

public enum PLACE_SUB_TYPE { None, Miner, Lamp, Potato };

// Unit related 
public enum UNIT_STATE {Idle, Busy, Working, Moving};

public enum NEED_TYPE { Sleep, Eat }


// Node related
public enum NODE_TYPE { Tunnel, Stone, Shaft, Room };

public enum RESOURCE_TYPE { None, Wood, Food, Stone, Iron, Gold };


// Items
public enum ITEM_TYPE { Lamp, Food };