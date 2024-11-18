using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : Singleton<PlayerMovement> {
    #region GLOBALS
    [SerializeField] float speed, speedAccSpeed;
    [SerializeField] float jumpHeight, jumpAccSpeed;
    [SerializeField] float jumpDeathDist;
    [SerializeField] float ropeClimbSpeed, ladderClimbSpeed;
    [HideInInspector] public float speedMod = 1f;

    [SerializeField] Collider mainCol, groundCol;
    public Rigidbody rb;
    [SerializeField] Rigidbody pushingRb;
    [SerializeField] Transform spriteTrans;

    Coroutine queuedJump = null;
    Coroutine coyoteTime = null;
    Coroutine jumpCanceler = null;

    //  ground things
    Collider usedGround;
    float lastGroundedY;
    [HideInInspector] public bool canTakeFallDamage = true;

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
    Vector2 pushOffset;
    Rigidbody cp = null;
    Rigidbody curPushing {
        get { return cp; }
        set {
            if(cp == value) return;
            if(cp != null) {
                Physics.IgnoreCollision(cp.GetComponent<Collider>(), mainCol, false);
                Physics.IgnoreCollision(cp.GetComponent<Collider>(), groundCol, false);
            }
            cp = value;
            if(cp != null) {
                Physics.IgnoreCollision(cp.GetComponent<Collider>(), mainCol, true);
                Physics.IgnoreCollision(cp.GetComponent<Collider>(), groundCol, true);
            }
            if(cp == null && curState == pMovementState.Pushing) curState = grounded ? pMovementState.Walking : pMovementState.Falling;
            else if(cp != null && curState != pMovementState.Pushing) curState = pMovementState.Pushing;
        }
    }

    //  driving things
    VehicleInstance cd = null;
    VehicleInstance curDriving {
        get { return cd; }
        set {
            if(cd == value) return;
            if(cd != null) {
                cd.playerExit();
                transform.parent = null;
                curState = grounded ? pMovementState.Walking : pMovementState.Falling;
                rb.isKinematic = false;
            }
            cd = value;
            if(cd != null) {
                cd.playerEnter();
                transform.position = cd.seatPos.position;
                transform.parent = cd.transform;
                curState = pMovementState.Driving;
                rb.isKinematic = true;
            }
        }
    }

    //  input things
    InputMaster controls;
    Vector2 savedInput;
    public bool facingRight { get; private set; } = true;
    float maxVelocity = 50f;
    Rigidbody inheritRb = null;

    //  jump things
    bool jumpHeld = false;

    //  specials
    pMovementState cs = pMovementState.None;
    public pMovementState curState {
        get { return cs; }
        private set {
            //  before changing
            if(cs == value) return;
            Vector3 carryover = Vector2.zero;
            if(cs == pMovementState.RopeClimbing) {
                carryover = heldRope.getExitVel();
                lastGroundedY = transform.position.y;
                heldRope = null;
            }
            else if(cs == pMovementState.LadderClimbing) {
                lastGroundedY = transform.position.y;
            }

            cs = value;

            //  after changing
            rb.useGravity = cs != pMovementState.LadderClimbing && cs != pMovementState.RopeClimbing && cs != pMovementState.LedgeClimbing;
            if(cs == pMovementState.LadderClimbing || cs == pMovementState.RopeClimbing)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f);
            else if(cs == pMovementState.LedgeClimbing)
                rb.linearVelocity = Vector3.zero;
            if(cs != pMovementState.Pushing && curPushing != null) {
                curPushing = null;
            }

            if(cs != pMovementState.Walking) PlayerAudioManager.I.stopWalking();

            updateInput(controls.Player.Move.ReadValue<Vector2>());

            if(!rb.isKinematic)
                rb.linearVelocity += carryover;
        }
    }
    bool cm = false;
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
                if(canTakeFallDamage && lastGroundedY - transform.position.y >= jumpDeathDist) {
                    Debug.Log("fall damage killed you");
                    canMove = false;
                    TransitionCanvas.I.loadGameAfterDeath(2f);
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
                    doJump(true);
            }
            else
                lastGroundedY = transform.position.y;

            //  sets cur state
            if(curState != pMovementState.LadderClimbing && curState != pMovementState.RopeClimbing)
                curState = grounded ? pMovementState.Walking : pMovementState.Falling;
            //  checks for standing on pushing
            if(usedGround != null && usedGround.gameObject.tag == "Box" && usedGround.TryGetComponent<Rigidbody>(out var asdf) && asdf == curPushing) {
                curPushing = null;
                curState = pMovementState.Walking;
            }
        }
    }

    [System.Serializable]
    public enum pMovementState {
        None, Walking, Falling, Pushing, LedgeClimbing, RopeClimbing, LadderClimbing, Driving
    }
    #endregion

    #region COLLISIONS
    private void OnCollisionEnter(Collision col) {
        //  pushables / boxes
        if(col.gameObject.tag == "Box") {
            if(grounded && usedGround != col.collider && col.gameObject.TryGetComponent<Rigidbody>(out var colRb) && colRb != curPushing) {
                curPushing = colRb;
                pushOffset = curPushing.transform.position - transform.position;
                facePos(col.gameObject.transform.position.x);
            }
        }

        //  ropes
        else if(curState != pMovementState.RopeClimbing && col.gameObject.tag == "Rope" && col.transform.parent.TryGetComponent<RopeInstance>(out var ri) && ri.canHold()) {
            if(!grounded) {
                rb.linearVelocity = Vector3.zero;
                curState = pMovementState.RopeClimbing;
                heldRope = ri;
            }
        }
    }
    private void OnTriggerEnter(Collider col) {
        //  ledge climbing
        if(grounded && curState == pMovementState.Falling && col.gameObject.tag == "Ledge") {
            climbLedge(col);
        }

        //  ladders
        else if(col.gameObject.tag == "Ladder") {
            if(savedInput.y > 0f)
                curState = pMovementState.LadderClimbing;
        }
    }
    private void OnTriggerStay(Collider col) {
        //  ledge climbing
        if(!grounded && curState == pMovementState.Falling && col.gameObject.tag == "Ledge") {
            climbLedge(col);
        }

        //  ladders
        else if(col.gameObject.tag == "Ladder") {
            if(savedInput.y > 0f)
                curState = pMovementState.LadderClimbing;
        }
    }
    private void OnTriggerExit(Collider col) {
        //  ladders
        if(col.gameObject.tag == "Ladder") {
            curState = grounded ? pMovementState.Walking : pMovementState.Falling;
        }

        else if(col.gameObject.tag == "BoxExitCol" && curPushing != null && curPushing == col.transform.parent.GetComponent<Rigidbody>())
            curPushing = null;
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
        canMove = true;
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
        if(Mathf.Abs(dir.y) < .15f) dir.y = 0f;
        if(Mathf.Abs(dir.x) < .15f) dir.x = 0f;
        dir.Normalize();
        savedInput = dir;
        if(Mathf.Abs(savedInput.x) > 0f)
            PlayerAudioManager.I.startWalking(.5f);
        else PlayerAudioManager.I.stopWalking();
        updateFaceDir();
    }
    void updateFaceDir() {
        facePos(savedInput.x);
    }
    void facePos(float x) {
        if(PauseCanvas.I.paused) return;
        if(curState == pMovementState.Falling) return;
        if(curState == pMovementState.Pushing) {
            facingRight = curPushing.position.x >= transform.position.x;
            spriteTrans.transform.rotation = Quaternion.Euler(0f, facingRight ? 0f : 180f, 0f);
        }
        else if(Mathf.Abs(savedInput.x) != 0f) {
            facingRight = x >= 0;
            //  flips character
            spriteTrans.transform.rotation = Quaternion.Euler(0f, facingRight ? 0f : 180f, 0f);
        }
    }
    void move() {
        if(rb.isKinematic && curState != pMovementState.Driving) return;
        Vector2 target = (canMove || !grounded) ? rb.linearVelocity : Vector3.zero;
        float accTarget = speedAccSpeed;

        if(canMove) {
            switch(curState) {
                case pMovementState.None:   //  return to stand-still
                    break;

                case pMovementState.Walking:    //  update x axis velocity
                    target.x = savedInput.x * speed * speedMod * 100f * Time.fixedDeltaTime;
                    break;

                case pMovementState.Pushing:
                    if(curPushing == null) {
                        curState = grounded ? pMovementState.Walking : pMovementState.Falling;
                        return;
                    }
                    //  checks if moving away while not holding push button
                    if(controls.Player.Interact.ReadValue<float>() == 0f && ((savedInput.x > 0f) != (pushOffset.x > 0f))) {
                        curState = grounded ? pMovementState.Walking : pMovementState.Falling;
                        return;
                    }
                    //  checks if pushing obj too far away
                    Vector2 pOffset = curPushing.transform.position - transform.position;
                    if(controls.Player.Interact.ReadValue<float>() != 0f && pOffset.magnitude > pushOffset.magnitude + 1f) {
                        curState = grounded ? pMovementState.Walking : pMovementState.Falling;
                        return;
                    }
                    target.x = savedInput.x * (speed * .6f) * speedMod * 100f * Time.fixedDeltaTime;
                    //  checks facing dir
                    if(curPushing.position.x < transform.position.x && !facingRight)
                        facePos(curPushing.position.x);
                    else if(curPushing.position.x > transform.position.x && facingRight)
                        facePos(curPushing.position.x);
                    var pTarget = transform.position;
                    transform.position = new Vector3(curPushing.transform.position.x - pushOffset.x, pTarget.y, 0f);
                    /*  NEW OLD SHIT
                    /*  OLD SHIT
                    pushing = true;
                    //  inputted pushing / pulling
                    if(controls.Player.Interact.ReadValue<float>() == 0f) {
                        //  checks for physics based pushing / pulling
                        var xOffset = curPushing.transform.position.x - transform.position.x;
                        if((savedInput.x > 0f) != (xOffset > 0f)) {
                            pushing = false;
                        }
                    }
                    /* old shit
                    if(controls.Player.Interact.ReadValue<float>() != 0f || (savedInput.x > 0f == xOffset > 0f)) {
                        if(touchingCurPushing) {
                            var sMod = savedInput.x > 0f == xOffset > 0f ? .9f : 1.15f;
                            curPushing.linearVelocity = new Vector3(target.x * sMod, curPushing.linearVelocity.y);
                        }
                        else
                            curPushing.linearVelocity = new Vector3(rb.linearVelocity.x, curPushing.linearVelocity.y);
                    }*/
                    break;

                case pMovementState.Falling:
                    target = rb.linearVelocity;
                    //  slight air control
                    var max = speed * speedMod * 100f * Time.fixedDeltaTime;
                    var mod = savedInput.x * speed * speedMod * 3f * Time.fixedDeltaTime;
                    var modPerc = Mathf.Clamp01(Mathf.Abs(rb.linearVelocity.magnitude) / max);
                    target.x = Mathf.Clamp(target.x + mod * modPerc, -max, max);
                    break;

                case pMovementState.RopeClimbing:
                    rb.linearVelocity = (heldRope.getGrabbedPos() - transform.position) * 25f;
                    target = rb.linearVelocity;
                    accTarget = 1f;
                    rb.mass = 0f;

                    //  shimmies
                    if(savedInput.y > 0f)
                        heldRope.moveUp(ropeClimbSpeed * Time.fixedDeltaTime);
                    else if(savedInput.y < 0f) {
                        if(heldRope.moveDown(ropeClimbSpeed * Time.fixedDeltaTime)) {
                            curState = pMovementState.Falling;
                            return;
                        }
                    }

                    //  swings
                    if(savedInput != Vector2.zero)
                        heldRope.addSwingForce(savedInput * speed * speedMod * 100f * Time.fixedDeltaTime);
                    break;

                case pMovementState.LedgeClimbing:  //  hold while character does the climbing animation
                    target = rb.linearVelocity;
                    break;

                case pMovementState.LadderClimbing:
                    target.x = savedInput.x * speed * speedMod * 50f * Time.fixedDeltaTime;   //  rigid body x movement
                    target.y = savedInput.y * ladderClimbSpeed * speedMod * 100f * Time.fixedDeltaTime;   //  rigid body y movement
                    accTarget = 1f;
                    break;
                case pMovementState.Driving:
                    if(curDriving == null) {
                        curState = grounded ? pMovementState.Walking : pMovementState.Falling;
                        return;
                    }
                    transform.position = curDriving.seatPos.position;
                    target.x = savedInput.x * curDriving.speed * 100f * Time.fixedDeltaTime;
                    break;
            }
        }

        var movingRb = curState == pMovementState.Pushing ? curPushing : curState == pMovementState.Driving ? curDriving.rb : rb;
        //  does the thing
        var temp = Vector2.MoveTowards(movingRb.linearVelocity, target, accTarget * 100f * Time.fixedDeltaTime);
        if(grounded && savedInput.x != 0f)
            temp.y = movingRb.linearVelocity.y;

        if(savedInput.magnitude == 0f && inheritRb != null)
            movingRb.linearVelocity = inheritRb.linearVelocity;
        else {
            var iv = inheritRb == null ? Vector2.zero : (Vector2)inheritRb.linearVelocity;
            movingRb.linearVelocity = new Vector2(Mathf.Clamp(temp.x, iv.x - maxVelocity, iv.x + maxVelocity), Mathf.Clamp(temp.y, iv.y - maxVelocity, iv.y + maxVelocity));
        }
        AnimationManager.I.CheckAnimation(savedInput.x, !grounded, curState);
    }
    public void setNewState(pMovementState newState) {
        curState = newState;
    }

    public void setInheritRb(Rigidbody r) {
        inheritRb = r;
    }
    public Rigidbody getInheritRb() {
        return inheritRb;
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
        if(!canMove || curState == pMovementState.Driving)
            return;
        //  if grounded, jump immedietely 
        if(grounded)
            doJump(true);
        else if(curState == pMovementState.LadderClimbing)
            doJump(false, 2f);
        else if(curState == pMovementState.RopeClimbing)
            doJump(true, 1f, 5f);

        //  checks if coyote time applies
        else if(coyoteTime != null) {
            StopCoroutine(coyoteTime);
            coyoteTime = null;
            doJump(true);
        }

        //  queue up the jump
        else if(queuedJump == null) {
            queuedJump = StartCoroutine(jumpWaiter());
        }
    }
    void cancelJump() {
        jumpHeld = false;
        if(queuedJump != null) {
            StopCoroutine(queuedJump);
            queuedJump = null;
        }
        if(jumpCanceler == null && !grounded)
            jumpCanceler = StartCoroutine(jumpCancelWaiter());
    }
    void doJump(bool keepXVel, float xMod = 1f, float minX = 0f) {
        curState = pMovementState.Falling;
        jumpHeld = false;
        Vector2 target;
        //target.x = (keepXVel || true ? rb.linearVelocity.x : savedInput.x > 0f ? 1f : -1f) * xMod;
        target.x = (savedInput.x < 0f ? Mathf.Min(rb.linearVelocity.x, savedInput.x) : Mathf.Max(rb.linearVelocity.x, savedInput.x)) * xMod;
        target.x = target.x < 0f ? target.x = Mathf.Min(target.x, -minX) : Mathf.Max(target.x, minX);
        target.y = jumpHeight * 100f * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(Mathf.Clamp(target.x, -maxVelocity, maxVelocity), target.y);
        StartCoroutine(jumpStateChecker());
        //  resets the jump canceler
        if(jumpCanceler != null)
            StopCoroutine(jumpCanceler);
        jumpCanceler = null;
    }

    //  coroutines
    IEnumerator jumpCancelWaiter() {
        float endY = (-jumpHeight);
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
        if(canMove && allowedTime > 0f)
            doJump(true);
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

    #region DRIVING LOGIC
    public void setCurDriving(VehicleInstance vi) {
        curDriving = vi;
    }
    #endregion


    #region LEDGE CLIMBING
    void climbLedge(Collider col) {
        if(curState == pMovementState.LedgeClimbing) return;    //  already climbing
        if(Mathf.Abs(transform.position.y - lastGroundedY) < 1f) return;    //  barely off the ground
        var offset = transform.position - col.gameObject.transform.position;
        if(curState == pMovementState.Falling && offset.y < 1.5f && offset.x < 0f == facingRight && savedInput.x != 0f) {
            var tallestChild = col.gameObject.transform.GetChild(0);
            foreach(var i in col.gameObject.transform.GetComponentsInChildren<Transform>()) {
                if(i.position.y > tallestChild.position.y)
                    tallestChild = i;
            }

            //  checks if already on tallest child
            if(tallestChild.position.y - transform.position.y < 1f) return;

            //  checks if facing end pos
            var xOffset = tallestChild.position.x - transform.position.x;
            if((xOffset < 0f && savedInput.x < 0f) || (xOffset > 0f && savedInput.x > 0f)) {
                curState = pMovementState.LedgeClimbing;
                rb.isKinematic = true;
                mainCol.isTrigger = true;
                canMove = false;
                rb.linearVelocity = Vector3.zero;
                transform.DOKill();
                transform.DOMove(tallestChild.position, .5f).OnComplete(() => {
                    mainCol.isTrigger = false;
                    canMove = true;
                    rb.linearVelocity = Vector3.zero;
                    curState = grounded ? pMovementState.Walking : pMovementState.Falling;
                    rb.isKinematic = false;
                });
            }
        }
    }
    #endregion


    #region GROUND CHECKS
    public void touchedGround(Collider col) {
        if(curState != pMovementState.Driving && (col.gameObject.tag == "Ground" || col.gameObject.tag == "Box")) {
            usedGround = col;
            grounded = true;
        }
    }
    public void leftGround(Collider col) {
        if(((curState != pMovementState.Pushing && curState != pMovementState.Driving) || controls.Player.Interact.ReadValue<float>() == 0f) && (col.gameObject.tag == "Ground" || col.gameObject.tag == "Box") && gameObject.activeInHierarchy) {
            grounded = false;
            usedGround = null;

            //  start coyote time
            coyoteTime = StartCoroutine(coyoteWaiter());
        }
    }
    public void setFalling() {
        grounded = false;
        usedGround = null;
        curState = pMovementState.Falling;
    }
    public Collider getUsedGround() {
        return usedGround;
    }
    #endregion

    public Rigidbody getCurPushing() {
        return curPushing;
    }
}
