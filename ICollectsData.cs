using CustomDataTypes;

public interface ICollectsData<T>
{
    void CollectData(ResponseData<T> responseData);
}