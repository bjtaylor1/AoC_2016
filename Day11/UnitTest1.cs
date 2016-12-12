using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Day11
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestGetMovableCombinations()
        {
            var state = new State { Floors = new[] { new List<string> { "AB", "AC", "AD" }, } };
            state.GetMovableCombinations().Select(s => string.Join(",", s.OrderBy(i => i))).Should().HaveCount(6).And.Contain(new[]
            {
                "AB",
                "AC",
                "AD",
                "AB,AC",
                "AB,AD",
                "AC,AD"
                });
        }

        [Test]
        public void TestEquality()
        {
            var state1 = new State
            {
                Floors = new[]
                {
                    new List<string> {"AB", "AC", "AD"},
                    new List<string> {"AC", "AD"},
                    new List<string> {"AD"},
                    new List<string> {"AD", "AG", "AH"}
                },
                Pos = 1
            };
            var state2 = new State
            {
                Floors = new[]
                {
                    new List<string> {"AB", "AC", "AD"},
                    new List<string> {"AC", "AD"},
                    new List<string> {"AD"},
                    new List<string> {"AD", "AG", "AH"}
                },
                Pos = 1
            };
            state1.Equals(state2).Should().BeTrue();
        }

        [Test]
        public void TestInequality()
        {
            var state1 = new State
            {
                Floors = new[]
                {
                    new List<string> {"AB", "AC", "AD"},
                    new List<string> {"AC", "AD"},
                    new List<string> {"AD"},
                    new List<string> {"AD", "AG"}
                },
                Pos = 1
            };
            var state2 = new State
            {
                Floors = new[]
                {
                    new List<string> {"AB", "AC", "AD"},
                    new List<string> {"AC", "AD"},
                    new List<string> {"AD"},
                    new List<string> {"AD", "AG", "AH"}
                },
                Pos = 1
            };
            state1.Equals(state2).Should().BeFalse();
        }

        [Test]
        public void Advance()
        {
            var state = new State
            {
                Floors = new[]
            {
                new List<string> { "AB", "AC", "AD" },
                new List<string>(),
                new List<string>(),
                new List<string>()
            }
            };
            var newState = state.ApplyMove(new[] { "AB", "AC" }, 1);
            newState.Floors[1].Should().HaveCount(2).And.Contain(new[] { "AB", "AC" });
            newState.Floors[0].Should().HaveCount(1).And.Contain(new[] { "AD" });
            newState.Pos.Should().Be(1);
        }

        [Test]
        public void Sort()
        {
            var goodState = new State
            {
                Floors = new[]
                {
                    new List<string>(),
                    new List<string>(),
                    new List<string> {"AB", "AC", "AD"}, /* they're higher up */
                    new List<string>()
                }
            };
            var badState = new State
            {
                Floors = new[]
                {
                    new List<string> {"AB", "AC", "AD"},
                    new List<string>(),
                    new List<string>(),
                    new List<string>()
                }
            };
            var states = new List<State> { badState, goodState };
            states.Sort();
            states.First().Should().Be(goodState);
        }


        [TestCase(false, "THG", "LIM")]
        [TestCase(false, "THG", "LIM", "PRG", "PRM")]
        [TestCase(true, "THG", "LIG", "LIM")]
        [TestCase(true, "THG", "THM", "LIG", "LIM")]
        [TestCase(false, "THG", "THG", "THM", "LIG", "LIM", "LIM")]
        [TestCase(true, "THG", "LIG", "PRG")]
        [TestCase(true, "THM", "LIM", "PRM")]
        public void IsValidTest(bool isValid, params string[] items)
        {
            var state = new State { Floors = new[] { new List<string>(items) } };
            state.IsValid().Should().Be(isValid);
        }

        [Test]
        public void ExampleResult()
        {
            var state = new State
            {
                Floors = new[]
                {
                    new List<string> {"HyM", "LiM"},
                    new List<string> {"HyG"},
                    new List<string> {"LiG"},
                    new List<string>()
                }
            };
            state = CheckAndAdvance(state, new State
            {
                Floors = new[]
                {
                    new List<string> {"LiM"},
                    new List<string> {"HyG", "HyM"},
                    new List<string> {"LiG"},
                    new List<string>()
                },
                Pos = 1
            });
            state = CheckAndAdvance(state, new State
            {
                Floors = new[]
                {
                    new List<string> {"LiM"},
                    new List<string> {},
                    new List<string> { "HyG", "HyM", "LiG" },
                    new List<string>()
                },
                Pos = 2
            });
            state = CheckAndAdvance(state, new State
            {
                Floors = new[]
                {
                    new List<string> {"LiM"},
                    new List<string> {"HyM"},
                    new List<string> { "HyG",  "LiG" },
                    new List<string>()
                },
                Pos = 1
            });
            state = CheckAndAdvance(state, new State
            {
                Floors = new[]
                {
                    new List<string> { "HyM", "LiM" },
                    new List<string> {},
                    new List<string> { "HyG",  "LiG" },
                    new List<string>()
                },
                Pos = 0
            });
            state = CheckAndAdvance(state, new State
            {
                Floors = new[]
                {
                    new List<string> {},
                    new List<string> { "HyM", "LiM"},
                    new List<string> { "HyG",  "LiG" },
                    new List<string>()
                },
                Pos = 1
            });
            state = CheckAndAdvance(state, new State
            {
                Floors = new[]
                {
                    new List<string> {},
                    new List<string> {},
                    new List<string> { "HyG", "HyM" ,  "LiG", "LiM"},
                    new List<string>()
                },
                Pos = 2
            });
            state = CheckAndAdvance(state, new State
            {
                Floors = new[]
                {
                    new List<string> {},
                    new List<string> {},
                    new List<string> { "HyG",  "LiG"  },
                    new List<string> { "HyM", "LiM"}
                },
                Pos = 3
            });
            state = CheckAndAdvance(state, new State
            {
                Floors = new[]
                {
                    new List<string> {},
                    new List<string> {},
                    new List<string> { "HyG", "HyM",  "LiG"  },
                    new List<string> { "LiM"}
                },
                Pos = 2
            });
            state = CheckAndAdvance(state, new State
            {
                Floors = new[]
                {
                    new List<string> {},
                    new List<string> {},
                    new List<string> { "HyM"  },
                    new List<string> { "HyG",  "LiG", "LiM"}
                },
                Pos = 3
            });
            state = CheckAndAdvance(state, new State
            {
                Floors = new[]
                {
                    new List<string> {},
                    new List<string> {},
                    new List<string> { "HyM", "LiM"  },
                    new List<string> { "HyG",  "LiG"}
                },
                Pos = 2
            });
            state = CheckAndAdvance(state, new State
            {
                Floors = new[]
                {
                    new List<string> {},
                    new List<string> {},
                    new List<string> { },
                    new List<string> { "HyG", "HyM",  "LiG", "LiM" }
                },
                Pos = 3
            });
            state.IsComplete().Should().BeTrue();
            state.Depth.Should().Be(11);
        }


        [Test]
        public void Example()
        {
            var initialState = new State
            {
                Floors = new[]
                {
                    new List<string> {"HyM", "LiM"},
                    new List<string> {"HyG"},
                    new List<string> {"LiG"},
                    new List<string>(),
                }
            };
            Solve(initialState);
        }

        [Test]
        public void DoPuzzle()
        {
            var initialState = new State
            {
                Floors = new[]
                {
                    new List<string> {"ThG", "ThM", "PlG", "StG"},
                    new List<string> {"PlM", "StM"},
                    new List<string> {"PrG", "PrM", "RuG", "RuM"},
                    new List<string> {}
                }
            };
            Solve(initialState);
        }

        private static void Solve(State initialState)
        {
            foreach (var floor in initialState.Floors)
            {
                floor.Sort();
            }

            var sortedStates = new List<State> {initialState};
            State completeState = null;
            while (true)
            {
                var bestState = sortedStates.OrderBy(s => s.GetScore()).First(s => !s.Visited);
                bestState.Visited = true;
                var nextStates = bestState.GetTransitions().Where(s => s.IsValid() && !sortedStates.Contains(s)).ToArray();
                //if(nextStates.Any()) sortedStates.RemoveAt(0);
                sortedStates.AddRange(nextStates);

                completeState = nextStates.FirstOrDefault(s => s.IsComplete());
                if (completeState != null) break;
                //LogManager.GetCurrentClassLogger().Debug($"{nextStates.Length}");
                
                LogManager.GetCurrentClassLogger().Info($"States: {sortedStates.Count}");
                //allStates = allStates.Take((int) 1e5).ToList();
            }
            for (int index = completeState.Path.Count - 1; index >= 0; index--)
            {
                var state = completeState.Path[index];
                Console.Out.WriteLine(state.Render() + "\n");
            }
            Console.Out.WriteLine(completeState.Depth);
        }

        private static State CheckAndAdvance(State s, State newState)
        {
            var transitions = s.GetTransitions();
            var result = transitions.FirstOrDefault(i => i.Equals(newState));
            result.Should().NotBeNull();
            Console.Out.WriteLine(result.GetScore());
            return result;
        }


    }

    public class State : IEquatable<State>, IComparable<State>
    {
        public bool Equals(State other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Pos != other.Pos) return false;
            var floorHashes = FloorHashes();
            var otherFloorHashes = other.FloorHashes();
            var floorsEqual = floorHashes.Equals(otherFloorHashes);
            return floorsEqual;
        }

        private string FloorHashes()
        {
//            var floorHashes = string.Join(", ", Floors.Select(f => f.OrderBy(s => s)));
            var floorHashes = string.Join(", ", Floors.Select(f => new string(f.Select(i => i.Last()).OrderBy(c => c).ToArray())));
            return floorHashes;
        }

        public bool Degenerate()
        {
            var degenerate =
                Floors[0].Any() &&
                !Floors[1].Any() &&
                !Floors[2].Any() &&
                Floors[3].Any();
            if (degenerate) LogManager.GetLogger("Degenerate").Info("Degenerate!");
            return degenerate;
        }

        public string Render()
        {
            var render = string.Join("\n", Floors.Select((f, i) => new { f, i }).OrderByDescending(a => a.i).Select(a =>
               {
                   var abbreviatedItems = a.f.Select(s => new string(new[] { s[0], s[2] }));
                   var code = $"F{a.i + 1} " + (Pos == a.i ? "E " : "  ") + string.Join(" ", abbreviatedItems);
                   return code;
               }));
            return render;
        }

        public int CompareTo(State other)
        {
            return GetScore().CompareTo(other.GetScore());
        }

        public int GetScore()
        {
            //low == good
            var topHeaviness = Floors[3].Count * 4 +
                               Floors[2].Count * 3 +
                               Floors[1].Count * 2 +
                               Floors[0].Count;
            return -topHeaviness;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((State)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Floors != null ? FloorHashes().GetHashCode() * 397 : 0) ^ Pos;
            }
        }

        public static bool operator ==(State left, State right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(State left, State right)
        {
            return !Equals(left, right);
        }

        public List<string>[] Floors = new List<string>[3];
        public int Pos = 0;
        public int Depth; /* how many moves it takes to get to this stage */

        public State()
        {
            Path = new List<State> { this };
        }

        public List<State> Path;
        public State(State state, int movement, int depth) : this()
        {
            Floors = state.Floors.Select(s => new List<string>(s)).ToArray();
            Pos = state.Pos + movement;
            Depth = depth;
            Path.AddRange(state.Path);
        }

        public bool IsValid()
        {
            var isValid = Floors.All(ContentsCompatible) && Depth < 100;
            return isValid;
        }

        public bool IsComplete()
        {
            return Floors[0].Count + Floors[1].Count + Floors[2].Count == 0;
        }

        public static bool ContentsCompatible(ICollection<string> items)
        {
            //remove pairs
            var microchips = items.Where(s => s.EndsWith("M")).ToList();
            var generators = items.Where(s => s.EndsWith("G")).ToList();
            for (int index = microchips.Count - 1; index >= 0; index--)
            {
                var microchip = microchips[index];
                var firstMatchingGenerator = generators.FindIndex(g => g.StartsWith(microchip.Substring(0, 2)));
                if (firstMatchingGenerator != -1)
                {
                    generators.RemoveAt(firstMatchingGenerator);
                    microchips.RemoveAt(index);
                }
            }
            var valid = !(microchips.Any() && generators.Any());
            return valid;
        }

        public string[][] GetMovableCombinations()
        {
            var movableCombinations = new List<string[]>();
            for (int i = 0; i < Floors[Pos].Count; i++)
            {
                movableCombinations.Add(new[] { Floors[Pos][i] });
                for (int j = i + 1; j < Floors[Pos].Count; j++)
                {
                    movableCombinations.Add(new[] { Floors[Pos][i], Floors[Pos][j] });
                }
            }

            return movableCombinations.Where(ContentsCompatible).ToArray();
        }

        public State ApplyMove(string[] items, int floorChange)
        {
            var newState = new State(this, floorChange, Depth + 1);
            newState.Floors[Pos].RemoveAll(items.Contains);
            newState.Floors[Pos + floorChange].AddRange(items);
            newState.Floors[Pos + floorChange].Sort();
            return newState;
        }

        public State[] GetTransitions()
        {
            var states = new List<State>();
            var movableCombinations = GetMovableCombinations();
            if (Pos > 0)
            {
                states.AddRange(movableCombinations.Select(s => ApplyMove(s, -1)));
            }
            if (Pos < 3)
            {
                states.AddRange(movableCombinations.Select(s => ApplyMove(s, 1)));
            }
            var transitions = states.Where(s => !Path.Contains(s) && !s.Degenerate()).ToArray();
            return transitions;
        }

        public bool Visited { get; set; }
    }
}
