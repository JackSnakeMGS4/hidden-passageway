﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigSitePuzzle : MonoBehaviour {

    public GameObject puzzleCubePrefab;
    public List<GameObject> breakableCubes;
    public bool[] cubeState;
    private int gridIndex;
    private int gridCols = 7;
    private int gridRows = 7;
    private GameObject parent;
    private DigSitePuzzleTracker puzzleTracker;
    public bool solutionCheckNeeded = false;

    void Start () {

        parent = this.transform.parent.gameObject;
        puzzleTracker = parent.GetComponent<DigSitePuzzleTracker>();

        cubeState = new bool[gridCols * gridRows];
        CreateCubeGrid(7,7);
    }
	
	void Update () {
        if (solutionCheckNeeded) {
            CheckForSolution();
            Debug.Log("Calling CheckForSolution()");
            solutionCheckNeeded = false;
        }
    }
    
    public void CreateCubeGrid(int cols, int rows) {

        //Temporarily resets puzzle gameObject rotation to (0,0,0), 
        //so that the cubes are created in the correct position, regardless 
        //of the placement orientation of the puzzle prefab in the world.
        Vector3 gridFrameRotation = transform.rotation.eulerAngles;
        Vector3 tempGridFrameRotation = gridFrameRotation;
        tempGridFrameRotation = new Vector3(0,0,0);
        transform.rotation = Quaternion.Euler(tempGridFrameRotation);

        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < cols; x++) {

                gridIndex = ColRowToGridIndex(x, y);
            
                //Create new cube at next position
                GameObject tempGO = GameObject.Instantiate(puzzleCubePrefab);
                tempGO.transform.position = gameObject.transform.position + new Vector3(0.4f * x, 0.4f * y, 0f);

                //Child new cube to this gameObject and set local scale to (1, 1, 1)
                tempGO.transform.SetParent(gameObject.transform);
                Vector3 scale = tempGO.transform.localScale;
                scale.Set(1, 1, 1);
                tempGO.transform.localScale = scale;
                
                //Rename new cube
                tempGO.name = "Cube " + gridIndex;

                //Set grid index on new cube
                tempGO.GetComponent<DigSitePuzzleCube>().SetIndex(gridIndex);

                //Randomize cube state
                if (Random.Range(0f,1f) > 0.6) {
                    cubeState[gridIndex] = true;
                } else { //end if
                    cubeState[gridIndex] = false;
                    breakableCubes.Add(tempGO);
                } // end else

            } // end of for x loop
        } // end of for y loop

        //Sets the puzzle gameObject rotation (along with all the new cubes) back to the intended rotation.
        transform.rotation = Quaternion.Euler(gridFrameRotation);

    } // end of CreateCubeGrid()

    public bool IsCubeUnbreakable(int index) {

        if (cubeState[index]) {
            return true;
        } else { // end if cube state is true (unbreakable)
            return false;
        } // end else cube state is false (breakable)

    } // end isCubeUnbreakable

    public int ColRowToGridIndex(int col, int row) {
        return col + (gridCols * row);
    }  // end of ColRowToGridIndex()

    public void CheckForSolution() {

        //Remove destroyed cubes from list
        breakableCubes.RemoveAll(GameObject => GameObject == null);

        Debug.Log(breakableCubes);
        if (breakableCubes.Count == 0) { //solves if 1 left, because last cube is not detroyed yet, when check is made.
            puzzleTracker.MarkSolved(gameObject.name);
            Debug.Log("Solution Found. Calling MarkSolved() for object " + gameObject.name);
        } // end of if

    } // end of CheckForSolution()

    public void SetSolutionCheckNeeded() {
        solutionCheckNeeded = true;
    }

} // end of class