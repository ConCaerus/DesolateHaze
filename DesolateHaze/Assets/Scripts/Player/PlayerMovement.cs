using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : Singleton<PlayerMovement> {
    #region GLOBALS
    [SerializeField] float speed, speedAccSpeed;
    [SerializeField] float jumpHeight, jumpAccSpeed;
    [SerializeField] float jumpDeathDist;
    [SerializeField] float ropeClimbSpeed;

    public Rigidbody rb;
    [SerializeField] Transform spriteTrans;

    Coroutine queuedJump = null;
    Coroutine coyoteTime = null;
    Coroutine jumpCanceler = null;
    Coroutine grabWaiter = null;

    //  ground things
    Collider usedGround;
    float lastGroundedY;

    //  rope things
    RopeInstance hr = null;
    RopeInstance heldRope {
        get { return hr; }
        set {
            if(hr == value) return;
            if(hr != null)
                hr.dropPlayer();
            hr = value;
            if(hr != null)
                hr.holdPlayer(rb);
        }
    }

    //  push things
    Transform curPushing = null;
    Vector3 pushOffset = Vector2.zero;

    //  input things
    InputMaster controls;
    Vector2 savedInput;
    public bool facingRight { get; private set; } = true;
    float maxVelocity = 50f;

    //  jump things
    bool jumpHeld = false;

    //  specials
    pMovementState cs = pMovementState.None;
    pMovementState curState {
        get { return cs; }
        set {
            if(cs == value) return;
            Vector3 carryover = Vector2.zero;
            if(cs == pMovementState.RopeClimbing) {
                carryover = heldRope.getExitVel();
                heldRope = null;
            }
            cs = value;
            rb.useGravity = cs != pMovementState.LadderClimbing && cs != pMovementState.RopeClimbing;
            updateInput(controls.Player.Move.ReadValue<Vector2>());

            rb.linearVelocity += carryover;
        }
    }
    bool cm = true;
    public bool canMove {
        get { return cm; }
        set {
            cm = value;
            if(cm)
                updateInput(controls.Player.Move.ReadValue<Vector2>());
        }
    }
    bool g = false;
    public bool grounded {
        get { return g; }
        private set {
            g = value;
            rb.angularDamping = g ? .05f : 0f;
            if(g) {
                //  checks for fall damage
                if(lastGroundedY - transform.position.y >= jumpDeathDist) {
                    Debug.Log("fall damage killed you");
                }

                //  ends coyote time
                if(coyoteTime != null)
                    StopCoroutine(coyoteTime);
                coyoteTime = null;

                //  ends jump canceler
                if(jumpCanceler != null)
                    StopCoroutine(jumpCanceler);
                jumpCanceler = null;

                //  jumps if jumps
                if(jumpHeld)
                    doJump();
            }
            else
                lastGroundedY = transform.position.y;

            //  sets cur state
            if(curState != pMovementState.LedgeClimbing && curState != pMovementState.LadderClimbing && curState != pMovementState.RopeClimbing)
                curState = grounded ? pMovementState.Walking : pMovementState.Falling;
        }
    }

    enum pMovementState {
        None, Walking, Falling, Pushing, LedgeClimbing, RopeClimbing, LadderClimbing
    }
    #endregion

    #region COLLISIONS
    private void OnCollisionEnter(Collision col) {
        //  pushables / boxes
        if(col.gameObject.tag == "Box") {
            if(grounded && usedGround != col.collider) {
                curPushing = col.gameObject.transform;
                pushOffset = col.gameObject.transform.position - transform.position;
                curState = pMovementState.Pushing;
                facePos(col.gameObject.transform.position.x);
            }
        }

        //  ropes
        else if(curState != pMovementState.RopeClimbing && col.gameObject.tag == "Rope" && col.transform.parent.TryGetComponent<RopeInstance>(out var ri) && ri.canHold()) {
            if(!grounded) {
                curState = pMovementState.RopeClimbing;
                heldRope = ri;
                rb.linearVelocity = Vector3.zero;
            }
        }

        //  hazards
        else if(col.gameObject.tag == "Hazard" && col.gameObject.TryGetComponent<HazardInstance>(out var hi)) {
            hi.triggerHazard(this);
        }
    }
    private void OnTriggerEnter(Collider col) {
        //  ledge climbing
        if(grabWaiter == null && col.gameObject.tag == "Ledge") {
            var offset = col.transform.position - transform.position;
            if(offset.y > -1.25f && offset.x >= 0f == facingRight) {
                curState = pMovementState.LedgeClimbing;
                transform.DOMove(col.gameObject.transform.GetChild(0).position, .5f).OnComplete(() => { curState = grounded ? pMovementState.Walking : pMovementState.Falling; });
            }
        }

        //  ladders
        else if(col.gameObject.tag == "Ladder") {
            if(!grounded)
                curState = pMovementState.LadderClimbing;
        }

        //  hazards
        else if(col.gameObject.tag == "Hazard" && col.gameObject.TryGetComponent<HazardInstance>(out var hi)) {
            hi.triggerHazard(this);
        }
    }
    private void OnTriggerExit(Collider col) {
        //  pushables / boxes
        if(col.gameObject.tag == "Box") {
            curState = grounded ? pMovementState.Walking : pMovementState.Falling;
        }

        //  ladders
        else if(col.gameObject.tag == "Ladder") {
            curState = grounded ? pMovementState.Walking : pMovementState.Falling;
        }
    }
    #endregion


    private void Start() {
        DOTween.Init();
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Move.performed += ctx => updateInput(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => updateInput(ctx.ReadValue<Vector2>());
        controls.Player.Jump.performed += ctx => jump();
        controls.Player.Jump.canceled += ctx => cancelJump();

        lastGroundedY = transform.position.y;

        if(Saver.loadPlayer() != null) {
            transform.position = Saver.loadPlayer().pos;
        }
    }
    private void FixedUpdate() {
        move();
    }
    private void OnDisable() {
        controls.Disable();
    }

    #region INPUT LOGIC
    void updateInput(Vector2 dir) {
        if(!canMove || PauseCanvas.I.paused) return;
        savedInput = dir.normalized;
        updateFaceDir();
    }
    void updateFaceDir() {
        if(PauseCanvas.I.paused) return;
        if(curState != pMovementState.Falling && curState != pMovementState.Pushing && Mathf.Abs(savedInput.x) != 0f) {
            facingRight = savedInput.x >= 0;
            //  flips character
            spriteTrans.transform.rotation = Quaternion.Euler(0f, facingRight ? 0f : 180f, 0f);
        }
    }
    void facePos(float x) {
        if(PauseCanvas.I.paused) return;
        facingRight = x >= transform.position.x;
        //  flips character
        spriteTrans.transform.rotation = Quaternion.Euler(0f, facingRight ? 0f : 180f, 0f);
    }
    void move() {
        Vector2 target = (canMove || !grounded) ? rb.linearVelocity : Vector3.zero;

        if(canMove) {
            switch(curState) {
                case pMovementState.None:   //  return to stand-still
                    break;

                case pMovementState.Walking:    //  update x axis velocity
                    target.x = savedInput.x * speed * 100f * Time.fixedDeltaTime;
                    break;

                case pMovementState.Pushing:
                    target.x = savedInput.x * (speed / 2f) * 100f * Time.fixedDeltaTime;
                    if(controls.Player.Interact.ReadValue<float>() != 0f)
                        curPushing.position = Vector3.MoveTowards(curPushing.transform.position, transform.position + pushOffset, (speed / 2f) * 100f * Time.fixedDeltaTime);
                    break;

                case pMovementState.Falling:    //  give slight air control
                    if(Mathf.Abs(rb.linearVelocity.x) > 0f) {
                        var mod = savedInput.x * speed * 3f * Time.fixedDeltaTime;
                        if(Mathf.Abs(target.x + mod) < Mathf.Abs(target.x)) //  only can slow down jump
                            target.x += mod;
                    }
                    else target = Vector2.zero;
                    break;

                case pMovementState.RopeClimbing:
                    target = Vector3.zero;

                    //  shimmies
                    if(savedInput.y > 0f)
                        heldRope.moveUp(ropeClimbSpeed * Time.fixedDeltaTime);
                    else if(savedInput.y < 0f)
                        heldRope.moveDown(ropeClimbSpeed * Time.fixedDeltaTime);

                    //  swings
                    transform.position = heldRope.getGrabbedPos();
                    if(savedInput != Vector2.zero)
                        heldRope.addSwingForce(savedInput * speed * 100f * Time.fixedDeltaTime);
                    break;

                case pMovementState.LedgeClimbing:  //  hold while character does the climbing animation
                    target = Vector2.zero;
                    break;

                case pMovementState.LadderClimbing:
                    target = Vector2.right * savedInput.x * speed * 50f * Time.fixedDeltaTime;   //  rigid body x movement

                    //  linear y movement
                    var linearTarget = new Vector3(0f, savedInput.y * 100f);
                    linearTarget += transform.position;
                    transform.position = Vector3.MoveTowards(transform.position, linearTarget, speed * Time.fixedDeltaTime);
                    break;
            }
        }
        //  does the thing
        var temp = Vector2.MoveTowards(rb.linearVelocity, target, speedAccSpeed * 100f * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(Mathf.Clamp(temp.x, -maxVelocity, maxVelocity), Mathf.Clamp(temp.y, -maxVelocity, maxVelocity));
    }

    public void climbRope(RopeInstance r) {
        curState = pMovementState.RopeClimbing;
        rb.linearVelocity = Vector3.zero;
        heldRope = r;
    }
    public void climbLadder() {
        curState = pMovementState.LadderClimbing;
    }
    #endregion

    #region JUMP LOGIC
    void jump() {
        jumpHeld = true;
        if(!canMove)
            return;
        //  if grounded, jump immedietely 
        if(grounded)
            doJump();
        else if(curState == pMovementState.LadderClimbing || curState == pMovementState.RopeClimbing)
            doJump(3f);

        //  checks if coyote time applies
        else if(coyoteTime != null) {
            StopCoroutine(coyoteTime);
            coyoteTime = null;
            doJump();
        }

        //  queue up the jump
        else if(queuedJump == null) {
            queuedJump = StartCoroutine(jumpWaiter());
        }
    }
    void cancelJump() {
        jumpHeld = false;
        if(jumpCanceler == null && !grounded)
            jumpCanceler = StartCoroutine(jumpCancelWaiter());
    }
    void doJump(float xMod = 1f) {
        curState = pMovementState.Falling;
        jumpHeld = false;
        var target = new Vector3(savedInput.x * xMod, jumpHeight) * 100f * Time.fixedDeltaTime;
        target += rb.linearVelocity;
        rb.linearVelocity = target;
        StartCoroutine(jumpStateChecker());
        //  resets the jump canceler
        if(jumpCanceler != null)
            StopCoroutine(jumpCanceler);
        jumpCanceler = null;
    }

    //  coroutines
    IEnumerator jumpCancelWaiter() {
        float endY = (-jumpHeight) * 100f * Time.deltaTime;
        while(rb.linearVelocity.y > endY && !grounded) {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, new Vector2(rb.linearVelocity.x, endY), jumpAccSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        jumpCanceler = null;
    }   //  if the player cancels their jump, make them fall
    IEnumerator jumpWaiter() {
        float allowedTime = .5f; //  max time allowed for this loop to run before I give up on the player
        float s = Time.time, e = Time.time; //  start and end time, etc.
        while(!grounded && allowedTime > 0f) {
            e = Time.time;
            allowedTime -= e - s;
            s = Time.time;
            yield return new WaitForEndOfFrame();
        }

        //  does the jump if can do the jump
        if(allowedTime > 0f)
            doJump();
        queuedJump = null;
    }   //  for queuing up a jump before the player is back on the ground
    IEnumerator coyoteWaiter() {
        float allowedTime = .125f;   //  max time of ungrounded allowed
        yield return new WaitForSeconds(allowedTime);
        coyoteTime = null;
    }   //  Wille//  if the player hasn't cancelled the jump, but the velocity is 0 or negative, make them fall faster
    IEnumerator jumpStateChecker() {
        //  wait for jump to start
        while(rb.linearVelocity.y <= 0f || grounded)
            yield return new WaitForEndOfFrame();
        //  wait for state to change
        while(rb.linearVelocity.y > 0f && !grounded)
            yield return new WaitForEndOfFrame();
    }
    #endregion

    #region GROUND CHECKS
    public void touchedGround(Collider col) {
        if(col.gameObject.tag == "Ground" || col.gameObject.tag == "Box") {
            usedGround = col;
            grounded = true;
        }
    }
    public void leftGround(Collider col) {
        if((col.gameObject.tag == "Ground" || col.gameObject.tag == "Box") && gameObject.activeInHierarchy) {
            grounded = false;
            usedGround = null;

            //  start coyote time
            coyoteTime = StartCoroutine(coyoteWaiter());
        }
    }
    #endregion
}
