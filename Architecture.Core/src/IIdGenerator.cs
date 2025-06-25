namespace Architecture.Core;

public interface IIdGenerator<out TId>
{
    TId NewId();
}