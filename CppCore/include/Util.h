#pragma once
#include <cstdint>
#include <forward_list>
#include <functional>
#include <algorithm>

typedef std::uint8_t byte;
typedef std::int8_t sbyte;
typedef unsigned int uint;

template<typename T> inline T operator| (T a, T b)
{
    return (T)((int)a | (int)b);
}
template<typename T> inline T operator& (T a, T b)
{
    return (T)((int)a & (int)b);
}
template<typename T> inline T operator^ (T a, T b)
{
    return (T)((int)a ^ (int)b);
}
template<typename T> inline T& operator|= (T& a, T b)
{
    return (T&)((int&)a |= (int)b);
}
template<typename T> inline T& operator&= (T& a, T b)
{
    return (T&)((int&)a &= (int)b);
}
template<typename T> inline T& operator^= (T& a, T b)
{
    return (T&)((int&)a ^= (int)b);
}

template<class... TArgs> class Event
{
public:
    Event() :m_handlers()
    { }
    template <class T> void AddHandler(T&& func)
    {
        m_handlers.emplace_front(std::forward<T>(func));
    }
    template<class T> void RemoveHandler(T&& foo)
    {
        m_handlers.remove_if(
            [temp = std::function<void(TArgs...)>(std::forward<T>(foo))](const auto& f) {
            return f.target<void(TArgs...)>() == temp.target<void(TArgs...)>();
        });
    }
    void RemoveAll()
    {
        m_handlers.clear();
    }

protected:
    void Fire(TArgs... args) const
    {
        std::for_each(m_handlers.cbegin(), m_handlers.cend(), [args = std::move(args)...](const auto& f) {
            f(args...);
        });
    }

private:
    std::forward_list<std::function<void(TArgs...)>> m_handlers;

};