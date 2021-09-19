using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBuilder : MonoBehaviour
{
	private struct MazeCell
	{
		public bool hasBeenVisited;
		public bool[] walls;
	}

	private static readonly uint NUM_CELLS = 50;
	private static readonly float CORRIDOR_WIDTH = 2.0f;
	private static readonly float WALL_THICK = 0.5f;
	private static readonly float WALL_HEIGHT = 2.0f;

	private Vector2Int currentCell;
	private MazeCell[] cells = new MazeCell[NUM_CELLS * NUM_CELLS];
	private uint numVisitedCell = 0;
	private bool isMazeBuilt = false;

	// Start is called before the first frame update
	void Start()
	{
		for (uint row = 0; row < NUM_CELLS; row++)
		{
			for (uint column = 0; column < NUM_CELLS; column++)
			{
				cells[row * NUM_CELLS + column].hasBeenVisited = false;
				cells[row * NUM_CELLS + column].walls = new bool[4] { true, true, true, true };				
			}
		}

		currentCell = new Vector2Int((int)Random.Range(0, NUM_CELLS), (int)Random.Range(0, NUM_CELLS));
		var currenCellInfo = cells[currentCell.y * NUM_CELLS + currentCell.x];
		currenCellInfo.hasBeenVisited = true;
		numVisitedCell++;
	}

	// Update is called once per frame
	void Update()
	{
		if (numVisitedCell < cells.Length)
		{
			var currentTime = Time.realtimeSinceStartup;

			while (Time.realtimeSinceStartup - currentTime < 0.01)
			{
				var neighborIndex = Random.Range(0, 4);
				Vector2Int neighborCell = new Vector2Int(currentCell.x, currentCell.y);

				switch (neighborIndex)
				{
					case 0:
						neighborCell += new Vector2Int(0, -1);

						break;

					case 1:
						neighborCell += new Vector2Int(-1, 0);

						break;

					case 2:
						neighborCell += new Vector2Int(0, 1);

						break;

					case 3:
						neighborCell += new Vector2Int(1, 0);

						break;
				}

				if (neighborCell.x >= 0 && neighborCell.x < NUM_CELLS && neighborCell.y >= 0 && neighborCell.y < NUM_CELLS)
				{
					var neighborCellInfo = cells[neighborCell.y * NUM_CELLS + neighborCell.x];

					if (!neighborCellInfo.hasBeenVisited)
					{
						var currenCellInfo = cells[currentCell.y * NUM_CELLS + currentCell.x];
						currenCellInfo.walls[neighborIndex] = false;
						cells[neighborCell.y * NUM_CELLS + neighborCell.x].hasBeenVisited = true;
						numVisitedCell++;

						cells[neighborCell.y * NUM_CELLS + neighborCell.x].walls[(neighborIndex + 2) % 4] = false;
					}

					currentCell = neighborCell;
				}

				Debug.Log("CurrentCell: " + currentCell + " NeighborCell: " + neighborCell + " NeighborIndex: " + neighborIndex);
				Debug.Log("" + numVisitedCell / (float)cells.Length + "% COMPLETED");
			}
		}
		else if(!isMazeBuilt)
		{
			float mazeSize = NUM_CELLS * (WALL_THICK + CORRIDOR_WIDTH) + WALL_THICK;

			GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
			floor.transform.localScale = 0.1f * mazeSize * Vector3.one;

			for (uint row = 0; row < NUM_CELLS; row++)
			{
				for (uint column = 0; column < NUM_CELLS; column++)
				{
					var cornerPosition = new Vector3(column * (WALL_THICK + CORRIDOR_WIDTH) - mazeSize * 0.5f, 0, row * (WALL_THICK + CORRIDOR_WIDTH) - mazeSize * 0.5f);

					GameObject corner = GameObject.CreatePrimitive(PrimitiveType.Cube);
					corner.transform.localScale = new Vector3(WALL_THICK, WALL_HEIGHT, WALL_THICK);
					corner.transform.position = cornerPosition + new Vector3(0, WALL_HEIGHT * 0.5f, 0);
					corner.name = "Corner_" + column + "_" + row;
					corner.GetComponent<MeshRenderer>().material.color = Color.green;

					if (cells[row * NUM_CELLS + column].walls[0])
					{
						GameObject upperWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
						upperWall.transform.localScale = new Vector3(CORRIDOR_WIDTH, WALL_HEIGHT, WALL_THICK);
						upperWall.transform.position = cornerPosition + new Vector3((WALL_THICK + CORRIDOR_WIDTH) * 0.5f, WALL_HEIGHT * 0.5f, 0);
						upperWall.name = "HorizontalWall_" + column + "_" + row;
						upperWall.GetComponent<MeshRenderer>().material.color = Color.green;
					}

					if (cells[row * NUM_CELLS + column].walls[1])
					{
						GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
						leftWall.transform.localScale = new Vector3(WALL_THICK, WALL_HEIGHT, CORRIDOR_WIDTH);
						leftWall.transform.position = cornerPosition + new Vector3(0, WALL_HEIGHT * 0.5f, (WALL_THICK + CORRIDOR_WIDTH) * 0.5f);
						leftWall.name = "VerticalWall_" + column + "_" + row;
						leftWall.GetComponent<MeshRenderer>().material.color = Color.green;
					}					

					if (row == NUM_CELLS - 1)
					{
						GameObject endCorner = GameObject.CreatePrimitive(PrimitiveType.Cube);
						endCorner.transform.localScale = new Vector3(WALL_THICK, WALL_HEIGHT, WALL_THICK);
						endCorner.transform.position = cornerPosition + new Vector3(0, WALL_HEIGHT * 0.5f, WALL_HEIGHT + WALL_THICK);
						endCorner.GetComponent<MeshRenderer>().material.color = Color.green;

						if (cells[row * NUM_CELLS + column].walls[2])
						{
							GameObject bottomWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
							bottomWall.transform.localScale = new Vector3(CORRIDOR_WIDTH, WALL_HEIGHT, WALL_THICK);
							bottomWall.transform.position = cornerPosition + new Vector3((WALL_THICK + CORRIDOR_WIDTH) * 0.5f, WALL_HEIGHT * 0.5f, WALL_THICK + CORRIDOR_WIDTH);
							bottomWall.GetComponent<MeshRenderer>().material.color = Color.green;
						}
					}

					if (column == NUM_CELLS - 1)
					{
						GameObject endCorner = GameObject.CreatePrimitive(PrimitiveType.Cube);
						endCorner.transform.localScale = new Vector3(WALL_THICK, WALL_HEIGHT, WALL_THICK);
						endCorner.transform.position = cornerPosition + new Vector3(WALL_HEIGHT + WALL_THICK, WALL_HEIGHT * 0.5f, 0);
						endCorner.GetComponent<MeshRenderer>().material.color = Color.green;

						if (cells[row * NUM_CELLS + column].walls[3])
						{
							GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
							rightWall.transform.localScale = new Vector3(WALL_THICK, WALL_HEIGHT, CORRIDOR_WIDTH);
							rightWall.transform.position = cornerPosition + new Vector3(WALL_THICK + CORRIDOR_WIDTH, WALL_HEIGHT * 0.5f, (WALL_THICK + CORRIDOR_WIDTH) * 0.5f);
							rightWall.GetComponent<MeshRenderer>().material.color = Color.green;
						}
					}

					if (row == NUM_CELLS - 1 && column == NUM_CELLS - 1)
					{
						GameObject endCorner = GameObject.CreatePrimitive(PrimitiveType.Cube);
						endCorner.transform.localScale = new Vector3(WALL_THICK, WALL_HEIGHT, WALL_THICK);
						endCorner.transform.position = cornerPosition + new Vector3(WALL_HEIGHT + WALL_THICK, WALL_HEIGHT * 0.5f, WALL_HEIGHT + WALL_THICK);
						endCorner.GetComponent<MeshRenderer>().material.color = Color.green;
					}
				}
			}

			isMazeBuilt = true;
		}
	}
}
