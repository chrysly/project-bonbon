using System;
using System.Collections.Generic;

public class MinHeap<T> where T : IComparable<T> {
    private const int InitialCapacity = 5;
    protected int _size;
    protected T[] _backingArray;

    public MinHeap() {
        _backingArray = new T[InitialCapacity];
        _size = 0;
    }
    public MinHeap(List<T> list) : this() {
        if (list == null)
        {
            throw new ArgumentNullException("Provided list for heap creation is null!");
        }
        
        CopyList(list);
        BuildHeap();
    }
    
    private void CopyList(List<T> list) {
        foreach(T item in list) {
            if (_size + 1 == _backingArray.Length) {
                T[] resizedArray = new T[_backingArray.Length * 2];
                for(int i = 1; i <= _size; i++) {
                    resizedArray[i] = _backingArray[i];
                }
                _backingArray = resizedArray;
            }
            _backingArray[_size + 1] = item;
            _size++;
        }
    }
    protected void BuildHeap() {
        for (int i = _size / 2; i > 0; i--)
        {
            DownHeap(i);
        }
    }
    public void Add(T data) {
        if (_size + 1 == _backingArray.Length) {
            T[] resizedArray = new T[_backingArray.Length * 2];
            for(int i = 1; i <= _size; i++) {
                resizedArray[i] = _backingArray[i];
            }
            _backingArray = resizedArray;
        }
        _backingArray[_size + 1] = data;
        _size++;
        UpHeap(_size);
    }
    protected void UpHeap(int currentIdx) {
        if (currentIdx > _size || currentIdx < 1) {
            throw new IndexOutOfRangeException("Attempted to access outside of heap, this is a problem with internal implementation!");
        } else if (currentIdx > 1) {
            while (_backingArray[currentIdx].CompareTo(_backingArray[currentIdx / 2]) < 0) {
                T temp = _backingArray[currentIdx];
                _backingArray[currentIdx] = _backingArray[currentIdx / 2];
                _backingArray[currentIdx / 2] = temp;

                int parentIdx = currentIdx / 2;
                if (parentIdx < 1) {
                    break;
                }
                currentIdx = parentIdx;
            }
        }
    }
    public T Remove() {
        T removedData = _backingArray[1];
        _backingArray[1] = _backingArray[_size];
        _backingArray[_size] = default;

        _size--;
        if (_size != 0)
        {
            DownHeap(1);
        }
        return removedData;
    }
    protected void DownHeap(int currentIdx) {
        if (currentIdx > _size || currentIdx < 1) {
            throw new IndexOutOfRangeException("Attempted to access outside of heap, this is a problem with internal implementation!");
        }
        
        T current = _backingArray[currentIdx];
        int leftIdx = currentIdx * 2;
        int rightIdx = leftIdx + 1;

        int compareLeft = (leftIdx <= _size) ?  current.CompareTo(_backingArray[leftIdx]) : 0;
        int compareRight = (rightIdx <= _size) ? current.CompareTo(_backingArray[rightIdx]) : 0;

        if (compareLeft > 0 && compareRight > 0) {
            if (_backingArray[leftIdx].CompareTo(_backingArray[rightIdx]) < 0) {
                _backingArray[currentIdx] = _backingArray[leftIdx];
                _backingArray[leftIdx] = current;
                DownHeap(leftIdx);
            } else {
                _backingArray[currentIdx] = _backingArray[rightIdx];
                _backingArray[rightIdx] = current;
                DownHeap(rightIdx);
            }
        } else if (compareLeft > 0) {
            _backingArray[currentIdx] = _backingArray[leftIdx];
            _backingArray[leftIdx] = current;
            DownHeap(leftIdx);
        } else if (compareRight > 0) {
            _backingArray[currentIdx] = _backingArray[rightIdx];
            _backingArray[rightIdx] = current;
            DownHeap(rightIdx);
        }
    }
    public T Peek() {
        return _backingArray[1];
    }
}