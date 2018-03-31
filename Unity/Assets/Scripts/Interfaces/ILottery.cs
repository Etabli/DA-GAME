using System.Collections.Generic;

public interface ILottery<T>
{
    void Enter(T entrant);
    void Enter(T entrant, int tickets);
    void Remove(T entrant);
    void Remove(T entrant, int tickets);

    T Draw();
    List<T> Draw(int n);
    T Draw(HashSet<T> blacklist);

    int GetTickets(T entrant);

    void StartBatchDraw();
    void StartBatchDraw(HashSet<T> blacklist);
    void EndBatchDraw();

    void ChangeSeed(int seed);
    void ChangeSeed();

    void CombineInto(ILottery<T> lottery);
    ILottery<T> CombineWith(ILottery<T> lottery);
}
