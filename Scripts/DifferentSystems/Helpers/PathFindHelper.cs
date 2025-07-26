using System;
using System.Collections.Generic;
using System.Linq;

public static class PathFindHelper
{
    public static List<Door> Doors => _doors;
    private static List<Door> _doors = new List<Door>();
    
    //All connections except corridor and playroom (no door)
    private static List<Tuple<PlaceEnum, PlaceEnum>> _placeConnections = new List<Tuple<PlaceEnum, PlaceEnum>>() 
    { 
        new Tuple<PlaceEnum, PlaceEnum>(PlaceEnum.Bathroom, PlaceEnum.Corridor),
        new Tuple<PlaceEnum, PlaceEnum>(PlaceEnum.CabinetLobby, PlaceEnum.Corridor),
        new Tuple<PlaceEnum, PlaceEnum>(PlaceEnum.Outside, PlaceEnum.Corridor),
        new Tuple<PlaceEnum, PlaceEnum>(PlaceEnum.Kitchen, PlaceEnum.Playroom),
        new Tuple<PlaceEnum, PlaceEnum>(PlaceEnum.Bedroom, PlaceEnum.Playroom),
        new Tuple<PlaceEnum, PlaceEnum>(PlaceEnum.Pantyroom, PlaceEnum.Playroom),
        new Tuple<PlaceEnum, PlaceEnum>(PlaceEnum.CabinetLobby, PlaceEnum.CabinetMainRoom),
    };

    public static Stack<PointOfInterest> GetPathWithDoors(PlaceEnum start, PointOfInterest finish)
    {
        var result = new Stack<PointOfInterest>();
        result.Push(finish);

        var allowedWays = _placeConnections;
        RecursivePathFinding(result, allowedWays, start, finish.Place);

        return result;
    }

    private static bool RecursivePathFinding(Stack<PointOfInterest> result, List<Tuple<PlaceEnum, PlaceEnum>> allowedWays, PlaceEnum place, PlaceEnum finish)
    {
        if (!allowedWays.Any())
        {
            return false;
        }

        var possibleWays = allowedWays.Where(a => a.Item1 == place || a.Item2 == place);

        if (!possibleWays.Any())
        {
            return false;
        }

        allowedWays = allowedWays.Where(a => !possibleWays.Contains(a)).ToList();

        if (possibleWays.Any(a => a.Item1 == finish || a.Item2 == finish))
        {
            //result.Push(PushDoor(place, finish));
            return true;
        }

        foreach (var possibleWay in possibleWays)
        {
            var nextPlace = possibleWay.Item1 == place ? possibleWay.Item2 : possibleWay.Item1;
            if (RecursivePathFinding(result, allowedWays, nextPlace, finish))
            {
                //result.Push(PushDoor(place, nextPlace));
                return true;
            }
        }

        return false;
    }

    private static PointOfInterest PushDoor(PlaceEnum start, PlaceEnum finish) => _doors
                .Single(a => a.Places.Select(a => a.Place).Contains(start) && a.Places.Select(a => a.Place).Contains(finish))
                .Places.Single(a => a.Place == start);
}
