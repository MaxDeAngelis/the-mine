using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobFactory {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///                                             PRIVATE VARIABLES                                                ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private JOB_TYPE _jobType;
	private BUILD_TYPE _buildType;
	private DECOR_TYPE _decorType;
	private FURNITURE_TYPE _furnitureType;
	private ITEM_TYPE _itemType;
	private DEBUG_TYPE _debugType;
	private NODE_TYPE _nodeType;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///                                                CONSTRUCTOR                                                   ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public JobFactory (JOB_TYPE job, BUILD_TYPE build, DECOR_TYPE decor, FURNITURE_TYPE furniture, ITEM_TYPE item, DEBUG_TYPE debug, NODE_TYPE node) {
		_jobType = job;
		_buildType = build;
		_decorType = decor;
		_furnitureType = furniture;
		_itemType = item;
		_debugType = debug;
		_nodeType = node;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///                                             PRIVATE FUNCTIONS                                                ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called from build job to create a Debug type job
	/// </summary>
	/// <param name="node"> The node where to create job </param>
	/// <returns>Job the job</returns>
	private Job _createDebug (Node node) {
		Job newJob = null;
		switch (_debugType) {
			case DEBUG_TYPE.Miner:
				newJob = new DebugJob (node, ItemManager.Instance.miner);
				break;
			case DEBUG_TYPE.Potato:
				newJob = new DebugJob (node, ItemManager.Instance.potato);
				break;
		}
		return newJob;
	}

	/// <summary>
	/// Called from build job to create a Node type job
	/// </summary>
	/// <param name="node"> The node where to create job </param>
	/// <returns>Job the job</returns>
	private Job _createNode (Node node) {
		Job newJob = null;
		switch (_nodeType) {
			case NODE_TYPE.Tunnel:
				newJob = new BuildNodeTunnel (node, 3, 0);
				break;
			case NODE_TYPE.Shaft:
				newJob = new BuildNodeShaft (node, 4, 0);
				break;
			case NODE_TYPE.Room:
				newJob = new BuildNodeRoom (node, 4, 0);
				break;
		}

		return newJob;
	}

	/// <summary>
	/// Called to create a build job
	/// </summary>
	/// <param name="node">The node where the job is created</param>
	/// <returns>The Job</returns>
	private Job _createBuild (Node node) {
		Job newJob = null;
		switch (_buildType) {
			case BUILD_TYPE.Node:
				return _createNode (node);
			case BUILD_TYPE.Decor:
				switch (_decorType) {
					case DECOR_TYPE.Lamp:
						Debug.Log ("Factory");
						Debug.Log (node);
						newJob = new BuildDecorLamp (node, 2, 0);
						break;
				}
				break;
			case BUILD_TYPE.Furniture:
				switch (_furnitureType) {
					case FURNITURE_TYPE.Bed:
						newJob = new BuildFurnitureBed (node, 3, 0);
						break;
				}
				break;
			case BUILD_TYPE.Item:
				switch (_itemType) {
					case ITEM_TYPE.Food:
						// TODO: Maybe another layer of food
						break;
				}
				break;
		}
		return newJob;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///                                              PUBLIC FUNCTIONS                                                ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public Job createJob (Node node) {
		if (_jobType == JOB_TYPE.Debug) {
			return _createDebug (node);
		} else if (_jobType == JOB_TYPE.Build) {

		}

		Job newJob = null;
		switch (_jobType) {
			case JOB_TYPE.Debug:
				return _createDebug (node);
			case JOB_TYPE.Build:
				return _createBuild (node);
			case JOB_TYPE.Move:
				newJob = new Move (node);
				break;
			case JOB_TYPE.Cancel:
				newJob = new Cancel (node);
				break;
		}

		return newJob;
	}
}