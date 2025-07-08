// Kore Event Register : A list of the major events that a consumer application would want to pay attention to.
// - Each event is a string dictionary, so it can acquire new properties as needed, without additional classes.

using System.Collections.Generic;
using System.Linq;

using KoreCommon;

namespace KoreSim;

public class KoreEventRegister
{
    // --------------------------------------------------------------------------------------------
    // MARK: Data
    // --------------------------------------------------------------------------------------------

    private readonly List<KoreStringDictionary> EventList = new();
    private readonly object EventListLock = new();

    private int MaxEventCount = 1000;

    // --------------------------------------------------------------------------------------------
    // MARK: Event Keywords
    // --------------------------------------------------------------------------------------------

    public const string KeyEventType = "EventType";

    public const string EventEntityCreated = "EntityCreated"; // KoreEventRegister.EventEntityCreated
    public const string EventEntityDeleted = "EntityDeleted"; // KoreEventRegister.EventEntityDeleted
    public const string EventEntityElementCreated = "EntityElementCreated"; // KoreEventRegister.EventEntityElementCreated
    public const string EventEntityElementDeleted = "EntityElementDeleted"; // KoreEventRegister.EventEntityElementDeleted

    public const string KeyEntityName = "EntityName"; // KoreEventRegister.KeyEntityName
    public const string KeyElementName = "ElementName"; // KoreEventRegister.KeyElementName


    // --------------------------------------------------------------------------------------------
    // MARK: List management
    // --------------------------------------------------------------------------------------------

    public int EventCount
    {
        get
        {
            lock (EventListLock)
                return EventList.Count;
        }
    }

    public bool HasEvent => EventCount > 0;

    public void AddEvent(KoreStringDictionary eventDict)
    {
        lock (EventListLock)
        {
            EventList.Add(eventDict);
            LimitQueueSize();
        }
    }

    public KoreStringDictionary PeekNextEvent()
    {
        lock (EventListLock)
        {
            return EventList.Count > 0 ? EventList[0] : new KoreStringDictionary();
        }
    }

    public KoreStringDictionary ConsumeNextEvent()
    {
        lock (EventListLock)
        {
            if (EventList.Count == 0)
                return new KoreStringDictionary();

            var ev = EventList[0];
            EventList.RemoveAt(0);
            return ev;
        }
    }

    public void ClearAllEvents()
    {
        lock (EventListLock)
        {
            EventList.Clear();
        }
    }

    // Remove oldest events if the list exceeds the maximum size
    private void LimitQueueSize()
    {
        lock (EventListLock)
        {
            while (EventList.Count > MaxEventCount)
            {
                EventList.RemoveRange(0, EventList.Count - MaxEventCount);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Complex Consumers
    // --------------------------------------------------------------------------------------------

    public KoreStringDictionary? ConsumeNextEventByType(string eventType)
    {
        lock (EventListLock)
        {
            for (int i = 0; i < EventList.Count; i++)
            {
                KoreStringDictionary ev = EventList[i];

                if (ev.Has(KeyEventType))
                {
                    if (ev[KeyEventType] == eventType)
                    {
                        EventList.RemoveAt(i);
                        return ev;
                    }
                }
            }
        }
        return null;
    }

    public List<KoreStringDictionary> ConsumeAllEventsByType(string eventType)
    {
        lock (EventListLock)
        {
            List<KoreStringDictionary> matchingEvents = new();

            for (int i = 0; i < EventList.Count; i++)
            {
                KoreStringDictionary ev = EventList[i];

                if (ev.Has(KeyEventType) && ev[KeyEventType] == eventType)
                {
                    matchingEvents.Add(ev);
                }
            }

            // Remove all matching events from the list
            EventList.RemoveAll(ev => ev.Has(KeyEventType) && ev[KeyEventType] == eventType);

            return matchingEvents;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Prefab Event
    // --------------------------------------------------------------------------------------------

    public static KoreStringDictionary CreateEvent_CreateEntity(string entityName)
    {
        return new KoreStringDictionary
        {
            [KeyEventType] = EventEntityCreated,
            [KeyEntityName] = entityName
        };
    }

    public static KoreStringDictionary CreateEvent_DeleteEntity(string entityName)
    {
        return new KoreStringDictionary
        {
            [KeyEventType] = EventEntityDeleted,
            [KeyEntityName] = entityName
        };
    }

    public static KoreStringDictionary CreateEvent_CreateEntityElement(string entityName, string elementName)
    {
        return new KoreStringDictionary
        {
            [KeyEventType] = EventEntityElementCreated,
            [KeyEntityName] = entityName,
            [KeyElementName] = elementName
        };
    }

    public static KoreStringDictionary CreateEvent_DeleteEntityElement(string entityName, string elementName)
    {
        return new KoreStringDictionary
        {
            [KeyEventType] = EventEntityElementDeleted,
            [KeyEntityName] = entityName,
            [KeyElementName] = elementName
        };
    }
}
