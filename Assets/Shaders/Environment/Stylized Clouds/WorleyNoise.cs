using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorleyNoise : MonoBehaviour {
    
    public void CreateWorleyNoiseBuffer(int numCellsPerAxis, string bufferName) {
        //Mapping out 3D noise grid in array
        Vector3[] pointGrid = new Vector3[numCellsPerAxis * numCellsPerAxis * numCellsPerAxis];
        float cellSize = 1f / numCellsPerAxis;

        for (int x = 0; x < numCellsPerAxis; x++) {
            for (int y = 0; y < numCellsPerAxis; y++) {
                for (int z = 0; z < numCellsPerAxis; z++) {
                    Vector3 randomOffset = new Vector3(Random.value, Random.value, Random.value);
                    Vector3 position = (new Vector3(x, y, z) + randomOffset) * cellSize;
                    int index = x + numCellsPerAxis + (y + z * numCellsPerAxis);
                    pointGrid[index] = position;
                }
            }
        }
    }

    private void CreateBuffer(System.Array data, int stride, string bufferName, int kernel = 0) {
        ComputeBuffer buffer = new ComputeBuffer(data.Length, stride, ComputeBufferType.Raw);
        
    }
}
