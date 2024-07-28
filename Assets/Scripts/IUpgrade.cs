using System;

public interface IUpgrade
{
    public Action<bool, int> OnUnlock { get; set; }
}
