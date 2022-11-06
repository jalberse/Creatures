using System.Collections.Generic;
using UnityEngine;

public class Branch
{
    public Vector3 rootPosition;
    public Vector3 position;

    public Branch(Vector3 rootPos, Vector3 pos)
    {
        rootPosition = rootPos;
        position = pos;
    }

    public float length()
    {
        return (position - rootPosition).magnitude;
    }

    public Vector3 direction()
    {
        return (position - rootPosition).normalized;
    }
}

public class TreeCreature : MonoBehaviour
{
    public GameObject boidCardPrefab;
    public Texture2D cardTexture;
    private List<GameObject> cards;
    private float growthTimeAccumulator;
    // Add a new branch each growthRate seconds.
    private float growthRate = 0.2f;
    private int treeDepth = 8;
    // Be careful increasing this; the program may hang due to large exponential growth!
    private int maxSplits = 5;
    // TODO we might want to randomize this some, or make the later distances shorter.
    private float branchDistanceBase = 2.5f;
    private float cardDistance = 0.2f;
    private List<Vector3> treePositions;

    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();
        growthTimeAccumulator = 0.0f;
        this.cards = new List<GameObject>();

        List<List<Branch>> branches = new List<List<Branch>>();
        // Precompute the positions of the tree. We just randomnly march upwards from each level of the tree.
        for (int level = 0; level < treeDepth; level++ )
        {
            List<Branch> thisLevelBranches = new List<Branch>();
            if (level == 0)
            {
                var branch = new Branch(this.transform.position, this.transform.position + Vector3.up * branchDistanceBase);
                thisLevelBranches.Add(branch);
            }
            else
            {
                foreach (Branch rootBranch in branches[level - 1])
                {
                    var numBranches = random.Next(maxSplits) + 1;
                    for (int j = 0; j < numBranches; j++)
                    {
                        var x = (float)random.NextDouble() * 2.0f - 1.0f;
                        var y = (float)random.NextDouble();
                        var z = (float)random.NextDouble() * 2.0f - 1.0f;
                        Vector3 diff = new Vector3(x, y, z).normalized * branchDistanceBase / (float)level;
                        thisLevelBranches.Add(new Branch(rootBranch.position, rootBranch.position + diff));
                    }
                }
            }
            branches.Add(thisLevelBranches);
        }

        treePositions = new List<Vector3>();
        foreach (List<Branch> level in branches)
        {
            foreach (Branch branch in level)
            {
                var numCardsForBranch = branch.length() / cardDistance;
                for ( int k = 0; k < numCardsForBranch; k++)
                {
                    treePositions.Add(branch.rootPosition + branch.direction() * cardDistance * k);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        growthTimeAccumulator += Time.deltaTime;
        if(treePositions.Count > 0 && growthTimeAccumulator > growthRate)
        {
            growthTimeAccumulator = 0.0f;
            // TODO rotate to lookat the previous position
            var card = Instantiate(boidCardPrefab, treePositions[0], Quaternion.identity);
            card.GetComponent<MeshRenderer>().material.mainTexture = cardTexture;
            cards.Add(card);
            treePositions.RemoveAt(0);
        }
    }
}
