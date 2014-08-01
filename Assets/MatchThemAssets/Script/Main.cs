using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class Main : MonoBehaviour
{
    public GameObject indicator;
    public GameObject[,]  arrayOfShapes;
    public GameObject[] listOfGems;
    public GameObject emptyGameobject;
    public GameObject particleEffect;
    public GameObject particleEffectWhenMatch;
    public bool canTransitDiagonally = false;
    public int scoreIncrement;
    public AudioClip MatchSound;
    public int gridWidth;
    public int gridHeight;
    
    private int scoreTotal = 0;
    private ArrayList currentParticleEffets = new ArrayList ();
    private GameObject currentIndicator;
    private GameObject firstObject;
    private GameObject secondObject;

    void Start ()
    {
        arrayOfShapes = new GameObject[gridWidth, gridHeight];
        
        for (int i = 0; i <= gridWidth-1; i++) {
            for (int j = 0; j <= gridHeight-1; j++) {
                var gameObject = GameObject.Instantiate(
                    listOfGems [Random.Range (0, listOfGems.Length)] as GameObject, 
                    new Vector3 (i, j, 0), 
                    transform.rotation
                ) as GameObject;
                
                arrayOfShapes [i, j] = gameObject;
            }
        }
        InvokeRepeating ("DoShapeEffect", 1f, 0.21F);
    }
    
    void DoShapeEffect ()
    {
        foreach (GameObject row in currentParticleEffets) {
           Destroy (row);
        }
        for (int i = 0; i <= 2; i++){
            currentParticleEffets.Add(GameObject.Instantiate(
                particleEffect, 
                new Vector3 (
                    Random.Range (0, arrayOfShapes.GetUpperBound (0) + 1), 
                    Random.Range (0, arrayOfShapes.GetUpperBound (1) + 1), 
                    -1
                ), new Quaternion (0, 0, Random.Range (0, 1000f), 100)
            ) as GameObject);
        }
    }
    
    void Update ()
    {
        bool shouldTransit = false;
        if (Input.GetButtonDown ("Fire1") && HOTween.GetTweenInfos () == null) {
            Destroy (currentIndicator);
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
            if (hit.transform != null) {  
                if (firstObject == null)
                    firstObject = hit.transform.gameObject;
                else {
                    secondObject = hit.transform.gameObject;
                    shouldTransit = true;
                }

                currentIndicator = GameObject.Instantiate(
                    indicator, new Vector3 (
                        hit.transform.gameObject.transform.position.x, 
                        hit.transform.gameObject.transform.position.y, 
                        -1
                    ), transform.rotation
                ) as GameObject;
                
                if (shouldTransit) {
                    var distance = firstObject.transform.position - secondObject.transform.position;
                    
                    if (Mathf.Abs (distance.x) <= 1 && Mathf.Abs (distance.y) <= 1) {   
                        if (!canTransitDiagonally) {
                            if (distance.x != 0 && distance.y != 0) {
                                Destroy (currentIndicator); 
                                firstObject = null;
                                secondObject = null; 
                                return;
                            }
                        }
                        
                        DoSwapMotion (firstObject.transform, secondObject.transform);
                        DoSwapTile (firstObject, secondObject, ref arrayOfShapes);
                    } else {
                        firstObject = null;
                        secondObject = null;
                    }
                    Destroy (currentIndicator);
                       
                }
            }
        }
        
        if (HOTween.GetTweenInfos () == null) {
            var Matches = FindMatch (arrayOfShapes);
            
            if (Matches.Count > 0) {
                scoreTotal += Matches.Count * scoreIncrement;
           
                foreach (GameObject go in Matches) {
                    audio.PlayOneShot (MatchSound);
                    
                    var destroyingParticle = GameObject.Instantiate(
                        particleEffectWhenMatch as GameObject, 
                        new Vector3 (go.transform.position.x, go.transform.position.y, -2), 
                        transform.rotation
                    ) as GameObject;
                    
                    Destroy (destroyingParticle, 1f);
                    
                    arrayOfShapes [(int)go.transform.position.x, (int)go.transform.position.y] = GameObject.Instantiate(
                        emptyGameobject, 
                        new Vector3 ((int)go.transform.position.x, (int)go.transform.position.y, -1), 
                        transform.rotation
                    ) as GameObject;
                    
                    Destroy (go, 0.1f);
                }
                firstObject = null;
                secondObject = null;
                
                DoEmptyDown (ref arrayOfShapes);
            }  
       
        else if (firstObject != null && secondObject != null
             ) {
                
                DoSwapMotion (firstObject.transform, secondObject.transform);
                
                DoSwapTile (firstObject, secondObject, ref arrayOfShapes);
                firstObject = null;
                secondObject = null;
            } 
        }
        (GetComponent (typeof(TextMesh))as TextMesh).text = scoreTotal.ToString ();
    }
    
    private ArrayList FindMatch (GameObject[,] cells)
    {
        ArrayList stack = new ArrayList ();
        
        for (var x = 0; x <= cells.GetUpperBound(0); x++) {
            for (var y = 0; y <= cells.GetUpperBound(1); y++) {
                var thiscell = cells [x, y];
                
                if (thiscell.name == "Empty(Clone)")
                    continue;
                int matchCount = 0;
                int y2 = cells.GetUpperBound (1);
                int y1;
                
                for (y1 = y + 1; y1 <= y2; y1++) {
                    if (cells [x, y1].name == "Empty(Clone)" || thiscell.name != cells [x, y1].name)
                        break;
                    matchCount++;
                }
                
                if (matchCount >= 2) {
                    y1 = Mathf.Min (cells.GetUpperBound (1), y1 - 1);
                    for (var y3 = y; y3 <= y1; y3++) {
                        if (!stack.Contains (cells [x, y3])) {
                            stack.Add (cells [x, y3]);
                        }
                    }
                }
            }
        }
        
        for (var y = 0; y < cells.GetUpperBound(1) + 1; y++) {
            for (var x = 0; x < cells.GetUpperBound(0) + 1; x++) {
                var thiscell = cells [x, y];
                if (thiscell.name == "Empty(Clone)")
                    continue;
                int matchCount = 0;
                int x2 = cells.GetUpperBound (0);
                int x1;
                for (x1 = x + 1; x1 <= x2; x1++) {
                    if (cells [x1, y].name == "Empty(Clone)" || thiscell.name != cells [x1, y].name)
                        break;
                    matchCount++;
                }
                if (matchCount >= 2) {
                    x1 = Mathf.Min (cells.GetUpperBound (0), x1 - 1);
                    for (var x3 = x; x3 <= x1; x3++) {
                        if (!stack.Contains (cells [x3, y])) {
                            stack.Add (cells [x3, y]);
                        }
                    }
                }
            }
        }
        return stack;
    }
    
    void DoSwapMotion (Transform a, Transform b)
    {
        Vector3 posA = a.localPosition;
        Vector3 posB = b.localPosition;
        TweenParms parms = new TweenParms ().Prop ("localPosition", posB).Ease (EaseType.EaseOutQuart);
        HOTween.To (a, 0.25f, parms).WaitForCompletion ();
        parms = new TweenParms ().Prop ("localPosition", posA).Ease (EaseType.EaseOutQuart);
        HOTween.To (b, 0.25f, parms).WaitForCompletion ();
    }
    
    void DoSwapTile (GameObject a, GameObject b, ref GameObject[,] cells)
    {
        GameObject cell = cells [(int)a.transform.position.x, (int)a.transform.position.y];
        cells [(int)a.transform.position.x, (int)a.transform.position.y] = cells [(int)b.transform.position.x, (int)b.transform.position.y];
        cells [(int)b.transform.position.x, (int)b.transform.position.y] = cell;
    }

    
    private void DoEmptyDown (ref GameObject[,] cells)
    {   
        for (int x= 0; x <= cells.GetUpperBound(0); x++) {
            for (int y = 0; y <= cells.GetUpperBound(1); y++) {
                var thisCell = cells [x, y];
                if (thisCell.name == "Empty(Clone)") {
                    for (int y2 = y; y2 <= cells.GetUpperBound(1); y2++) {
                        if (cells [x, y2].name != "Empty(Clone)") {
                            cells [x, y] = cells [x, y2];
                            cells [x, y2] = thisCell;
                            break;
                        }
                    }
                }
            }
        }
        
        for (int x = 0; x <= cells.GetUpperBound(0); x++) {
            for (int y = 0; y <= cells.GetUpperBound(1); y++) {
                if (cells [x, y].name == "Empty(Clone)") {
                    Destroy (cells [x, y]);
                    cells [x, y] = GameObject.Instantiate(
                        listOfGems[Random.Range (0, listOfGems.Length)] as GameObject, 
                        new Vector3 (x, cells.GetUpperBound (1) + 2, 0), 
                        transform.rotation
                    ) as GameObject;
                }
            }
        }

        for (int x = 0; x <= cells.GetUpperBound(0); x++) {
            for (int y = 0; y <= cells.GetUpperBound(1); y++) {
                TweenParms parms = new TweenParms ().Prop ("position", new Vector3 (x, y, -1)).Ease (EaseType.EaseOutQuart);
                HOTween.To (cells [x, y].transform, .4f, parms);
            }
        }
    }
}
