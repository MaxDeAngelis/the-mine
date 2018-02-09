using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PUBLIC VARIABLES                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private float SLEEP_INTERVAL = 20f;
    private float HUNGER_INTERVAL = 10f;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float movementSpeed = 0.5f;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private UNIT_STATE _state = UNIT_STATE.Idle;
	private Job _currentJob;
	private PathFinder _pathFinder = new PathFinder();
	private Node _nextPathNode;
    private IEnumerator _doJobCoroutine;

	// TODO: will want some concept of normal speed for walk penilties 
	// private float _normalMovementSpeed;

    // Progress Marker
    private GameObject _progressMarker;
    private IEnumerator _progressCoroutine;

    // Talking
    private TextMesh _talk;
    private IEnumerator _speachCoroutine;

    // Needs
    private bool _isFulfillingNeed = false;

    // Sleep
    private IEnumerator _sleepCoroutine;
    private bool _isSleepy = false;

    // Hunger
    private IEnumerator _hungerCoroutine;
    private bool _isHungry = false;

    // Stats
    private float _life = 100f;
    private float _maxLife = 100f;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called when the Game Object is started
	/// </summary>
	private void Awake () {
        _talk = gameObject.FindChildWithTag("Marker-Text").GetComponent<TextMesh>();
        _talk.gameObject.GetComponent<Renderer>().sortingLayerName = "Marker-Text";
	}

	/// <summary>
	/// Called once per frame at a consistent 60FPS
	/// </summary>
	private void FixedUpdate () {
        if (!isMoving() && !_isFulfillingNeed) {
            _checkNeeds();
        }

		if (isWorking()) {
			return;
        } else if (isIdle()) {
            Node currentNode = MapManager.Instance.getNode(transform.position);
            _currentJob = JobManager.Instance.getJob(currentNode);

			if (_currentJob != null) {
                _currentJob.addWorker(this);
				_state = UNIT_STATE.Busy;
			}
		} else if (isMoving()) {
			Vector3 target = _nextPathNode.transform.position;
			Vector3 dir = (target - transform.position).normalized;		
			dir *= movementSpeed * Time.deltaTime;									
			GetComponent<Rigidbody2D>().velocity = dir;	

			if (Vector3.Distance (transform.position, target) < 0.1f) {
				transform.position = target;
				if (_pathFinder.isEmpty()) {
					GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
					_pathFinder.nullify();
                    _state = UNIT_STATE.Working;

                    // Clear
                    MapManager.Instance.setNodeMarker(_currentJob.getLocationNode(), false, Color.yellow, "");

                    // Job
                    _doJobCoroutine = _doJob();
                    StartCoroutine(_doJobCoroutine);

                    // Progress
                    _progressCoroutine = _showProgress();
                    StartCoroutine(_progressCoroutine);
				} else {
					_nextPathNode = _pathFinder.getNextNode();
				}
			}
        } else if (_currentJob != null) {
			List<Node> jobWorkLocations = _currentJob.getWorkLocations();

			// If the Job has work nodes then find the best path
			if (jobWorkLocations.Count > 0) {
				_state = UNIT_STATE.Moving;

				foreach(Node location in jobWorkLocations) {
					_pathFinder.findPath(_getNodeStandingOn(), location);
				}
			} 

			// If no path was found then return job
			if (_pathFinder.isEmpty()) {
				_state = UNIT_STATE.Idle;

                _currentJob.removeWorker(this);
				JobManager.Instance.blockJob(_currentJob);
			} else {
				/* DEBUG */ _pathFinder.highlight();
				_nextPathNode = _pathFinder.getNextNode();
			}
		}
	}
        
	/// <summary>
	/// Called to find the ground node that the unit is standing on
	/// </summary>
	/// <returns>The node that the unit is standing on</returns>
	private Node _getNodeStandingOn() {
		Vector3 locationStandingOn = transform.position;
		locationStandingOn.x = Mathf.RoundToInt(locationStandingOn.x);
		locationStandingOn.y = Mathf.RoundToInt(locationStandingOn.y);
		locationStandingOn.z = 0;

		return MapManager.Instance.getNode(locationStandingOn);
	}

    /// <summary>
    /// Called to check if the unit needs anything
    /// </summary>
    private void _checkNeeds() {
        Job _newJob = null;
        // If sleep timer is not running then start one
        if (_sleepCoroutine == null) {
            _sleepCoroutine = _sleepTimer();
            StartCoroutine(_sleepCoroutine);
        }

        // If the hunger timer is not running then start it
        if (_hungerCoroutine == null) {
            _hungerCoroutine = _hungerTimer();
            StartCoroutine(_hungerCoroutine);
        }

        // If unit is sleepy then cancel current job and sleep
        if (_isSleepy) {
            // Cancel timer for now
            StopCoroutine(_sleepCoroutine);
            _sleepCoroutine = null;

            speak("I'm sleepy", 1f);

            // Start new sleep job
            _newJob = new Sleep();
        } else if (_isHungry) {
            // Cancel timer for now
            StopCoroutine(_hungerCoroutine);
            _hungerCoroutine = null;

            speak("I'm hungry", 1f);

            // Start new sleep job
            _newJob = new Eat();

            // If there is no food available then take damage
            if (!((Eat)_newJob).isFoodAvailable()) {
                takeDamage(10f);
                _newJob = null;
                needFulfilled(NEED_TYPE.Eat);
            }
        }
            
        // If unit is hungry or sleepy then cancel work
        if (_newJob != null) {
            _isFulfillingNeed = true;

            // If we have a job return it
            if (_currentJob != null) {
                _currentJob.removeWorker(this);
                JobManager.Instance.returnJob(_currentJob);
                cancelJob();
            }

            _currentJob = _newJob;
            _currentJob.addWorker(this);
            _state = UNIT_STATE.Busy;
        }
    }
        
    /// <summary>
    /// Starts a sleep timer to know when the next time the unit needs to sleep
    /// </summary>
    /// <returns>The sleep timer.</returns>
    private IEnumerator _sleepTimer() {
        yield return new WaitForSeconds(SLEEP_INTERVAL);
        _isSleepy = true;
    }

    /// <summary>
    /// Starts a sleep timer to know when the next time the unit needs to sleep
    /// </summary>
    /// <returns>The sleep timer.</returns>
    private IEnumerator _hungerTimer() {
        yield return new WaitForSeconds(HUNGER_INTERVAL);
        _isHungry = true;
    }

    /// <summary>
    /// Called to actually do the work of the current Job
    /// </summary>
    private IEnumerator _doJob() {
        speak(_currentJob.getActionText(), _currentJob.getDuration() / 2f);
        Vector3 target = _currentJob.getLocation();
        target.y = transform.position.y;

        // Yield for the duration of the boost
        yield return new WaitForSeconds(_currentJob.getDuration());

        if (_currentJob != null) {
            StopCoroutine(_progressCoroutine);
            Destroy(_progressMarker);

            _currentJob.complete();
            _currentJob = null;
        }
        _state = UNIT_STATE.Idle;

    }

    /// <summary>
    /// Called to show the progress of a job
    /// </summary>
    private IEnumerator _showProgress() {
        float currentScale = 0f;
        float currentY = -0.45f;
        float deltaTime = _currentJob.getDuration() / 100;
        float dx = 0.01f;
        Color color = Color.gray;
        color.a = 0.5f;

        // Create a new marker
        _progressMarker = MonoBehaviour.Instantiate(ItemManager.Instance.marker) as GameObject;
        _progressMarker.transform.SetParent(_currentJob.getLocationNode().transform);
        _progressMarker.transform.localPosition = new Vector3(0f, currentY, 0f);
        _progressMarker.transform.localScale = new Vector3(0.9f, 0f, 1f);
        _progressMarker.GetComponent<SpriteRenderer>().material.color = color;

        // Loop and adjust until full scale
        while (currentScale < 0.9f) {
            currentScale += dx;
            currentY += dx / 2;
            _progressMarker.transform.localScale = new Vector3(0.9f, currentScale, 1f);
            _progressMarker.transform.localPosition = new Vector3(0f, currentY, 1f);
            yield return new WaitForSeconds(deltaTime);
        }
    }

    /// <summary>
    /// Called to update the health bar for the unit
    /// </summary>
    private IEnumerator _updateHealth() {
        GameObject lifeContainer = gameObject.FindChildWithTag("Unit-Life-Container");
        Transform life = lifeContainer.FindChildWithTag("Unit-Life").transform;
        lifeContainer.SetActive(true);

        // Calculate the current calculations
        float currentHealh = (_life / _maxLife);
        float currentScale = life.localScale.x;
        float currentX = life.localPosition.x;
        float deltaTime = 0.002f;
        float dx = 0.01f;

        // Loop and adjust until zero
        while (currentScale > currentHealh) {
            currentScale -= dx;
            currentX -= dx / 2;
            life.localScale = new Vector3(currentScale, life.localScale.y, life.localScale.z);
            life.localPosition = new Vector3(currentX, life.localPosition.y, life.localPosition.z);
            yield return new WaitForSeconds(deltaTime);
        }
    }

    /// <summary>
    /// Enumerator for speaking, shows text for given time
    /// </summary>
    /// <param name="text">Text to say</param>
    /// <param name="duration">Duration to show text</param>
    private IEnumerator _say(string text, float duration) {
        _talk.text = text;
        _talk.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        _talk.gameObject.SetActive(false);
    }

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to see if the unit is idle
	/// </summary>
	/// <returns><c>true</c>, if unit is idle, <c>false</c> otherwise.</returns>
	public bool isIdle() {
		return (_state == UNIT_STATE.Idle);
	}
	/// <summary>
	/// Called to see if the unit is moving
	/// </summary>
	/// <returns><c>true</c>, if unit is moving, <c>false</c> otherwise.</returns>
	public bool isMoving() {
		return (_state == UNIT_STATE.Moving);
	}

	/// <summary>
	/// Called to check if the unit is currently working
	/// </summary>
	/// <returns><c>true</c>, if working, <c>false</c> otherwise.</returns>
	public bool isWorking() {
		return (_state == UNIT_STATE.Working);
	}

    /// <summary>
    /// Called when the unit is taking damage
    /// </summary>
    /// <param name="amount">Amount to reduce health</param>
    public void takeDamage(float amount) {
        float newLife = _life - amount;

        if (newLife <= 0f) {
            _life = 0f;
            Destroy(gameObject);
        } else {
            _life -= amount;
            StartCoroutine(_updateHealth());
        }
    }

    /// <summary>
    /// Called to fulfill a need
    /// </summary>
    /// <param name="type">Type of need</param>
    public void needFulfilled(NEED_TYPE type) {
        _isFulfillingNeed = false;

        switch (type) {
            case NEED_TYPE.Eat:
                _isHungry = false;
                break;
            case NEED_TYPE.Sleep:
                _isSleepy = false;
                break;
        }
    }

    /// <summary>
    /// Called to cancel the current job and return to idle
    /// </summary>
    public void cancelJob() {
        _currentJob = null;
        _state = UNIT_STATE.Idle;
        if (_doJobCoroutine != null) {
            StopCoroutine(_doJobCoroutine);
        }

        if (_progressCoroutine != null) {
            StopCoroutine(_progressCoroutine);
            Destroy(_progressMarker);
        }
    }

    /// <summary>
    /// Called to have the unit speak
    /// </summary>
    /// <param name="text">Text to say</param>
    /// <param name="duration">Duration to show text</param>
    public void speak(string text, float duration) {
        if (_speachCoroutine != null) {
            StopCoroutine(_speachCoroutine);
        }

        _speachCoroutine = _say(text, duration);

        StartCoroutine(_speachCoroutine);
    }
}
