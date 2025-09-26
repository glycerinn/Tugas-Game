using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateQueue : MonoBehaviour
{
    public GameObject[] fishPrefab;
    public GameObject[] trashPrefab;
    public Transform spawnPos;
    public int SortableCount = 10;
    public float spacing = 15f;
    private List<Sortable> Sortables_ = new List<Sortable>();
    GameObject obj;
    Sortable sortable;
    private bool Busy;

    void Start()
    {
        for (int i = 0; i < SortableCount; i++)
        {
            SpawnQueue(i);
        }
    }

    void Update()
    {
        if (Sortables_.Count == 0 || Busy) return;

        Sortable currentItem = Sortables_[0];

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentItem.GetSortableType() == Sortable.SortableType.Fish)
            {
                Vector3 targetPos = currentItem.transform.position + Vector3.up * 5;
                StartCoroutine(SortAndShift(currentItem, targetPos));
            }
            // else GameOver();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentItem.GetSortableType() == Sortable.SortableType.Trash)
            {
                Vector3 targetPos = currentItem.transform.position + Vector3.down * 5;
                StartCoroutine(SortAndShift(currentItem, targetPos));
            }
            // else GameOver();
        }
    }

    IEnumerator SortAndShift(Sortable sortable, Vector3 targetPos)
    {
        Busy = true;

        // Animate up/down move
        while (Vector3.Distance(sortable.transform.position, targetPos) > 0.01f)
        {
            sortable.transform.position = Vector3.MoveTowards(sortable.transform.position, targetPos, 10 * Time.deltaTime);
            yield return null;
        }

        Destroy(sortable.gameObject);
        Sortables_.RemoveAt(0);

        // Shift remaining items left smoothly
        List<Vector3> targetPositions = new List<Vector3>();
        for (int i = 0; i < Sortables_.Count; i++)
        {
            targetPositions.Add(spawnPos.position + Vector3.right * (i * spacing));
        }

        bool shifting = true;
        while (shifting)
        {
            shifting = false;
            for (int i = 0; i < Sortables_.Count; i++)
            {
                Sortables_[i].transform.position = Vector3.MoveTowards(Sortables_[i].transform.position, targetPositions[i], 5 * Time.deltaTime);
                if (Vector3.Distance(Sortables_[i].transform.position, targetPositions[i]) > 0.01f)
                    shifting = true;
            }
            yield return null;
        }

        SpawnQueue(Sortables_.Count);
        Busy = false;
    }

    public void SpawnQueue(int index)
    {
        GameObject prefab;
        if (Random.value > 0.5f)
        {
            prefab = fishPrefab[Random.Range(0, fishPrefab.Length)];
        }
        else
        {
            prefab = trashPrefab[Random.Range(0, trashPrefab.Length)];
        }
        obj = Instantiate(prefab, spawnPos.position + Vector3.right * (index * spacing), Quaternion.identity);
        sortable = obj.GetComponent<Sortable>();
        Sortables_.Add(sortable);
    }
}
