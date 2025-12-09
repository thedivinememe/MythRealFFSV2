using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MythRealFFSV2.Combat
{
    /// <summary>
    /// Represents a hex coordinate using axial coordinate system (q, r)
    /// Flat-top hexagon orientation
    /// </summary>
    [System.Serializable]
    public struct HexCoordinate : IEquatable<HexCoordinate>
    {
        public int q; // Column
        public int r; // Row

        public HexCoordinate(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        /// <summary>
        /// Convert to cube coordinates for distance calculations
        /// </summary>
        public CubeCoordinate ToCube()
        {
            int x = q;
            int z = r;
            int y = -x - z;
            return new CubeCoordinate(x, y, z);
        }

        public static HexCoordinate operator +(HexCoordinate a, HexCoordinate b)
        {
            return new HexCoordinate(a.q + b.q, a.r + b.r);
        }

        public static HexCoordinate operator -(HexCoordinate a, HexCoordinate b)
        {
            return new HexCoordinate(a.q - b.q, a.r - b.r);
        }

        public bool Equals(HexCoordinate other)
        {
            return q == other.q && r == other.r;
        }

        public override bool Equals(object obj)
        {
            return obj is HexCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(q, r);
        }

        public override string ToString()
        {
            return $"({q}, {r})";
        }

        public static bool operator ==(HexCoordinate a, HexCoordinate b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(HexCoordinate a, HexCoordinate b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Invalid coordinate constant
        /// </summary>
        public static HexCoordinate Invalid => new HexCoordinate(-1, -1);
    }

    /// <summary>
    /// Cube coordinates for distance calculations
    /// Constraint: x + y + z = 0
    /// </summary>
    public struct CubeCoordinate
    {
        public int x, y, z;

        public CubeCoordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Convert back to axial coordinates
        /// </summary>
        public HexCoordinate ToAxial()
        {
            return new HexCoordinate(x, z);
        }

        /// <summary>
        /// Calculate distance between two cube coordinates
        /// </summary>
        public static int Distance(CubeCoordinate a, CubeCoordinate b)
        {
            return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z)) / 2;
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }
    }

    /// <summary>
    /// Manages the hex grid battlefield with occupancy tracking
    /// </summary>
    public class HexBattlefield
    {
        public int width;
        public int height;

        private Dictionary<HexCoordinate, CombatantData> occupiedCells;

        public HexBattlefield(int width, int height)
        {
            this.width = width;
            this.height = height;
            occupiedCells = new Dictionary<HexCoordinate, CombatantData>();
        }

        /// <summary>
        /// Check if a hex coordinate is within battlefield bounds
        /// </summary>
        public bool IsValidPosition(HexCoordinate coord)
        {
            return coord.q >= 0 && coord.q < width && coord.r >= 0 && coord.r < height;
        }

        /// <summary>
        /// Check if a hex is occupied by a combatant
        /// </summary>
        public bool IsOccupied(HexCoordinate coord)
        {
            return occupiedCells.ContainsKey(coord);
        }

        /// <summary>
        /// Get the combatant at a position, or null if empty
        /// </summary>
        public CombatantData GetOccupant(HexCoordinate coord)
        {
            return occupiedCells.TryGetValue(coord, out var combatant) ? combatant : null;
        }

        /// <summary>
        /// Place a combatant on the battlefield
        /// </summary>
        public void PlaceCombatant(CombatantData combatant, HexCoordinate position)
        {
            if (!IsValidPosition(position))
            {
                Debug.LogWarning($"Cannot place combatant at invalid position {position}");
                return;
            }

            if (IsOccupied(position))
            {
                Debug.LogWarning($"Position {position} is already occupied!");
                return;
            }

            occupiedCells[position] = combatant;
        }

        /// <summary>
        /// Move a combatant from one position to another
        /// </summary>
        public void MoveCombatant(CombatantData combatant, HexCoordinate from, HexCoordinate to)
        {
            if (occupiedCells.ContainsKey(from))
            {
                occupiedCells.Remove(from);
            }

            PlaceCombatant(combatant, to);
        }

        /// <summary>
        /// Calculate distance between two hex coordinates
        /// </summary>
        public int GetDistance(HexCoordinate a, HexCoordinate b)
        {
            return GetDistanceStatic(a, b);
        }

        /// <summary>
        /// Static distance calculation for use without battlefield instance
        /// </summary>
        public static int GetDistanceStatic(HexCoordinate a, HexCoordinate b)
        {
            CubeCoordinate cubeA = a.ToCube();
            CubeCoordinate cubeB = b.ToCube();
            return CubeCoordinate.Distance(cubeA, cubeB);
        }

        /// <summary>
        /// Get all neighboring hexes (up to 6 for flat-top hex)
        /// </summary>
        public List<HexCoordinate> GetNeighbors(HexCoordinate coord)
        {
            // Flat-top hex direction offsets
            HexCoordinate[] directions = new HexCoordinate[]
            {
                new HexCoordinate(+1,  0), // East
                new HexCoordinate(+1, -1), // Northeast
                new HexCoordinate( 0, -1), // Northwest
                new HexCoordinate(-1,  0), // West
                new HexCoordinate(-1, +1), // Southwest
                new HexCoordinate( 0, +1), // Southeast
            };

            List<HexCoordinate> neighbors = new List<HexCoordinate>();

            foreach (var dir in directions)
            {
                HexCoordinate neighbor = coord + dir;
                if (IsValidPosition(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Get all combatants within a certain range of a position
        /// </summary>
        public List<CombatantData> GetCombatantsInRange(HexCoordinate center, int range, int teamFilter = -1)
        {
            List<CombatantData> result = new List<CombatantData>();

            foreach (var kvp in occupiedCells)
            {
                if (GetDistance(center, kvp.Key) <= range)
                {
                    if (teamFilter == -1 || kvp.Value.teamId == teamFilter)
                    {
                        result.Add(kvp.Value);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Convert meters to hex tiles
        /// </summary>
        public static float MetersToHexes(float meters, float metersPerHex = 1.5f)
        {
            return meters / metersPerHex;
        }

        /// <summary>
        /// Clear all occupants (for resetting battlefield)
        /// </summary>
        public void Clear()
        {
            occupiedCells.Clear();
        }
    }

    /// <summary>
    /// Pathfinding utilities for hex grid
    /// </summary>
    public static class HexPathfinder
    {
        /// <summary>
        /// Find a path from start to end using A* algorithm
        /// Returns null if no path exists
        /// </summary>
        public static List<HexCoordinate> FindPath(HexBattlefield grid, HexCoordinate start,
                                                   HexCoordinate end, int maxSteps,
                                                   bool ignoreOccupied = false)
        {
            if (!grid.IsValidPosition(start) || !grid.IsValidPosition(end))
                return null;

            if (start.Equals(end))
                return new List<HexCoordinate> { start };

            // A* implementation
            Dictionary<HexCoordinate, int> gScore = new Dictionary<HexCoordinate, int>();
            Dictionary<HexCoordinate, int> fScore = new Dictionary<HexCoordinate, int>();
            Dictionary<HexCoordinate, HexCoordinate> cameFrom = new Dictionary<HexCoordinate, HexCoordinate>();

            List<HexCoordinate> openSet = new List<HexCoordinate> { start };
            HashSet<HexCoordinate> closedSet = new HashSet<HexCoordinate>();

            gScore[start] = 0;
            fScore[start] = grid.GetDistance(start, end);

            while (openSet.Count > 0)
            {
                // Get node with lowest fScore
                HexCoordinate current = openSet.OrderBy(n => fScore.GetValueOrDefault(n, int.MaxValue)).First();

                if (current.Equals(end))
                {
                    // Reconstruct path
                    return ReconstructPath(cameFrom, current);
                }

                openSet.Remove(current);
                closedSet.Add(current);

                // Check all neighbors
                foreach (var neighbor in grid.GetNeighbors(current))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    // Skip occupied cells unless it's the end position
                    if (!ignoreOccupied && grid.IsOccupied(neighbor) && !neighbor.Equals(end))
                        continue;

                    int tentativeGScore = gScore.GetValueOrDefault(current, int.MaxValue) + 1;

                    // Stop if we've exceeded max steps
                    if (tentativeGScore > maxSteps)
                        continue;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if (tentativeGScore >= gScore.GetValueOrDefault(neighbor, int.MaxValue))
                    {
                        continue;
                    }

                    // This path is the best so far
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + grid.GetDistance(neighbor, end);
                }
            }

            // No path found
            return null;
        }

        /// <summary>
        /// Get all cells reachable within a movement range
        /// </summary>
        public static List<HexCoordinate> GetReachableCells(HexBattlefield grid, HexCoordinate start,
                                                            int movementRange)
        {
            List<HexCoordinate> reachable = new List<HexCoordinate>();
            Queue<HexCoordinate> frontier = new Queue<HexCoordinate>();
            Dictionary<HexCoordinate, int> distances = new Dictionary<HexCoordinate, int>();

            frontier.Enqueue(start);
            distances[start] = 0;

            while (frontier.Count > 0)
            {
                HexCoordinate current = frontier.Dequeue();
                int currentDist = distances[current];

                if (currentDist >= movementRange)
                    continue;

                foreach (var neighbor in grid.GetNeighbors(current))
                {
                    // Skip occupied cells
                    if (grid.IsOccupied(neighbor))
                        continue;

                    // Skip if already visited with shorter distance
                    if (distances.ContainsKey(neighbor))
                        continue;

                    distances[neighbor] = currentDist + 1;
                    frontier.Enqueue(neighbor);
                    reachable.Add(neighbor);
                }
            }

            return reachable;
        }

        /// <summary>
        /// Reconstruct path from cameFrom dictionary
        /// </summary>
        private static List<HexCoordinate> ReconstructPath(Dictionary<HexCoordinate, HexCoordinate> cameFrom,
                                                           HexCoordinate current)
        {
            List<HexCoordinate> path = new List<HexCoordinate> { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, current);
            }

            return path;
        }
    }

    /// <summary>
    /// Utilities for resolving Area of Effect patterns on hex grid
    /// </summary>
    public static class HexAOEResolver
    {
        /// <summary>
        /// Parse AOE description and return affected hexes
        /// </summary>
        public static List<HexCoordinate> GetAOEPattern(string aoeDescription, HexCoordinate center,
                                                        HexCoordinate? direction = null)
        {
            string aoeLower = aoeDescription.ToLower();

            // Self target
            if (aoeLower.Contains("self"))
                return new List<HexCoordinate> { center };

            // Single target
            if (aoeLower.Contains("single"))
                return new List<HexCoordinate> { center };

            // Radius patterns
            if (aoeLower.Contains("radius") || aoeLower.Contains("surrounding"))
            {
                int radius = ExtractNumber(aoeLower, 1);
                return GetRadiusPattern(center, radius);
            }

            // Square patterns
            if (aoeLower.Contains("square"))
            {
                int size = ExtractNumber(aoeLower, 3);
                return GetSquarePattern(center, size / 2); // Convert to radius
            }

            // Line patterns
            if (aoeLower.Contains("line"))
            {
                int length = ExtractNumber(aoeLower, 3);
                HexCoordinate dir = direction ?? new HexCoordinate(1, 0); // Default: east
                return GetLinePattern(center, dir, length);
            }

            // Cone patterns
            if (aoeLower.Contains("cone"))
            {
                int range = ExtractNumber(aoeLower, 3);
                HexCoordinate dir = direction ?? new HexCoordinate(1, 0);
                return GetConePattern(center, dir, range);
            }

            // Default: single target
            return new List<HexCoordinate> { center };
        }

        /// <summary>
        /// Get all hexes within radius (circular pattern)
        /// </summary>
        private static List<HexCoordinate> GetRadiusPattern(HexCoordinate center, int radius)
        {
            List<HexCoordinate> hexes = new List<HexCoordinate>();

            for (int q = -radius; q <= radius; q++)
            {
                for (int r = Math.Max(-radius, -q - radius); r <= Math.Min(radius, -q + radius); r++)
                {
                    HexCoordinate hex = new HexCoordinate(center.q + q, center.r + r);
                    hexes.Add(hex);
                }
            }

            return hexes;
        }

        /// <summary>
        /// Get hexes in approximate square pattern
        /// </summary>
        private static List<HexCoordinate> GetSquarePattern(HexCoordinate center, int radius)
        {
            // For simplicity, use radius pattern
            return GetRadiusPattern(center, radius);
        }

        /// <summary>
        /// Get hexes in a line from center in direction
        /// </summary>
        private static List<HexCoordinate> GetLinePattern(HexCoordinate center, HexCoordinate direction, int length)
        {
            List<HexCoordinate> hexes = new List<HexCoordinate> { center };

            // Normalize direction to one of the 6 hex directions
            HexCoordinate normalizedDir = NormalizeDirection(direction);

            for (int i = 1; i < length; i++)
            {
                HexCoordinate hex = new HexCoordinate(
                    center.q + normalizedDir.q * i,
                    center.r + normalizedDir.r * i
                );
                hexes.Add(hex);
            }

            return hexes;
        }

        /// <summary>
        /// Get hexes in a cone pattern
        /// </summary>
        private static List<HexCoordinate> GetConePattern(HexCoordinate center, HexCoordinate direction, int range)
        {
            List<HexCoordinate> hexes = new List<HexCoordinate> { center };

            // Simplified cone: include hexes within range that are roughly in direction
            HexCoordinate normalizedDir = NormalizeDirection(direction);

            for (int dist = 1; dist <= range; dist++)
            {
                // Add hexes at this distance
                for (int spread = -dist / 2; spread <= dist / 2; spread++)
                {
                    // Calculate hex position (simplified cone spread)
                    HexCoordinate hex = new HexCoordinate(
                        center.q + normalizedDir.q * dist + spread,
                        center.r + normalizedDir.r * dist
                    );
                    hexes.Add(hex);
                }
            }

            return hexes;
        }

        /// <summary>
        /// Normalize direction to nearest hex direction
        /// </summary>
        private static HexCoordinate NormalizeDirection(HexCoordinate direction)
        {
            // Six main hex directions for flat-top
            HexCoordinate[] mainDirections = new HexCoordinate[]
            {
                new HexCoordinate(+1,  0), // East
                new HexCoordinate(+1, -1), // Northeast
                new HexCoordinate( 0, -1), // Northwest
                new HexCoordinate(-1,  0), // West
                new HexCoordinate(-1, +1), // Southwest
                new HexCoordinate( 0, +1), // Southeast
            };

            // Find closest direction
            int minDist = int.MaxValue;
            HexCoordinate closest = mainDirections[0];

            foreach (var dir in mainDirections)
            {
                int dist = Math.Abs(direction.q - dir.q) + Math.Abs(direction.r - dir.r);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = dir;
                }
            }

            return closest;
        }

        /// <summary>
        /// Extract first number from string, or return default
        /// </summary>
        private static int ExtractNumber(string text, int defaultValue)
        {
            var match = System.Text.RegularExpressions.Regex.Match(text, @"\d+");
            return match.Success ? int.Parse(match.Value) : defaultValue;
        }
    }
}
